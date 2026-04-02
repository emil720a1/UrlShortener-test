using Microsoft.EntityFrameworkCore;
using UrlShortener.Application;
using UrlShortener.Domain.Entities;

namespace UrlShortener.Infrastructure.Repositories;

public class AboutRepository : IAboutRepository
{
    private readonly ApplicationDbContext _context;

    public AboutRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<AboutContent?> GetAsync()
    {
        return await _context.AboutContent.FirstOrDefaultAsync();
    }

    public async Task UpdateAsync(AboutContent content)
    {
        _context.AboutContent.Update(content);
    }

    public async Task AddAsync(AboutContent content)
    {
        await _context.AboutContent.AddAsync(content);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
