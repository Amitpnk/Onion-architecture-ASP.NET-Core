using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Microsoft.FeatureManagement;
using Moq;
using NUnit.Framework;
using OA.Domain.Auth;
using OA.Domain.Enum;
using OA.Domain.Settings;
using OA.Service.Contract;
using OA.Service.Exceptions;
using OA.Service.Implementation;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace OA.Test.Unit.Services;

public class AccountServiceTests
{
    private const string Origin = "https://localhost";

    private static readonly JWTSettings JwtSettings = new()
    {
        Key = "1105D15CB0D48F5781C103A18D5599E4FF25C9102FA694ABDF1DA6828BF153DE",
        Issuer = "Identity",
        Audience = "IdentityUser",
        DurationInMinutes = 60
    };

    private Mock<UserManager<ApplicationUser>> _userManager = null!;
    private Mock<SignInManager<ApplicationUser>> _signInManager = null!;
    private Mock<IEmailService> _emailService = null!;
    private Mock<IFeatureManager> _featureManager = null!;
    private AccountService _service = null!;

    [SetUp]
    public void SetUp()
    {
        _userManager = new Mock<UserManager<ApplicationUser>>(
            Mock.Of<IUserStore<ApplicationUser>>(), null!, null!, null!, null!, null!, null!, null!, null!);
        _signInManager = new Mock<SignInManager<ApplicationUser>>(
            _userManager.Object,
            Mock.Of<IHttpContextAccessor>(),
            Mock.Of<IUserClaimsPrincipalFactory<ApplicationUser>>(),
            null!, null!, null!, null!);
        _emailService = new Mock<IEmailService>();
        _featureManager = new Mock<IFeatureManager>();

        _service = new AccountService(
            _userManager.Object,
            Options.Create(JwtSettings),
            _signInManager.Object,
            _emailService.Object,
            _featureManager.Object);
    }

    #region AuthenticateAsync

    [Test]
    public void Authenticate_UnknownEmail_ThrowsApiException()
    {
        _userManager.Setup(m => m.FindByEmailAsync("nobody@test.com")).ReturnsAsync((ApplicationUser?)null);

        var ex = Assert.ThrowsAsync<ApiException>(() =>
            _service.AuthenticateAsync(new AuthenticationRequest { Email = "nobody@test.com", Password = "x" }, "127.0.0.1"));

        Assert.That(ex!.Message, Is.EqualTo("No Accounts Registered with nobody@test.com."));
    }

    [Test]
    public void Authenticate_WrongPassword_ThrowsApiException()
    {
        var user = CreateUser(emailConfirmed: true);
        _userManager.Setup(m => m.FindByEmailAsync(user.Email!)).ReturnsAsync(user);
        _signInManager.Setup(m => m.PasswordSignInAsync(user.UserName!, "wrong", false, false))
            .ReturnsAsync(SignInResult.Failed);

        var ex = Assert.ThrowsAsync<ApiException>(() =>
            _service.AuthenticateAsync(new AuthenticationRequest { Email = user.Email!, Password = "wrong" }, "127.0.0.1"));

        Assert.That(ex!.Message, Is.EqualTo($"Invalid Credentials for '{user.Email}'."));
    }

    [Test]
    public void Authenticate_UnconfirmedEmail_ThrowsApiException()
    {
        var user = CreateUser(emailConfirmed: false);
        _userManager.Setup(m => m.FindByEmailAsync(user.Email!)).ReturnsAsync(user);
        _signInManager.Setup(m => m.PasswordSignInAsync(user.UserName!, "Password@123", false, false))
            .ReturnsAsync(SignInResult.Success);

        var ex = Assert.ThrowsAsync<ApiException>(() =>
            _service.AuthenticateAsync(new AuthenticationRequest { Email = user.Email!, Password = "Password@123" }, "127.0.0.1"));

        Assert.That(ex!.Message, Is.EqualTo($"Account Not Confirmed for '{user.Email}'."));
    }

