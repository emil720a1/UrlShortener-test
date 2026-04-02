using CSharpFunctionalExtensions;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using NUnit.Framework;
using UrlShortener.Application;
using UrlShortener.Application.Services;
using UrlShortener.Domain.Entities;

namespace UrlShortener.Tests;

[TestFixture]
public class UrlShortenerServiceTests
{
    private Mock<IUrlRepository> _urlRepositoryMock;
    private Mock<IValidator<string>> _validatorMock;
    private IMemoryCache _cache; // Using actual MemoryCache instead of Mock
    private UrlShortenerService _service;

    [SetUp]
    public void SetUp()
    {
        _urlRepositoryMock = new Mock<IUrlRepository>();
        _validatorMock = new Mock<IValidator<string>>();
        
        // Proper setup for MemoryCache
        _cache = new MemoryCache(new MemoryCacheOptions());

        _service = new UrlShortenerService(_urlRepositoryMock.Object, _validatorMock.Object, _cache);
    }

    [TearDown]
    public void TearDown()
    {
        _cache.Dispose();
    }

    [Test]
    public async Task ShortenUrlAsync_WhenUrlIsInvalid_ReturnsFailureResult()
    {
        // Arrange
        var invalidUrl = "not-a-url";
        var userId = "test-user-id";
        
        var validationFailure = new ValidationFailure("Url", "Invalid URL format.");
        _validatorMock.Setup(v => v.ValidateAsync(invalidUrl, default))
            .ReturnsAsync(new ValidationResult(new[] { validationFailure }));

        // Act
        var result = await _service.ShortenUrlAsync(invalidUrl, userId);
        
        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.First().Code.Should().Be("url.invalid");
        
        _urlRepositoryMock.Verify(r => r.AddAsync(It.IsAny<UrlRecord>()), Times.Never);
        _urlRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Never);  
    }
    
    [Test]
    public async Task ShortenUrlAsync_WhenUrlIsValid_ReturnsSuccessResult()
    {
        // Arrange
        var validUrl = "https://dou.ua";
        var userId = "test-user-id";

        _validatorMock.Setup(v => v.ValidateAsync(validUrl, default))
            .ReturnsAsync(new ValidationResult()); // Valid result

        _urlRepositoryMock.Setup(r => r.GetByOriginalUrlAsync(validUrl))
            .ReturnsAsync((UrlRecord?)null);

        _urlRepositoryMock.Setup(r => r.GetByShortCodeAsync(It.IsAny<string>()))
            .ReturnsAsync((UrlRecord?)null);
        
        // Act
        var result = await _service.ShortenUrlAsync(validUrl, userId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNullOrWhiteSpace();
        result.Value.Length.Should().Be(6);
        
        _urlRepositoryMock.Verify(r => r.AddAsync(It.IsAny<UrlRecord>()), Times.Once);
        _urlRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Test]
    public async Task ShortenUrlAsync_WhenUrlAlreadyExists_ReturnsExistingShortCode()
    {
        // Arrange
        var validUrl = "https://example.com";
        var userId = "test-user-id";
        var existingRecord = new UrlRecord(validUrl, "a1b2c3", userId);

        _validatorMock.Setup(v => v.ValidateAsync(validUrl, default))
            .ReturnsAsync(new ValidationResult()); 

        _urlRepositoryMock.Setup(r => r.GetByOriginalUrlAsync(validUrl))
            .ReturnsAsync(existingRecord);

        // Act
        var result = await _service.ShortenUrlAsync(validUrl, userId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be("a1b2c3");
        
        _urlRepositoryMock.Verify(r => r.AddAsync(It.IsAny<UrlRecord>()), Times.Never);
        _urlRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Test]
    public async Task GetOriginalUrlAsync_WhenFoundInDb_ReturnsOriginalUrlAndCachesIt()
    {
        // Arrange
        var shortCode = "123456";
        var originalUrl = "https://example.com";
        var record = new UrlRecord(originalUrl, shortCode, "user1");
        
        _urlRepositoryMock.Setup(r => r.GetByShortCodeAsync(shortCode))
            .ReturnsAsync(record);

        // Act
        var result = await _service.GetOriginalUrlAsync(shortCode);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(originalUrl);
        
        // Verify it was saved to cache
        _cache.TryGetValue($"url:{shortCode}", out string? cachedUrl).Should().BeTrue();
        cachedUrl.Should().Be(originalUrl);
        
        _urlRepositoryMock.Verify(r => r.GetByShortCodeAsync(shortCode), Times.Once);
    }

    [Test]
    public async Task GetOriginalUrlAsync_WhenNotFound_ReturnsFailure()
    {
        // Arrange
        var shortCode = "notfnd";
        _urlRepositoryMock.Setup(r => r.GetByShortCodeAsync(shortCode))
            .ReturnsAsync((UrlRecord?)null);

        // Act
        var result = await _service.GetOriginalUrlAsync(shortCode);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.First().Code.Should().Be("url.not_found");
        
        _urlRepositoryMock.Verify(r => r.GetByShortCodeAsync(shortCode), Times.Once);
    }

    [Test]
    public async Task GetOriginalUrlAsync_WhenInCache_DoesNotCallDatabase()
    {
        // Arrange
        var shortCode = "incach";
        var originalUrl = "https://cached.com";
        
        // Pre-populate cache
        _cache.Set($"url:{shortCode}", originalUrl);

        // Act
        var result = await _service.GetOriginalUrlAsync(shortCode);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(originalUrl);
        
        // Ensure Database was never hit!
        _urlRepositoryMock.Verify(r => r.GetByShortCodeAsync(It.IsAny<string>()), Times.Never);
    }
}