using System;
using System.Collections.Generic;

public class EkmanEmotionModel : EmotionModel
{
    public EkmanEmotionModel() : base("EkmanEmotionModel") { }

    protected override Dictionary<string, Trait> GenerateCoreTraits()
    {
        return new Dictionary<string, Trait>
        {
            { "Joy", new Trait("Joy") },
            { "Sadness", new Trait("Sadness") },
            { "Anger", new Trait("Anger") },
            { "Fear", new Trait("Fear") },
            { "Disgust", new Trait("Disgust") },
            { "Surprise", new Trait("Surprise") }
        };
    }
}