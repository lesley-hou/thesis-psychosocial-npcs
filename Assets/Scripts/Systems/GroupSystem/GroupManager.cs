using System.Collections.Generic;
using System.Linq;
using CharacterSystem;

namespace GroupSystem
{
    public class GroupManager
    {
        private List<Group> groups = new();

        public void CreateGroup(string name)
        {
            if (!groups.Any(g => g.Name == name))
                groups.Add(new Group(name));
        }

        public Group GetGroup(string name) =>
            groups.FirstOrDefault(g => g.Name == name);

        public void DeleteGroup(string name)
        {
            var group = GetGroup(name);
            if (group != null)
                groups.Remove(group);
        }

        public bool IsMember(Character character, string groupName)
        {
            var group = GetGroup(groupName);
            return group != null && group.HasMember(character);
        }

        public List<Group> GetGroupsOf(Character character) =>
            groups.Where(g => g.HasMember(character)).ToList();

        public IReadOnlyList<Group> GetAllGroups() => groups.AsReadOnly();
    }
}