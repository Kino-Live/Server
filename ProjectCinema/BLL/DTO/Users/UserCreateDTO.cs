using ProjectCinema.Entities;
using ProjectCinema.Enums;
using System.ComponentModel.DataAnnotations;

namespace ProjectCinema.BLL.DTO.Users
{
    public class UserCreateDTO
    {
        [Required]
        [StringLength(256, MinimumLength = 2)]
        public string? FirstName { get; set; }
        [Required]
        [StringLength(256, MinimumLength = 2)]
        public string? LastName { get; set; }
        [Required]
        [StringLength(256, MinimumLength = 2)]
        public string? Username { get; set; }
        [Required]
        [EmailAddress]
        public string? Email { get; set; }
        [Phone]
        public string? PhoneNumber { get; set; }
        [Required]
        public int Password { get ; set; }
        [Required]
        public int ConfirmPassword { get; set; }
        [Required]
        public DateOnly DateOfBirth { get; set; }
    }
}
