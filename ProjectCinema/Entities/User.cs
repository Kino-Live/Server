using ProjectCinema.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectCinema.Entities
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }

        [Required]
        [StringLength(256, MinimumLength = 2)]
        public string FirstName { get; set; } = null!;

        [Required]
        [StringLength(256, MinimumLength = 2)]
        public string LastName { get; set; } = null!;

        [Required]
        [StringLength(256, MinimumLength = 2)]
        public string Username { get; set; } = null!;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        public byte[] PasswordHash { get; set; } = null!;

        [Phone]
        public string PhoneNumber { get; set; } = null!;

        [Required]
        public UserRole UserRole { get; set; }

        public DateOnly DateOfBirth { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public ICollection<Booking>? Bookings { get; set; }
    }
}
