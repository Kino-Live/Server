using ProjectCinema.Enums;

namespace ProjectCinema.BLL.DTO.Users
{
    /// <summary>
    /// Response after successful authentication (login/register)
    /// Contains tokens and user information that client needs to store
    /// </summary>
    public class AuthResponseDTO
    {
        /// <summary>
        /// JWT Access Token - used for API requests (expires in 15-60 minutes)
        /// Client sends it in Authorization header: "Bearer {token}"
        /// </summary>
        public string Token { get; set; } = null!;

        /// <summary>
        /// Refresh Token - used to get new Access Token when it expires (lasts 7-30 days)
        /// Client stores it and sends to /api/auth/refresh endpoint
        /// </summary>
        public string RefreshToken { get; set; } = null!;

        /// <summary>
        /// User ID - for tracking and future API calls
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// User email - for display purposes
        /// </summary>
        public string Email { get; set; } = null!;

        /// <summary>
        /// Username - for display purposes
        /// </summary>
        public string Username { get; set; } = null!;

        /// <summary>
        /// User role - for UI permissions (User/Admin)
        /// </summary>
        public UserRole Role { get; set; }

        /// <summary>
        /// When the access token expires
        /// Client uses this to know when to refresh
        /// </summary>
        public DateTime ExpiresAt { get; set; }
    }
}

