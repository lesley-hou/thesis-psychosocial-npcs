using System.Collections.Generic;
using System.Linq;
using CharacterSystem;

namespace GroupSystem
{
    public class GroupMembership
    {
        private Character owner;
        private List<Group> groups = new();

        public GroupMembership(Character owner)
        {
            this.owner = owner;
        }

        public IReadOnlyList<Group> Groups => groups.AsReadOnly();

        public void JoinGroup(Group group)
        {
            if (!groups.Contains(group))
            {
                groups.Add(group);
                group.AddMember(owner);
            }
        }

        public void LeaveGroup(Group group)
        {
            if (groups.Contains(group))
            {
                groups.Remove(group);
                group.RemoveMember(owner);
            }
        }

        public bool IsInGroup(Group group) => groups.Contains(group);
    }
}