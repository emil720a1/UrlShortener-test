using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using UrlShortener.Application;
using UrlShortener.Domain.Entities;

namespace UrlShortener.Web.Pages;

[AllowAnonymous]
public class AboutModel : PageModel
{
    private readonly IAboutRepository _aboutRepository;

    public AboutModel(IAboutRepository aboutRepository)
    {
        _aboutRepository = aboutRepository;
    }

    [BindProperty]
    public string AboutContentValue { get; set; } = string.Empty;

    public DateTime LastUpdated { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var aboutContent = await _aboutRepository.GetAsync();
        
        AboutContentValue = aboutContent?.Content ?? "This URL shortener uses a Clean Architecture approach with ASP.NET Core 9, EF Core, and React (Vite).\n\nShort codes are generated using a cryptographically secure Base62 algorithm (6 characters, ~56 billion unique combinations).\n\nLinks are cached in-memory via IMemoryCache for fast redirect resolution.";
        LastUpdated = aboutContent?.LastUpdated ?? DateTime.UtcNow;
        
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!User.IsInRole("Admin"))
            return Forbid();

        var aboutContent = await _aboutRepository.GetAsync();
        
        if (aboutContent == null)
        {
            aboutContent = new AboutContent(AboutContentValue);
            await _aboutRepository.AddAsync(aboutContent);
        }
        else
        {
            aboutContent.UpdateContent(AboutContentValue);
            await _aboutRepository.UpdateAsync(aboutContent);
        }

        await _aboutRepository.SaveChangesAsync();

        return RedirectToPage();
    }
}
