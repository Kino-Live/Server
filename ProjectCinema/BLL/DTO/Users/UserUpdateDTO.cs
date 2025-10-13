using ProjectCinema.Enums;
using System.ComponentModel.DataAnnotations;

namespace ProjectCinema.BLL.DTO.Users
{
    public class UserUpdateDTO
    {
        [StringLength(256, MinimumLength = 2, ErrorMessage = "First name must be between 2 and 256 characters")]
        public string? FirstName { get; set; }

        [StringLength(256, MinimumLength = 2, ErrorMessage = "Last name must be between 2 and 256 characters")]
        public string? LastName { get; set; }

        [StringLength(256, MinimumLength = 2, ErrorMessage = "Username must be between 2 and 256 characters")]
        public string? Username { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string? Email { get; set; }

        [Phone(ErrorMessage = "Invalid phone number format")]
        public string? PhoneNumber { get; set; }

        public DateOnly? DateOfBirth { get; set; }

        public UserRole? UserRole { get; set; }
    }
}
