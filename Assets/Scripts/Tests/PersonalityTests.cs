using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using PersonalitySystemModels;
using AspectSystem;

public class PersonalityTests
{
    private MBTIPersonalityModel mbtiModel;
    private FFMPersonalityModel ffmModel;

    [SetUp]
    public void SetUp()
    {
        mbtiModel = new MBTIPersonalityModel();
        ffmModel = new FFMPersonalityModel();
    }

    // MBTI PERSONALITY MODEL TESTS

    [Test]
    public void MBTI_CanAddAndRetrieveTrait()
    {
        mbtiModel.AddTrait("EI");
        Assert.IsNotNull(mbtiModel.GetTrait("EI"));
    }

    [Test]
    public void MBTI_TraitSelection_WorksProperly()
    {
        mbtiModel.AddTrait("EI");
        mbtiModel.SetTraitValue("EI", 0.7f);
        Assert.AreEqual("Extraversion", mbtiModel.GetDominantTrait("EI"));

        mbtiModel.SetTraitValue("EI", -0.7f);
        Assert.AreEqual("Introversion", mbtiModel.GetDominantTrait("EI"));
    }

    [Test]
    public void MBTI_TraitValue_ClampsProperly()
    {
        mbtiModel.AddTrait("EI");
        mbtiModel.SetTraitValue("EI", 2.0f);
        Assert.AreEqual("Extraversion", mbtiModel.GetDominantTrait("EI"));

        mbtiModel.SetTraitValue("EI", -2.0f);
        Assert.AreEqual("Introversion", mbtiModel.GetDominantTrait("EI"));
    }

    [Test]
    public void MBTI_Trait_StoresDirectValueCorrectly()
    {
        mbtiModel.SetTraitValue("EI", 0.8f);
        Assert.AreEqual(0.8f, mbtiModel.GetTraitValue("EI"));
    }

    [Test]
    public void MBTI_InvalidTraitSelected_DoesNotCrash()
    {
        Assert.DoesNotThrow(() => mbtiModel.SetTraitValue("XYZ", 0.5f));

        string result = mbtiModel.GetDominantTrait("XYZ");
        Assert.AreEqual("Unknown", result);
    }

    [Test]
    public void MBTI_CanAddSubTrait()
    {
        mbtiModel.AddTrait("EI");
        Trait parentTrait = mbtiModel.GetTrait("EI");

        mbtiModel.AddSubTrait(parentTrait, "Outgoing", 0.7f);
        Assert.IsNotNull(mbtiModel.GetTrait("Outgoing"));
    }

    [Test]
    public void MBTI_CanHaveMultipleSubTraits()
    {
        mbtiModel.AddTrait("EI");
        Trait parentTrait = mbtiModel.GetTrait("EI");

        mbtiModel.AddSubTrait(parentTrait, "Outgoing", 0.7f);
        mbtiModel.AddSubTrait(parentTrait, "Reserved", 0.3f);

        Assert.IsNotNull(mbtiModel.GetTrait("Outgoing"));
        Assert.IsNotNull(mbtiModel.GetTrait("Reserved"));
    }

    [Test]
    public void MBTI_SubTraits_CanHaveSubSubTraits()
    {
        mbtiModel.AddTrait("EI");
        Trait parentTrait = mbtiModel.GetTrait("EI");

        mbtiModel.AddSubTrait(parentTrait, "Outgoing", 0.7f);
        Trait outgoingTrait = mbtiModel.GetTrait("Outgoing");

        mbtiModel.AddSubTrait(outgoingTrait, "Charismatic", 0.5f);
        Trait charismaticTrait = mbtiModel.GetTrait("Charismatic");

        mbtiModel.AddSubTrait(charismaticTrait, "Persuasive", 0.5f);

        Assert.IsNotNull(mbtiModel.GetTrait("Charismatic"));
        Assert.IsNotNull(mbtiModel.GetTrait("Persuasive"));
    }

    [Test]
    public void MBTI_SubTrait_ContributesToParentCorrectly()
    {
        mbtiModel.AddTrait("EI");
        Trait eiTrait = mbtiModel.GetTrait("EI");

        mbtiModel.AddSubTrait(eiTrait, "Outgoing", 0.7f);
        mbtiModel.SetTraitValue("Outgoing", 0.8f);

        mbtiModel.AddSubTrait(eiTrait, "Independent", 0.3f);
        mbtiModel.SetTraitValue("Independent", 0.9f);

        // Expected: (0.8 * 0.7) + (0.9 * 0.3) = 0.83
        Assert.AreEqual(0.83f, mbtiModel.GetTraitValue("EI"));
    }

