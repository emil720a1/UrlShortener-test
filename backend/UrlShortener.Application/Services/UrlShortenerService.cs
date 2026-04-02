using CSharpFunctionalExtensions;
using FluentValidation;
using Microsoft.Extensions.Caching.Memory;
using UrlShortener.Domain.Entities;
using UrlShortener.Shared;

namespace UrlShortener.Application.Services;

public class UrlShortenerService : IUrlShortenerService
{
    private const string Base62Chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
    private const int ShortCodeLength = 6;

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
        if (existing != null)
            return Result.Failure<string, Failure>(Error.Validation("url.exists", "This URL has already been shortened!").ToFailure());

        string shortCode;
        do
        {
            shortCode = GenerateBase62Code();
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
            return Result.Success<string, Failure>(cachedUrl!);
        }

        var urlRecord = await _urlRepository.GetByShortCodeAsync(shortCode);

        if (urlRecord == null)
        {
            return Result.Failure<string, Failure>(
                Error.NotFound("url.not_found", "Unfortunately, the link is not found", null).ToFailure());
        }

        var cacheOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromHours(1));

        _cache.Set(cacheKey, urlRecord.OriginalUrl, cacheOptions);

        return Result.Success<string, Failure>(urlRecord.OriginalUrl);
    }

    public async Task<Result<IEnumerable<UrlRecord>, Failure>> GetAllUrlsAsync()
    {
        var urls = await _urlRepository.GetAllAsync();
        return Result.Success<IEnumerable<UrlRecord>, Failure>(urls);
    }

    public async Task<Result<IEnumerable<UrlRecord>, Failure>> GetUrlsByUserIdAsync(string userId)
    {
        var urls = await _urlRepository.GetAllByUserIdAsync(userId);
        return Result.Success<IEnumerable<UrlRecord>, Failure>(urls);
    }

    public async Task<Result<UrlRecord, Failure>> GetUrlByCodeAsync(string shortCode)
    {
        var urlRecord = await _urlRepository.GetByShortCodeAsync(shortCode);
        if (urlRecord == null)
            return Result.Failure<UrlRecord, Failure>(Error.NotFound("url.not_found", "Link not found", null).ToFailure());

        return Result.Success<UrlRecord, Failure>(urlRecord);
    }

    public async Task<Result<bool, Failure>> DeleteUrlAsync(string shortCode, string userId, bool isAdmin)
    {
        var urlRecord = await _urlRepository.GetByShortCodeAsync(shortCode);

        if (urlRecord == null)
            return Result.Failure<bool, Failure>(Error.NotFound("url.not_found", "url not found", null).ToFailure());

        if (urlRecord.CreatedByUserId != userId && !isAdmin)
            return Result.Failure<bool, Failure>(Error.Validation("url.unauthorized", "No delete permissions", null).ToFailure());

        await _urlRepository.DeleteAsync(urlRecord);
        await _urlRepository.SaveChangesAsync();

        string cacheKey = $"url:{shortCode}";
        _cache.Remove(cacheKey);

        return Result.Success<bool, Failure>(true);
    }

    /// <summary>
    /// Generates a cryptographically random Base62 short code.
    /// Provides 62^6 ≈ 56 billion unique combinations vs 36^6 ≈ 2B for hex.
    /// </summary>
    private static string GenerateBase62Code()
    {
        var bytes = new byte[ShortCodeLength];
        System.Security.Cryptography.RandomNumberGenerator.Fill(bytes);

        var chars = new char[ShortCodeLength];
        for (int i = 0; i < ShortCodeLength; i++)
        {
            chars[i] = Base62Chars[bytes[i] % Base62Chars.Length];
        }
        return new string(chars);
    }
}