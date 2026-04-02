using UrlShortener.Domain;
using UrlShortener.Domain;
using UrlShortener.Domain.Entities;

namespace UrlShortener.Application;

public interface IUrlRepository
{
    Task<UrlRecord?> GetByShortCodeAsync(string shortCode);
    Task<UrlRecord?> GetByOriginalUrlAsync(string originalUrl);
    Task<IEnumerable<UrlRecord>> GetAllAsync();

    Task AddAsync(UrlRecord urlRecord);
    Task DeleteAsync(UrlRecord urlRecord);
    Task SaveChangesAsync();
}