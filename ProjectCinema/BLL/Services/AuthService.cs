using ProjectCinema.BLL.DTO.Users;
using ProjectCinema.BLL.Interfaces;
using ProjectCinema.Entities;
using ProjectCinema.Enums;
using ProjectCinema.Repositories.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace ProjectCinema.BLL.Services
{
    /// <summary>
    /// Service for user authentication operations
    /// Handles registration, login, password hashing, and token management
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtTokenService _jwtTokenService;

        public AuthService(IUserRepository userRepository, IJwtTokenService jwtTokenService)
        {
            _userRepository = userRepository;
            _jwtTokenService = jwtTokenService;
        }

        /// <summary>
        /// Register a new user with hashed password
        /// </summary>
        public async Task<AuthResponseDTO> RegisterAsync(RegisterDTO registerDto)
        {
            // Check if user already exists
            var existingUserByEmail = await _userRepository.GetByEmailAsync(registerDto.Email);
            if (existingUserByEmail != null)
            {
                throw new InvalidOperationException("User with this email already exists");
            }

            var existingUserByUsername = await _userRepository.GetByUsernameAsync(registerDto.Username);
            if (existingUserByUsername != null)
            {
                throw new InvalidOperationException("User with this username already exists");
            }

            // Generate salt and hash password
            var (passwordHash, passwordSalt) = HashPassword(registerDto.Password);

            // Create new user
            var user = new User
            {
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                Username = registerDto.Username,
                Email = registerDto.Email,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                PhoneNumber = registerDto.PhoneNumber,
                DateOfBirth = registerDto.DateOfBirth,
                UserRole = UserRole.User, // All registered users are regular users
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            // Add user to database
            await _userRepository.AddAsync(user);
            await _userRepository.SaveAsync();

            // Generate tokens
            var accessToken = _jwtTokenService.GenerateAccessToken(user.UserId, user.Email, user.Username, user.UserRole);
            var refreshToken = _jwtTokenService.GenerateRefreshToken();

            // Update user with refresh token
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiry = DateTime.Now.AddDays(7); // Refresh token valid for 7 days
            await _userRepository.UpdateAsync(user);
            await _userRepository.SaveAsync();

            // Return authentication response
            return new AuthResponseDTO
            {
                Token = accessToken,
                RefreshToken = refreshToken,
                UserId = user.UserId,
                Email = user.Email,
                Username = user.Username,
                Role = user.UserRole,
                ExpiresAt = DateTime.Now.AddHours(1) // Access token valid for 1 hour
            };
        }

        /// <summary>
        /// Authenticate user and generate tokens
        /// </summary>
        public async Task<AuthResponseDTO> LoginAsync(LoginUserDTO loginDto)
        {
            // Find user by email or username
            var user = await _userRepository.GetByEmailAsync(loginDto.UsernameOrEmail) ??
                      await _userRepository.GetByUsernameAsync(loginDto.UsernameOrEmail);

            if (user == null)
            {
                throw new UnauthorizedAccessException("Invalid credentials");
            }

            // Verify password
            if (!VerifyPassword(loginDto.Password, user.PasswordHash, user.PasswordSalt))
            {
                throw new UnauthorizedAccessException("Invalid credentials");
            }

            // Generate tokens
            var accessToken = _jwtTokenService.GenerateAccessToken(user.UserId, user.Email, user.Username, user.UserRole);
            var refreshToken = _jwtTokenService.GenerateRefreshToken();

            // Update user with new refresh token
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiry = DateTime.Now.AddDays(7);
            user.UpdatedAt = DateTime.Now;
            await _userRepository.UpdateAsync(user);
            await _userRepository.SaveAsync();

            // Return authentication response
            return new AuthResponseDTO
            {
                Token = accessToken,
                RefreshToken = refreshToken,
                UserId = user.UserId,
                Email = user.Email,
                Username = user.Username,
                Role = user.UserRole,
                ExpiresAt = DateTime.Now.AddHours(1)
            };
        }

        /// <summary>
        /// Refresh access token using refresh token
        /// </summary>
        public async Task<AuthResponseDTO> RefreshTokenAsync(string refreshToken)
        {
            // Find user by refresh token
            var user = await _userRepository.GetByRefreshTokenAsync(refreshToken);
            if (user == null)
            {
                throw new UnauthorizedAccessException("Invalid refresh token");
            }

            // Check if refresh token is expired
            if (user.RefreshTokenExpiry < DateTime.Now)
            {
                throw new UnauthorizedAccessException("Refresh token expired");
            }

            // Generate new tokens
            var newAccessToken = _jwtTokenService.GenerateAccessToken(user.UserId, user.Email, user.Username, user.UserRole);
            var newRefreshToken = _jwtTokenService.GenerateRefreshToken();

            // Update user with new refresh token
            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiry = DateTime.Now.AddDays(7);
            user.UpdatedAt = DateTime.Now;
            await _userRepository.UpdateAsync(user);
            await _userRepository.SaveAsync();

            // Return new authentication response
            return new AuthResponseDTO
            {
                Token = newAccessToken,
                RefreshToken = newRefreshToken,
                UserId = user.UserId,
                Email = user.Email,
                Username = user.Username,
                Role = user.UserRole,
                ExpiresAt = DateTime.Now.AddHours(1)
            };
        }

        /// <summary>
        /// Validate JWT access token
        /// </summary>
        public async Task<bool> ValidateTokenAsync(string token)
        {
            return _jwtTokenService.ValidateToken(token);
        }

        /// <summary>
        /// Revoke refresh token (logout)
        /// </summary>
        public async Task<bool> LogoutAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return false;
            }

            // Clear refresh token
            user.RefreshToken = null;
            user.RefreshTokenExpiry = null;
            user.UpdatedAt = DateTime.Now;
            await _userRepository.UpdateAsync(user);
            await _userRepository.SaveAsync();

            return true;
        }

        /// <summary>
        /// Hash password with salt
        /// </summary>
        private (byte[] hash, byte[] salt) HashPassword(string password)
        {
            // Generate random salt
            var salt = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            // Hash password with salt
            var passwordBytes = Encoding.UTF8.GetBytes(password);
            var saltedPassword = new byte[passwordBytes.Length + salt.Length];
            Array.Copy(passwordBytes, 0, saltedPassword, 0, passwordBytes.Length);
            Array.Copy(salt, 0, saltedPassword, passwordBytes.Length, salt.Length);

            var hash = SHA512.HashData(saltedPassword);

            return (hash, salt);
        }

        /// <summary>
        /// Verify password against stored hash and salt
        /// </summary>
        private bool VerifyPassword(string password, byte[] storedHash, byte[] storedSalt)
        {
            var passwordBytes = Encoding.UTF8.GetBytes(password);
            var saltedPassword = new byte[passwordBytes.Length + storedSalt.Length];
            Array.Copy(passwordBytes, 0, saltedPassword, 0, passwordBytes.Length);
            Array.Copy(storedSalt, 0, saltedPassword, passwordBytes.Length, storedSalt.Length);

            var computedHash = SHA512.HashData(saltedPassword);

            // Compare hashes
            return computedHash.SequenceEqual(storedHash);
        }
    }
}
