using System.Collections.Generic;
using CharacterSystem;

namespace GroupSystem
{
    public class Group
    {
        public string Name { get; }

        private HashSet<Character> members = new();

        public Group(string name)
        {
            Name = name;
        }

        public void AddMember(Character character) => members.Add(character);

        public void RemoveMember(Character character) => members.Remove(character);

        public bool HasMember(Character character) => members.Contains(character);

        public IReadOnlyCollection<Character> GetMembers() => members;
    }
}