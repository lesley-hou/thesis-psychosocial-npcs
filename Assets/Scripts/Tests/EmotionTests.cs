using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using AspectSystem;

public class EmotionTests
{
    private EkmanEmotionModel ekman;
    private ParrottEmotionModel parrott;

    [SetUp]
    public void Setup()
    {
        ekman = new EkmanEmotionModel();
        parrott = new ParrottEmotionModel();
    }

    // EKMAN EMOTION MODEL TESTS

    [Test]
    public void Ekman_StartsWithNoActiveTraits()
    {
        Assert.AreEqual(0, ekman.Traits.Count, "Ekman model should start with no active traits.");
    }

    [Test]
    public void Ekman_AddTrait_AddsValidCoreTrait()
    {
        ekman.AddTrait("Joy");
        Assert.IsNotNull(ekman.GetTrait("Joy"));
    }

    [Test]
    public void Ekman_AddTrait_FailsIfNotInCore()
    {
        ekman.AddTrait("Test");
        Assert.IsNull(ekman.GetTrait("Test"));
    }

    [Test]
    public void Ekman_CanSetAndRetrieveTraitValue_AfterAdding()
    {
        ekman.AddTrait("Joy");
        ekman.SetTraitValue("Joy", 0.75f);
        Assert.AreEqual(0.75f, ekman.GetTraitValue("Joy"), 0.001f);
    }

    [Test]
    public void Ekman_GetTrait_ReturnsNullIfNotAdded()
    {
        Assert.IsNull(ekman.GetTrait("Sadness"), "Trait 'Sadness' should not exist before being added.");
    }

    [Test]
    public void Ekman_SubTraitAffectsParentComputation()
    {
        ekman.AddTrait("Sadness");
        Trait sadness = ekman.GetTrait("Sadness");

        ekman.AddSubTrait(sadness, "Gloom", 0.5f, -0.6f);
        ekman.AddSubTrait(sadness, "Misery", 0.5f, -0.2f);

        float expected = (-0.6f * 0.5f) + (-0.2f * 0.5f);
        float actual = sadness.ComputeTraitValue();

        Assert.AreEqual(expected, actual, 0.001f, "Computed value should match weighted sub-traits.");
    }

    // PARROTT EMOTION MODEL TESTS
    [Test]
    public void Parrott_StartsWithNoActiveTraits()
    {
        Assert.AreEqual(0, parrott.Traits.Count, "Parrott model should start with no active traits.");
    }

    [Test]
    public void Parrott_AddTrait_AddsValidCoreTrait()
    {
        parrott.AddTrait("Joy");
        Assert.IsNotNull(parrott.GetTrait("Joy"));
    }

    [Test]
    public void Parrott_AddTrait_FailsIfNotInCore()
    {
        parrott.AddTrait("TestEmotion");
        Assert.IsNull(parrott.GetTrait("TestEmotion"));
    }

    [Test]
    public void Parrott_CanAddValidSubTrait()
    {
        parrott.AddTrait("Joy");
        Trait joy = parrott.GetTrait("Joy");

        bool result = parrott.AddSubTrait(joy, "Cheerfulness", 0.6f);
        Assert.IsTrue(result);
        Assert.IsNotNull(parrott.GetTrait("Cheerfulness"));
    }

    [Test]
    public void Parrott_AddInvalidSubTrait_Fails()
    {
        parrott.AddTrait("Joy");
        Trait joy = parrott.GetTrait("Joy");

        bool result = parrott.AddSubTrait(joy, "InvalidSubEmotion", 0.5f);
        Assert.IsFalse(result);
        Assert.IsNull(parrott.GetTrait("InvalidSubEmotion"));
    }

    [Test]
    public void Parrott_SubTraitValue_AffectsParentValue()
    {
        parrott.AddTrait("Joy");
        Trait joy = parrott.GetTrait("Joy");

        parrott.AddSubTrait(joy, "Cheerfulness", 0.7f);
        parrott.SetTraitValue("Cheerfulness", 0.8f);

        float expected = 0.8f * 0.7f;
        Assert.AreEqual(expected, parrott.GetTraitValue("Joy"), 0.001f);
    }

    [Test]
    public void Parrott_SecondaryEmotion_CanHaveTertiarySubTrait()
    {
        parrott.AddTrait("Joy");
        Trait joy = parrott.GetTrait("Joy");

        parrott.AddSubTrait(joy, "Cheerfulness", 0.7f);
        Trait cheerfulness = parrott.GetTrait("Cheerfulness");

        bool result = parrott.AddSubTrait(cheerfulness, "Happiness", 0.5f);
        Assert.IsTrue(result);
        Assert.IsNotNull(parrott.GetTrait("Happiness"));
    }

