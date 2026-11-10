using NUnit.Framework;
using OA.Domain.Enum;
using OA.Persistence.Seeds;

namespace OA.Test.Unit.Persistence;

public class SeedDataTests
{
    [Test]
    public void DefaultRoles_ContainEveryRole()
    {
        var names = DefaultRoles.IdentityRoleList().Select(r => r.Name);

        Assert.That(names, Is.EquivalentTo(Enum.GetNames<Roles>()));
    }

    [Test]
    public void DefaultRoles_NormalizedNames_MatchIdentityNormalizer()
    {
        // UserManager.AddToRoleAsync looks roles up by upper-cased name, so the
        // seed must be normalized the same way or case-sensitive stores miss it.
        foreach (var role in DefaultRoles.IdentityRoleList())
        {
            Assert.That(role.NormalizedName, Is.EqualTo(role.Name!.ToUpperInvariant()), role.Name);
        }
    }

    [Test]
    public void DefaultUsers_NormalizedFields_AreUpperCase()
    {
        foreach (var user in DefaultUser.IdentityBasicUserList())
        {
            Assert.That(user.NormalizedEmail, Is.EqualTo(user.Email!.ToUpperInvariant()), user.Email);
            Assert.That(user.NormalizedUserName, Is.EqualTo(user.UserName!.ToUpperInvariant()), user.UserName);
        }
    }
}
