using System;
using System.Collections.Generic;
using UnityEngine;

namespace PersonalitySystemModels
{
    public class FFMPersonalityModel : PersonalityModel
    {
        public FFMPersonalityModel() : base("FFMPersonalityModel") { }

        protected override Dictionary<string, Trait> GenerateCoreTraits()
        {
            return new Dictionary<string, Trait>
            {
                { "Openness", new Trait("Openness") },
                { "Conscientiousness", new Trait("Conscientiousness") },
                { "Extraversion", new Trait("Extraversion") },
                { "Agreeableness", new Trait("Agreeableness") },
                { "Neuroticism", new Trait("Neuroticism") }
            };
        }
    }
}