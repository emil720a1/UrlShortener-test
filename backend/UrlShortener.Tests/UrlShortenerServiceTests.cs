using CSharpFunctionalExtensions;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using UrlShortener.Application;
using UrlShortener.Application.Services;
using UrlShortener.Domain.Entities;
using Xunit;

namespace UrlShortener.Tests;

public class UrlShortenerServiceTests
{
    private readonly Mock<IUrlRepository> _urlRepositoryMock;
    private readonly Mock<IValidator<string>> _validatorMock;
    private readonly IMemoryCache _cache;
    private readonly UrlShortenerService _service;

    public UrlShortenerServiceTests()
    {
        _urlRepositoryMock = new Mock<IUrlRepository>();
        _validatorMock = new Mock<IValidator<string>>();
        _cache = new MemoryCache(new MemoryCacheOptions());

        _service = new UrlShortenerService(_urlRepositoryMock.Object, _validatorMock.Object, _cache);
    }

    [Fact]
    public async Task ShortenUrlAsync_ShouldGenerate6CharResult_WhenValid()
    {
        // Arrange
        var url = "https://google.com";
        _validatorMock.Setup(v => v.ValidateAsync(url, default))
            .ReturnsAsync(new ValidationResult());
        _urlRepositoryMock.Setup(r => r.GetByOriginalUrlAsync(url))
            .ReturnsAsync((UrlRecord?)null);
        _urlRepositoryMock.Setup(r => r.GetByShortCodeAsync(It.IsAny<string>()))
            .ReturnsAsync((UrlRecord?)null);

        // Act
        var result = await _service.ShortenUrlAsync(url, "user-1");

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveLength(6);
    }

    [Fact]
    public async Task ShortenUrlAsync_ShouldReturnFailure_WhenUrlInvalid()
    {
        // Arrange
        var url = "invalid-url";
        _validatorMock.Setup(v => v.ValidateAsync(url, default))
            .ReturnsAsync(new ValidationResult(new[] { new ValidationFailure("Url", "Invalid") }));

        // Act
        var result = await _service.ShortenUrlAsync(url, "user-1");

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.First().Code.Should().Be("url.invalid");
    }

    [Fact]
    public async Task DeleteUrlAsync_ShouldReturnFailure_WhenUserNotOwnerAndNotAdmin()
    {
        // Arrange
        var code = "abc123";
        var record = new UrlRecord("https://x.com", code, "owner-id");
        _urlRepositoryMock.Setup(r => r.GetByShortCodeAsync(code))
            .ReturnsAsync(record);

        // Act
        var result = await _service.DeleteUrlAsync(code, "other-user-id", false);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.First().Code.Should().Be("url.unauthorized");
    }

    [Fact]
    public async Task DeleteUrlAsync_ShouldReturnSuccess_WhenUserIsAdminEvenIfNotOwner()
    {
        // Arrange
        var code = "abc123";
        var record = new UrlRecord("https://x.com", code, "owner-id");
        _urlRepositoryMock.Setup(r => r.GetByShortCodeAsync(code))
            .ReturnsAsync(record);

        // Act
        var result = await _service.DeleteUrlAsync(code, "admin-id", true);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _urlRepositoryMock.Verify(r => r.DeleteAsync(record), Times.Once);
    }
    
    [Fact]
    public async Task DeleteUrlAsync_ShouldReturnSuccess_WhenUserIsOwner()
    {
        // Arrange
        var code = "abc123";
        var userId = "owner-id";
        var record = new UrlRecord("https://x.com", code, userId);
        _urlRepositoryMock.Setup(r => r.GetByShortCodeAsync(code))
            .ReturnsAsync(record);

        // Act
        var result = await _service.DeleteUrlAsync(code, userId, false);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _urlRepositoryMock.Verify(r => r.DeleteAsync(record), Times.Once);
    }
}