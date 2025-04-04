using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using SocialSystem;
using CharacterSystem;

public class RelationshipTests
{
    [Test]
    public void Relationship_Creation_InitialValues()
    {
        var bob = new Character("Bob");
        var sue = new Character("Sue");

        var relationship = new Relationship(bob, sue);

        Assert.AreEqual(bob, relationship.Source);
        Assert.AreEqual(sue, relationship.Target);
        Assert.AreEqual(0f, relationship.Dominance);
        Assert.AreEqual(0f, relationship.Agreeableness);
    }

    [Test]
    public void Relationship_Clamp_Dominance()
    {
        var bob = new Character("Bob");
        var sue = new Character("Sue");
        var relationship = new Relationship(bob, sue);

        relationship.SetDominance(2f);
        Assert.AreEqual(1f, relationship.Dominance);

        relationship.SetDominance(-2f);
        Assert.AreEqual(-1f, relationship.Dominance);
    }

    [Test]
    public void Relationship_Clamp_Agreeableness()
    {
        var bob = new Character("Bob");
        var sue = new Character("Sue");
        var relationship = new Relationship(bob, sue);

        relationship.SetAgreeableness(2f);
        Assert.AreEqual(1f, relationship.Agreeableness);

        relationship.SetAgreeableness(-2f);
        Assert.AreEqual(-1f, relationship.Agreeableness);
    }

    // RELATIONSHIP MANAGER TESTS

    [Test]
    public void RelationshipManager_AddAndGetRelationship()
    {
        var bob = new Character("Bob");
        var sue = new Character("Sue");
        var rm = new RelationshipManager(bob);

        rm.AddRelationship(sue);

        var rel = rm.GetRelationship(sue);
        Assert.NotNull(rel);
        Assert.AreEqual(sue, rel.Target);
    }

    [Test]
    public void RelationshipManager_RemoveRelationship()
    {
        var bob = new Character("Bob");
        var sue = new Character("Sue");
        var rm = new RelationshipManager(bob);

        rm.AddRelationship(sue);
        rm.RemoveRelationship(sue);

        Assert.IsNull(rm.GetRelationship(sue));
    }

    [Test]
    public void RelationshipManager_DoesNot_AddDuplicateRelationship()
    {
        var bob = new Character("Bob");
        var sue = new Character("Sue");
        var rm = new RelationshipManager(bob);

        rm.AddRelationship(sue);
        rm.AddRelationship(sue); 

        Assert.AreEqual(1, rm.GetAllRelationships().Count);
    }

    [Test]
    public void RelationshipManager_GetMostDominant()
    {
        var bob = new Character("Bob");
        var sue = new Character("Sue");
        var janet = new Character("Janet");
        var rm = new RelationshipManager(bob);

        rm.AddRelationship(sue);
        rm.AddRelationship(janet);

        rm.GetRelationship(sue).SetDominance(0.5f);
        rm.GetRelationship(janet).SetDominance(0.8f);

        var mostDominant = rm.GetMostDominant(new List<Character> { sue, janet });
        Assert.AreEqual(janet, mostDominant.Target);
    }

    [Test]
    public void RelationshipManager_GetMostDominant_ReturnsNull_IfEmptyList()
    {
        var bob = new Character("Bob");
        var rm = new RelationshipManager(bob);

        var mostDominant = rm.GetMostDominant(new List<Character>());
        Assert.IsNull(mostDominant);
    }

    [Test]
    public void RelationshipManager_GetMostAgreeable()
    {
        var bob = new Character("Bob");
        var sue = new Character("Sue");
        var janet = new Character("Janet");
        var rm = new RelationshipManager(bob);

        rm.AddRelationship(sue);
        rm.AddRelationship(janet);

        rm.GetRelationship(sue).SetAgreeableness(0.2f);
        rm.GetRelationship(janet).SetAgreeableness(0.6f);

        var mostAgreeable = rm.GetMostAgreeable(new List<Character> { sue, janet });
        Assert.AreEqual(janet, mostAgreeable.Target);
    }

    [Test]
    public void RelationshipManager_GetMostAgreeable_ReturnsNull_IfEmptyList()
    {
        var bob = new Character("Bob");
        var rm = new RelationshipManager(bob);

        var mostAgreeable = rm.GetMostAgreeable(new List<Character>());
        Assert.IsNull(mostAgreeable);
    }

