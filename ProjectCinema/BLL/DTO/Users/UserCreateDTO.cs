using ProjectCinema.Entities;
using ProjectCinema.Enums;
using System.ComponentModel.DataAnnotations;

namespace ProjectCinema.BLL.DTO.Users
{
    public class UserCreateDTO
    {
        [Required]
        [StringLength(256, MinimumLength = 2, ErrorMessage = "First name must be between 2 and 256 characters")]
        public string FirstName { get; set; } = null!;

        [Required]
        [StringLength(256, MinimumLength = 2, ErrorMessage = "Last name must be between 2 and 256 characters")]
        public string LastName { get; set; } = null!;

        [Required]
        [StringLength(256, MinimumLength = 2, ErrorMessage = "Username must be between 2 and 256 characters")]
        public string Username { get; set; } = null!;

        [Required]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = null!;

        [Phone(ErrorMessage = "Invalid phone number format")]
        public string? PhoneNumber { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 100 characters")]
        public string Password { get; set; } = null!;

        [Required]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Confirm password must be between 6 and 100 characters")]
        public string ConfirmPassword { get; set; } = null!;

        [Required]
        public DateOnly DateOfBirth { get; set; }

        [Required]
        public UserRole UserRole { get; set; }
    }
}
