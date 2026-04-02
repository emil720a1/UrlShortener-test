using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using UrlShortener.Application.Services;
using UrlShortener.Application.Validators;

namespace UrlShortener.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IUrlShortenerService, UrlShortenerService>();
        
        services.AddScoped<IValidator<string>, UrlValidator>();
        
        return services;
    }
}