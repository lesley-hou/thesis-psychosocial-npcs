using System.Collections.Generic;
using CharacterSystem;

namespace SocialSystem
{
    public class Role
    {
        public string RoleName { get; private set; }
        public Character Owner { get; private set; }
        public Character Target { get; private set; }

        public List<RoleGoal> Goals { get; private set; } = new();
        public List<RoleAction> Actions { get; private set; } = new();
        public Dictionary<string, float> Values { get; private set; } = new();

        public float Importance { get; set; } = 1.0f;

        public Role(string roleName, Character owner, Character target = null)
        {
            RoleName = roleName;
            Owner = owner;
            Target = target;
        }

        public void AddGoal(RoleGoal goal) => Goals.Add(goal);

        public void AddAction(RoleAction action) => Actions.Add(action);

        public void SetValue(string key, float value) => Values[key] = value;

        public float GetValue(string key) =>
            Values.TryGetValue(key, out var val) ? val : 0f;

        public void SetOwner(Character owner) => Owner = owner;

        public void SetTarget(Character target) => Target = target;
    }
}