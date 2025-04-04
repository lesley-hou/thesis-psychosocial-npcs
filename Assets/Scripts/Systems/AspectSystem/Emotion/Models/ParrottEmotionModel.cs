using System;
using System.Collections.Generic;

public class ParrottEmotionModel : EmotionModel
{
    public ParrottEmotionModel() : base("ParrottEmotionModel") { }

    private static readonly Dictionary<string, HashSet<string>> ParrottHierarchy = new()
    {

        // Primary to secondary mapping
        ["Love"] = new() { "Affection", "Lust", "Longing" },
        ["Joy"] = new() { "Cheerfulness", "Zest", "Contentment", "Pride", "Optimism", "Enthrallment", "Relief" },
        ["Surprise"] = new() { "Surprise" },
        ["Anger"] = new() { "Irritability", "Exasperation", "Rage", "Disgust", "Envy", "Torment" },
        ["Sadness"] = new() { "Suffering", "Sadness", "Disappointment", "Shame", "Neglect", "Sympathy" },
        ["Fear"] = new() { "Horror", "Nervousness" },

        // Secondary to tertiary mapping
        ["Affection"] = new() { "Adoration", "Fondness", "Liking", "Attractiveness", "Caring", "Tenderness", "Compassion", "Sentimentality" },
        ["Lust"] = new() { "Desire", "Passion", "Infatuation" },
        ["Longing"] = new() { "Longing" },

        ["Cheerfulness"] = new() { "Amusement", "Bliss", "Gaiety", "Glee", "Jolliness", "Joviality", "Joy", "Delight", "Enjoyment", "Gladness", "Happiness", "Jubilation", "Elation", "Satisfaction", "Ecstacy", "Euphoria" },
        ["Zest"] = new() { "Enthusiasm", "Zeal", "Excitement", "Thrill", "Exhilaration" },
        ["Contentment"] = new() { "Pleasure" },
        ["Pride"] = new() { "Triumph" },
        ["Optimism"] = new() { "Hope", "Eagerness" },
        ["Enthrallment"] = new() { "Enthrallment", "Rapture" },
        ["Relief"] = new() { "Relief" },

        ["Surprise"] = new() { "Amazement", "Astonishment" },

        ["Irritability"] = new() { "Aggravation", "Agitation", "Annoyance", "Grouchy", "Grumpy", "Crosspatch" },
        ["Exasperation"] = new() { "Frustration" },
        ["Rage"] = new() { "Anger", "Outrage", "Fury", "Wrath", "Hostility", "Ferocity", "Bitter", "Hatred", "Scom", "Spite", "Vengefulness", "Dislike", "Resentment" },
        ["Disgust"] = new() { "Revulsion", "Loathing", "Contempt" },
        ["Envy"] = new() { "Jealousy" },
        ["Torment"] = new() { "Torment" },

        ["Suffering"] = new() { "Agony", "Hurt", "Anguish" },
        ["Sadness"] = new() { "Depression", "Despair", "Gloom", "Glumness", "Unhappy", "Grief", "Sorrow", "Woe", "Misery", "Melancholy" },
        ["Disappointment"] = new() { "Dismay", "Displeasure" },
        ["Shame"] = new() { "Guilt", "Regret", "Remorse" },
        ["Neglect"] = new() { "Alienation", "Defeat", "Dejection", "Embarrassment", "Homesickness", "Humiliation", "Insecurity", "Insult", "Isolation", "Loneliness", "Rejection" },
        ["Sympathy"] = new() { "Pity", "Sympathy" },

        ["Horror"] = new() { "Terror", "Horror", "Alarm", "Shock", "Fear", "Fright", "Panic", "Hysteria", "Mortification" },
        ["Nervousness"] = new() { "Anxiety", "Suspense", "Uneasiness", "Apprehension", "Worry", "Distress", "Dread" }
    };

    protected override Dictionary<string, Trait> GenerateCoreTraits() => new()
    {
        { "Anger", new Trait("Anger") },
        { "Love", new Trait("Love") },
        { "Fear", new Trait("Fear") },
        { "Joy", new Trait("Joy") },
        { "Sadness", new Trait("Sadness") },
        { "Surprise", new Trait("Surprise") }
    };

    public override bool AddSubTrait(Trait parentTrait, string subTraitName, float weight, float? initialValue = null)
    {
        if (parentTrait == null || !Traits.ContainsKey(parentTrait.Name))
        {
            Console.WriteLine($"[Parrott] Invalid parent trait.");
            return false;
        }

        if (!ParrottHierarchy.TryGetValue(parentTrait.Name, out var validSubTraits) || !validSubTraits.Contains(subTraitName))
        {
            Console.WriteLine($"[Parrott] '{subTraitName}' is not a valid sub-trait of '{parentTrait.Name}'");
            return false;
        }

        if (parentTrait.SubTraits.ContainsKey(subTraitName))
        {
            Console.WriteLine($"[Parrott] Sub-trait '{subTraitName}' already exists under '{parentTrait.Name}'.");
            return false;
        }

        float valueToUse = initialValue ?? 0f;
        var subTrait = new Trait(subTraitName, valueToUse, weight);

        parentTrait.SubTraits[subTraitName] = subTrait;
        Traits[subTraitName] = subTrait;

        return true;
    }
}