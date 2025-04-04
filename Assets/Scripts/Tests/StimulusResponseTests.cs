using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using StimulusResponseSystem;
using SocialSystem;
using CharacterSystem;
using AspectSystem;
using PersonalitySystemModels;
using AgentSystem;
using GroupSystem;

public class StimulusResponseTests
{
    private CharacterManager npc;
    private StimulusRelevanceFilter relevanceFilter;
    private StimulusDispatcher dispatcher;
    private DecisionMaker decisionMaker;

    private FFMPersonalityModel ffm;
    private EkmanEmotionModel emotion;

    [SetUp]
    public void Setup()
    {
        npc = new CharacterManager("Bob");
        npc.RoleManager.AddRole(new Role("Citizen", npc.Character));
        
        var ffmModel = new FFMPersonalityModel();
        var ekmanModel = new EkmanEmotionModel(); 

        // Register aspects
        npc.Character.AddAspect("Personality", new PersonalityAspect(new List<AspectModel> { ffmModel }));
        npc.Character.AddAspect("Emotion", new EmotionAspect(new List<AspectModel> { ekmanModel }));

        ffm = npc.Character.GetAspect("Personality")?.GetModel<FFMPersonalityModel>();
        emotion = npc.Character.GetAspect("Emotion")?.GetModel<EkmanEmotionModel>();
        
        ffm?.AddTrait("Extraversion");
        ffm?.SetTraitValue("Extraversion", 1.0f);

        relevanceFilter = new StimulusRelevanceFilter(npc);
        dispatcher = new StimulusDispatcher();
        decisionMaker = new DecisionMaker(npc, npc.Character, ekmanModel.ModelName);
    }

    // STIMULUS TESTS

    [Test]
    public void Stimulus_Correctly_Stores_All_Parameters()
    {
        Vector3 position = new Vector3(5, 0, 5);
        Stimulus stimulus = new Stimulus("TestStimulus", StimulusType.Emotional | StimulusType.Social, 0.8f, "Player", StimulusPropagation.Local, 10f, position);

        Assert.AreEqual("TestStimulus", stimulus.Name);
        Assert.AreEqual(StimulusType.Emotional | StimulusType.Social, stimulus.Type);
        Assert.AreEqual(0.8f, stimulus.Magnitude);
        Assert.AreEqual("Player", stimulus.Source);
        Assert.AreEqual(StimulusPropagation.Local, stimulus.Propagation);
        Assert.AreEqual(10f, stimulus.FalloffRadius);
        Assert.AreEqual(position, stimulus.Position);
    }

    [Test]
    public void Stimulus_IsEmotional_And_IsSocial_Detection_Works()
    {
        Stimulus stimulus = new Stimulus("MixedStimulus", StimulusType.Emotional | StimulusType.Social, 0.5f, "Event", StimulusPropagation.Direct, 0f, Vector3.zero);

        Assert.IsTrue(stimulus.IsEmotional(), "Stimulus should be recognized as Emotional");
        Assert.IsTrue(stimulus.IsSocial(), "Stimulus should be recognized as Social");

        Stimulus onlyEmotional = new Stimulus("EmotionStimulus", StimulusType.Emotional, 0.5f, "Event", StimulusPropagation.Direct, 0f, Vector3.zero);
        Assert.IsTrue(onlyEmotional.IsEmotional());
        Assert.IsFalse(onlyEmotional.IsSocial());

        Stimulus onlySocial = new Stimulus("SocialStimulus", StimulusType.Social, 0.5f, "Event", StimulusPropagation.Direct, 0f, Vector3.zero);
        Assert.IsFalse(onlySocial.IsEmotional());
        Assert.IsTrue(onlySocial.IsSocial());
    }

    [Test]
    public void Stimulus_Clamps_Magnitude_To_One()
    {
        Stimulus stimulus = new Stimulus("OverpoweredStimulus", StimulusType.Emotional, 5f, "Event", StimulusPropagation.Direct, 0f, Vector3.zero);
        Assert.AreEqual(1f, stimulus.Magnitude, "Stimulus magnitude should be clamped to 1");
    }

    // BEHAVIOR TESTS

    [Test]
    public void Behavior_Stores_Name_Correctly()
    {
        Behavior behavior = new Behavior("Smile");
        Assert.AreEqual("Smile", behavior.Name);
    }

    [Test]
    public void Behavior_Without_Roles_Is_Universal()
    {
        Behavior behavior = new Behavior("Wave");
        Assert.IsNotNull(behavior.AssociatedRoles);
        Assert.AreEqual(0, behavior.AssociatedRoles.Count, "Should be valid for any role");
    }

