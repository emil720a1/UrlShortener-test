using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using UrlShortener.Application.DTO_s.UserDto_s;
using UrlShortener.Web.Controllers;
using UrlShortener.Web.Services;
using Xunit;

namespace UrlShortener.Tests;

public class AuthControllerTests
{
    private Mock<UserManager<IdentityUser>> _userManagerMock = null!;
    private Mock<IJwtService> _jwtServiceMock = null!;
    private AuthController _controller = null!;

    public AuthControllerTests()
    {
        var storeMock = new Mock<IUserStore<IdentityUser>>();
        _userManagerMock = new Mock<UserManager<IdentityUser>>(
            storeMock.Object, null!, null!, null!, null!, null!, null!, null!, null!);

        _jwtServiceMock = new Mock<IJwtService>();
        _jwtServiceMock
            .Setup(j => j.GenerateTokenAsync(It.IsAny<IdentityUser>()))
            .ReturnsAsync("mocked-jwt-token");

        _controller = new AuthController(_userManagerMock.Object, _jwtServiceMock.Object);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
    }

    [Fact]
    public async Task Register_WhenSuccess_ReturnsOk()
    {
        var request = new RegisterDto("test@test.com", "Password123!");

        _userManagerMock.Setup(u => u.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);

        var result = await _controller.Register(request) as OkObjectResult;

        result.Should().NotBeNull();
        result!.StatusCode.Should().Be(200);

        _userManagerMock.Verify(u => u.CreateAsync(It.IsAny<IdentityUser>(), request.Password), Times.Once);
    }

    [Fact]
    public async Task Register_WhenFails_ReturnsBadRequestWithErrors()
    {
        var request = new RegisterDto("test@test.com", "123");
        var errors = new List<IdentityError>
        {
            new() { Code = "PasswordTooShort", Description = "Password too short" }
        };
        var identityResult = IdentityResult.Failed(errors.ToArray());

        _userManagerMock.Setup(u => u.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
            .ReturnsAsync(identityResult);

        var result = await _controller.Register(request) as BadRequestObjectResult;

        result.Should().NotBeNull();
        result!.StatusCode.Should().Be(400);

        _userManagerMock.Verify(u => u.CreateAsync(It.IsAny<IdentityUser>(), request.Password), Times.Once);
    }

    [Fact]
    public async Task Login_WhenValidCredentials_ReturnsOkWithToken()
    {
        var request = new LoginDto("test@test.com", "Password123!");
        var user = new IdentityUser { Id = "test-user-id", Email = request.Email, UserName = request.Email };

        _userManagerMock.Setup(u => u.FindByEmailAsync(request.Email)).ReturnsAsync(user);
        _userManagerMock.Setup(u => u.CheckPasswordAsync(user, request.Password)).ReturnsAsync(true);
        _userManagerMock.Setup(u => u.GetRolesAsync(user)).ReturnsAsync(new List<string>());

        var result = await _controller.Login(request) as OkObjectResult;

        result.Should().NotBeNull();
        result!.StatusCode.Should().Be(200);

        _userManagerMock.Verify(u => u.FindByEmailAsync(request.Email), Times.Once);
        _userManagerMock.Verify(u => u.CheckPasswordAsync(user, request.Password), Times.Once);
        _jwtServiceMock.Verify(j => j.GenerateTokenAsync(user), Times.Once);
    }

    [Fact]
    public async Task Login_WhenInvalidCredentials_ReturnsUnauthorized()
    {
        var request = new LoginDto("test@test.com", "WrongPassword");
        var user = new IdentityUser { Id = "test-user-id", Email = request.Email, UserName = request.Email };

        _userManagerMock.Setup(u => u.FindByEmailAsync(request.Email)).ReturnsAsync(user);
        _userManagerMock.Setup(u => u.CheckPasswordAsync(user, It.IsAny<string>())).ReturnsAsync(false);

        var result = await _controller.Login(request) as ObjectResult;

        result.Should().NotBeNull();
        result!.StatusCode.Should().Be(401);

        _userManagerMock.Verify(u => u.FindByEmailAsync(request.Email), Times.Once);
        _userManagerMock.Verify(u => u.CheckPasswordAsync(user, request.Password), Times.Once);
        _jwtServiceMock.Verify(j => j.GenerateTokenAsync(It.IsAny<IdentityUser>()), Times.Never);
    }
}
