using CSharpFunctionalExtensions;
using UrlShortener.Domain.Entities;
using UrlShortener.Shared;

namespace UrlShortener.Application;

public interface IUrlShortenerService
{
    Task<Result<string, Failure>> ShortenUrlAsync(string originalUrl, string userId);
    
    Task<Result<string, Failure>> GetOriginalUrlAsync(string shortCode);
    
    Task<Result<IEnumerable<UrlRecord>, Failure>> GetAllUrlsAsync();
    Task<Result<IEnumerable<UrlRecord>, Failure>> GetUrlsByUserIdAsync(string userId);
    Task<Result<UrlRecord, Failure>> GetUrlByCodeAsync(string shortCode);
    Task<Result<bool, Failure>> DeleteUrlAsync(string shortCode, string userId, bool isAdmin);
}