    [Test]
    public void Behavior_With_Roles_Assigns_Correctly()
    {
        List<Role> roles = new List<Role>
        {
            new Role("Friend", null),
            new Role("Colleague", null)
        };

        Behavior behavior = new Behavior("Help", roles);

        Assert.AreEqual(2, behavior.AssociatedRoles.Count);
        Assert.IsTrue(behavior.AssociatedRoles.Exists(r => r.RoleName == "Friend"));
        Assert.IsTrue(behavior.AssociatedRoles.Exists(r => r.RoleName == "Colleague"));
    }

    // RELEVANCE FILTER TESTS

    [Test]
    public void RelevanceFilter_Rejects_Stimulus_Too_Far()
    {
        Stimulus stimulus = new Stimulus("TestStimulus", StimulusType.Emotional, 1f, "TestSource", StimulusPropagation.Local, 5f, Vector3.zero);
        npc.Character.Position = new Vector3(10f, 0, 0);
        bool relevant = relevanceFilter.IsRelevant(stimulus, out float priority);
        Assert.IsFalse(relevant);
    }

    [Test]
    public void RelevanceFilter_Accepts_Stimulus_In_Range()
    {
        Stimulus stimulus = new Stimulus("TestStimulus", StimulusType.Emotional, 1f, "TestSource", StimulusPropagation.Local, 10f, Vector3.zero);
        npc.Character.Position = new Vector3(3f, 0, 0);
        bool relevant = relevanceFilter.IsRelevant(stimulus, out float priority);
        Assert.IsTrue(relevant);
    }

    [Test]
    public void RelevanceFilter_Accepts_Direct_Stimulus_Regardless_Of_Distance()
    {
        Stimulus stimulus = new Stimulus("TestStimulus", StimulusType.Emotional, 1f, "TestSource", StimulusPropagation.Direct, 0f, Vector3.zero);
        npc.Character.Position = new Vector3(100f, 0, 0);

        bool relevant = relevanceFilter.IsRelevant(stimulus, out float remaining);

        Assert.IsTrue(relevant);
        Assert.AreEqual(1f, remaining);
    }

    [Test]
    public void RelevanceFilter_Accepts_Broadcast_Stimulus()
    {
        Stimulus stimulus = new Stimulus("TestStimulus", StimulusType.Social, 0.8f, "TestSource", StimulusPropagation.Broadcast, 0f, Vector3.zero);
        npc.Character.Position = new Vector3(100f, 0, 0);

        bool relevant = relevanceFilter.IsRelevant(stimulus, out float remaining);

        Assert.IsTrue(relevant);
        Assert.AreEqual(0.8f, remaining);
    }

    // STIMULUS DISPATCHER TESTS

    [Test]
    public void StimulusDispatcher_Returns_Behaviors_For_Known_Stimulus()
    {
        Stimulus stimulus = new Stimulus("Compliment", StimulusType.Social, 1f, "TestSource", StimulusPropagation.Direct, 0f, Vector3.zero);
        List<Behavior> behaviors = dispatcher.GetCandidateBehaviors(stimulus);

        Assert.IsNotNull(behaviors);
        Assert.IsTrue(behaviors.Count > 0, "Dispatcher should provide at least one behavior for known stimulus.");
    }

    [Test]
    public void StimulusDispatcher_Returns_Empty_For_Unknown_Stimulus()
    {
        Stimulus stimulus = new Stimulus("UnknownStimulus", StimulusType.Social, 1f, "TestSource", StimulusPropagation.Direct, 0f, Vector3.zero);
        List<Behavior> behaviors = dispatcher.GetCandidateBehaviors(stimulus);

        Assert.IsNotNull(behaviors);
        Assert.AreEqual(0, behaviors.Count, "Dispatcher should return an empty list for unknown stimulus.");
    }

    // DECISION MAKER TESTS

    [Test]
    public void DecisionMaker_Ignores_Stimulus_When_Magnitude_Is_Zero()
    {
        var stimulus = new Stimulus("TestStimulus", StimulusType.Emotional, 0f, "TestSource", StimulusPropagation.Direct, 0f, Vector3.zero);
        var effects = new List<StimulusEffect>
        {
            new StimulusEffect(AffectedSystem.Emotion, "Joy", 0.5f)
        };

        var behavior = decisionMaker.ProcessStimulus(stimulus, effects, new List<Behavior> { new Behavior("Smile") }, 0f);

        Assert.IsNull(behavior);
    }

    [Test]
    public void DecisionMaker_Rejects_When_No_Behaviors_Available()
    {
        var stimulus = new Stimulus("Compliment", StimulusType.Social, 1f, "TestSource", StimulusPropagation.Direct, 0f, Vector3.zero);
        var effects = new List<StimulusEffect>
        {
            new StimulusEffect(AffectedSystem.Emotion, "Joy", 0.5f)
        };

        var behavior = decisionMaker.ProcessStimulus(stimulus, effects, new List<Behavior>(), 1f);

        Assert.IsNull(behavior);
    }

