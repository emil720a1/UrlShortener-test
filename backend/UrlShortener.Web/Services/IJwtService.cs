using Microsoft.AspNetCore.Identity;

namespace UrlShortener.Web.Services;

public interface IJwtService
{
    Task<string> GenerateTokenAsync(IdentityUser user);
}
