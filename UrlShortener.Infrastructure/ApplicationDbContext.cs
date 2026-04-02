using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UrlShortener.Domain;
using UrlShortener.Domain.Entities;

namespace UrlShortener.Infrastructure;

public class ApplicationDbContext : IdentityDbContext<IdentityUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
        : base(options) { }
    
    public DbSet<UrlRecord> Urls => Set<UrlRecord>();
    public DbSet<AboutContent> AboutContent => Set<AboutContent>();
    
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<UrlRecord>(entity =>
        {
            entity.ToTable("Urls");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.OriginalUrl).IsRequired();
            
            entity.HasIndex(e => e.ShortCode).IsUnique();
            entity.HasIndex(e => e.OriginalUrl).IsUnique();
        });
    }
}