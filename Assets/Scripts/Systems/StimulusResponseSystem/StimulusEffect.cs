using UnityEngine;
using CharacterSystem;
using GroupSystem;
using SocialSystem;

public enum AffectedSystem
{
    Emotion,
    Relationship,
    GoalPlanning
}

public class StimulusEffect
{
    public AffectedSystem AffectedSystem { get; }
    public RoleGoal AffectedGoal { get; }
    public float BaseMagnitude { get; }

    // If character has a specific trait specified by the modulator, it may affect the magnitude more
    public string PersonalityModulator { get; }
    public float ModulationStrength { get; }

    // For the emotion system
    public string AffectedTrait { get; }
    public string TargetGroup { get; }

    // Person or group affected
    public Character TargetCharacter;
    public Group TargetGroupObject;

    public StimulusEffect(
        AffectedSystem aspect,
        string trait,
        float baseMagnitude,
        string personalityModulator = null,
        float modulationStrength = 0f,
        Character targetCharacter = null,
        Group targetGroup = null,
        RoleGoal affectedGoal = null)
    {
        AffectedSystem = aspect;
        AffectedTrait = trait;
        BaseMagnitude = baseMagnitude;
        PersonalityModulator = personalityModulator;
        ModulationStrength = modulationStrength;
        TargetCharacter = targetCharacter;
        TargetGroupObject = targetGroup;
        AffectedGoal = affectedGoal;
    }
}