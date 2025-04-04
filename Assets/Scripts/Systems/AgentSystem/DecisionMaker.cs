using System.Collections.Generic;
using UnityEngine;
using SocialSystem;
using AspectSystem;
using CharacterSystem;

namespace AgentSystem
{
    public class DecisionMaker
    {
        private CharacterManager characterManager;
        private Character character;
        private string emotionModelName;

        public DecisionMaker(CharacterManager characterManager, Character character, string emotionModelName)
        {
            this.characterManager = characterManager;
            this.character = character;
            this.emotionModelName = emotionModelName;
        }

        public Behavior ProcessStimulus(Stimulus stimulus, List<StimulusEffect> effects, List<Behavior> possibleBehaviors, float remainingMagnitude)
        {
            if ((effects == null || effects.Count == 0) && possibleBehaviors?.Count > 0)
            {
                // No effects passed in - mostly for easier testing
            }
            else if (!ShouldReactTo(stimulus, effects, remainingMagnitude))
            {
                return null;
            }

            foreach (var effect in effects)
            {
                ApplyStimulusEffect(effect, remainingMagnitude);
            }

            return SelectBehavior(possibleBehaviors);
        }

        private bool ShouldReactTo(Stimulus stimulus, List<StimulusEffect> effects, float remainingStimulusMagnitude)
        {
            foreach (var effect in effects)
            {
                float magnitude = effect.BaseMagnitude;

                if (!string.IsNullOrEmpty(effect.PersonalityModulator))
                {
                    float modulator = Mathf.Clamp01(GetTraitValue("Personality", "FFMPersonalityModel", effect.PersonalityModulator));
                    magnitude *= 1f + effect.ModulationStrength * modulator;
                }

                if (magnitude * remainingStimulusMagnitude > 0f)
                    return true;
            }

            return false;
        }

        private void ApplyStimulusEffect(StimulusEffect effect, float stimulusMagnitude)
        {
            float adjusted = stimulusMagnitude * effect.BaseMagnitude;

            if (!string.IsNullOrEmpty(effect.PersonalityModulator))
            {
                float modulator = Mathf.Clamp01(GetTraitValue("Personality", "FFMPersonalityModel", effect.PersonalityModulator));
                adjusted *= 1f + effect.ModulationStrength * modulator;
            }

            switch (effect.AffectedSystem)
            {
                case AffectedSystem.Emotion:
                    ModifyTrait("Emotion", emotionModelName, effect.AffectedTrait, adjusted);
                    break;

                case AffectedSystem.Relationship:
                    ApplyRelationshipStimulus(effect, adjusted);
                    break;

                case AffectedSystem.GoalPlanning:
                    ApplyGoalPlanningStimulus(effect, adjusted);
                    break;
            }
        }

        private Behavior SelectBehavior(List<Behavior> possibleBehaviors)
        {
            if (possibleBehaviors == null || possibleBehaviors.Count == 0)
                return null;

            Role currentRole = characterManager.RoleManager.GetMostImportantRole();
            List<Behavior> roleFiltered = new();

            foreach (var behavior in possibleBehaviors)
            {
                if (behavior.AssociatedRoles.Count == 0 || 
                    (currentRole != null && behavior.AssociatedRoles.Exists(r => r.RoleName == currentRole.RoleName)))
                {
                    roleFiltered.Add(behavior);
                }
            }

            if (roleFiltered.Count == 0)
                return null;

            Dictionary<Behavior, float> weights = new();
            float totalWeight = 0f;

            foreach (var behavior in roleFiltered)
            {
                float weight = ComputeBehaviorWeight(behavior);
                weights[behavior] = weight;
                totalWeight += weight;
            }

            float rand = Random.Range(0f, totalWeight);
            float cumulative = 0f;

            foreach (var kvp in weights)
            {
                cumulative += kvp.Value;
                if (rand <= cumulative)
                    return kvp.Key;
            }

            return roleFiltered[0];
        }

