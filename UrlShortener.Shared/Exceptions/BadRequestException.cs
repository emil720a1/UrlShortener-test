using System.Text.Json;

namespace UrlShortener.Shared.Exceptions;

public class BadRequestException : Exception
{
    protected BadRequestException(Error[] errors) 
        : base(JsonSerializer.Serialize(errors))
    {
        
    }
}