    [Test]
    public void DecisionMaker_Updates_Emotion_When_Effect_Applied()
    {
        emotion?.AddTrait("Joy");
        emotion?.SetTraitValue("Joy", 0f);

        var stimulus = new Stimulus("Compliment", StimulusType.Social, 1f, "TestSource", StimulusPropagation.Direct, 0f, Vector3.zero);

        var effects = new List<StimulusEffect>
        {
            new StimulusEffect(AffectedSystem.Emotion, "Joy", 0.5f)
        };

        var behaviors = new List<Behavior>
        {
            new Behavior("Smile")
        };

        float before = emotion?.GetTraitValue("Joy") ?? 0f;

        decisionMaker.ProcessStimulus(stimulus, effects, behaviors, 1f);

        float after = emotion?.GetTraitValue("Joy") ?? 0f;

        Assert.Greater(after, before, "Joy value should have increased.");
    }

    [Test]
    public void DecisionMaker_Selects_High_Extraversion_Behavior()
    {
        var stimulus = new Stimulus("Compliment", StimulusType.Social, 1f, "TestSource", StimulusPropagation.Direct, 0f, Vector3.zero);

        var effects = new List<StimulusEffect>
        {
            new StimulusEffect(AffectedSystem.Emotion, "Joy", 0.5f)
        };

        var behaviors = new List<Behavior>
        {
            new Behavior("Smile"),
            new Behavior("Withdraw")
        };

        int smileCount = 0;
        for (int i = 0; i < 20; i++) // Repeat more than once to see average behavior
        {
            var result = decisionMaker.ProcessStimulus(stimulus, effects, behaviors, 1f);
            if (result != null && result.Name == "Smile")
                smileCount++;
        }

        Assert.Greater(smileCount, 10, "Extraverted NPC should favor 'Smile' more than once.");
    }

    [Test]
    public void DecisionMaker_Applies_EmotionEffect_When_Trait_Exists()
    {
        emotion?.AddTrait("Joy");
        emotion?.SetTraitValue("Joy", 0f);

        var effect = new StimulusEffect(AffectedSystem.Emotion, "Joy", 0.5f);
        decisionMaker.ProcessStimulus(new Stimulus("Stimulus", StimulusType.Emotional, 1f, "TestSource", StimulusPropagation.Direct, 0f, Vector3.zero, null), 
                                    new List<StimulusEffect> { effect }, new List<Behavior>(), 1f);

        Assert.Greater(emotion?.GetTraitValue("Joy") ?? 0f, 0f);
    }

    [Test]
    public void DecisionMaker_DoesNothing_When_EmotionTrait_DoesNotExist()
    {
        var effect = new StimulusEffect(AffectedSystem.Emotion, "UnknownTrait", 0.5f);
        Assert.DoesNotThrow(() =>
        {
            decisionMaker.ProcessStimulus(new Stimulus("Stimulus", StimulusType.Emotional, 1f, "TestSource", StimulusPropagation.Direct, 0f, Vector3.zero, null), 
                                        new List<StimulusEffect> { effect }, new List<Behavior>(), 1f);
        });
    }

    [Test]
    public void DecisionMaker_Modifies_Relationship_With_TargetCharacter()
    {
        var sue = new Character("Sue");
        npc.RelationshipManager.AddRelationship(sue);
        var rel = npc.RelationshipManager.GetRelationship(sue);
        rel.SetDominance(0f);

        var effect = new StimulusEffect(AffectedSystem.Relationship, "Dominance", 0.5f, targetCharacter: sue);
        decisionMaker.ProcessStimulus(new Stimulus("Stimulus", StimulusType.Social, 1f, "TestSource", StimulusPropagation.Direct, 0f, Vector3.zero, null),
                                    new List<StimulusEffect> { effect }, new List<Behavior>(), 1f);

        Assert.Greater(rel.Dominance, 0f);
    }

    [Test]
    public void DecisionMaker_DoesNothing_When_NoRelationship_To_TargetCharacter()
    {
        var sue = new Character("Sue");

        var effect = new StimulusEffect(AffectedSystem.Relationship, "Dominance", 0.5f, targetCharacter: sue);
        Assert.DoesNotThrow(() =>
        {
            decisionMaker.ProcessStimulus(new Stimulus("Stimulus", StimulusType.Social, 1f, "TestSource", StimulusPropagation.Direct, 0f, Vector3.zero, null),
                                        new List<StimulusEffect> { effect }, new List<Behavior>(), 1f);
        });
    }

