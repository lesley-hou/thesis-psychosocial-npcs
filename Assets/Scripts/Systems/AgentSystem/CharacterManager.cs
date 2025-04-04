using System.Collections.Generic;
using CharacterSystem;
using GroupSystem;
using SocialSystem;
using StimulusResponseSystem;

namespace AgentSystem
{
    public class CharacterManager
    {
        public Character Character { get; }
        public RoleManager RoleManager { get; }
        public RelationshipManager RelationshipManager { get; }
        public GroupMembership GroupMembership { get; }
        public CharacterImportance ImportanceSystem { get; private set; }

        private DecisionMaker decisionMaker;

        public CharacterManager(string name)
        {
            Character = new Character(name);
            RoleManager = new RoleManager(Character);
            RelationshipManager = new RelationshipManager(Character);
            GroupMembership = new GroupMembership(Character); 
            decisionMaker = new DecisionMaker(this, Character, "EkmanEmotionModel");
        }

        public Behavior ProcessStimulus(
            Stimulus stimulus,
            List<StimulusEffect> effects,
            List<Behavior> candidateBehaviors,
            float remainingMagnitude)
        {
            return decisionMaker.ProcessStimulus(stimulus, effects, candidateBehaviors, remainingMagnitude);
        }

        public float GetImportance() => ImportanceSystem?.CalculateOverallImportance() ?? 0f;

        public float GetPriority() => ImportanceSystem?.CalculatePriority() ?? 0f;
    }
}