using System.Text.Json;

namespace UrlShortener.Shared.Exceptions;

public class NotFoundException : Exception
{
    protected NotFoundException(Error[] errors)
        : base(JsonSerializer.Serialize(errors))
    {
        
    }
}