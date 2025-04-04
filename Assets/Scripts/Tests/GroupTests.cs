using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using CharacterSystem;
using GroupSystem;

public class GroupTests
{
    // GROUP TESTS

    [Test]
    public void Group_Adds_And_Removes_Members()
    {
        var group = new Group("CloseFriends");
        var character = new Character("Alice");

        group.AddMember(character);
        Assert.IsTrue(group.HasMember(character));

        group.RemoveMember(character);
        Assert.IsFalse(group.HasMember(character));
    }

    [Test]
    public void Group_GetMembers_Returns_Correct_List()
    {
        var group = new Group("Family");
        var alice = new Character("Alice");
        var bob = new Character("Bob");

        group.AddMember(alice);
        group.AddMember(bob);

        var members = group.GetMembers();
        Assert.Contains(alice, new List<Character>(members));
        Assert.Contains(bob, new List<Character>(members));
        Assert.AreEqual(2, members.Count);
    }

    // GROUP MANAGER TESTS

    [Test]
    public void GroupManager_Creates_And_Retrieves_Group()
    {
        var manager = new GroupManager();
        manager.CreateGroup("Villagers");

        var group = manager.GetGroup("Villagers");
        Assert.IsNotNull(group);
        Assert.AreEqual("Villagers", group.Name);
    }

    [Test]
    public void GroupManager_Prevents_Duplicate_Groups()
    {
        var manager = new GroupManager();
        manager.CreateGroup("Merchants");
        manager.CreateGroup("Merchants"); 

        Assert.AreEqual(1, manager.GetAllGroups().Count);
    }

    [Test]
    public void GroupManager_DeleteGroup_Removes_Properly()
    {
        var manager = new GroupManager();
        manager.CreateGroup("Guards");

        manager.DeleteGroup("Guards");

        Assert.IsNull(manager.GetGroup("Guards"));
    }

    [Test]
    public void GroupManager_IsMember_Works()
    {
        var manager = new GroupManager();
        manager.CreateGroup("Farmers");

        var group = manager.GetGroup("Farmers");
        var alice = new Character("Alice");

        group.AddMember(alice);

        Assert.IsTrue(manager.IsMember(alice, "Farmers"));
    }

    [Test]
    public void GroupManager_GetGroupsOf_Returns_All_Groups()
    {
        var manager = new GroupManager();
        manager.CreateGroup("CloseFriends");
        manager.CreateGroup("GuildMembers");

        var alice = new Character("Alice");
        manager.GetGroup("CloseFriends").AddMember(alice);
        manager.GetGroup("GuildMembers").AddMember(alice);

        var groups = manager.GetGroupsOf(alice);

        Assert.AreEqual(2, groups.Count);
        Assert.IsTrue(groups.Exists(g => g.Name == "CloseFriends"));
        Assert.IsTrue(groups.Exists(g => g.Name == "GuildMembers"));
    }

    [Test]
    public void GroupMembership_Join_And_Leave_Group()
    {
        var alice = new Character("Alice");
        var group = new Group("Bandits");

        var membership = new GroupMembership(alice);

        membership.JoinGroup(group);
        Assert.IsTrue(membership.IsInGroup(group));
        Assert.IsTrue(group.HasMember(alice));

        membership.LeaveGroup(group);
        Assert.IsFalse(membership.IsInGroup(group));
        Assert.IsFalse(group.HasMember(alice));
    }

    [Test]
    public void GroupMembership_Prevents_Duplicate_Join()
    {
        var alice = new Character("Alice");
        var group = new Group("Healers");

        var membership = new GroupMembership(alice);

        membership.JoinGroup(group);
        membership.JoinGroup(group); 

        Assert.AreEqual(1, membership.Groups.Count);
    }
}