namespace UrlShortener.Domain.Entities;

public class UrlRecord
{
    public Guid Id { get; private set; }
    public string OriginalUrl { get; private set; } = string.Empty;
    public string ShortCode { get; private set; } = string.Empty;
    
    public string CreatedByUserId { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; }

    public UrlRecord(string originalUrl, string shortCode, string createdByUserId)
    {
        if (string.IsNullOrWhiteSpace(originalUrl))
            throw new ArgumentException("Original URL cannot be empty.", nameof(originalUrl));
        
        if (string.IsNullOrWhiteSpace(shortCode))
            throw new ArgumentException("Short code cannot be empty.", nameof(shortCode));
        
        if (string.IsNullOrWhiteSpace(createdByUserId))
            throw new ArgumentException("User ID must be provided.", nameof(createdByUserId));
        
        Id = Guid.NewGuid();
        OriginalUrl = originalUrl;
        ShortCode = shortCode;
        CreatedByUserId = createdByUserId;
        CreatedAt = DateTime.UtcNow;
    }
    
    protected UrlRecord(){ }
}