using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using CharacterSystem;

namespace SocialSystem
{
    public class RelationshipManager
    {
        private Character owner;
        private List<Relationship> relationships = new();

        public RelationshipManager(Character owner)
        {
            this.owner = owner;
        }

        public void AddRelationship(Character target)
        {
            if (target == null)
            {
                Debug.LogWarning("[RelationshipManager] Target cannot be null.");
                return;
            }

            if (GetRelationship(target) != null)
            {
                Debug.LogWarning($"[RelationshipManager] Relationship with '{target.Name}' already exists.");
                return;
            }

            relationships.Add(new Relationship(owner, target));
        }

        public void RemoveRelationship(Character target)
        {
            Relationship rel = GetRelationship(target);
            if (rel != null)
            {
                relationships.Remove(rel);
            }
            else
            {
                Debug.LogWarning($"[RelationshipManager] No relationship with '{target.Name}' found to remove.");
            }
        }

        public Relationship GetRelationship(Character target) =>
            relationships.FirstOrDefault(r => r.Target == target);

        public List<Relationship> GetAllRelationships() =>
            new List<Relationship>(relationships);

        // Get the most dominant relationship with a target from a list of agents
        public Relationship GetMostDominant(List<Character> agents) =>
            relationships
                .Where(r => agents.Contains(r.Target))
                .OrderByDescending(r => r.Dominance)
                .FirstOrDefault();

        // Get the most agreeable relationship with a target from a list of agents
        public Relationship GetMostAgreeable(List<Character> agents) =>
            relationships
                .Where(r => agents.Contains(r.Target))
                .OrderByDescending(r => r.Agreeableness)
                .FirstOrDefault();

        public void ClearRelationships()
        {
            relationships.Clear();
        }
    }
}