using System;
using System.Collections.Generic; 
using UnityEngine;

public class Trait
{
    public string Name { get; }
    public float Value { get; private set; }
    public float Weight { get; set; } = 1.0f;
    public Dictionary<string, Trait> SubTraits { get; } = new();

    public Trait(string name, float value = 0.0f, float weight = 1.0f)
    {
        Name = name;
        SetValue(value);
        Weight = Math.Clamp(weight, 0f, 1f);
    }

    public string GetName()
    {
        return Name;
    }

    public void SetValue(float newValue)
    {
        Value = Math.Clamp(newValue, -1.0f, 1.0f);
    }

    public float ComputeTraitValue()
    {
        if (SubTraits.Count == 0)
            return Value;

        float weightedSum = 0f;
        foreach (var sub in SubTraits.Values)
            weightedSum += sub.Weight * sub.ComputeTraitValue();

        return weightedSum;
    }
}