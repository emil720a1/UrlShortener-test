namespace UrlShortener.Domain.Entities;

public class AboutContent
{
    public int Id { get; private set; }
    public string Content { get; private set; } = string.Empty;
    public DateTime LastUpdated { get; private set; }

    public AboutContent(string content)
    {
        Id = 1;
        Content = content ?? string.Empty;
        LastUpdated = DateTime.UtcNow;
    }

    public void UpdateContent(string newContent)
    {
        Content = newContent ?? string.Empty;
        LastUpdated = DateTime.UtcNow;
    }
    
    protected AboutContent(){ }
}