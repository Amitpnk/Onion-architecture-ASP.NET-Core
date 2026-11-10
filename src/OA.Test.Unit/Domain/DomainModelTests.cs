using NUnit.Framework;
using OA.Domain.Auth;
using OA.Domain.Common;

namespace OA.Test.Unit.Domain;

public class DomainModelTests
{
    [Test]
    public void Response_WithData_IsSuccessful()
    {
        var response = new Response<int>(5, "done");

        Assert.Multiple(() =>
        {
            Assert.That(response.Succeeded, Is.True);
            Assert.That(response.Data, Is.EqualTo(5));
            Assert.That(response.Message, Is.EqualTo("done"));
        });
    }

    [Test]
    public void Response_WithMessageOnly_IsFailure()
    {
        var response = new Response<string>("You are not Authorized");

        Assert.Multiple(() =>
        {
            Assert.That(response.Succeeded, Is.False);
            Assert.That(response.Data, Is.Null);
            Assert.That(response.Message, Is.EqualTo("You are not Authorized"));
        });
    }

    [Test]
    public void RefreshToken_NotExpiredAndNotRevoked_IsActive()
    {
        var token = new RefreshToken { Expires = DateTime.UtcNow.AddMinutes(5) };

        Assert.That(token.IsExpired, Is.False);
        Assert.That(token.IsActive, Is.True);
    }

    [Test]
    public void RefreshToken_PastExpiry_IsExpiredAndInactive()
    {
        var token = new RefreshToken { Expires = DateTime.UtcNow.AddMinutes(-1) };

        Assert.That(token.IsExpired, Is.True);
        Assert.That(token.IsActive, Is.False);
    }

    [Test]
    public void RefreshToken_Revoked_IsInactive()
    {
        var token = new RefreshToken { Expires = DateTime.UtcNow.AddDays(1), Revoked = DateTime.UtcNow };

        Assert.That(token.IsActive, Is.False);
    }

    [Test]
    public void ApplicationUser_OwnsToken_ChecksRefreshTokens()
    {
        var user = new ApplicationUser
        {
            RefreshTokens = [new RefreshToken { Token = "abc" }]
        };

        Assert.That(user.OwnsToken("abc"), Is.True);
        Assert.That(user.OwnsToken("xyz"), Is.False);
    }

    [Test]
    public void ApplicationUser_WithoutRefreshTokens_OwnsNothing()
    {
        var user = new ApplicationUser();

        Assert.That(user.OwnsToken("abc"), Is.False);
    }
}
