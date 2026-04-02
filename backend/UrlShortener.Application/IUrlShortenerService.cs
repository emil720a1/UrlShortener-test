using CSharpFunctionalExtensions;
using UrlShortener.Shared;

namespace UrlShortener.Application;

public interface IUrlShortenerService
{
    Task<Result<string, Failure>> ShortenUrlAsync(string originalUrl, string userId);
    
    Task<Result<string, Failure>> GetOriginalUrlAsync(string shortCode);
}