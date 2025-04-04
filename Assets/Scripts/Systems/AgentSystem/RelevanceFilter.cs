using System.Linq;
using UnityEngine;

namespace AgentSystem
{
    public class StimulusRelevanceFilter
    {
        private CharacterManager character;

        public StimulusRelevanceFilter(CharacterManager character)
        {
            this.character = character;
        }

        public bool IsRelevant(Stimulus stimulus, out float remainingMagnitude)
        {
            remainingMagnitude = stimulus.Magnitude;
            float distance = Vector3.Distance(character.Character.Position, stimulus.Position);

            switch (stimulus.Propagation)
            {
                case StimulusPropagation.Direct:
                    break;

                case StimulusPropagation.Local:
                    if (distance > stimulus.FalloffRadius)
                    {
                        remainingMagnitude = 0f;
                        return false;
                    }
                    remainingMagnitude *= 1f - (distance / stimulus.FalloffRadius);
                    break;

                case StimulusPropagation.Broadcast:
                    if (!string.IsNullOrEmpty(stimulus.TargetGroup) &&
                        !character.GroupMembership.Groups.Any(g => g.Name == stimulus.TargetGroup))
                    {
                        remainingMagnitude = 0f;
                        return false;
                    }
                    break;
            }

            return true;
        }
    }
}