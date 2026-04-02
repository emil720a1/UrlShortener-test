using CSharpFunctionalExtensions;
using FluentValidation;
using Microsoft.Extensions.Caching.Memory;
using UrlShortener.Domain;
using UrlShortener.Domain;
using UrlShortener.Domain.Entities;
using UrlShortener.Shared;

namespace UrlShortener.Application.Services;

public class UrlShortenerService : IUrlShortenerService
{
    private readonly IUrlRepository _urlRepository;
    private readonly IValidator<string> _validator;
    private readonly IMemoryCache _cache;
    
    public UrlShortenerService(
        IUrlRepository urlRepository,
        IValidator<string> validator,
        IMemoryCache cache)
    {
        _urlRepository = urlRepository;
        _validator = validator;
        _cache = cache;
    }
    
    public async Task<Result<string, Failure>> ShortenUrlAsync(string originalUrl, string userId)
    {
        var validationResult = await _validator.ValidateAsync(originalUrl);
        if (!validationResult.IsValid)
        {
            var errorMsg = validationResult.Errors.First().ErrorMessage;
            return Result.Failure<string, Failure>(Error.Validation("url.invalid", errorMsg).ToFailure());
        }
        
        var existing = await _urlRepository.GetByOriginalUrlAsync(originalUrl);
        if (existing != null) return existing.ShortCode;

        string shortCode;
        do
        {
            shortCode = Guid.NewGuid().ToString("N").Substring(0, 6);
        } while (await _urlRepository.GetByShortCodeAsync(shortCode) != null);

        var urlRecord = new UrlRecord(originalUrl, shortCode, userId);
        
        await _urlRepository.AddAsync(urlRecord);
        await _urlRepository.SaveChangesAsync();
        
        return Result.Success<string, Failure>(shortCode);
    }

    public async Task<Result<string, Failure>> GetOriginalUrlAsync(string shortCode)
    {
        string cacheKey = $"url:{shortCode}";

        if (_cache.TryGetValue(cacheKey, out string? cachedUrl))
        {
            return Result.Success<string, Failure>(cachedUrl);
        }

        var urlRecord = await _urlRepository.GetByShortCodeAsync(shortCode);

        if (urlRecord == null)
        {
            return Result.Failure<string, Failure>
            (Error.NotFound("url.not_found", "Unfortunately, the link is not found", null).ToFailure());
        }

        var cacheOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromHours(1));

        _cache.Set(cacheKey, urlRecord.OriginalUrl, cacheOptions);

        return Result.Success<string, Failure>(urlRecord.OriginalUrl);
    }
}