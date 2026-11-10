using NUnit.Framework;
using OA.Domain.Auth;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;

namespace OA.Test.Integration.ApiEndpoints;

public class ApiAccountTest : TestClientProvider
{
    // Seeded by OA.Persistence.Seeds.DefaultUser
    private const string SuperAdminEmail = "superadmin@gmail.com";
    private const string BasicUserEmail = "basicuser@gmail.com";
    private const string SeedPassword = "Password@123";

    [Test]
    public async Task Authenticate_SeededSuperAdmin_ReturnsJwtWithRoles()
    {
        var response = await Client.PostJsonAsync("api/Account/authenticate",
            new AuthenticationRequest { Email = SuperAdminEmail, Password = SeedPassword });

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), await response.Content.ReadAsStringAsync());
        var body = await response.ReadAsAsync<ApiResponse<AuthenticationData>>();
        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(body.Data!.JWToken);
        Assert.Multiple(() =>
        {
            Assert.That(body.Succeeded, Is.True);
            Assert.That(body.Data.Email, Is.EqualTo(SuperAdminEmail));
            Assert.That(body.Data.UserName, Is.EqualTo("superadmin"));
            Assert.That(body.Data.IsVerified, Is.True);
            Assert.That(body.Data.Roles, Does.Contain("SuperAdmin"));
            Assert.That(jwt.Issuer, Is.EqualTo("Identity"));
            Assert.That(jwt.Claims.Where(c => c.Type == "roles").Select(c => c.Value), Does.Contain("SuperAdmin"));
        });
    }

    [Test]
    public async Task Authenticate_IssuedToken_GrantsAccessToProtectedEndpoint()
    {
        var token = await LoginAsync(SuperAdminEmail, SeedPassword);
        using var client = CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.GetAsync("api/v1/Customer");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task Authenticate_WrongPassword_ReturnsError()
    {
        var response = await Client.PostJsonAsync("api/Account/authenticate",
            new AuthenticationRequest { Email = SuperAdminEmail, Password = "WrongPassword!" });

        var error = await response.ReadAsAsync<ErrorResponse>();
        Assert.That(response.IsSuccessStatusCode, Is.False);
        Assert.That(error.ErrorMessage, Is.EqualTo($"Invalid Credentials for '{SuperAdminEmail}'."));
    }

    [Test]
    public async Task Authenticate_UnknownEmail_ReturnsError()
    {
        var response = await Client.PostJsonAsync("api/Account/authenticate",
            new AuthenticationRequest { Email = "nobody@example.com", Password = SeedPassword });

        var error = await response.ReadAsAsync<ErrorResponse>();
        Assert.That(response.IsSuccessStatusCode, Is.False);
        Assert.That(error.ErrorMessage, Is.EqualTo("No Accounts Registered with nobody@example.com."));
    }

    [Test]
    public async Task Register_InvalidModel_ReturnsBadRequest()
    {
        var response = await Client.PostJsonAsync("api/Account/register", new RegisterRequest
        {
            FirstName = "John",
            LastName = "Wick",
            Email = "not-an-email",
            UserName = "jw",                  // shorter than 6
            Password = "Password@123",
            ConfirmPassword = "Different@123" // does not match
        });

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        var problem = await response.Content.ReadAsStringAsync();
        Assert.That(problem, Does.Contain("Email").And.Contain("UserName").And.Contain("ConfirmPassword"));
    }

    [Test]
    public async Task Register_ExistingUserName_ReturnsError()
    {
        var response = await Client.PostJsonAsync("api/Account/register", new RegisterRequest
        {
            FirstName = "Another",
            LastName = "Admin",
            Email = "another-admin@example.com",
            UserName = "superadmin",
            Password = "Password@123",
            ConfirmPassword = "Password@123"
        });

        var error = await response.ReadAsAsync<ErrorResponse>();
        Assert.That(response.IsSuccessStatusCode, Is.False);
        Assert.That(error.ErrorMessage, Is.EqualTo("Username 'superadmin' is already taken."));
    }

    [Test]
    public async Task ForgotPassword_UnknownEmail_ReturnsOk_AndSendsNothing()
    {
        var response = await Client.PostJsonAsync("api/Account/forgot-password",
            new ForgotPasswordRequest { Email = "nobody@example.com" });

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(EmailService.SentMails.Any(m => m.ToEmail == "nobody@example.com"), Is.False);
    }

    [Test]
    public async Task ForgotAndResetPassword_FullFlow_AllowsLoginWithNewPassword()
    {
        // 1. Request a reset token (delivered by email)
        var forgot = await Client.PostJsonAsync("api/Account/forgot-password",
            new ForgotPasswordRequest { Email = BasicUserEmail });
        Assert.That(forgot.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var mail = EmailService.SentMails.Last(m => m.ToEmail == BasicUserEmail);
        Assert.That(mail.Subject, Is.EqualTo("Reset Password"));
        var resetToken = mail.Body[(mail.Body.IndexOf(" - ", StringComparison.Ordinal) + 3)..];

        // 2. Reset the password with that token
        const string newPassword = "BrandNew@456";
        var reset = await Client.PostJsonAsync("api/Account/reset-password", new ResetPasswordRequest
        {
            Email = BasicUserEmail,
            Token = resetToken,
            Password = newPassword,
            ConfirmPassword = newPassword
        });
        Assert.That(reset.StatusCode, Is.EqualTo(HttpStatusCode.OK), await reset.Content.ReadAsStringAsync());

        // 3. Old password no longer works, new one does
        var oldLogin = await Client.PostJsonAsync("api/Account/authenticate",
            new AuthenticationRequest { Email = BasicUserEmail, Password = SeedPassword });
        Assert.That(oldLogin.IsSuccessStatusCode, Is.False);

        var token = await LoginAsync(BasicUserEmail, newPassword);
        Assert.That(token, Is.Not.Empty);
    }

    [Test]
    public async Task ResetPassword_InvalidToken_ReturnsError()
    {
        var response = await Client.PostJsonAsync("api/Account/reset-password", new ResetPasswordRequest
        {
            Email = SuperAdminEmail,
            Token = "not-a-real-token",
            Password = "Whatever@123",
            ConfirmPassword = "Whatever@123"
        });

        var error = await response.ReadAsAsync<ErrorResponse>();
        Assert.That(response.IsSuccessStatusCode, Is.False);
        Assert.That(error.ErrorMessage, Is.EqualTo("Error occured while reseting the password."));
    }

    private async Task<string> LoginAsync(string email, string password)
    {
        var response = await Client.PostJsonAsync("api/Account/authenticate",
            new AuthenticationRequest { Email = email, Password = password });
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), await response.Content.ReadAsStringAsync());
        var body = await response.ReadAsAsync<ApiResponse<AuthenticationData>>();
        return body.Data!.JWToken;
    }
}
