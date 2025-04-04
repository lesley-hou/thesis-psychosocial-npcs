using UnityEngine;
using CharacterSystem;

namespace SocialSystem
{
    public class Relationship
    {
        public Character Source { get; private set; }
        public Character Target { get; private set; }

        // Isbister's social variables
        public float Dominance { get; set; } = 0f;
        public float Agreeableness { get; set; } = 0f;

        public Relationship(Character source, Character target)
        {
            Source = source;
            Target = target;
        }

        public void SetDominance(float value)
        {
            Dominance = Mathf.Clamp(value, -1f, 1f);
        }

        public void SetAgreeableness(float value)
        {
            Agreeableness = Mathf.Clamp(value, -1f, 1f);
        }
    }
}