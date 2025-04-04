using UnityEngine;

[System.Flags]
public enum StimulusType
{
    None = 0,
    Emotional = 1 << 0,
    Social = 1 << 1
}

// Stimulus that can affect other actors in the game world
public enum StimulusPropagation
{
    Direct,   // Only affects involved actors
    Local,    // Affects actors in a radius
    Broadcast // Could spread socially (e.g., gossip)
}

public class Stimulus
{
    public string Name { get; }
    public StimulusType Type { get; }
    public float Magnitude { get; }
    public string Source { get; }
    public StimulusPropagation Propagation { get; }

    // Only if Witnessed/Broadcast
    public float FalloffRadius { get; }
    public Vector3 Position { get; }
    public string TargetGroup { get; }

    public Stimulus(string name, StimulusType type, float magnitude, string source,
        StimulusPropagation propagation, float falloffRadius, Vector3 position, string targetGroup = null)
    {
        Name = name;
        Type = type;
        Magnitude = Mathf.Clamp01(magnitude);
        Source = source;
        Propagation = propagation;
        FalloffRadius = falloffRadius;
        Position = position;
        TargetGroup = targetGroup;
    }

    public bool IsEmotional() => (Type & StimulusType.Emotional) != 0;
    public bool IsSocial() => (Type & StimulusType.Social) != 0;
}