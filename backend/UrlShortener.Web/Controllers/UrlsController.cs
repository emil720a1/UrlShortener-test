using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UrlShortener.Application;
using UrlShortener.Application.DTO_s;

namespace UrlShortener.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UrlsController : ControllerBase
{
    private readonly IUrlShortenerService _urlService;

    public UrlsController(IUrlShortenerService urlShortenerService)
    {
        _urlService = urlShortenerService;
    }

    [Authorize]
    [HttpPost("shorten")]
    public async Task<IActionResult> Shorten([FromBody] ShortenUrlRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();
        
        var result = await _urlService.ShortenUrlAsync(request.OriginalUrl, userId);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(new { ShortCode = result.Value });
    }

    [HttpGet("{code}")]
    public async Task<IActionResult> RedirectTo(string code)
    {
        var result = await _urlService.GetOriginalUrlAsync(code);
        if (result.IsFailure)
        {
            return NotFound(result.Error);
        }
        return Redirect(result.Value);
    }
}