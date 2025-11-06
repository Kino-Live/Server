using System.ComponentModel.DataAnnotations;

namespace ProjectCinema.BLL.DTO.Users
{
    /// <summary>
    /// DTO for password reset confirmation
    /// </summary>
    public class PasswordResetConfirmDTO
    {
        [Required(ErrorMessage = "Token is required")]
        public string Token { get; set; } = null!;

        [Required(ErrorMessage = "New password is required")]
        [StringLength(100, MinimumLength = 10, ErrorMessage = "Password must be between 10 and 100 characters")]
        public string NewPassword { get; set; } = null!;

        [Required(ErrorMessage = "Confirm password is required")]
        [StringLength(100, MinimumLength = 10, ErrorMessage = "Confirm password must be between 10 and 100 characters")]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; } = null!;
    }
}

