using FluentValidation;
using Microsoft.Extensions.Options;
using ProjectCinema.BLL.DTO.Users;
using ProjectCinema.Settings;
using System.Text.RegularExpressions;

namespace ProjectCinema.Validations.UserValidation
{
    /// <summary>
    /// Validator for password reset request
    /// </summary>
    public class PasswordResetRequestDTOValidator : AbstractValidator<PasswordResetRequestDTO>
    {
        public PasswordResetRequestDTOValidator(IOptions<PasswordResetOptions> passwordResetOptions)
        {
            var options = passwordResetOptions.Value;

            RuleFor(r => r.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format")
                .MaximumLength(256).WithMessage("Email is too long");

            RuleFor(r => r.RedirectUrl)
                .Must(url => string.IsNullOrEmpty(url) || IsValidUrl(url))
                .WithMessage("Invalid redirect URL format")
                .Must(url => string.IsNullOrEmpty(url) || IsAllowedHost(url, options.AllowedRedirectHosts))
                .WithMessage("Redirect URL host is not in the allowed list")
                .When(r => !string.IsNullOrEmpty(r.RedirectUrl));
        }

        private static bool IsValidUrl(string url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out var uri) &&
                   (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
        }

        private static bool IsAllowedHost(string url, string[] allowedHosts)
        {
            if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
                return false;

            var host = uri.Host.ToLowerInvariant();
            return allowedHosts.Any(allowedHost => 
                host == allowedHost.ToLowerInvariant() || 
                host == $"www.{allowedHost.ToLowerInvariant()}");
        }
    }
}

