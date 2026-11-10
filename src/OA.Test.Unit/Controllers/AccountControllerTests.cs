using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using OA.Controllers;
using OA.Domain.Auth;
using OA.Domain.Common;
using OA.Service.Contract;
using System.Net;

namespace OA.Test.Unit.Controllers;

public class AccountControllerTests
{
    private Mock<IAccountService> _accountService = null!;
    private DefaultHttpContext _httpContext = null!;
    private AccountController _controller = null!;

    [SetUp]
    public void SetUp()
    {
        _accountService = new Mock<IAccountService>();
        _accountService
            .Setup(s => s.AuthenticateAsync(It.IsAny<AuthenticationRequest>(), It.IsAny<string>()))
            .ReturnsAsync(new Response<AuthenticationResponse>(new AuthenticationResponse()));

        _httpContext = new DefaultHttpContext();
        _controller = new AccountController(_accountService.Object)
        {
            ControllerContext = new ControllerContext { HttpContext = _httpContext }
        };
    }

    [Test]
    public async Task Authenticate_UsesForwardedForHeader_WhenPresent()
    {
        _httpContext.Request.Headers["X-Forwarded-For"] = "203.0.113.7";
        _httpContext.Connection.RemoteIpAddress = IPAddress.Parse("10.0.0.1");

        await _controller.AuthenticateAsync(new AuthenticationRequest());

        _accountService.Verify(s => s.AuthenticateAsync(It.IsAny<AuthenticationRequest>(), "203.0.113.7"));
    }

    [Test]
    public async Task Authenticate_UsesRemoteIpAsIPv4_WhenNoForwardedHeader()
    {
        _httpContext.Connection.RemoteIpAddress = IPAddress.Parse("::ffff:192.168.1.20");

        await _controller.AuthenticateAsync(new AuthenticationRequest());

        _accountService.Verify(s => s.AuthenticateAsync(It.IsAny<AuthenticationRequest>(), "192.168.1.20"));
    }

    [Test]
    public async Task Authenticate_NoRemoteIpAndNoHeader_PassesEmptyIp_InsteadOfThrowing()
    {
        _httpContext.Connection.RemoteIpAddress = null;

        var result = await _controller.AuthenticateAsync(new AuthenticationRequest());

        Assert.That(result, Is.TypeOf<OkObjectResult>());
        _accountService.Verify(s => s.AuthenticateAsync(It.IsAny<AuthenticationRequest>(), string.Empty));
    }
}
