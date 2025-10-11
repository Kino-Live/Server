using System.ComponentModel.DataAnnotations;

namespace ProjectCinema.BLL.DTO.Users
{
    public class LoginUserDTO
    {
        [Required]
        public string? Username { get; set; }
        [Required]
        [EmailAddress]
        public string? Email { get; set; }
        [Required]
        public int Password { get; set; }
    }
}