    [Test]
    public void RelationshipManager_RemoveNonExistent_DoesNothing()
    {
        var bob = new Character("Bob");
        var sue = new Character("Sue");
        var rm = new RelationshipManager(bob);

        Assert.DoesNotThrow(() => rm.RemoveRelationship(sue));
    }
}

public class RoleTests
{
    // ROLE TESTS

    [Test]
    public void Role_Initializes_Correctly()
    {
        var bob = new Character("Bob");
        var sue = new Character("Sue");
        var role = new Role("Friend", bob, sue);

        Assert.AreEqual("Friend", role.RoleName);
        Assert.AreEqual(bob, role.Owner);
        Assert.AreEqual(sue, role.Target);
        Assert.AreEqual(1.0f, role.Importance);
        Assert.IsNotNull(role.Goals);
        Assert.IsNotNull(role.Actions);
        Assert.IsNotNull(role.Values);
    }

    [Test]
    public void Role_AddGoal_And_AddAction()
    {
        var bob = new Character("Bob");
        var role = new Role("Guard", bob);

        var goal = new RoleGoal("Protect the castle", 2.0f);
        var action = new RoleAction("Patrol", "Patrol the walls");

        role.AddGoal(goal);
        role.AddAction(action);

        Assert.Contains(goal, role.Goals);
        Assert.Contains(action, role.Actions);
    }

    [Test]
    public void Role_Set_And_Get_Value()
    {
        var role = new Role("Merchant", new Character("Sue"));
        role.SetValue("ProfitMargin", 0.25f);

        Assert.AreEqual(0.25f, role.GetValue("ProfitMargin"));
        Assert.AreEqual(0f, role.GetValue("NonExistentKey")); 
    }

    [Test]
    public void Role_SetOwner_And_SetTarget_Works()
    {
        var role = new Role("Enemy", null);
        var bob = new Character("Bob");
        var sue = new Character("Sue");

        role.SetOwner(bob);
        role.SetTarget(sue);

        Assert.AreEqual(bob, role.Owner);
        Assert.AreEqual(sue, role.Target);
    }

    // ROLE MANAGER TESTS

    [Test]
    public void RoleManager_Add_And_Get_Role()
    {
        var bob = new Character("Bob");
        var rm = new RoleManager(bob);

        var role = new Role("Citizen", bob);
        rm.AddRole(role);

        Assert.AreEqual(role, rm.GetRole("Citizen"));
    }

    [Test]
    public void RoleManager_Prevent_Duplicate_Roles()
    {
        var bob = new Character("Bob");
        var rm = new RoleManager(bob);
        var role = new Role("Citizen", bob);

        rm.AddRole(role);
        rm.AddRole(role);

        Assert.AreEqual(1, rm.GetAllRoles().Count);
    }

    [Test]
    public void RoleManager_Remove_Role()
    {
        var bob = new Character("Bob");
        var role = new Role("Merchant", bob);
        var rm = new RoleManager(bob);

        rm.AddRole(role);
        rm.RemoveRole(role);

        Assert.IsNull(rm.GetRole("Merchant"));
    }

    [Test]
    public void RoleManager_GetMostImportantRole_Works()
    {
        var bob = new Character("Bob");
        var rm = new RoleManager(bob);

        var roleA = new Role("Guard", bob) { Importance = 1f };
        var roleB = new Role("King", bob) { Importance = 5f };

        rm.AddRole(roleA);
        rm.AddRole(roleB);

        var mostImportant = rm.GetMostImportantRole();

        Assert.AreEqual("King", mostImportant.RoleName);
    }

    [Test]
    public void RoleManager_ClearRoles()
    {
        var bob = new Character("Bob");
        var rm = new RoleManager(bob);

        rm.AddRole(new Role("Citizen", bob));
        rm.AddRole(new Role("Guard", bob));

        rm.ClearRoles();

        Assert.AreEqual(0, rm.GetAllRoles().Count);
    }

    [Test]
    public void RoleManager_UpdateImportance()
    {
        var bob = new Character("Bob");
        var rm = new RoleManager(bob);

        var role = new Role("Merchant", bob) { Importance = 1f };
        rm.AddRole(role);

        rm.UpdateRoleImportance("Merchant", 3f);

        Assert.AreEqual(3f, role.Importance);
    }
}