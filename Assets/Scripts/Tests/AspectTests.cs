using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using PersonalitySystemModels;
using AspectSystem;

public class PersonalityAspectTests
{
    private PersonalityAspect personality;

    [SetUp]
    public void Setup()
    {
        List<AspectModel> models = new List<AspectModel>
        {
            new MBTIPersonalityModel(),
            new FFMPersonalityModel()
        };

        personality = new PersonalityAspect(models);
    }

    private void BuildPersonalityTraitModel() 
    {
        var mbti = personality.PersonalityModels["MBTIPersonalityModel"];
        var ffm = personality.PersonalityModels["FFMPersonalityModel"];

        mbti.AddTrait("EI");
        ffm.AddTrait("Extraversion");

        // Add subtraits
        Trait ei = mbti.GetTrait("EI");
        Trait extraversion = ffm.GetTrait("Extraversion");

        mbti.AddSubTrait(ei, "Open", 0.5f, -0.1f);
        mbti.AddSubTrait(ei, "Confident", 0.7f, -0.4f);
        mbti.AddSubTrait(ei, "Outgoing", 0.8f, -0.5f);

        ffm.AddSubTrait(extraversion, "Confident", 0.7f, 0.3f);
        ffm.AddSubTrait(extraversion, "Outgoing", 0.8f, 0.6f);
        ffm.AddSubTrait(extraversion, "Independent", 0.9f, 0.1f);
    }

    [Test]
    public void PersonalityAspect_InitializesModelsCorrectly()
    {
        Assert.AreEqual(2, personality.Models.Count);
        Assert.IsTrue(personality.Models.ContainsKey("MBTIPersonalityModel"));
        Assert.IsTrue(personality.Models.ContainsKey("FFMPersonalityModel"));
    }

    [Test]
    public void GetTrait_ReturnsNullForMissingTrait()
    {
        BuildPersonalityTraitModel();

        var ffm = personality.PersonalityModels["FFMPersonalityModel"];
        var trait = ffm.GetTrait("Nonexistent");
        Assert.IsNull(trait);
    }

    [Test]
    public void MBTI_GetDominantTrait_ReturnsCorrectDominance()
    {
        var mbti = personality.PersonalityModels["MBTIPersonalityModel"];
        mbti.AddTrait("EI");
        mbti.SetTraitValue("EI", 0.4f);

        var dominant = ((MBTIPersonalityModel)mbti).GetDominantTrait("EI");
        Assert.AreEqual("Extraversion", dominant);
    }

    [Test]
    public void Trait_Extraversion_ComputesCorrectWeightedValue()
    {
        BuildPersonalityTraitModel();
        Trait extraversion = personality.PersonalityModels["FFMPersonalityModel"].GetTrait("Extraversion");

        // Confident:   0.3 * 0.7 = 0.21
        // Outgoing:    0.6 * 0.8 = 0.48
        // Independent: 0.1 * 0.9 = 0.09
        float expectedExtraversion = 0.21f + 0.48f + 0.09f;

        float computedExtraversion = extraversion.ComputeTraitValue();
        Assert.AreEqual(expectedExtraversion, computedExtraversion, 0.001f, "Extraversion computed value should match weighted sum of sub-traits");
    }

    [Test]
    public void Trait_EI_ComputesCorrectWeightedValue()
    {
        BuildPersonalityTraitModel();
        Trait ei = personality.PersonalityModels["MBTIPersonalityModel"].GetTrait("EI");

        // Open:      -0.1 * 0.5 = -0.05
        // Confident: -0.4 * 0.7 = -0.28
        // Outgoing:  -0.5 * 0.8 = -0.40
        float expectedEI = -0.05f - 0.28f - 0.40f;

        float computedEI = ei.ComputeTraitValue();
        Assert.AreEqual(expectedEI, computedEI, 0.001f, "EI computed value should match weighted sum of sub-traits");
    }
}