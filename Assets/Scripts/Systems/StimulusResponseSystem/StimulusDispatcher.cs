using System.Collections.Generic;
using SocialSystem;

namespace StimulusResponseSystem
{
    public class StimulusDispatcher
    {
        private Dictionary<string, List<Behavior>> stimulusBehaviorMap = new();

        public StimulusDispatcher()
        {
            // Negative comment
            stimulusBehaviorMap["NegativeComment"] = new List<Behavior>
            {
                new Behavior("Disagree"),
                new Behavior("Ignore"),
                new Behavior("Withdraw"),
                new Behavior("Defend", new List<Role> { new Role("Friend", null), new Role("Family", null) }),
                new Behavior("Confront", new List<Role> { new Role("Supervisor", null), new Role("Rival", null) })
            };

            // Receiving a compliment
            stimulusBehaviorMap["Compliment"] = new List<Behavior>
            {
                new Behavior("Smile"),
                new Behavior("Thank"),
                new Behavior("Compliment Back"),
                new Behavior("Downplay"),
                new Behavior("Ignore")
            };

            // Distressing situation observed
            stimulusBehaviorMap["DistressObserved"] = new List<Behavior>
            {
                new Behavior("Comfort", new List<Role> { new Role("Friend", null), new Role("Family", null), new Role("Colleague", null) }),
                new Behavior("Offer Help"),
                new Behavior("Withdraw"),
                new Behavior("Ignore")
            };

            // Request for help
            stimulusBehaviorMap["HelpRequest"] = new List<Behavior>
            {
                new Behavior("Offer Help"),
                new Behavior("Refuse"),
                new Behavior("Comply", new List<Role> { new Role("Subordinate", null), new Role("Student", null) }),
                new Behavior("Ignore")
            };

            // Invitation to a social event
            stimulusBehaviorMap["SocialInvitation"] = new List<Behavior>
            {
                new Behavior("Accept"),
                new Behavior("Politely Decline"),
                new Behavior("Refuse"),
                new Behavior("Postpone"),
                new Behavior("Ignore")
            };

            // Gossip
            stimulusBehaviorMap["GossipHeard"] = new List<Behavior>
            {
                new Behavior("Spread Gossip"),
                new Behavior("Keep Secret"),
                new Behavior("Confront Subject"),
                new Behavior("Ignore")
            };
        }

        public List<Behavior> GetCandidateBehaviors(Stimulus stimulus) =>
            stimulusBehaviorMap.TryGetValue(stimulus.Name, out var behaviors)
                ? behaviors
                : new List<Behavior>();
    }
}