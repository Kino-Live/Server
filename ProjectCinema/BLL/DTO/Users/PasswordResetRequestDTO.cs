using System.ComponentModel.DataAnnotations;

namespace ProjectCinema.BLL.DTO.Users
{
    /// <summary>
    /// DTO for password reset request
    /// </summary>
    public class PasswordResetRequestDTO
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = null!;

        /// <summary>
        /// Optional redirect URL for web/mobile clients. Must be in AllowedRedirectHosts whitelist.
        /// If not provided, DefaultRedirectUrl from configuration will be used.
        /// </summary>
        public string? RedirectUrl { get; set; }
    }
}