    [Test]
    public void DecisionMaker_Modifies_Relationships_In_TargetGroup()
    {
        var group = new GroupSystem.Group("TestGroup");
        var sue = new Character("Sue");
        npc.RelationshipManager.AddRelationship(sue);
        group.AddMember(sue);

        var effect = new StimulusEffect(AffectedSystem.Relationship, "Agreeableness", 0.5f, targetGroup: group);
        decisionMaker.ProcessStimulus(new Stimulus("Stimulus", StimulusType.Social, 1f, "TestSource", StimulusPropagation.Broadcast, 0f, Vector3.zero, "TestGroup"),
                                    new List<StimulusEffect> { effect }, new List<Behavior>(), 1f);

        var rel = npc.RelationshipManager.GetRelationship(sue);
        Assert.Greater(rel.Agreeableness, 0f);
    }

    [Test]
    public void DecisionMaker_GroupEffect_Skips_When_NoRelationships()
    {
        var group = new GroupSystem.Group("TestGroup");
        var sue = new Character("Sue");
        group.AddMember(sue); // but no relationship is added

        var effect = new StimulusEffect(AffectedSystem.Relationship, "Agreeableness", 0.5f, targetGroup: group);
        Assert.DoesNotThrow(() =>
        {
            decisionMaker.ProcessStimulus(new Stimulus("Stimulus", StimulusType.Social, 1f, "TestSource", StimulusPropagation.Broadcast, 0f, Vector3.zero, "TestGroup"),
                                        new List<StimulusEffect> { effect }, new List<Behavior>(), 1f);
        });
    }

    [Test]
    public void DecisionMaker_Selects_Behavior_When_Role_Matches()
    {
        var matchingBehavior = new Behavior("Defend", new List<Role> { new Role("Citizen", npc.Character) });
        var behaviors = new List<Behavior> { matchingBehavior };
        var effects = new List<StimulusEffect> { new StimulusEffect(AffectedSystem.Emotion, "Joy", 0.1f) };

        var selected = decisionMaker.ProcessStimulus(
            new Stimulus("Stimulus", StimulusType.Social, 1f, "TestSource", StimulusPropagation.Direct, 0f, Vector3.zero, null),
            effects, behaviors, 1f);

        Assert.IsNotNull(selected);
        Assert.AreEqual("Defend", selected.Name);
    }

    [Test]
    public void DecisionMaker_Ignores_Behavior_When_Role_Does_Not_Match()
    {
        var nonMatchingBehavior = new Behavior("Defend", new List<Role> { new Role("Guard", npc.Character) });
        var behaviors = new List<Behavior> { nonMatchingBehavior };

        var selected = decisionMaker.ProcessStimulus(new Stimulus("Stimulus", StimulusType.Social, 1f, "TestSource", StimulusPropagation.Direct, 0f, Vector3.zero, null),
                                                    new List<StimulusEffect>(), behaviors, 1f);

        Assert.IsNull(selected);
    }

    [Test]
    public void DecisionMaker_Adjusts_Goal_Priority_When_Goal_Exists()
    {
        var role = npc.RoleManager.GetRole("Citizen");
        var goal = new RoleGoal("SecureFood", 0.5f);
        role.AddGoal(goal);

        var stimulus = new Stimulus("FoodShortage", StimulusType.Social, 1f, "Environment", StimulusPropagation.Direct, 0f, Vector3.zero, null);
        var effects = new List<StimulusEffect>
        {
            new StimulusEffect(AffectedSystem.GoalPlanning, "SecureFood", 0.3f)
        };

        decisionMaker.ProcessStimulus(stimulus, effects, new List<Behavior>(), 1f);

        Assert.AreEqual(0.8f, goal.Priority, 0.001f);
    }

    [Test]
    public void DecisionMaker_Does_Not_Change_Goal_If_Not_Present()
    {
        var role = new Role("Citizen", npc.Character);
        role.AddGoal(new RoleGoal("BuildShelter", 0.5f));
        npc.RoleManager.AddRole(role);

        var stimulus = new Stimulus("FoodShortage", StimulusType.Social, 1f, "Environment", StimulusPropagation.Direct, 0f, Vector3.zero, null);
        var effects = new List<StimulusEffect>
        {
            new StimulusEffect(AffectedSystem.GoalPlanning, "SecureFood", 0.5f)
        };

        decisionMaker.ProcessStimulus(stimulus, effects, new List<Behavior>(), 1f);

        Assert.AreEqual(0.5f, role.Goals[0].Priority, 0.001f);
    }

    [Test]
    public void DecisionMaker_Ignores_GoalStimulus_When_No_Role()
    {
        var stimulus = new Stimulus("FoodShortage", StimulusType.Social, 1f, "Environment", StimulusPropagation.Direct, 0f, Vector3.zero, null);
        var effects = new List<StimulusEffect>
        {
            new StimulusEffect(AffectedSystem.GoalPlanning, "SecureFood", 0.5f)
        };

        Assert.DoesNotThrow(() => decisionMaker.ProcessStimulus(stimulus, effects, new List<Behavior>(), 1f));
    }
}