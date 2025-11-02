using ProjectCinema.Enums;

namespace ProjectCinema.BLL.Interfaces
{
    /// <summary>
    /// Interface for JWT token operations
    /// Handles token generation, validation, and claims extraction
    /// </summary>
    public interface IJwtTokenService
    {
        /// <summary>
        /// Generate JWT access token for user
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="email">User email</param>
        /// <param name="username">Username</param>
        /// <param name="role">User role</param>
        /// <returns>JWT token string</returns>
        string GenerateAccessToken(int userId, string email, string username, UserRole role);

        /// <summary>
        /// Generate refresh token (random string)
        /// </summary>
        /// <returns>Refresh token string</returns>
        string GenerateRefreshToken();

        /// <summary>
        /// Validate JWT token and extract claims
        /// </summary>
        /// <param name="token">JWT token to validate</param>
        /// <returns>True if token is valid</returns>
        bool ValidateToken(string token);

        /// <summary>
        /// Extract user ID from JWT token
        /// </summary>
        /// <param name="token">JWT token</param>
        /// <returns>User ID or null if invalid</returns>
        int? GetUserIdFromToken(string token);

        /// <summary>
        /// Extract user role from JWT token
        /// </summary>
        /// <param name="token">JWT token</param>
        /// <returns>User role or null if invalid</returns>
        UserRole? GetUserRoleFromToken(string token);

        /// <summary>
        /// Extract email from JWT token
        /// </summary>
        /// <param name="token">JWT token</param>
        /// <returns>Email or null if invalid</returns>
        string? GetEmailFromToken(string token);

        /// <summary>
        /// Check if token is expired
        /// </summary>
        /// <param name="token">JWT token</param>
        /// <returns>True if token is expired</returns>
        bool IsTokenExpired(string token);
    }
}
