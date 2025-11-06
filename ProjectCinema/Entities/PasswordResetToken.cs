using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectCinema.Entities
{
    public class PasswordResetToken
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PasswordResetTokenId { get; set; }

        [Required]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public User? User { get; set; }

        [Required]
        [StringLength(64)]
        public string TokenHash { get; set; } = null!;

        [Required]
        public DateTime ExpiresAt { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        public DateTime? UsedAt { get; set; }

        [StringLength(45)]
        public string? RequestIp { get; set; }

        [StringLength(512)]
        public string? UserAgent { get; set; }
    }
}