    [Test]
    public void MBTI_NestedSubTraits_ContributeCorrectly()
    {
        mbtiModel.AddTrait("EI");
        Trait ei = mbtiModel.GetTrait("EI");

        mbtiModel.AddSubTrait(ei, "Outgoing", 0.5f);
        mbtiModel.AddSubTrait(ei, "Assertive", 0.3f);
        mbtiModel.AddSubTrait(ei, "Sociable", 0.2f);

        Trait outgoing = mbtiModel.GetTrait("Outgoing");
        Trait assertive = mbtiModel.GetTrait("Assertive");
        Trait sociable = mbtiModel.GetTrait("Sociable");

        mbtiModel.AddSubTrait(outgoing, "Charismatic", 0.75f);
        mbtiModel.AddSubTrait(outgoing, "Independent", 0.25f);

        mbtiModel.SetTraitValue("Outgoing", 0.6f); // This value should be ignored
        mbtiModel.SetTraitValue("Assertive", 0.3f);   
        mbtiModel.SetTraitValue("Sociable", 0.9f);    
        mbtiModel.SetTraitValue("Charismatic", 0.4f);  
        mbtiModel.SetTraitValue("Independent", 0.2f);

        // Sub-sub trait calculation: (0.4 * 0.75) + (0.2 * 0.25) = 0.35
        // Sub-trait calculation: (0.6 * 0.5) + (0.3 * 0.3) + (0.9 * 0.2) = 0.445
        // Final trait value: 0.445
        float actual = mbtiModel.GetTraitValue("EI");
        Assert.AreEqual(0.445f, actual, 0.01f, $"Expected EI to be 0.445 but got {actual}");
    }

    [Test]
    public void MBTI_MultipleSubTraits_ContributeCorrectly()
    {
        mbtiModel.AddTrait("EI");
        Trait eiTrait = mbtiModel.GetTrait("EI");

        mbtiModel.AddSubTrait(eiTrait, "Outgoing", 0.5f);
        mbtiModel.AddSubTrait(eiTrait, "Reserved", 0.5f);
        mbtiModel.SetTraitValue("Outgoing", 0.6f);
        mbtiModel.SetTraitValue("Reserved", 0.4f);

        Assert.AreEqual(0.5f, mbtiModel.GetTraitValue("EI"));
    }

    [Test]
    public void MBTI_GetDominantTrait_ReturnsCorrectDominantType()
    {
        mbtiModel.AddTrait("EI");
        mbtiModel.AddTrait("SN");
        mbtiModel.AddTrait("TF");
        mbtiModel.AddTrait("JP");

        mbtiModel.SetTraitValue("EI", 0.6f);  
        mbtiModel.SetTraitValue("SN", -0.9f);  
        mbtiModel.SetTraitValue("TF", 0.0f);  
        mbtiModel.SetTraitValue("JP", -0.1f); 

        Assert.AreEqual("Extraversion", mbtiModel.GetDominantTrait("EI"));
        Assert.AreEqual("Intuition", mbtiModel.GetDominantTrait("SN"));
        Assert.AreEqual("Thinking", mbtiModel.GetDominantTrait("TF"));
        Assert.AreEqual("Perceiving", mbtiModel.GetDominantTrait("JP"));
    }

    // FFM PERSONALITY MODEL TESTS

    [Test]
    public void FFM_CanAddAndRetrieveTrait()
    {
        ffmModel.AddTrait("Extraversion");
        Assert.IsNotNull(ffmModel.GetTrait("Extraversion"));
    }

    [Test]
    public void FFM_TraitValue_ClampsProperly()
    {
        ffmModel.AddTrait("Extraversion");
        ffmModel.SetTraitValue("Extraversion", 1.5f);
        Assert.AreEqual(1.0f, ffmModel.GetTraitValue("Extraversion"));

        ffmModel.SetTraitValue("Extraversion", -1.5f);
        Assert.AreEqual(-1.0f, ffmModel.GetTraitValue("Extraversion"));
    }

    [Test]
    public void FFM_InvalidTraitSelected_DoesNotCrash()
    {
        Assert.DoesNotThrow(() => ffmModel.SetTraitValue("XYZ", 0.5f));

        float result = ffmModel.GetTraitValue("XYZ");
        Assert.AreEqual(0f, result);
    }

    [Test]
    public void FFM_TraitSelection_WorksProperly()
    {
        ffmModel.AddTrait("Extraversion");
        ffmModel.SetTraitValue("Extraversion", 0.8f);
        Assert.AreEqual(0.8f, ffmModel.GetTraitValue("Extraversion"));
    }