    [Test]
    public async Task Authenticate_ValidCredentials_ReturnsSignedJwtWithUserClaimsAndRoles()
    {
        var user = CreateUser(emailConfirmed: true);
        var roles = new List<string> { Roles.Admin.ToString(), Roles.Basic.ToString() };
        _userManager.Setup(m => m.FindByEmailAsync(user.Email!)).ReturnsAsync(user);
        _userManager.Setup(m => m.GetRolesAsync(user)).ReturnsAsync(roles);
        _userManager.Setup(m => m.GetClaimsAsync(user)).ReturnsAsync(new List<Claim> { new("department", "sales") });
        _signInManager.Setup(m => m.PasswordSignInAsync(user.UserName!, "Password@123", false, false))
            .ReturnsAsync(SignInResult.Success);

        var response = await _service.AuthenticateAsync(
            new AuthenticationRequest { Email = user.Email!, Password = "Password@123" }, "10.0.0.1");

        var data = response.Data;
        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(data.JWToken);
        Assert.Multiple(() =>
        {
            Assert.That(response.Succeeded, Is.True);
            Assert.That(response.Message, Is.EqualTo($"Authenticated {user.UserName}"));
            Assert.That(data.Id, Is.EqualTo(user.Id));
            Assert.That(data.Email, Is.EqualTo(user.Email));
            Assert.That(data.UserName, Is.EqualTo(user.UserName));
            Assert.That(data.IsVerified, Is.True);
            Assert.That(data.Roles, Is.EquivalentTo(roles));
            Assert.That(data.RefreshToken, Has.Length.EqualTo(80), "40 random bytes rendered as hex");

            Assert.That(jwt.Issuer, Is.EqualTo(JwtSettings.Issuer));
            Assert.That(jwt.Audiences, Is.EquivalentTo(new[] { JwtSettings.Audience }));
            Assert.That(jwt.Subject, Is.EqualTo(user.UserName));
            Assert.That(jwt.Claims.Single(c => c.Type == "uid").Value, Is.EqualTo(user.Id));
            Assert.That(jwt.Claims.Single(c => c.Type == JwtRegisteredClaimNames.Email).Value, Is.EqualTo(user.Email));
            Assert.That(jwt.Claims.Where(c => c.Type == "roles").Select(c => c.Value), Is.EquivalentTo(roles));
            Assert.That(jwt.Claims.Single(c => c.Type == "department").Value, Is.EqualTo("sales"));
            Assert.That(jwt.ValidTo, Is.EqualTo(DateTime.UtcNow.AddMinutes(JwtSettings.DurationInMinutes)).Within(TimeSpan.FromMinutes(1)));
        });
    }

    #endregion

    #region RegisterAsync

    [Test]
    public void Register_UserNameTaken_ThrowsApiException()
    {
        var request = CreateRegisterRequest();
        _userManager.Setup(m => m.FindByNameAsync(request.UserName)).ReturnsAsync(new ApplicationUser());

        var ex = Assert.ThrowsAsync<ApiException>(() => _service.RegisterAsync(request, Origin));

        Assert.That(ex!.Message, Is.EqualTo($"Username '{request.UserName}' is already taken."));
        _userManager.Verify(m => m.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Never);
    }

    [Test]
    public void Register_EmailTaken_ThrowsApiException()
    {
        var request = CreateRegisterRequest();
        _userManager.Setup(m => m.FindByNameAsync(request.UserName)).ReturnsAsync((ApplicationUser?)null);
        _userManager.Setup(m => m.FindByEmailAsync(request.Email)).ReturnsAsync(new ApplicationUser());

        var ex = Assert.ThrowsAsync<ApiException>(() => _service.RegisterAsync(request, Origin));

        Assert.That(ex!.Message, Is.EqualTo($"Email {request.Email} is already registered."));
        _userManager.Verify(m => m.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Never);
    }

    [Test]
    public void Register_IdentityRejectsUser_ThrowsFirstIdentityError()
    {
        var request = CreateRegisterRequest();
        SetupNewUserLookups(request);
        _userManager.Setup(m => m.CreateAsync(It.IsAny<ApplicationUser>(), request.Password))
            .ReturnsAsync(IdentityResult.Failed(
                new IdentityError { Description = "Passwords must have at least one digit." },
                new IdentityError { Description = "Second error" }));

        var ex = Assert.ThrowsAsync<ApiException>(() => _service.RegisterAsync(request, Origin));

        Assert.That(ex!.Message, Is.EqualTo("Passwords must have at least one digit."));
        _userManager.Verify(m => m.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Never);
    }

