using System.ComponentModel.DataAnnotations;

namespace ProjectCinema.BLL.DTO.Users
{
    public class RefreshTokenDTO
    {
        [Required(ErrorMessage = "Refresh token is required")]
        public string RefreshToken { get; set; } = null!;
    }
}

