using System;
using System.Collections.Generic;

namespace PersonalitySystemModels
{
    public class MBTIPersonalityModel : PersonalityModel
    {
        private Dictionary<string, Trait> TraitPairs;

        public MBTIPersonalityModel() : base("MBTIPersonalityModel")
        {
            TraitPairs = GenerateCoreTraits();
            foreach (var trait in TraitPairs)
            {
                Traits[trait.Key] = trait.Value; 
            }
        }

        protected override Dictionary<string, Trait> GenerateCoreTraits()
        {
            return new Dictionary<string, Trait>
            {
                { "EI", new Trait("EI") },
                { "SN", new Trait("SN") },
                { "TF", new Trait("TF") },
                { "JP", new Trait("JP") }
            };
        }

        // Gets the dominant trait from a pair.
        // e.g. If the trait code is "EI" and the value is 0.5, the dominant trait is "Extraversion"
        public string GetDominantTrait(string traitCode)
        {
            if (!Traits.ContainsKey(traitCode))
                return "Unknown";

            float value = Traits[traitCode].Value;

            var traitPairs = new Dictionary<string, (string Positive, string Negative)>
            {
                { "EI", ("Extraversion", "Introversion") },
                { "SN", ("Sensing", "Intuition") },
                { "TF", ("Thinking", "Feeling") },
                { "JP", ("Judging", "Perceiving") }
            };

            if (!traitPairs.ContainsKey(traitCode))
                return "Unknown";

            return value >= 0 ? traitPairs[traitCode].Positive : traitPairs[traitCode].Negative;
        }
    }
}