        private float ComputeBehaviorWeight(Behavior behavior)
        {
            float weight = 0.2f;

            bool hasFFM = character.HasAspect("Personality") && character.GetAspect("Personality").HasModel("FFMPersonalityModel");
            bool hasMBTI = character.HasAspect("Personality") && character.GetAspect("Personality").HasModel("MBTIPersonalityModel");

            float extraversion = hasFFM ? GetTraitValue("Personality", "FFMPersonalityModel", "Extraversion") : 0.5f;
            float agreeableness = hasFFM ? GetTraitValue("Personality", "FFMPersonalityModel", "Agreeableness") : 0.5f;
            float neuroticism = hasFFM ? GetTraitValue("Personality", "FFMPersonalityModel", "Neuroticism") : 0.5f;
            float conscientiousness = hasFFM ? GetTraitValue("Personality", "FFMPersonalityModel", "Conscientiousness") : 0.5f;
            float openness = hasFFM ? GetTraitValue("Personality", "FFMPersonalityModel", "Openness") : 0.5f;

            float ei = hasMBTI ? GetTraitValue("Personality", "MBTIPersonalityModel", "EI") : 0.5f;
            float tf = hasMBTI ? GetTraitValue("Personality", "MBTIPersonalityModel", "TF") : 0.5f;
            float sn = hasMBTI ? GetTraitValue("Personality", "MBTIPersonalityModel", "SN") : 0.5f;
            float jp = hasMBTI ? GetTraitValue("Personality", "MBTIPersonalityModel", "JP") : 0.5f;

            switch (behavior.Name)
            {
                case "Smile":
                    weight += extraversion * 10f + (ei > 0.5f ? 5f : 0f);
                    break;

                case "Thank":
                    weight += agreeableness * 10f + (ei > 0.5f ? 5f : 0f);
                    break;

                case "Compliment Back":
                    weight += extraversion * 8f + ei * 5f;
                    break;

                case "Withdraw":
                    weight += (1f - extraversion) * 10f + (hasMBTI && ei < 0.5f ? 5f : 0f);
                    break;

                case "Ignore":
                    weight += (1f - extraversion) * 8f + (hasMBTI && ei < 0.5f ? 4f : 0f);
                    break;

                case "Downplay":
                    weight += (1f - extraversion) * 8f + (hasMBTI && ei < 0.5f ? 4f : 0f);
                    break;

                case "Postpone":
                    weight += (1f - extraversion) * 8f + (ei < 0.5f ? 4f : 0f);
                    break;

                case "Defend":
                    weight += (1f - agreeableness) * 8f + (tf > 0.5f ? 5f : 0f);
                    break;

                case "Disagree":
                    weight += agreeableness * 5f + openness * 4f + (tf < 0.5f ? 3f : 0f);
                    break;

                case "Confront":
                    weight += (1f - agreeableness) * 8f + neuroticism * 5f + (tf > 0.5f ? 4f : 0f);
                    break;

                case "Comfort":
                    weight += agreeableness * 8f + (ei > 0.5f ? 4f : 0f);
                    break;

                case "Offer Help":
                    weight += agreeableness * 8f + (tf < 0.5f ? 4f : 0f);
                    break;

                case "Refuse":
                    weight += (1f - conscientiousness) * 8f + (jp < 0.5f ? 4f : 0f);
                    break;

                case "Comply":
                    weight += agreeableness * 5f + conscientiousness * 5f + (jp > 0.5f ? 2.5f : 0f);
                    break;

                case "Spread Gossip":
                    weight += extraversion * 8f + neuroticism * 4f + (sn < 0.5f ? 2f : 0f);
                    break;

                case "Keep Secret":
                    weight += conscientiousness * 8f + (sn > 0.5f ? 2f : 0f);
                    break;

                case "Accept":
                    weight += agreeableness * 8f + (ei > 0.5f ? 4f : 0f);
                    break;

                case "Politely Decline":
                    weight += (1f - agreeableness) * 8f + (ei < 0.5f ? 4f : 0f);
                    break;
            }

            // For easier testing
            Debug.Log($"[{character.Name}] Behavior: {behavior.Name} | Weight: {weight:F2} | Traits => " +
            $"Extraversion: {extraversion:F2}, Agreeableness: {agreeableness:F2}, Neuroticism: {neuroticism:F2}, " +
            $"Conscientiousness: {conscientiousness:F2}, Openness: {openness:F2}, EI: {ei:F2}, TF: {tf:F2}, SN: {sn:F2}, JP: {jp:F2}");

            return Mathf.Max(weight, 0.001f);
        }

        private float GetTraitValue(string aspectName, string modelName, string traitName)
        {
            return character.GetAspect(aspectName)?.GetModel(modelName)?.GetTraitValue(traitName) ?? 0f;
        }

        private void ModifyTrait(string aspectName, string modelName, string traitName, float amount)
        {
            character.GetAspect(aspectName)?.GetModel(modelName)?.ModifyTrait(traitName, amount);
        }

        private void ApplyRelationshipStimulus(StimulusEffect effect, float amount)
        {
            if (effect.TargetCharacter != null)
            {
                var rel = characterManager.RelationshipManager.GetRelationship(effect.TargetCharacter);
                if (rel != null)
                    ModifyRelationshipSocialVariable(rel, effect, amount);
            }
            else if (effect.TargetGroupObject != null)
            {
                foreach (var target in effect.TargetGroupObject.GetMembers())
                {
                    var rel = characterManager.RelationshipManager.GetRelationship(target);
                    if (rel != null)
                        ModifyRelationshipSocialVariable(rel, effect, amount);
                }
            }
        }

        private void ModifyRelationshipSocialVariable(Relationship relationship, StimulusEffect effect, float amount)
        {
            switch (effect.AffectedTrait)
            {
                case "Dominance":
                    relationship.SetDominance(relationship.Dominance + amount);
                    break;

                case "Agreeableness":
                    relationship.SetAgreeableness(relationship.Agreeableness + amount);
                    break;
            }
        }

        private void ApplyGoalPlanningStimulus(StimulusEffect effect, float amount)
        {
            Role role = characterManager.RoleManager.GetMostImportantRole();
            if (role == null) return;

            var goal = role.Goals.Find(g => g.Description == effect.AffectedTrait);
            if (goal != null)
                goal.Priority = Mathf.Clamp01(goal.Priority + amount);
        }
    }
}