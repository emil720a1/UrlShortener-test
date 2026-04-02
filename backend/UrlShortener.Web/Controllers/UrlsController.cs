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

    [AllowAnonymous]
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
    
    [AllowAnonymous]
    [HttpGet("all")]
    public async Task<IActionResult> GetAllUrls()
    {
        var result = await _urlService.GetAllUrlsAsync();
        if (result.IsFailure)
            return BadRequest(result.Error);

        return Ok(result.Value);
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetUserUrls()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var result = await _urlService.GetUrlsByUserIdAsync(userId);
        if (result.IsFailure)
            return BadRequest(result.Error);

        return Ok(result.Value);
    }

    [Authorize]
    [HttpGet("info/{code}")]
    public async Task<IActionResult> GetUrlInfo(string code)
    {
        var result = await _urlService.GetUrlByCodeAsync(code);
        if (result.IsFailure)
            return NotFound(result.Error);

        return Ok(result.Value);
    }

    [Authorize]
    [HttpDelete("{code}")]
    public async Task<IActionResult> DeleteUrl(string code)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var isAdmin = User.IsInRole("Admin");
        var result = await _urlService.DeleteUrlAsync(code, userId, isAdmin);
        
        if (result.IsFailure)
        {
            var errorCode = result.Error.FirstOrDefault()?.Code;
            if (errorCode == "url.not_found")
                return NotFound(result.Error);
            if (errorCode == "url.unauthorized")
                return StatusCode(403, result.Error);
                
            return BadRequest(result.Error);
        }

        return NoContent();
    }
}