    [Test]
    public void Parrott_InvalidTertiarySubTrait_Fails()
    {
        parrott.AddTrait("Joy");
        Trait joy = parrott.GetTrait("Joy");

        parrott.AddSubTrait(joy, "Cheerfulness", 0.7f);
        Trait cheerfulness = parrott.GetTrait("Cheerfulness");

        bool result = parrott.AddSubTrait(cheerfulness, "FakeEmotion", 0.5f);
        Assert.IsFalse(result);
        Assert.IsNull(parrott.GetTrait("FakeEmotion"));
    }

    [Test]
    public void Parrott_TertiarySubTraitValue_PropagatesCorrectly()
    {
        parrott.AddTrait("Joy");
        Trait joy = parrott.GetTrait("Joy");

        parrott.AddSubTrait(joy, "Cheerfulness", 0.7f);
        Trait cheerfulness = parrott.GetTrait("Cheerfulness");

        parrott.AddSubTrait(cheerfulness, "Happiness", 0.5f);
        parrott.SetTraitValue("Happiness", 0.9f);

        float cheerfulnessValue = 0.9f * 0.5f;
        float joyValue = cheerfulnessValue * 0.7f;

        Assert.AreEqual(cheerfulnessValue, parrott.GetTraitValue("Cheerfulness"), 0.001f);
        Assert.AreEqual(joyValue, parrott.GetTraitValue("Joy"), 0.001f);
    }

    [Test]
public void Parrott_MultipleSubTraits_ContributeCorrectly()
{
    parrott.AddTrait("Joy");
    Trait joy = parrott.GetTrait("Joy");

    parrott.AddSubTrait(joy, "Cheerfulness", 0.5f);
    parrott.AddSubTrait(joy, "Zest", 0.5f);

    parrott.SetTraitValue("Cheerfulness", 0.8f);
    parrott.SetTraitValue("Zest", 0.4f);

    // Joy = (0.8 * 0.5) + (0.4 * 0.5) = 0.6
    float expected = (0.8f * 0.5f) + (0.4f * 0.5f);
    Assert.AreEqual(expected, parrott.GetTraitValue("Joy"), 0.001f);
}

[Test]
public void Parrott_SubTraitsWithTertiaryTraits_ComputeProperly()
{
    parrott.AddTrait("Joy");
    Trait joy = parrott.GetTrait("Joy");

    parrott.AddSubTrait(joy, "Cheerfulness", 0.7f);
    parrott.AddSubTrait(joy, "Zest", 0.3f);

    Trait cheerfulness = parrott.GetTrait("Cheerfulness");
    Trait zest = parrott.GetTrait("Zest");

    parrott.AddSubTrait(cheerfulness, "Happiness", 0.5f);
    parrott.AddSubTrait(cheerfulness, "Jubilation", 0.5f);

    parrott.AddSubTrait(zest, "Enthusiasm", 1.0f);

    // Set tertiary trait values
    parrott.SetTraitValue("Happiness", 0.8f);
    parrott.SetTraitValue("Jubilation", 0.6f);
    parrott.SetTraitValue("Enthusiasm", 0.9f);

    // Compute:
    // Cheerfulness = (0.8 * 0.5) + (0.6 * 0.5) = 0.7
    // Zest = 0.9 * 1.0 = 0.9
    // Joy = (0.7 * 0.7) + (0.9 * 0.3) = 0.76

    float cheerfulnessExpected = (0.8f * 0.5f) + (0.6f * 0.5f);
    float zestExpected = 0.9f;
    float joyExpected = (cheerfulnessExpected * 0.7f) + (zestExpected * 0.3f);

    Assert.AreEqual(cheerfulnessExpected, parrott.GetTraitValue("Cheerfulness"), 0.001f);
    Assert.AreEqual(zestExpected, parrott.GetTraitValue("Zest"), 0.001f);
    Assert.AreEqual(joyExpected, parrott.GetTraitValue("Joy"), 0.001f);
}

[Test]
public void Parrott_TertiaryTrait_WithoutOtherContributors_ComputesCorrectly()
{
    parrott.AddTrait("Fear");
    Trait fear = parrott.GetTrait("Fear");

    parrott.AddSubTrait(fear, "Horror", 1.0f);
    Trait horror = parrott.GetTrait("Horror");

    parrott.AddSubTrait(horror, "Terror", 1.0f);

    parrott.SetTraitValue("Terror", 0.5f);

    // Horror = 0.5
    // Fear = 0.5

    Assert.AreEqual(0.5f, parrott.GetTraitValue("Horror"), 0.001f);
    Assert.AreEqual(0.5f, parrott.GetTraitValue("Fear"), 0.001f);
}
}