using System.Security.Claims;
using CSharpFunctionalExtensions;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using UrlShortener.Application;
using UrlShortener.Application.DTO_s;
using UrlShortener.Shared;
using UrlShortener.Web.Controllers;
using Xunit;

namespace UrlShortener.Tests;

public class UrlsControllerTests
{
    private readonly Mock<IUrlShortenerService> _serviceMock;
    private readonly UrlsController _controller;

    public UrlsControllerTests()
    {
        _serviceMock = new Mock<IUrlShortenerService>();
        _controller = new UrlsController(_serviceMock.Object);
    }

    private void SetupControllerUser(string userId)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId)
        };
        var identity = new ClaimsIdentity(claims, "TestAuthType");
        var claimsPrincipal = new ClaimsPrincipal(identity);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };
    }

    [Fact]
    public async Task Shorten_WhenServiceSucceeds_ReturnsOkWithShortCode()
    {
        var userId = "test-user";
        SetupControllerUser(userId);
        
        var request = new ShortenUrlRequest("https://example.com");
        var shortCode = "a1b2c3";
        
        _serviceMock.Setup(s => s.ShortenUrlAsync(request.OriginalUrl, userId))
            .ReturnsAsync(Result.Success<string, Failure>(shortCode));

        var result = await _controller.Shorten(request) as OkObjectResult;

        result.Should().NotBeNull();
        result!.StatusCode.Should().Be(200);
        
        _serviceMock.Verify(s => s.ShortenUrlAsync(request.OriginalUrl, userId), Times.Once);
    }

    [Fact]
    public async Task Shorten_WhenServiceFails_ReturnsBadRequest()
    {
        var userId = "test-user";
        SetupControllerUser(userId);
        
        var request = new ShortenUrlRequest("invalid-url");
        var error = Error.Validation("url.invalid", "Invalid URL");

        _serviceMock.Setup(s => s.ShortenUrlAsync(request.OriginalUrl, userId))
            .ReturnsAsync(Result.Failure<string, Failure>(error.ToFailure()));

        var result = await _controller.Shorten(request) as BadRequestObjectResult;

        result.Should().NotBeNull();
        result!.StatusCode.Should().Be(400);
        
        _serviceMock.Verify(s => s.ShortenUrlAsync(request.OriginalUrl, userId), Times.Once);
    }

    [Fact]
    public async Task RedirectTo_WhenUrlExists_ReturnsRedirect()
    {
        var shortCode = "a1b2c3";
        var originalUrl = "https://example.com";
        
        _serviceMock.Setup(s => s.GetOriginalUrlAsync(shortCode))
            .ReturnsAsync(Result.Success<string, Failure>(originalUrl));

        var result = await _controller.RedirectTo(shortCode) as RedirectResult;

        result.Should().NotBeNull();
        result!.Url.Should().Be(originalUrl);
        
        _serviceMock.Verify(s => s.GetOriginalUrlAsync(shortCode), Times.Once);
    }

    [Fact]
    public async Task RedirectTo_WhenUrlDoesNotExist_ReturnsNotFound()
    {
        var shortCode = "notfnd";
        var error = Error.NotFound("url.not_found", "Not found", null);
        
        _serviceMock.Setup(s => s.GetOriginalUrlAsync(shortCode))
            .ReturnsAsync(Result.Failure<string, Failure>(error.ToFailure()));

        var result = await _controller.RedirectTo(shortCode) as NotFoundObjectResult;

        result.Should().NotBeNull();
        result!.StatusCode.Should().Be(404);
        
        _serviceMock.Verify(s => s.GetOriginalUrlAsync(shortCode), Times.Once);
    }
}
