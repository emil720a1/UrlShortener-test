using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;
using UrlShortener.Application.DTO_s.UserDto_s;
using UrlShortener.Web.Controllers;

namespace UrlShortener.Tests;

[TestFixture]
public class AuthControllerTests
{
    private Mock<UserManager<IdentityUser>> _userManagerMock;
    private IConfiguration _configuration;
    private AuthController _controller;

    [SetUp]
    public void SetUp()
    {
        var storeMock = new Mock<IUserStore<IdentityUser>>();
        _userManagerMock = new Mock<UserManager<IdentityUser>>(
            storeMock.Object, null!, null!, null!, null!, null!, null!, null!, null!);

        var inMemorySettings = new Dictionary<string, string?>
        {
            {"Jwt:Key", "MySuperSecretKeyForTestingThatIsLongEnough!!"},
            {"Jwt:Issuer", "TestIssuer"},
            {"Jwt:Audience", "TestAudience"}
        };
        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        _controller = new AuthController(_userManagerMock.Object, _configuration);
    }

    [Test]
    public async Task Register_WhenSuccess_ReturnsOk()
    {
        var request = new RegisterDto("test@test.com", "Password123!");
        
        _userManagerMock.Setup(u => u.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);

        var result = await _controller.Register(request) as OkObjectResult;

        result.Should().NotBeNull();
        result!.StatusCode.Should().Be(200);
        result.Value.Should().BeEquivalentTo(new { Message = "Реєстрація успішна!" });
        
        _userManagerMock.Verify(u => u.CreateAsync(It.IsAny<IdentityUser>(), request.Password), Times.Once);
    }

    [Test]
    public async Task Register_WhenFails_ReturnsBadRequestWithErrors()
    {
        var request = new RegisterDto("test@test.com", "123");
        var errors = new List<IdentityError>
        {
            new IdentityError { Code = "PasswordTooShort", Description = "Password too short" }
        };
        var identityResult = IdentityResult.Failed(errors.ToArray());

        _userManagerMock.Setup(u => u.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
            .ReturnsAsync(identityResult);

        var result = await _controller.Register(request) as BadRequestObjectResult;

        result.Should().NotBeNull();
        result.StatusCode.Should().Be(400);
        // MVC BadRequest(errors) often returns the exact object or a ProblemDetails
        result.Value.Should().BeEquivalentTo(errors);
        
        _userManagerMock.Verify(u => u.CreateAsync(It.IsAny<IdentityUser>(), request.Password), Times.Once);
    }

    [Test]
    public async Task Login_WhenValidCredentials_ReturnsOkWithToken()
    {
        var request = new LoginDto("test@test.com", "Password123!");
        var user = new IdentityUser { Id = "test-user-id", Email = request.Email, UserName = request.Email };

        _userManagerMock.Setup(u => u.FindByEmailAsync(request.Email))
            .ReturnsAsync(user);

        _userManagerMock.Setup(u => u.CheckPasswordAsync(user, request.Password))
            .ReturnsAsync(true);

        var result = await _controller.Login(request) as OkObjectResult;

        result.Should().NotBeNull();
        result.StatusCode.Should().Be(200);
        
        var responseValue = result.Value;
        var tokenProperty = responseValue?.GetType().GetProperty("Token");
        tokenProperty.Should().NotBeNull();
        
        var tokenValue = tokenProperty?.GetValue(responseValue) as string;
        tokenValue.Should().NotBeNullOrWhiteSpace();
        
        _userManagerMock.Verify(u => u.FindByEmailAsync(request.Email), Times.Once);
        _userManagerMock.Verify(u => u.CheckPasswordAsync(user, request.Password), Times.Once);
    }

    [Test]
    public async Task Login_WhenInvalidCredentials_ReturnsUnauthorized()
    {
        var request = new LoginDto("test@test.com", "WrongPassword");
        var user = new IdentityUser { Id = "test-user-id", Email = request.Email, UserName = request.Email };

        _userManagerMock.Setup(u => u.FindByEmailAsync(request.Email))
            .ReturnsAsync(user);

        _userManagerMock.Setup(u => u.CheckPasswordAsync(user, It.IsAny<string>()))
            .ReturnsAsync(false);

        var result = await _controller.Login(request) as ObjectResult;

        result.Should().NotBeNull();
        result.StatusCode.Should().Be(401);
        result.Value.Should().BeEquivalentTo(new { Message = "Неправильний email або пароль." });
        
        _userManagerMock.Verify(u => u.FindByEmailAsync(request.Email), Times.Once);
        _userManagerMock.Verify(u => u.CheckPasswordAsync(user, request.Password), Times.Once);
    }
}
