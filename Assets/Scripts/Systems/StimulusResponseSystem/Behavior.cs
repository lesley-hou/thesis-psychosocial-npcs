using System.Collections.Generic;
using SocialSystem;

public class Behavior
{
    public string Name { get; }

    // Behaviors may be associated with certain role(s)
    public List<Role> AssociatedRoles { get; }

    public Behavior(string name, List<Role> associatedRoles = null)
    {
        Name = name;
        AssociatedRoles = associatedRoles ?? new List<Role>(); // Empty = valid for all roles
    }
}