    [Test]
    public async Task Register_Success_CreatesUserWithBasicRole_AndReturnsVerificationUrl()
    {
        var request = CreateRegisterRequest();
        SetupNewUserLookups(request);
        ApplicationUser? created = null;
        _userManager.Setup(m => m.CreateAsync(It.IsAny<ApplicationUser>(), request.Password))
            .Callback<ApplicationUser, string>((u, _) => created = u)
            .ReturnsAsync(IdentityResult.Success);
        _userManager.Setup(m => m.AddToRoleAsync(It.IsAny<ApplicationUser>(), Roles.Basic.ToString()))
            .ReturnsAsync(IdentityResult.Success);
        _userManager.Setup(m => m.GenerateEmailConfirmationTokenAsync(It.IsAny<ApplicationUser>()))
            .ReturnsAsync("confirm-token");

        var response = await _service.RegisterAsync(request, Origin);

        var encodedCode = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes("confirm-token"));
        Assert.Multiple(() =>
        {
            Assert.That(created, Is.Not.Null);
            Assert.That(created!.UserName, Is.EqualTo(request.UserName));
            Assert.That(created.Email, Is.EqualTo(request.Email));
            Assert.That(created.FirstName, Is.EqualTo(request.FirstName));
            Assert.That(created.LastName, Is.EqualTo(request.LastName));
            Assert.That(response.Succeeded, Is.True);
            Assert.That(response.Data, Is.EqualTo(created.Id));
            Assert.That(response.Message, Does.Contain($"{Origin}/api/account/confirm-email/?userId={created.Id}&code={encodedCode}"));
        });
        _userManager.Verify(m => m.AddToRoleAsync(created!, Roles.Basic.ToString()), Times.Once);
    }

    [Test]
    public async Task Register_Success_EmailFeatureDisabled_DoesNotSendEmail()
    {
        var request = CreateRegisterRequest();
        SetupSuccessfulRegistration(request);
        _featureManager.Setup(m => m.IsEnabledAsync(nameof(FeatureManagement.EnableEmailService))).ReturnsAsync(false);

        await _service.RegisterAsync(request, Origin);

        _emailService.Verify(m => m.SendEmailAsync(It.IsAny<MailRequest>()), Times.Never);
    }

    [Test]
    public async Task Register_Success_EmailFeatureEnabled_SendsConfirmationEmail()
    {
        var request = CreateRegisterRequest();
        SetupSuccessfulRegistration(request);
        _featureManager.Setup(m => m.IsEnabledAsync(nameof(FeatureManagement.EnableEmailService))).ReturnsAsync(true);

        await _service.RegisterAsync(request, Origin);

        _emailService.Verify(m => m.SendEmailAsync(It.Is<MailRequest>(r =>
            r.ToEmail == request.Email &&
            r.Subject == "Confirm Registration" &&
            r.Body.Contains("/api/account/confirm-email/"))), Times.Once);
    }

    #endregion

    #region ConfirmEmailAsync

    [Test]
    public async Task ConfirmEmail_ValidCode_DecodesCodeAndConfirms()
    {
        var user = CreateUser(emailConfirmed: false);
        var encoded = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes("raw-token"));
        _userManager.Setup(m => m.FindByIdAsync(user.Id)).ReturnsAsync(user);
        _userManager.Setup(m => m.ConfirmEmailAsync(user, "raw-token")).ReturnsAsync(IdentityResult.Success);

        var response = await _service.ConfirmEmailAsync(user.Id, encoded);

        Assert.That(response.Succeeded, Is.True);
        Assert.That(response.Data, Is.EqualTo(user.Id));
        Assert.That(response.Message, Does.StartWith($"Account Confirmed for {user.Email}."));
    }

    [Test]
    public void ConfirmEmail_InvalidCode_ThrowsApiException()
    {
        var user = CreateUser(emailConfirmed: false);
        var encoded = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes("bad-token"));
        _userManager.Setup(m => m.FindByIdAsync(user.Id)).ReturnsAsync(user);
        _userManager.Setup(m => m.ConfirmEmailAsync(user, "bad-token")).ReturnsAsync(IdentityResult.Failed());

        var ex = Assert.ThrowsAsync<ApiException>(() => _service.ConfirmEmailAsync(user.Id, encoded));

        Assert.That(ex!.Message, Is.EqualTo($"An error occured while confirming {user.Email}."));
    }

    #endregion

    #region ForgotPassword / ResetPassword

    [Test]
    public async Task ForgotPassword_UnknownEmail_DoesNothing()
    {
        _userManager.Setup(m => m.FindByEmailAsync("nobody@test.com")).ReturnsAsync((ApplicationUser?)null);

        await _service.ForgotPassword(new ForgotPasswordRequest { Email = "nobody@test.com" }, Origin);

        _emailService.Verify(m => m.SendEmailAsync(It.IsAny<MailRequest>()), Times.Never);
        _userManager.Verify(m => m.GeneratePasswordResetTokenAsync(It.IsAny<ApplicationUser>()), Times.Never);
    }

    [Test]
    public async Task ForgotPassword_KnownEmail_SendsResetToken()
    {
        var user = CreateUser(emailConfirmed: true);
        _userManager.Setup(m => m.FindByEmailAsync(user.Email!)).ReturnsAsync(user);
        _userManager.Setup(m => m.GeneratePasswordResetTokenAsync(user)).ReturnsAsync("reset-token");

        await _service.ForgotPassword(new ForgotPasswordRequest { Email = user.Email! }, Origin);

        _emailService.Verify(m => m.SendEmailAsync(It.Is<MailRequest>(r =>
            r.ToEmail == user.Email &&
            r.Subject == "Reset Password" &&
            r.Body.Contains("reset-token"))), Times.Once);
    }

    [Test]
    public void ResetPassword_UnknownEmail_ThrowsApiException()
    {
        _userManager.Setup(m => m.FindByEmailAsync("nobody@test.com")).ReturnsAsync((ApplicationUser?)null);

        var ex = Assert.ThrowsAsync<ApiException>(() => _service.ResetPassword(CreateResetRequest("nobody@test.com")));

        Assert.That(ex!.Message, Is.EqualTo("No Accounts Registered with nobody@test.com."));
    }

    [Test]
    public async Task ResetPassword_ValidToken_ReturnsSuccess()
    {
        var user = CreateUser(emailConfirmed: true);
        var request = CreateResetRequest(user.Email!);
        _userManager.Setup(m => m.FindByEmailAsync(user.Email!)).ReturnsAsync(user);
        _userManager.Setup(m => m.ResetPasswordAsync(user, request.Token, request.Password)).ReturnsAsync(IdentityResult.Success);

        var response = await _service.ResetPassword(request);

        Assert.That(response.Succeeded, Is.True);
        Assert.That(response.Data, Is.EqualTo(user.Email));
    }

    [Test]
    public void ResetPassword_InvalidToken_ThrowsApiException()
    {
        var user = CreateUser(emailConfirmed: true);
        var request = CreateResetRequest(user.Email!);
        _userManager.Setup(m => m.FindByEmailAsync(user.Email!)).ReturnsAsync(user);
        _userManager.Setup(m => m.ResetPasswordAsync(user, request.Token, request.Password)).ReturnsAsync(IdentityResult.Failed());

        var ex = Assert.ThrowsAsync<ApiException>(() => _service.ResetPassword(request));

        Assert.That(ex!.Message, Is.EqualTo("Error occured while reseting the password."));
    }

    #endregion

    #region Helpers

    private static ApplicationUser CreateUser(bool emailConfirmed) => new()
    {
        Id = Guid.NewGuid().ToString(),
        UserName = "johnwick",
        Email = "john@wick.com",
        FirstName = "John",
        LastName = "Wick",
        EmailConfirmed = emailConfirmed
    };

    private static RegisterRequest CreateRegisterRequest() => new()
    {
        FirstName = "John",
        LastName = "Wick",
        Email = "john@wick.com",
        UserName = "johnwick",
        Password = "Password@123",
        ConfirmPassword = "Password@123"
    };

    private static ResetPasswordRequest CreateResetRequest(string email) => new()
    {
        Email = email,
        Token = "reset-token",
        Password = "NewPassword@123",
        ConfirmPassword = "NewPassword@123"
    };

    private void SetupNewUserLookups(RegisterRequest request)
    {
        _userManager.Setup(m => m.FindByNameAsync(request.UserName)).ReturnsAsync((ApplicationUser?)null);
        _userManager.Setup(m => m.FindByEmailAsync(request.Email)).ReturnsAsync((ApplicationUser?)null);
    }

    private void SetupSuccessfulRegistration(RegisterRequest request)
    {
        SetupNewUserLookups(request);
        _userManager.Setup(m => m.CreateAsync(It.IsAny<ApplicationUser>(), request.Password)).ReturnsAsync(IdentityResult.Success);
        _userManager.Setup(m => m.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);
        _userManager.Setup(m => m.GenerateEmailConfirmationTokenAsync(It.IsAny<ApplicationUser>())).ReturnsAsync("confirm-token");
    }

    #endregion
}
