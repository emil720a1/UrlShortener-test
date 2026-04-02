using UrlShortener.Domain.Entities;

namespace UrlShortener.Application;

public interface IAboutRepository
{
    Task<AboutContent?> GetAsync();
    Task UpdateAsync(AboutContent content);
    Task AddAsync(AboutContent content);
    Task SaveChangesAsync();
}
