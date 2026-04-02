using FluentValidation;

namespace UrlShortener.Application.Validators;

public class UrlValidator : AbstractValidator<string>
{
    public UrlValidator()
    {
        RuleFor(url => url)
            .NotEmpty().WithMessage("URL не може бути порожнім.")
            .Must(BeValidUrl).WithMessage("Неправильний формат посилання. Має бути http:// або https://");
    }

    private bool BeValidUrl(string url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out var outUri)
               && (outUri.Scheme == Uri.UriSchemeHttp || outUri.Scheme == Uri.UriSchemeHttps);
    }
}