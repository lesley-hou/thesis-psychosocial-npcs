using System.Collections.Generic;
using UnityEngine;
using AspectSystem;

namespace CharacterSystem
{
    public class Character
    {
        public string Name { get; private set; }
        public Vector3 Position { get; set; }

        private Dictionary<string, Aspect> aspects = new();

        public Character(string name, Vector3 position)
        {
            Name = name;
            Position = position;
        }

        public Character(string name) : this(name, Vector3.zero) { }

        public void AddAspect(string name, Aspect aspect)
        {
            if (!aspects.ContainsKey(name))
                aspects[name] = aspect;
            else
                Debug.LogWarning($"Aspect '{name}' already exists.");
        }

        public Aspect GetAspect(string name) =>
            aspects.TryGetValue(name, out var aspect) ? aspect : null;

        public bool HasAspect(string name) => aspects.ContainsKey(name);
    }
}