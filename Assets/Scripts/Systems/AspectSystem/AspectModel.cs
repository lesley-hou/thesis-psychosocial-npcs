using System.Collections.Generic;
using UnityEngine;

namespace AspectSystem
{
    public abstract class AspectModel
    {
        public string ModelName { get; protected set; }
        public Dictionary<string, Trait> Traits { get; protected set; } = new();
        protected Dictionary<string, Trait> CoreTraits { get; private set; }

        protected AspectModel(string modelName)
        {
            ModelName = modelName;
            CoreTraits = GenerateCoreTraits();
        }

        protected abstract Dictionary<string, Trait> GenerateCoreTraits();

        public virtual Trait GetTrait(string traitName) =>
            Traits.TryGetValue(traitName, out var trait) ? trait : null;

        public Dictionary<string, Trait> GetCoreTraits() => CoreTraits;

        public virtual bool AddTrait(string traitName, float? initialValue = null)
        {
            if (CoreTraits.ContainsKey(traitName) && !Traits.ContainsKey(traitName))
            {
                float value = Mathf.Clamp(initialValue ?? 0f, -1f, 1f);
                Traits[traitName] = new Trait(traitName, value);
                return true;
            }

            Debug.LogWarning($"[AspectModel] Trait '{traitName}' is invalid.");
            return false;
        }

        public virtual bool AddSubTrait(Trait parentTrait, string subTraitName, float weight, float? initialValue = null)
        {
            if (parentTrait == null || !Traits.ContainsKey(parentTrait.GetName()))
            {
                Debug.LogWarning($"[AspectModel] Parent trait '{parentTrait?.GetName()}' not found.");
                return false;
            }

            if (!parentTrait.SubTraits.ContainsKey(subTraitName))
            {
                float valueToUse = initialValue ?? 0f;
                Trait subTrait = new(subTraitName, valueToUse, weight);
                parentTrait.SubTraits[subTraitName] = subTrait;
                Traits[subTraitName] = subTrait;
                return true;
            }

            Debug.LogWarning($"[AspectModel] Sub-trait '{subTraitName}' already exists.");
            return false;
        }

        public virtual void ModifyTrait(string traitName, float amount)
        {
            if (Traits.ContainsKey(traitName))
            {
                var trait = Traits[traitName];
                trait.SetValue(trait.ComputeTraitValue() + amount);
            }
            else
            {
                Debug.LogWarning($"[AspectModel] Trait '{traitName}' not found in model '{ModelName}'.");
            }
        }

        public virtual void SetTraitValue(string traitName, float value)
        {
            if (Traits.ContainsKey(traitName))
                Traits[traitName].SetValue(Mathf.Clamp(value, -1f, 1f));
        }

        public virtual float GetTraitValue(string traitName) =>
            Traits.TryGetValue(traitName, out var trait) ? trait.ComputeTraitValue() : 0f;
    }
}