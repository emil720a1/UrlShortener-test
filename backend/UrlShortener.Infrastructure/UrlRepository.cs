using Microsoft.EntityFrameworkCore;
using UrlShortener.Application;
using UrlShortener.Domain;
using UrlShortener.Domain.Entities;

namespace UrlShortener.Infrastructure;

public class UrlRepository : IUrlRepository
{
    private readonly ApplicationDbContext _context;

    public UrlRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<UrlRecord?> GetByShortCodeAsync(string shortCode) =>
        await _context.Urls.FirstOrDefaultAsync(u => u.ShortCode == shortCode);
    

    public async Task<UrlRecord?> GetByOriginalUrlAsync(string originalUrl) => 
        await _context.Urls.FirstOrDefaultAsync(u => u.OriginalUrl == originalUrl);

    public async Task<IEnumerable<UrlRecord>> GetAllAsync() => 
        await _context.Urls.OrderByDescending(u => u.CreatedAt).ToListAsync();
    
    public async Task AddAsync(UrlRecord urlRecord) =>
        await _context.Urls.AddAsync(urlRecord);

    public async Task DeleteAsync(UrlRecord urlRecord)
    {
        _context.Urls.Remove(urlRecord);
        await Task.CompletedTask;
    }

    public async Task SaveChangesAsync() =>
        await _context.SaveChangesAsync();
}
