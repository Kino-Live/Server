using ProjectCinema.BLL.DTO.Users;

namespace ProjectCinema.BLL.Interfaces
{
    /// <summary>
    /// Interface for password reset operations
    /// Handles password reset token generation, validation, and password change
    /// </summary>
    public interface IPasswordResetService
    {
        /// <summary>
        /// Request password reset - generates token, saves it, and sends email
        /// </summary>
        /// <param name="dto">Password reset request data</param>
        /// <param name="requestIp">IP address of the request</param>
        /// <param name="userAgent">User agent of the request</param>
        /// <returns>Task representing the operation. Always succeeds (doesn't reveal if email exists)</returns>
        Task RequestPasswordResetAsync(PasswordResetRequestDTO dto, string? requestIp, string? userAgent);

        /// <summary>
        /// Validate password reset token
        /// </summary>
        /// <param name="token">Reset token to validate</param>
        /// <returns>True if token is valid, false otherwise</returns>
        Task<bool> ValidateTokenAsync(string token);

        /// <summary>
        /// Confirm password reset - changes password and marks token as used
        /// </summary>
        /// <param name="dto">Password reset confirmation data</param>
        /// <returns>True if successful, false if token is invalid/expired/used</returns>
        Task<bool> ConfirmPasswordResetAsync(PasswordResetConfirmDTO dto);
    }
}

