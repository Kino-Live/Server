using ProjectCinema.BLL.DTO.Booking;
using ProjectCinema.BLL.DTO.Review;
using ProjectCinema.BLL.DTO.StreamingAccess;
using ProjectCinema.Entities;
using ProjectCinema.Enums;
using System.ComponentModel.DataAnnotations;

namespace ProjectCinema.BLL.DTO.Users
{
    public class UserDetailsDTO
    {
        public int UserId { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public UserRole UserRole { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public ICollection<BookingDTO>? Bookings { get; set; }
        public ICollection<ReviewDTO>? Reviews { get; set; }
        public ICollection<StreamingAccessDTO>? StreamingAccesses { get; set; }
    }
}