    [Test]
    public void FFM_Trait_StoresDirectValueCorrectly()
    {
        ffmModel.AddTrait("Extraversion");
        ffmModel.SetTraitValue("Extraversion", 0.8f);
        Assert.AreEqual(0.8f, ffmModel.GetTraitValue("Extraversion"));
    }

    [Test]
    public void FFM_CanAddSubTrait()
    {
        ffmModel.AddTrait("Extraversion");
        Trait parentTrait = ffmModel.GetTrait("Extraversion");

        ffmModel.AddSubTrait(parentTrait, "Outgoing", 0.7f);
        Assert.IsNotNull(ffmModel.GetTrait("Outgoing"));
    }

    [Test]
    public void FFM_CanHaveMultipleSubTraits()
    {
        ffmModel.AddTrait("Extraversion");
        Trait parentTrait = ffmModel.GetTrait("Extraversion");

        if (parentTrait != null)
        {
            ffmModel.AddSubTrait(parentTrait, "Outgoing", 0.7f);
            ffmModel.AddSubTrait(parentTrait, "Independent", 0.3f);

            Assert.AreEqual(0.0f, ffmModel.GetTraitValue("Outgoing"));
            Assert.AreEqual(0.0f, ffmModel.GetTraitValue("Independent"));
        }
        else
        {
            Assert.Fail("Parent trait 'Extraversion' not found.");
        }
    }

    [Test]
    public void FFM_SubTraits_CanHaveSubSubTraits()
    {
        ffmModel.AddTrait("Extraversion");
        Trait parentTrait = ffmModel.GetTrait("Extraversion");
        if (parentTrait != null)
        {
            ffmModel.AddSubTrait(parentTrait, "Outgoing", 0.7f);
            Trait outgoingTrait = ffmModel.GetTrait("Outgoing");

            if (outgoingTrait != null)
            {
                ffmModel.AddSubTrait(outgoingTrait, "Charismatic", 0.5f);
                Trait charismaticTrait = ffmModel.GetTrait("Charismatic");

                if (charismaticTrait != null)
                {
                    ffmModel.AddSubTrait(charismaticTrait, "Persuasive", 0.5f);
                }
            }

            Assert.AreEqual(0.0f, ffmModel.GetTraitValue("Charismatic"));
            Assert.AreEqual(0.0f, ffmModel.GetTraitValue("Persuasive"));
        }
        else
        {
            Assert.Fail("Parent trait 'Extraversion' not found.");
        }
    }

    [Test]
    public void FFM_MultipleSubTraits_ContributeCorrectly()
    {
        ffmModel.AddTrait("Extraversion");
        Trait extraversion = ffmModel.GetTrait("Extraversion");
        ffmModel.AddSubTrait(extraversion, "Outgoing", 0.5f);
        ffmModel.AddSubTrait(extraversion, "Assertive", 0.5f);
        ffmModel.SetTraitValue("Outgoing", 0.6f);
        ffmModel.SetTraitValue("Assertive", 0.4f);

        // Expected: (0.6 * 0.5) + (0.4 * 0.5) = 0.5
        Assert.AreEqual(0.5f, ffmModel.GetTraitValue("Extraversion"));
    }

    [Test]
    public void FFM_SubTrait_ContributesToParentCorrectly()
    {
        ffmModel.AddTrait("Extraversion");
        Trait extraversion = ffmModel.GetTrait("Extraversion");

        ffmModel.AddSubTrait(extraversion, "Outgoing", 0.7f);
        ffmModel.AddSubTrait(extraversion, "Independent", 0.3f);

        ffmModel.SetTraitValue("Outgoing", 0.8f);
        ffmModel.SetTraitValue("Independent", 0.9f);

        // Expected: (0.8 * 0.7) + (0.9 * 0.3) = 0.83
        Assert.AreEqual(0.83f, ffmModel.GetTraitValue("Extraversion"));
    }

    [Test]
    public void FFM_NestedSubTraits_ContributeCorrectly()
    {
        ffmModel.AddTrait("Extraversion");
        Trait extraversion = ffmModel.GetTrait("Extraversion");

        ffmModel.AddSubTrait(extraversion, "Outgoing", 0.7f);

        Trait outgoing = ffmModel.GetTrait("Outgoing");
        ffmModel.AddSubTrait(outgoing, "Charismatic", 0.5f, 0.4f);
        ffmModel.AddSubTrait(outgoing, "Independent", 0.3f, 0.9f);

        // Expected outgoing: (0.4 * 0.5) + (0.9 * 0.3) = 0.47
        // Expected extraversion: 0.47 * 0.7 (weight of Outgoing) = 0.329
        Assert.AreEqual(0.329f, ffmModel.GetTraitValue("Extraversion"));
    }
}
