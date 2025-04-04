using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using CharacterSystem;

namespace SocialSystem
{
    public class RoleManager
    {
        private Character owner;
        private List<Role> roles = new();

        public RoleManager(Character owner)
        {
            this.owner = owner;
        }

        public void AddRole(Role role)
        {
            if (!roles.Contains(role))
            {
                roles.Add(role);
            }
            else
            {
                Debug.LogWarning($"[RoleManager] Role '{role.RoleName}' already exists.");
            }
        }

        public void RemoveRole(Role role)
        {
            if (roles.Contains(role))
            {
                roles.Remove(role);
            }
        }

        public Role GetRole(string roleName) =>
            roles.FirstOrDefault(r => r.RoleName == roleName);

        public List<Role> GetAllRoles() =>
            new List<Role>(roles);

        public Role GetMostImportantRole()
        {
            if (roles.Count == 0) return null;
            return roles.OrderByDescending(r => r.Importance).First();
        }

        public void ClearRoles() => roles.Clear();

        public void UpdateRoleImportance(string roleName, float newImportance)
        {
            Role role = GetRole(roleName);
            if (role != null)
            {
                role.Importance = newImportance;
            }
        }
    }
}