using ProjectCinema.BLL.DTO.Users;

namespace ProjectCinema.BLL.Interfaces
{
    /// <summary>
    /// Interface for authentication operations
    /// Handles user registration, login, and token management
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// Register a new user with hashed password
        /// </summary>
        /// <param name="registerDto">User registration data</param>
        /// <returns>Authentication response with tokens</returns>
        Task<AuthResponseDTO> RegisterAsync(RegisterDTO registerDto);

        /// <summary>
        /// Authenticate user and generate tokens
        /// </summary>
        /// <param name="loginDto">User login credentials</param>
        /// <returns>Authentication response with tokens</returns>
        Task<AuthResponseDTO> LoginAsync(LoginUserDTO loginDto);

        /// <summary>
        /// Refresh access token using refresh token
        /// </summary>
        /// <param name="refreshToken">Valid refresh token</param>
        /// <returns>New authentication response with fresh tokens</returns>
        Task<AuthResponseDTO> RefreshTokenAsync(string refreshToken);

        /// <summary>
        /// Validate JWT access token
        /// </summary>
        /// <param name="token">JWT token to validate</param>
        /// <returns>True if token is valid</returns>
        Task<bool> ValidateTokenAsync(string token);

        /// <summary>
        /// Revoke refresh token (logout)
        /// </summary>
        /// <param name="userId">User ID to logout</param>
        /// <returns>True if successful</returns>
        Task<bool> LogoutAsync(int userId);
    }
}
