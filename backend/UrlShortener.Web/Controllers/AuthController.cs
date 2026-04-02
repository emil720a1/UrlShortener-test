using Microsoft.AspNetCore.Mvc;
using UrlShortener.Application.DTO_s.UserDto_s;
using UrlShortener.Web.Services;
using Microsoft.AspNetCore.Identity;

namespace UrlShortener.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IJwtService _jwtService;

    public AuthController(UserManager<IdentityUser> userManager, IJwtService jwtService)
    {
        _userManager = userManager;
        _jwtService = jwtService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto request)
    {
        var user = new IdentityUser { UserName = request.Email, Email = request.Email };
        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
            return BadRequest(result.Errors);

        return Ok(new { Message = "Реєстрація успішна!" });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
            return Unauthorized(new { Message = "Неправильний email або пароль." });

        var roles = await _userManager.GetRolesAsync(user);
        var token = await _jwtService.GenerateTokenAsync(user);

        Response.Cookies.Append("jwt", token, new CookieOptions
        {
            HttpOnly = true,
            Expires = DateTime.UtcNow.AddDays(1),
            SameSite = SameSiteMode.Lax,
            Secure = false // TODO: set to true in production (HTTPS only)
        });

        return Ok(new { Token = token, Roles = roles, UserId = user.Id });
    }
}