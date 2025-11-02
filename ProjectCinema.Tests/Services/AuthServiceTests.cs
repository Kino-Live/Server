using Moq;
using ProjectCinema.BLL.DTO.Users;
using ProjectCinema.BLL.Interfaces;
using ProjectCinema.BLL.Services;
using ProjectCinema.Entities;
using ProjectCinema.Enums;
using ProjectCinema.Repositories.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace ProjectCinema.Tests.Services
{
    public class AuthServiceTests
    {
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<IJwtTokenService> _mockJwtTokenService;
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _mockJwtTokenService = new Mock<IJwtTokenService>();
            _authService = new AuthService(_mockUserRepository.Object, _mockJwtTokenService.Object);
        }

        [Fact]
        public async Task RegisterAsync_ShouldSuccessfullyRegisterNewUser()
        {
            // Arrange
            var registerDto = new RegisterDTO
            {
                FirstName = "John",
                LastName = "Doe",
                Username = "johndoe",
                Email = "john@example.com",
                Password = "SecurePass123!",
                ConfirmPassword = "SecurePass123!",
                PhoneNumber = "+1234567890",
                DateOfBirth = new DateOnly(1990, 1, 1)
            };

            _mockUserRepository.Setup(x => x.GetByEmailAsync(registerDto.Email))
                .ReturnsAsync((User?)null);
            _mockUserRepository.Setup(x => x.GetByUsernameAsync(registerDto.Username))
                .ReturnsAsync((User?)null);

            var savedUser = new User();
            _mockUserRepository.Setup(x => x.AddAsync(It.IsAny<User>()))
                .Callback<User>(u => savedUser = u)
                .Returns(Task.CompletedTask);
            _mockUserRepository.Setup(x => x.UpdateAsync(It.IsAny<User>()))
                .Callback<User>(u => savedUser = u)
                .Returns(Task.CompletedTask);
            _mockUserRepository.Setup(x => x.SaveAsync())
                .Returns(Task.CompletedTask);

            _mockJwtTokenService.Setup(x => x.GenerateAccessToken(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<UserRole>()))
                .Returns("access_token_string");
            _mockJwtTokenService.Setup(x => x.GenerateRefreshToken())
                .Returns("refresh_token_string");

            // Act
            var result = await _authService.RegisterAsync(registerDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("access_token_string", result.Token);
            Assert.Equal("refresh_token_string", result.RefreshToken);
            Assert.Equal(registerDto.Email, result.Email);
            Assert.Equal(registerDto.Username, result.Username);
            Assert.Equal(UserRole.User, result.Role);
            Assert.True((result.ExpiresAt - DateTime.Now).TotalHours <= 1.1 && (result.ExpiresAt - DateTime.Now).TotalHours >= 0.9);

            _mockUserRepository.Verify(x => x.AddAsync(It.IsAny<User>()), Times.Once);
            _mockUserRepository.Verify(x => x.UpdateAsync(It.IsAny<User>()), Times.Once);
            _mockUserRepository.Verify(x => x.SaveAsync(), Times.Exactly(2));
        }

        [Fact]
        public async Task RegisterAsync_ShouldHashPasswordWithSalt()
        {
            // Arrange
            var registerDto = new RegisterDTO
            {
                FirstName = "John",
                LastName = "Doe",
                Username = "johndoe",
                Email = "john@example.com",
                Password = "SecurePass123!",
                ConfirmPassword = "SecurePass123!",
                DateOfBirth = new DateOnly(1990, 1, 1)
            };

            _mockUserRepository.Setup(x => x.GetByEmailAsync(registerDto.Email))
                .ReturnsAsync((User?)null);
            _mockUserRepository.Setup(x => x.GetByUsernameAsync(registerDto.Username))
                .ReturnsAsync((User?)null);

            User? savedUser = null;
            _mockUserRepository.Setup(x => x.AddAsync(It.IsAny<User>()))
                .Callback<User>(u => savedUser = u)
                .Returns(Task.CompletedTask);
            _mockUserRepository.Setup(x => x.UpdateAsync(It.IsAny<User>()))
                .Returns(Task.CompletedTask);
            _mockUserRepository.Setup(x => x.SaveAsync())
                .Returns(Task.CompletedTask);

            _mockJwtTokenService.Setup(x => x.GenerateAccessToken(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<UserRole>()))
                .Returns("access_token");
            _mockJwtTokenService.Setup(x => x.GenerateRefreshToken())
                .Returns("refresh_token");

            // Act
            await _authService.RegisterAsync(registerDto);

            // Assert
            Assert.NotNull(savedUser);
            Assert.NotNull(savedUser.PasswordHash);
            Assert.NotNull(savedUser.PasswordSalt);
            Assert.NotEmpty(savedUser.PasswordHash);
            Assert.NotEmpty(savedUser.PasswordSalt);
            Assert.NotEqual(registerDto.Password, Encoding.UTF8.GetString(savedUser.PasswordHash));

            // Verify password can be verified
            var passwordBytes = Encoding.UTF8.GetBytes(registerDto.Password);
            var saltedPassword = new byte[passwordBytes.Length + savedUser.PasswordSalt.Length];
            Array.Copy(passwordBytes, 0, saltedPassword, 0, passwordBytes.Length);
            Array.Copy(savedUser.PasswordSalt, 0, saltedPassword, passwordBytes.Length, savedUser.PasswordSalt.Length);
            var computedHash = SHA512.HashData(saltedPassword);
            Assert.True(computedHash.SequenceEqual(savedUser.PasswordHash));
        }

        [Fact]
        public async Task RegisterAsync_ShouldGenerateUniqueSaltForEachUser()
        {
            // Arrange
            var registerDto1 = new RegisterDTO
            {
                FirstName = "John",
                LastName = "Doe",
                Username = "johndoe",
                Email = "john@example.com",
                Password = "SamePassword123!",
                ConfirmPassword = "SamePassword123!",
                DateOfBirth = new DateOnly(1990, 1, 1)
            };

            var registerDto2 = new RegisterDTO
            {
                FirstName = "Jane",
                LastName = "Doe",
                Username = "janedoe",
                Email = "jane@example.com",
                Password = "SamePassword123!",
                ConfirmPassword = "SamePassword123!",
                DateOfBirth = new DateOnly(1990, 1, 1)
            };

            _mockUserRepository.Setup(x => x.GetByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((User?)null);
            _mockUserRepository.Setup(x => x.GetByUsernameAsync(It.IsAny<string>()))
                .ReturnsAsync((User?)null);

            User? savedUser1 = null;
            User? savedUser2 = null;
            _mockUserRepository.Setup(x => x.AddAsync(It.IsAny<User>()))
                .Callback<User>(u =>
                {
                    if (u.Email == "john@example.com")
                        savedUser1 = u;
                    else
                        savedUser2 = u;
                })
                .Returns(Task.CompletedTask);
            _mockUserRepository.Setup(x => x.UpdateAsync(It.IsAny<User>()))
                .Returns(Task.CompletedTask);
            _mockUserRepository.Setup(x => x.SaveAsync())
                .Returns(Task.CompletedTask);

            _mockJwtTokenService.Setup(x => x.GenerateAccessToken(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<UserRole>()))
                .Returns("access_token");
            _mockJwtTokenService.Setup(x => x.GenerateRefreshToken())
                .Returns("refresh_token");

            // Act
            await _authService.RegisterAsync(registerDto1);
            await _authService.RegisterAsync(registerDto2);

            // Assert
            Assert.NotNull(savedUser1);
            Assert.NotNull(savedUser2);
            Assert.NotEqual(savedUser1.PasswordSalt, savedUser2.PasswordSalt);
            Assert.NotEqual(savedUser1.PasswordHash, savedUser2.PasswordHash);
        }

        [Fact]
        public async Task RegisterAsync_ShouldCreateUserWithUserRole()
        {
            // Arrange
            var registerDto = new RegisterDTO
            {
                FirstName = "John",
                LastName = "Doe",
                Username = "johndoe",
                Email = "john@example.com",
                Password = "SecurePass123!",
                ConfirmPassword = "SecurePass123!",
                DateOfBirth = new DateOnly(1990, 1, 1)
            };

            _mockUserRepository.Setup(x => x.GetByEmailAsync(registerDto.Email))
                .ReturnsAsync((User?)null);
            _mockUserRepository.Setup(x => x.GetByUsernameAsync(registerDto.Username))
                .ReturnsAsync((User?)null);

            User? savedUser = null;
            _mockUserRepository.Setup(x => x.AddAsync(It.IsAny<User>()))
                .Callback<User>(u => savedUser = u)
                .Returns(Task.CompletedTask);
            _mockUserRepository.Setup(x => x.UpdateAsync(It.IsAny<User>()))
                .Returns(Task.CompletedTask);
            _mockUserRepository.Setup(x => x.SaveAsync())
                .Returns(Task.CompletedTask);

            _mockJwtTokenService.Setup(x => x.GenerateAccessToken(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<UserRole>()))
                .Returns("access_token");
            _mockJwtTokenService.Setup(x => x.GenerateRefreshToken())
                .Returns("refresh_token");

            // Act
            await _authService.RegisterAsync(registerDto);

            // Assert
            Assert.NotNull(savedUser);
            Assert.Equal(UserRole.User, savedUser.UserRole);
        }

        [Fact]
        public async Task RegisterAsync_ShouldGenerateAccessToken()
        {
            // Arrange
            var registerDto = new RegisterDTO
            {
                FirstName = "John",
                LastName = "Doe",
                Username = "johndoe",
                Email = "john@example.com",
                Password = "SecurePass123!",
                ConfirmPassword = "SecurePass123!",
                DateOfBirth = new DateOnly(1990, 1, 1)
            };

            _mockUserRepository.Setup(x => x.GetByEmailAsync(registerDto.Email))
                .ReturnsAsync((User?)null);
            _mockUserRepository.Setup(x => x.GetByUsernameAsync(registerDto.Username))
                .ReturnsAsync((User?)null);

            User? savedUser = null;
            _mockUserRepository.Setup(x => x.AddAsync(It.IsAny<User>()))
                .Callback<User>(u => savedUser = u)
                .Returns(Task.CompletedTask);
            _mockUserRepository.Setup(x => x.UpdateAsync(It.IsAny<User>()))
                .Returns(Task.CompletedTask);
            _mockUserRepository.Setup(x => x.SaveAsync())
                .Returns(Task.CompletedTask);

            _mockJwtTokenService.Setup(x => x.GenerateAccessToken(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<UserRole>()))
                .Returns("generated_access_token");
            _mockJwtTokenService.Setup(x => x.GenerateRefreshToken())
                .Returns("refresh_token");

            // Act
            var result = await _authService.RegisterAsync(registerDto);

            // Assert
            _mockJwtTokenService.Verify(x => x.GenerateAccessToken(
                It.IsAny<int>(),
                registerDto.Email,
                registerDto.Username,
                UserRole.User), Times.Once);
            Assert.Equal("generated_access_token", result.Token);
        }

        [Fact]
        public async Task RegisterAsync_ShouldGenerateRefreshToken()
        {
            // Arrange
            var registerDto = new RegisterDTO
            {
                FirstName = "John",
                LastName = "Doe",
                Username = "johndoe",
                Email = "john@example.com",
                Password = "SecurePass123!",
                ConfirmPassword = "SecurePass123!",
                DateOfBirth = new DateOnly(1990, 1, 1)
            };

            _mockUserRepository.Setup(x => x.GetByEmailAsync(registerDto.Email))
                .ReturnsAsync((User?)null);
            _mockUserRepository.Setup(x => x.GetByUsernameAsync(registerDto.Username))
                .ReturnsAsync((User?)null);

            User? savedUser = null;
            _mockUserRepository.Setup(x => x.AddAsync(It.IsAny<User>()))
                .Callback<User>(u => savedUser = u)
                .Returns(Task.CompletedTask);
            _mockUserRepository.Setup(x => x.UpdateAsync(It.IsAny<User>()))
                .Callback<User>(u => savedUser = u)
                .Returns(Task.CompletedTask);
            _mockUserRepository.Setup(x => x.SaveAsync())
                .Returns(Task.CompletedTask);

            _mockJwtTokenService.Setup(x => x.GenerateAccessToken(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<UserRole>()))
                .Returns("access_token");
            _mockJwtTokenService.Setup(x => x.GenerateRefreshToken())
                .Returns("generated_refresh_token");

            // Act
            var result = await _authService.RegisterAsync(registerDto);

            // Assert
            _mockJwtTokenService.Verify(x => x.GenerateRefreshToken(), Times.Once);
            Assert.Equal("generated_refresh_token", result.RefreshToken);
            Assert.NotNull(savedUser);
            Assert.Equal("generated_refresh_token", savedUser.RefreshToken);
        }

        [Fact]
        public async Task RegisterAsync_ShouldSaveRefreshTokenInDatabase()
        {
            // Arrange
            var registerDto = new RegisterDTO
            {
                FirstName = "John",
                LastName = "Doe",
                Username = "johndoe",
                Email = "john@example.com",
                Password = "SecurePass123!",
                ConfirmPassword = "SecurePass123!",
                DateOfBirth = new DateOnly(1990, 1, 1)
            };

            _mockUserRepository.Setup(x => x.GetByEmailAsync(registerDto.Email))
                .ReturnsAsync((User?)null);
            _mockUserRepository.Setup(x => x.GetByUsernameAsync(registerDto.Username))
                .ReturnsAsync((User?)null);

            User? savedUser = null;
            _mockUserRepository.Setup(x => x.AddAsync(It.IsAny<User>()))
                .Callback<User>(u => savedUser = u)
                .Returns(Task.CompletedTask);
            _mockUserRepository.Setup(x => x.UpdateAsync(It.IsAny<User>()))
                .Callback<User>(u => savedUser = u)
                .Returns(Task.CompletedTask);
            _mockUserRepository.Setup(x => x.SaveAsync())
                .Returns(Task.CompletedTask);

            _mockJwtTokenService.Setup(x => x.GenerateAccessToken(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<UserRole>()))
                .Returns("access_token");
            _mockJwtTokenService.Setup(x => x.GenerateRefreshToken())
                .Returns("refresh_token_123");

            // Act
            await _authService.RegisterAsync(registerDto);

            // Assert
            Assert.NotNull(savedUser);
            Assert.Equal("refresh_token_123", savedUser.RefreshToken);
            Assert.NotNull(savedUser.RefreshTokenExpiry);
            var expectedExpiry = DateTime.Now.AddDays(7);
            var timeDifference = Math.Abs((savedUser.RefreshTokenExpiry!.Value - expectedExpiry).TotalMinutes);
            Assert.True(timeDifference < 2, "RefreshTokenExpiry should be approximately 7 days from now");
        }

        [Fact]
        public async Task RegisterAsync_ShouldReturnAuthResponseDTOWithCorrectData()
        {
            // Arrange
            var registerDto = new RegisterDTO
            {
                FirstName = "John",
                LastName = "Doe",
                Username = "johndoe",
                Email = "john@example.com",
                Password = "SecurePass123!",
                ConfirmPassword = "SecurePass123!",
                PhoneNumber = "+1234567890",
                DateOfBirth = new DateOnly(1990, 1, 1)
            };

            _mockUserRepository.Setup(x => x.GetByEmailAsync(registerDto.Email))
                .ReturnsAsync((User?)null);
            _mockUserRepository.Setup(x => x.GetByUsernameAsync(registerDto.Username))
                .ReturnsAsync((User?)null);

            User? savedUser = null;
            int userIdCounter = 1;
            _mockUserRepository.Setup(x => x.AddAsync(It.IsAny<User>()))
                .Callback<User>(u =>
                {
                    u.UserId = userIdCounter++;
                    savedUser = u;
                })
                .Returns(Task.CompletedTask);
            _mockUserRepository.Setup(x => x.UpdateAsync(It.IsAny<User>()))
                .Returns(Task.CompletedTask);
            _mockUserRepository.Setup(x => x.SaveAsync())
                .Returns(Task.CompletedTask);

            _mockJwtTokenService.Setup(x => x.GenerateAccessToken(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<UserRole>()))
                .Returns("access_token_result");
            _mockJwtTokenService.Setup(x => x.GenerateRefreshToken())
                .Returns("refresh_token_result");

            // Act
            var result = await _authService.RegisterAsync(registerDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("access_token_result", result.Token);
            Assert.Equal("refresh_token_result", result.RefreshToken);
            Assert.Equal(savedUser!.UserId, result.UserId);
            Assert.Equal(registerDto.Email, result.Email);
            Assert.Equal(registerDto.Username, result.Username);
            Assert.Equal(UserRole.User, result.Role);
            Assert.True((result.ExpiresAt - DateTime.Now).TotalHours <= 1.1 && (result.ExpiresAt - DateTime.Now).TotalHours >= 0.9);
        }

        [Fact]
        public async Task RegisterAsync_ShouldThrowInvalidOperationExceptionWhenEmailExists()
        {
            // Arrange
            var registerDto = new RegisterDTO
            {
                FirstName = "John",
                LastName = "Doe",
                Username = "johndoe",
                Email = "existing@example.com",
                Password = "SecurePass123!",
                ConfirmPassword = "SecurePass123!",
                DateOfBirth = new DateOnly(1990, 1, 1)
            };

            var existingUser = new User { Email = "existing@example.com" };
            _mockUserRepository.Setup(x => x.GetByEmailAsync(registerDto.Email))
                .ReturnsAsync(existingUser);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _authService.RegisterAsync(registerDto));

            Assert.Equal("User with this email already exists", exception.Message);
            _mockUserRepository.Verify(x => x.AddAsync(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task RegisterAsync_ShouldThrowInvalidOperationExceptionWhenUsernameExists()
        {
            // Arrange
            var registerDto = new RegisterDTO
            {
                FirstName = "John",
                LastName = "Doe",
                Username = "existinguser",
                Email = "john@example.com",
                Password = "SecurePass123!",
                ConfirmPassword = "SecurePass123!",
                DateOfBirth = new DateOnly(1990, 1, 1)
            };

            _mockUserRepository.Setup(x => x.GetByEmailAsync(registerDto.Email))
                .ReturnsAsync((User?)null);
            
            var existingUser = new User { Username = "existinguser" };
            _mockUserRepository.Setup(x => x.GetByUsernameAsync(registerDto.Username))
                .ReturnsAsync(existingUser);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _authService.RegisterAsync(registerDto));

            Assert.Equal("User with this username already exists", exception.Message);
            _mockUserRepository.Verify(x => x.AddAsync(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task RegisterAsync_ShouldNotCreateUserOnSaveAsyncError()
        {
            // Arrange
            var registerDto = new RegisterDTO
            {
                FirstName = "John",
                LastName = "Doe",
                Username = "johndoe",
                Email = "john@example.com",
                Password = "SecurePass123!",
                ConfirmPassword = "SecurePass123!",
                DateOfBirth = new DateOnly(1990, 1, 1)
            };

            _mockUserRepository.Setup(x => x.GetByEmailAsync(registerDto.Email))
                .ReturnsAsync((User?)null);
            _mockUserRepository.Setup(x => x.GetByUsernameAsync(registerDto.Username))
                .ReturnsAsync((User?)null);

            _mockUserRepository.Setup(x => x.AddAsync(It.IsAny<User>()))
                .Returns(Task.CompletedTask);
            _mockUserRepository.Setup(x => x.SaveAsync())
                .ThrowsAsync(new Exception("Database error"));

            _mockJwtTokenService.Setup(x => x.GenerateAccessToken(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<UserRole>()))
                .Returns("access_token");
            _mockJwtTokenService.Setup(x => x.GenerateRefreshToken())
                .Returns("refresh_token");

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _authService.RegisterAsync(registerDto));

            // Verify that UpdateAsync was never called (user was not updated with refresh token)
            _mockUserRepository.Verify(x => x.UpdateAsync(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task RegisterAsync_ShouldSetCreatedAtAndUpdatedAt()
        {
            // Arrange
            var registerDto = new RegisterDTO
            {
                FirstName = "John",
                LastName = "Doe",
                Username = "johndoe",
                Email = "john@example.com",
                Password = "SecurePass123!",
                ConfirmPassword = "SecurePass123!",
                DateOfBirth = new DateOnly(1990, 1, 1)
            };

            _mockUserRepository.Setup(x => x.GetByEmailAsync(registerDto.Email))
                .ReturnsAsync((User?)null);
            _mockUserRepository.Setup(x => x.GetByUsernameAsync(registerDto.Username))
                .ReturnsAsync((User?)null);

            User? savedUser = null;
            var beforeRegistration = DateTime.Now;
            _mockUserRepository.Setup(x => x.AddAsync(It.IsAny<User>()))
                .Callback<User>(u => savedUser = u)
                .Returns(Task.CompletedTask);
            _mockUserRepository.Setup(x => x.UpdateAsync(It.IsAny<User>()))
                .Returns(Task.CompletedTask);
            _mockUserRepository.Setup(x => x.SaveAsync())
                .Returns(Task.CompletedTask);

            _mockJwtTokenService.Setup(x => x.GenerateAccessToken(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<UserRole>()))
                .Returns("access_token");
            _mockJwtTokenService.Setup(x => x.GenerateRefreshToken())
                .Returns("refresh_token");

            // Act
            await _authService.RegisterAsync(registerDto);
            var afterRegistration = DateTime.Now;

            // Assert
            Assert.NotNull(savedUser);
            Assert.True(savedUser.CreatedAt >= beforeRegistration && savedUser.CreatedAt <= afterRegistration);
            Assert.True(savedUser.UpdatedAt >= beforeRegistration && savedUser.UpdatedAt <= afterRegistration);
        }

        [Fact]
        public async Task RegisterAsync_ShouldSaveUserDataCorrectly()
        {
            // Arrange
            var registerDto = new RegisterDTO
            {
                FirstName = "John",
                LastName = "Doe",
                Username = "johndoe",
                Email = "john@example.com",
                Password = "SecurePass123!",
                ConfirmPassword = "SecurePass123!",
                PhoneNumber = "+1234567890",
                DateOfBirth = new DateOnly(1990, 1, 1)
            };

            _mockUserRepository.Setup(x => x.GetByEmailAsync(registerDto.Email))
                .ReturnsAsync((User?)null);
            _mockUserRepository.Setup(x => x.GetByUsernameAsync(registerDto.Username))
                .ReturnsAsync((User?)null);

            User? savedUser = null;
            _mockUserRepository.Setup(x => x.AddAsync(It.IsAny<User>()))
                .Callback<User>(u => savedUser = u)
                .Returns(Task.CompletedTask);
            _mockUserRepository.Setup(x => x.UpdateAsync(It.IsAny<User>()))
                .Returns(Task.CompletedTask);
            _mockUserRepository.Setup(x => x.SaveAsync())
                .Returns(Task.CompletedTask);

            _mockJwtTokenService.Setup(x => x.GenerateAccessToken(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<UserRole>()))
                .Returns("access_token");
            _mockJwtTokenService.Setup(x => x.GenerateRefreshToken())
                .Returns("refresh_token");

            // Act
            await _authService.RegisterAsync(registerDto);

            // Assert
            Assert.NotNull(savedUser);
            Assert.Equal(registerDto.FirstName, savedUser.FirstName);
            Assert.Equal(registerDto.LastName, savedUser.LastName);
            Assert.Equal(registerDto.Username, savedUser.Username);
            Assert.Equal(registerDto.Email, savedUser.Email);
            Assert.Equal(registerDto.PhoneNumber, savedUser.PhoneNumber);
            Assert.Equal(registerDto.DateOfBirth, savedUser.DateOfBirth);
        }

        // Helper method to hash password (same as in AuthService)
        private (byte[] hash, byte[] salt) HashPasswordHelper(string password)
        {
            var salt = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            var passwordBytes = Encoding.UTF8.GetBytes(password);
            var saltedPassword = new byte[passwordBytes.Length + salt.Length];
            Array.Copy(passwordBytes, 0, saltedPassword, 0, passwordBytes.Length);
            Array.Copy(salt, 0, saltedPassword, passwordBytes.Length, salt.Length);

            var hash = SHA512.HashData(saltedPassword);
            return (hash, salt);
        }

        [Fact]
        public async Task LoginAsync_ShouldSuccessfullyLoginUserWithValidCredentials()
        {
            // Arrange
            var password = "SecurePass123!";
            var (passwordHash, passwordSalt) = HashPasswordHelper(password);

            var existingUser = new User
            {
                UserId = 1,
                FirstName = "John",
                LastName = "Doe",
                Username = "johndoe",
                Email = "john@example.com",
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                UserRole = UserRole.User,
                CreatedAt = DateTime.Now.AddDays(-10),
                UpdatedAt = DateTime.Now.AddDays(-5)
            };

            var loginDto = new LoginUserDTO
            {
                UsernameOrEmail = "john@example.com",
                Password = password
            };

            _mockUserRepository.Setup(x => x.GetByEmailAsync(loginDto.UsernameOrEmail))
                .ReturnsAsync(existingUser);

            User? updatedUser = null;
            _mockUserRepository.Setup(x => x.UpdateAsync(It.IsAny<User>()))
                .Callback<User>(u => updatedUser = u)
                .Returns(Task.CompletedTask);
            _mockUserRepository.Setup(x => x.SaveAsync())
                .Returns(Task.CompletedTask);

            _mockJwtTokenService.Setup(x => x.GenerateAccessToken(
                existingUser.UserId,
                existingUser.Email,
                existingUser.Username,
                existingUser.UserRole))
                .Returns("access_token_result");
            _mockJwtTokenService.Setup(x => x.GenerateRefreshToken())
                .Returns("refresh_token_result");

            // Act
            var result = await _authService.LoginAsync(loginDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("access_token_result", result.Token);
            Assert.Equal("refresh_token_result", result.RefreshToken);
            Assert.Equal(existingUser.UserId, result.UserId);
            Assert.Equal(existingUser.Email, result.Email);
            Assert.Equal(existingUser.Username, result.Username);
            Assert.Equal(existingUser.UserRole, result.Role);
            Assert.True((result.ExpiresAt - DateTime.Now).TotalHours <= 1.1 && (result.ExpiresAt - DateTime.Now).TotalHours >= 0.9);
        }

        [Fact]
        public async Task LoginAsync_ShouldReturnAuthResponseDTOWithTokens()
        {
            // Arrange
            var password = "MyPassword123!";
            var (passwordHash, passwordSalt) = HashPasswordHelper(password);

            var existingUser = new User
            {
                UserId = 5,
                Username = "testuser",
                Email = "test@example.com",
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                UserRole = UserRole.Admin
            };

            var loginDto = new LoginUserDTO
            {
                UsernameOrEmail = "test@example.com",
                Password = password
            };

            _mockUserRepository.Setup(x => x.GetByEmailAsync(loginDto.UsernameOrEmail))
                .ReturnsAsync(existingUser);
            _mockUserRepository.Setup(x => x.UpdateAsync(It.IsAny<User>()))
                .Returns(Task.CompletedTask);
            _mockUserRepository.Setup(x => x.SaveAsync())
                .Returns(Task.CompletedTask);

            _mockJwtTokenService.Setup(x => x.GenerateAccessToken(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<UserRole>()))
                .Returns("new_access_token");
            _mockJwtTokenService.Setup(x => x.GenerateRefreshToken())
                .Returns("new_refresh_token");

            // Act
            var result = await _authService.LoginAsync(loginDto);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Token);
            Assert.NotNull(result.RefreshToken);
            Assert.Equal("new_access_token", result.Token);
            Assert.Equal("new_refresh_token", result.RefreshToken);
        }

        [Fact]
        public async Task LoginAsync_ShouldVerifyPasswordCorrectlyWithHashAndSalt()
        {
            // Arrange
            var correctPassword = "CorrectPass123!";
            var wrongPassword = "WrongPass123!";
            var (passwordHash, passwordSalt) = HashPasswordHelper(correctPassword);

            var existingUser = new User
            {
                UserId = 1,
                Username = "testuser",
                Email = "test@example.com",
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                UserRole = UserRole.User
            };

            var loginDto = new LoginUserDTO
            {
                UsernameOrEmail = "test@example.com",
                Password = correctPassword  // Правильный пароль
            };

            _mockUserRepository.Setup(x => x.GetByEmailAsync(loginDto.UsernameOrEmail))
                .ReturnsAsync(existingUser);
            _mockUserRepository.Setup(x => x.UpdateAsync(It.IsAny<User>()))
                .Returns(Task.CompletedTask);
            _mockUserRepository.Setup(x => x.SaveAsync())
                .Returns(Task.CompletedTask);

            _mockJwtTokenService.Setup(x => x.GenerateAccessToken(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<UserRole>()))
                .Returns("token");
            _mockJwtTokenService.Setup(x => x.GenerateRefreshToken())
                .Returns("refresh");

            // Act
            var result = await _authService.LoginAsync(loginDto);

            // Assert
            Assert.NotNull(result);  // Логин успешен, значит пароль верифицирован правильно
            _mockUserRepository.Verify(x => x.UpdateAsync(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public async Task LoginAsync_ShouldGenerateNewAccessToken()
        {
            // Arrange
            var password = "Password123!";
            var (passwordHash, passwordSalt) = HashPasswordHelper(password);

            var existingUser = new User
            {
                UserId = 10,
                Username = "testuser",
                Email = "test@example.com",
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                UserRole = UserRole.User
            };

            var loginDto = new LoginUserDTO
            {
                UsernameOrEmail = "test@example.com",
                Password = password
            };

            _mockUserRepository.Setup(x => x.GetByEmailAsync(loginDto.UsernameOrEmail))
                .ReturnsAsync(existingUser);
            _mockUserRepository.Setup(x => x.UpdateAsync(It.IsAny<User>()))
                .Returns(Task.CompletedTask);
            _mockUserRepository.Setup(x => x.SaveAsync())
                .Returns(Task.CompletedTask);

            _mockJwtTokenService.Setup(x => x.GenerateAccessToken(
                existingUser.UserId,
                existingUser.Email,
                existingUser.Username,
                existingUser.UserRole))
                .Returns("new_access_token");

            _mockJwtTokenService.Setup(x => x.GenerateRefreshToken())
                .Returns("refresh");

            // Act
            var result = await _authService.LoginAsync(loginDto);

            // Assert
            _mockJwtTokenService.Verify(x => x.GenerateAccessToken(
                existingUser.UserId,
                existingUser.Email,
                existingUser.Username,
                existingUser.UserRole), Times.Once);
            Assert.Equal("new_access_token", result.Token);
        }

        [Fact]
        public async Task LoginAsync_ShouldGenerateNewRefreshToken()
        {
            // Arrange
            var password = "Password123!";
            var (passwordHash, passwordSalt) = HashPasswordHelper(password);

            var existingUser = new User
            {
                UserId = 1,
                Username = "testuser",
                Email = "test@example.com",
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                UserRole = UserRole.User
            };

            var loginDto = new LoginUserDTO
            {
                UsernameOrEmail = "test@example.com",
                Password = password
            };

            _mockUserRepository.Setup(x => x.GetByEmailAsync(loginDto.UsernameOrEmail))
                .ReturnsAsync(existingUser);

            User? updatedUser = null;
            _mockUserRepository.Setup(x => x.UpdateAsync(It.IsAny<User>()))
                .Callback<User>(u => updatedUser = u)
                .Returns(Task.CompletedTask);
            _mockUserRepository.Setup(x => x.SaveAsync())
                .Returns(Task.CompletedTask);

            _mockJwtTokenService.Setup(x => x.GenerateAccessToken(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<UserRole>()))
                .Returns("access");
            _mockJwtTokenService.Setup(x => x.GenerateRefreshToken())
                .Returns("new_refresh_token");

            // Act
            var result = await _authService.LoginAsync(loginDto);

            // Assert
            _mockJwtTokenService.Verify(x => x.GenerateRefreshToken(), Times.Once);
            Assert.Equal("new_refresh_token", result.RefreshToken);
            Assert.NotNull(updatedUser);
            Assert.Equal("new_refresh_token", updatedUser.RefreshToken);
        }

        [Fact]
        public async Task LoginAsync_ShouldUpdateRefreshTokenInDatabase()
        {
            // Arrange
            var password = "Password123!";
            var (passwordHash, passwordSalt) = HashPasswordHelper(password);

            var existingUser = new User
            {
                UserId = 1,
                Username = "testuser",
                Email = "test@example.com",
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                UserRole = UserRole.User,
                RefreshToken = "old_refresh_token",
                RefreshTokenExpiry = DateTime.Now.AddDays(-1)
            };

            var loginDto = new LoginUserDTO
            {
                UsernameOrEmail = "test@example.com",
                Password = password
            };

            _mockUserRepository.Setup(x => x.GetByEmailAsync(loginDto.UsernameOrEmail))
                .ReturnsAsync(existingUser);

            User? updatedUser = null;
            _mockUserRepository.Setup(x => x.UpdateAsync(It.IsAny<User>()))
                .Callback<User>(u => updatedUser = u)
                .Returns(Task.CompletedTask);
            _mockUserRepository.Setup(x => x.SaveAsync())
                .Returns(Task.CompletedTask);

            _mockJwtTokenService.Setup(x => x.GenerateAccessToken(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<UserRole>()))
                .Returns("access");
            _mockJwtTokenService.Setup(x => x.GenerateRefreshToken())
                .Returns("new_refresh_token");

            // Act
            await _authService.LoginAsync(loginDto);

            // Assert
            Assert.NotNull(updatedUser);
            Assert.Equal("new_refresh_token", updatedUser.RefreshToken);
            Assert.NotNull(updatedUser.RefreshTokenExpiry);
            var expectedExpiry = DateTime.Now.AddDays(7);
            var timeDifference = Math.Abs((updatedUser.RefreshTokenExpiry!.Value - expectedExpiry).TotalMinutes);
            Assert.True(timeDifference < 2, "RefreshTokenExpiry should be approximately 7 days from now");
            _mockUserRepository.Verify(x => x.UpdateAsync(It.IsAny<User>()), Times.Once);
            _mockUserRepository.Verify(x => x.SaveAsync(), Times.Once);
        }

        [Fact]
        public async Task LoginAsync_ShouldThrowUnauthorizedAccessExceptionForInvalidEmail()
        {
            // Arrange
            var loginDto = new LoginUserDTO
            {
                UsernameOrEmail = "nonexistent@example.com",
                Password = "AnyPassword123!"
            };

            _mockUserRepository.Setup(x => x.GetByEmailAsync(loginDto.UsernameOrEmail))
                .ReturnsAsync((User?)null);
            _mockUserRepository.Setup(x => x.GetByUsernameAsync(loginDto.UsernameOrEmail))
                .ReturnsAsync((User?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => _authService.LoginAsync(loginDto));

            Assert.Equal("Invalid credentials", exception.Message);
            _mockJwtTokenService.Verify(x => x.GenerateAccessToken(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<UserRole>()), Times.Never);
        }

        [Fact]
        public async Task LoginAsync_ShouldThrowUnauthorizedAccessExceptionForInvalidUsername()
        {
            // Arrange
            var loginDto = new LoginUserDTO
            {
                UsernameOrEmail = "nonexistentuser",
                Password = "AnyPassword123!"
            };

            _mockUserRepository.Setup(x => x.GetByEmailAsync(loginDto.UsernameOrEmail))
                .ReturnsAsync((User?)null);
            _mockUserRepository.Setup(x => x.GetByUsernameAsync(loginDto.UsernameOrEmail))
                .ReturnsAsync((User?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => _authService.LoginAsync(loginDto));

            Assert.Equal("Invalid credentials", exception.Message);
            _mockJwtTokenService.Verify(x => x.GenerateAccessToken(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<UserRole>()), Times.Never);
        }

        [Fact]
        public async Task LoginAsync_ShouldThrowUnauthorizedAccessExceptionForWrongPassword()
        {
            // Arrange
            var correctPassword = "CorrectPass123!";
            var wrongPassword = "WrongPass123!";
            var (passwordHash, passwordSalt) = HashPasswordHelper(correctPassword);

            var existingUser = new User
            {
                UserId = 1,
                Username = "testuser",
                Email = "test@example.com",
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                UserRole = UserRole.User
            };

            var loginDto = new LoginUserDTO
            {
                UsernameOrEmail = "test@example.com",
                Password = wrongPassword  // Неправильный пароль
            };

            _mockUserRepository.Setup(x => x.GetByEmailAsync(loginDto.UsernameOrEmail))
                .ReturnsAsync(existingUser);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => _authService.LoginAsync(loginDto));

            Assert.Equal("Invalid credentials", exception.Message);
            _mockUserRepository.Verify(x => x.UpdateAsync(It.IsAny<User>()), Times.Never);
            _mockJwtTokenService.Verify(x => x.GenerateAccessToken(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<UserRole>()), Times.Never);
        }

        [Fact]
        public async Task LoginAsync_ShouldThrowUnauthorizedAccessExceptionWhenUserNotFound()
        {
            // Arrange
            var loginDto = new LoginUserDTO
            {
                UsernameOrEmail = "unknown@example.com",
                Password = "AnyPassword123!"
            };

            _mockUserRepository.Setup(x => x.GetByEmailAsync(loginDto.UsernameOrEmail))
                .ReturnsAsync((User?)null);
            _mockUserRepository.Setup(x => x.GetByUsernameAsync(loginDto.UsernameOrEmail))
                .ReturnsAsync((User?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => _authService.LoginAsync(loginDto));

            Assert.Equal("Invalid credentials", exception.Message);
            _mockUserRepository.Verify(x => x.GetByEmailAsync(loginDto.UsernameOrEmail), Times.Once);
            _mockUserRepository.Verify(x => x.GetByUsernameAsync(loginDto.UsernameOrEmail), Times.Once);
        }

        [Fact]
        public async Task LoginAsync_ShouldFindUserByEmail()
        {
            // Arrange
            var password = "Password123!";
            var (passwordHash, passwordSalt) = HashPasswordHelper(password);

            var existingUser = new User
            {
                UserId = 1,
                Username = "testuser",
                Email = "test@example.com",
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                UserRole = UserRole.User
            };

            var loginDto = new LoginUserDTO
            {
                UsernameOrEmail = "test@example.com",  // Email
                Password = password
            };

            _mockUserRepository.Setup(x => x.GetByEmailAsync(loginDto.UsernameOrEmail))
                .ReturnsAsync(existingUser);
            _mockUserRepository.Setup(x => x.UpdateAsync(It.IsAny<User>()))
                .Returns(Task.CompletedTask);
            _mockUserRepository.Setup(x => x.SaveAsync())
                .Returns(Task.CompletedTask);

            _mockJwtTokenService.Setup(x => x.GenerateAccessToken(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<UserRole>()))
                .Returns("token");
            _mockJwtTokenService.Setup(x => x.GenerateRefreshToken())
                .Returns("refresh");

            // Act
            var result = await _authService.LoginAsync(loginDto);

            // Assert
            Assert.NotNull(result);
            _mockUserRepository.Verify(x => x.GetByEmailAsync(loginDto.UsernameOrEmail), Times.Once);
            _mockUserRepository.Verify(x => x.GetByUsernameAsync(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task LoginAsync_ShouldFindUserByUsername()
        {
            // Arrange
            var password = "Password123!";
            var (passwordHash, passwordSalt) = HashPasswordHelper(password);

            var existingUser = new User
            {
                UserId = 1,
                Username = "testuser",
                Email = "test@example.com",
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                UserRole = UserRole.User
            };

            var loginDto = new LoginUserDTO
            {
                UsernameOrEmail = "testuser",  // Username (не email)
                Password = password
            };

            _mockUserRepository.Setup(x => x.GetByEmailAsync(loginDto.UsernameOrEmail))
                .ReturnsAsync((User?)null);  // По email не найден
            _mockUserRepository.Setup(x => x.GetByUsernameAsync(loginDto.UsernameOrEmail))
                .ReturnsAsync(existingUser);  // По username найден
            _mockUserRepository.Setup(x => x.UpdateAsync(It.IsAny<User>()))
                .Returns(Task.CompletedTask);
            _mockUserRepository.Setup(x => x.SaveAsync())
                .Returns(Task.CompletedTask);

            _mockJwtTokenService.Setup(x => x.GenerateAccessToken(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<UserRole>()))
                .Returns("token");
            _mockJwtTokenService.Setup(x => x.GenerateRefreshToken())
                .Returns("refresh");

            // Act
            var result = await _authService.LoginAsync(loginDto);

            // Assert
            Assert.NotNull(result);
            _mockUserRepository.Verify(x => x.GetByEmailAsync(loginDto.UsernameOrEmail), Times.Once);
            _mockUserRepository.Verify(x => x.GetByUsernameAsync(loginDto.UsernameOrEmail), Times.Once);
        }

        [Fact]
        public async Task LoginAsync_ShouldUpdateUpdatedAtTimestamp()
        {
            // Arrange
            var password = "Password123!";
            var (passwordHash, passwordSalt) = HashPasswordHelper(password);

            var existingUser = new User
            {
                UserId = 1,
                Username = "testuser",
                Email = "test@example.com",
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                UserRole = UserRole.User,
                UpdatedAt = DateTime.Now.AddDays(-10)  // Старая дата
            };

            var loginDto = new LoginUserDTO
            {
                UsernameOrEmail = "test@example.com",
                Password = password
            };

            _mockUserRepository.Setup(x => x.GetByEmailAsync(loginDto.UsernameOrEmail))
                .ReturnsAsync(existingUser);

            User? updatedUser = null;
            var beforeLogin = DateTime.Now;
            _mockUserRepository.Setup(x => x.UpdateAsync(It.IsAny<User>()))
                .Callback<User>(u => updatedUser = u)
                .Returns(Task.CompletedTask);
            _mockUserRepository.Setup(x => x.SaveAsync())
                .Returns(Task.CompletedTask);

            _mockJwtTokenService.Setup(x => x.GenerateAccessToken(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<UserRole>()))
                .Returns("token");
            _mockJwtTokenService.Setup(x => x.GenerateRefreshToken())
                .Returns("refresh");

            // Act
            await _authService.LoginAsync(loginDto);
            var afterLogin = DateTime.Now;

            // Assert
            Assert.NotNull(updatedUser);
            Assert.True(updatedUser.UpdatedAt >= beforeLogin && updatedUser.UpdatedAt <= afterLogin);
        }

        [Fact]
        public async Task RefreshTokenAsync_ShouldSuccessfullyRefreshTokenWithValidRefreshToken()
        {
            // Arrange
            var refreshToken = "valid_refresh_token_123";
            var existingUser = new User
            {
                UserId = 1,
                FirstName = "John",
                LastName = "Doe",
                Username = "johndoe",
                Email = "john@example.com",
                UserRole = UserRole.User,
                RefreshToken = refreshToken,
                RefreshTokenExpiry = DateTime.Now.AddDays(3)  // Еще не истек
            };

            _mockUserRepository.Setup(x => x.GetByRefreshTokenAsync(refreshToken))
                .ReturnsAsync(existingUser);

            User? updatedUser = null;
            _mockUserRepository.Setup(x => x.UpdateAsync(It.IsAny<User>()))
                .Callback<User>(u => updatedUser = u)
                .Returns(Task.CompletedTask);
            _mockUserRepository.Setup(x => x.SaveAsync())
                .Returns(Task.CompletedTask);

            _mockJwtTokenService.Setup(x => x.GenerateAccessToken(
                existingUser.UserId,
                existingUser.Email,
                existingUser.Username,
                existingUser.UserRole))
                .Returns("new_access_token_result");
            _mockJwtTokenService.Setup(x => x.GenerateRefreshToken())
                .Returns("new_refresh_token_result");

            // Act
            var result = await _authService.RefreshTokenAsync(refreshToken);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("new_access_token_result", result.Token);
            Assert.Equal("new_refresh_token_result", result.RefreshToken);
            Assert.Equal(existingUser.UserId, result.UserId);
            Assert.Equal(existingUser.Email, result.Email);
            Assert.Equal(existingUser.Username, result.Username);
            Assert.Equal(existingUser.UserRole, result.Role);
            Assert.True((result.ExpiresAt - DateTime.Now).TotalHours <= 1.1 && (result.ExpiresAt - DateTime.Now).TotalHours >= 0.9);
        }

        [Fact]
        public async Task RefreshTokenAsync_ShouldGenerateNewAccessToken()
        {
            // Arrange
            var refreshToken = "valid_refresh_token";
            var existingUser = new User
            {
                UserId = 5,
                Username = "testuser",
                Email = "test@example.com",
                UserRole = UserRole.Admin,
                RefreshToken = refreshToken,
                RefreshTokenExpiry = DateTime.Now.AddDays(5)
            };

            _mockUserRepository.Setup(x => x.GetByRefreshTokenAsync(refreshToken))
                .ReturnsAsync(existingUser);
            _mockUserRepository.Setup(x => x.UpdateAsync(It.IsAny<User>()))
                .Returns(Task.CompletedTask);
            _mockUserRepository.Setup(x => x.SaveAsync())
                .Returns(Task.CompletedTask);

            _mockJwtTokenService.Setup(x => x.GenerateAccessToken(
                existingUser.UserId,
                existingUser.Email,
                existingUser.Username,
                existingUser.UserRole))
                .Returns("generated_access_token");

            _mockJwtTokenService.Setup(x => x.GenerateRefreshToken())
                .Returns("refresh_token");

            // Act
            var result = await _authService.RefreshTokenAsync(refreshToken);

            // Assert
            _mockJwtTokenService.Verify(x => x.GenerateAccessToken(
                existingUser.UserId,
                existingUser.Email,
                existingUser.Username,
                existingUser.UserRole), Times.Once);
            Assert.Equal("generated_access_token", result.Token);
        }

        [Fact]
        public async Task RefreshTokenAsync_ShouldGenerateNewRefreshToken()
        {
            // Arrange
            var oldRefreshToken = "old_refresh_token";
            var existingUser = new User
            {
                UserId = 1,
                Username = "testuser",
                Email = "test@example.com",
                UserRole = UserRole.User,
                RefreshToken = oldRefreshToken,
                RefreshTokenExpiry = DateTime.Now.AddDays(2)
            };

            _mockUserRepository.Setup(x => x.GetByRefreshTokenAsync(oldRefreshToken))
                .ReturnsAsync(existingUser);

            User? updatedUser = null;
            _mockUserRepository.Setup(x => x.UpdateAsync(It.IsAny<User>()))
                .Callback<User>(u => updatedUser = u)
                .Returns(Task.CompletedTask);
            _mockUserRepository.Setup(x => x.SaveAsync())
                .Returns(Task.CompletedTask);

            _mockJwtTokenService.Setup(x => x.GenerateAccessToken(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<UserRole>()))
                .Returns("access_token");
            _mockJwtTokenService.Setup(x => x.GenerateRefreshToken())
                .Returns("new_generated_refresh_token");

            // Act
            var result = await _authService.RefreshTokenAsync(oldRefreshToken);

            // Assert
            _mockJwtTokenService.Verify(x => x.GenerateRefreshToken(), Times.Once);
            Assert.Equal("new_generated_refresh_token", result.RefreshToken);
            Assert.NotNull(updatedUser);
            Assert.Equal("new_generated_refresh_token", updatedUser.RefreshToken);
            Assert.NotEqual(oldRefreshToken, updatedUser.RefreshToken);  // Старый токен заменен новым
        }

        [Fact]
        public async Task RefreshTokenAsync_ShouldUpdateRefreshTokenInDatabase()
        {
            // Arrange
            var oldRefreshToken = "old_refresh_token_456";
            var existingUser = new User
            {
                UserId = 1,
                Username = "testuser",
                Email = "test@example.com",
                UserRole = UserRole.User,
                RefreshToken = oldRefreshToken,
                RefreshTokenExpiry = DateTime.Now.AddDays(1)  // Старый expiry
            };

            _mockUserRepository.Setup(x => x.GetByRefreshTokenAsync(oldRefreshToken))
                .ReturnsAsync(existingUser);

            User? updatedUser = null;
            _mockUserRepository.Setup(x => x.UpdateAsync(It.IsAny<User>()))
                .Callback<User>(u => updatedUser = u)
                .Returns(Task.CompletedTask);
            _mockUserRepository.Setup(x => x.SaveAsync())
                .Returns(Task.CompletedTask);

            _mockJwtTokenService.Setup(x => x.GenerateAccessToken(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<UserRole>()))
                .Returns("access");
            _mockJwtTokenService.Setup(x => x.GenerateRefreshToken())
                .Returns("new_refresh_token");

            // Act
            await _authService.RefreshTokenAsync(oldRefreshToken);

            // Assert
            Assert.NotNull(updatedUser);
            Assert.Equal("new_refresh_token", updatedUser.RefreshToken);
            Assert.NotNull(updatedUser.RefreshTokenExpiry);
            var expectedExpiry = DateTime.Now.AddDays(7);
            var timeDifference = Math.Abs((updatedUser.RefreshTokenExpiry!.Value - expectedExpiry).TotalMinutes);
            Assert.True(timeDifference < 2, "RefreshTokenExpiry should be approximately 7 days from now");
            _mockUserRepository.Verify(x => x.UpdateAsync(It.IsAny<User>()), Times.Once);
            _mockUserRepository.Verify(x => x.SaveAsync(), Times.Once);
        }

        [Fact]
        public async Task RefreshTokenAsync_ShouldThrowUnauthorizedAccessExceptionForInvalidRefreshToken()
        {
            // Arrange
            var invalidRefreshToken = "invalid_refresh_token";

            _mockUserRepository.Setup(x => x.GetByRefreshTokenAsync(invalidRefreshToken))
                .ReturnsAsync((User?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => _authService.RefreshTokenAsync(invalidRefreshToken));

            Assert.Equal("Invalid refresh token", exception.Message);
            _mockUserRepository.Verify(x => x.UpdateAsync(It.IsAny<User>()), Times.Never);
            _mockJwtTokenService.Verify(x => x.GenerateAccessToken(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<UserRole>()), Times.Never);
            _mockJwtTokenService.Verify(x => x.GenerateRefreshToken(), Times.Never);
        }

        [Fact]
        public async Task RefreshTokenAsync_ShouldThrowUnauthorizedAccessExceptionForExpiredRefreshToken()
        {
            // Arrange
            var refreshToken = "expired_refresh_token";
            var expiredUser = new User
            {
                UserId = 1,
                Username = "testuser",
                Email = "test@example.com",
                UserRole = UserRole.User,
                RefreshToken = refreshToken,
                RefreshTokenExpiry = DateTime.Now.AddMinutes(-5)  // Истекший токен (в прошлом)
            };

            _mockUserRepository.Setup(x => x.GetByRefreshTokenAsync(refreshToken))
                .ReturnsAsync(expiredUser);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => _authService.RefreshTokenAsync(refreshToken));

            Assert.Equal("Refresh token expired", exception.Message);
            _mockUserRepository.Verify(x => x.UpdateAsync(It.IsAny<User>()), Times.Never);
            _mockJwtTokenService.Verify(x => x.GenerateAccessToken(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<UserRole>()), Times.Never);
            _mockJwtTokenService.Verify(x => x.GenerateRefreshToken(), Times.Never);
        }

        [Fact]
        public async Task RefreshTokenAsync_ShouldThrowUnauthorizedAccessExceptionWhenUserNotFound()
        {
            // Arrange
            var refreshToken = "nonexistent_refresh_token";

            _mockUserRepository.Setup(x => x.GetByRefreshTokenAsync(refreshToken))
                .ReturnsAsync((User?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => _authService.RefreshTokenAsync(refreshToken));

            Assert.Equal("Invalid refresh token", exception.Message);
            _mockUserRepository.Verify(x => x.GetByRefreshTokenAsync(refreshToken), Times.Once);
            _mockUserRepository.Verify(x => x.UpdateAsync(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task RefreshTokenAsync_ShouldReturnAuthResponseDTOWithCorrectData()
        {
            // Arrange
            var refreshToken = "valid_token";
            var existingUser = new User
            {
                UserId = 10,
                FirstName = "Jane",
                LastName = "Smith",
                Username = "janesmith",
                Email = "jane@example.com",
                UserRole = UserRole.User,
                RefreshToken = refreshToken,
                RefreshTokenExpiry = DateTime.Now.AddDays(4)
            };

            _mockUserRepository.Setup(x => x.GetByRefreshTokenAsync(refreshToken))
                .ReturnsAsync(existingUser);
            _mockUserRepository.Setup(x => x.UpdateAsync(It.IsAny<User>()))
                .Returns(Task.CompletedTask);
            _mockUserRepository.Setup(x => x.SaveAsync())
                .Returns(Task.CompletedTask);

            _mockJwtTokenService.Setup(x => x.GenerateAccessToken(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<UserRole>()))
                .Returns("access_token");
            _mockJwtTokenService.Setup(x => x.GenerateRefreshToken())
                .Returns("refresh_token");

            // Act
            var result = await _authService.RefreshTokenAsync(refreshToken);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(existingUser.UserId, result.UserId);
            Assert.Equal(existingUser.Email, result.Email);
            Assert.Equal(existingUser.Username, result.Username);
            Assert.Equal(existingUser.UserRole, result.Role);
            Assert.NotNull(result.Token);
            Assert.NotNull(result.RefreshToken);
            Assert.True((result.ExpiresAt - DateTime.Now).TotalHours <= 1.1 && (result.ExpiresAt - DateTime.Now).TotalHours >= 0.9);
        }

        [Fact]
        public async Task RefreshTokenAsync_ShouldUpdateUpdatedAtTimestamp()
        {
            // Arrange
            var refreshToken = "valid_token";
            var existingUser = new User
            {
                UserId = 1,
                Username = "testuser",
                Email = "test@example.com",
                UserRole = UserRole.User,
                RefreshToken = refreshToken,
                RefreshTokenExpiry = DateTime.Now.AddDays(3),
                UpdatedAt = DateTime.Now.AddDays(-5)  // Старая дата
            };

            _mockUserRepository.Setup(x => x.GetByRefreshTokenAsync(refreshToken))
                .ReturnsAsync(existingUser);

            User? updatedUser = null;
            var beforeRefresh = DateTime.Now;
            _mockUserRepository.Setup(x => x.UpdateAsync(It.IsAny<User>()))
                .Callback<User>(u => updatedUser = u)
                .Returns(Task.CompletedTask);
            _mockUserRepository.Setup(x => x.SaveAsync())
                .Returns(Task.CompletedTask);

            _mockJwtTokenService.Setup(x => x.GenerateAccessToken(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<UserRole>()))
                .Returns("token");
            _mockJwtTokenService.Setup(x => x.GenerateRefreshToken())
                .Returns("refresh");

            // Act
            await _authService.RefreshTokenAsync(refreshToken);
            var afterRefresh = DateTime.Now;

            // Assert
            Assert.NotNull(updatedUser);
            Assert.True(updatedUser.UpdatedAt >= beforeRefresh && updatedUser.UpdatedAt <= afterRefresh);
        }

        [Fact]
        public async Task RefreshTokenAsync_ShouldReplaceOldRefreshTokenWithNewOne()
        {
            // Arrange
            var oldRefreshToken = "old_token_123";
            var existingUser = new User
            {
                UserId = 1,
                Username = "testuser",
                Email = "test@example.com",
                UserRole = UserRole.User,
                RefreshToken = oldRefreshToken,
                RefreshTokenExpiry = DateTime.Now.AddDays(2)
            };

            _mockUserRepository.Setup(x => x.GetByRefreshTokenAsync(oldRefreshToken))
                .ReturnsAsync(existingUser);

            User? updatedUser = null;
            _mockUserRepository.Setup(x => x.UpdateAsync(It.IsAny<User>()))
                .Callback<User>(u => updatedUser = u)
                .Returns(Task.CompletedTask);
            _mockUserRepository.Setup(x => x.SaveAsync())
                .Returns(Task.CompletedTask);

            _mockJwtTokenService.Setup(x => x.GenerateAccessToken(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<UserRole>()))
                .Returns("access");
            _mockJwtTokenService.Setup(x => x.GenerateRefreshToken())
                .Returns("completely_new_refresh_token");

            // Act
            var result = await _authService.RefreshTokenAsync(oldRefreshToken);

            // Assert
            Assert.NotNull(updatedUser);
            Assert.Equal("completely_new_refresh_token", updatedUser.RefreshToken);
            Assert.Equal("completely_new_refresh_token", result.RefreshToken);
            Assert.NotEqual(oldRefreshToken, updatedUser.RefreshToken);
        }

        [Fact]
        public async Task RefreshTokenAsync_ShouldCheckExpiryBeforeGeneratingTokens()
        {
            // Arrange
            var refreshToken = "expired_token";
            var expiredUser = new User
            {
                UserId = 1,
                Username = "testuser",
                Email = "test@example.com",
                UserRole = UserRole.User,
                RefreshToken = refreshToken,
                RefreshTokenExpiry = DateTime.Now.AddSeconds(-1)  // Только что истек
            };

            _mockUserRepository.Setup(x => x.GetByRefreshTokenAsync(refreshToken))
                .ReturnsAsync(expiredUser);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => _authService.RefreshTokenAsync(refreshToken));

            // Verify that tokens were NOT generated because expiry check failed first
            _mockJwtTokenService.Verify(x => x.GenerateAccessToken(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<UserRole>()), Times.Never);
            _mockJwtTokenService.Verify(x => x.GenerateRefreshToken(), Times.Never);
        }

        [Fact]
        public async Task ValidateTokenAsync_ShouldReturnTrueForValidToken()
        {
            // Arrange
            var validToken = "valid_jwt_token_string";

            _mockJwtTokenService.Setup(x => x.ValidateToken(validToken))
                .Returns(true);

            // Act
            var result = await _authService.ValidateTokenAsync(validToken);

            // Assert
            Assert.True(result);
            _mockJwtTokenService.Verify(x => x.ValidateToken(validToken), Times.Once);
        }

        [Fact]
        public async Task ValidateTokenAsync_ShouldReturnFalseForInvalidToken()
        {
            // Arrange
            var invalidToken = "invalid_jwt_token_string";

            _mockJwtTokenService.Setup(x => x.ValidateToken(invalidToken))
                .Returns(false);

            // Act
            var result = await _authService.ValidateTokenAsync(invalidToken);

            // Assert
            Assert.False(result);
            _mockJwtTokenService.Verify(x => x.ValidateToken(invalidToken), Times.Once);
        }

        [Fact]
        public async Task ValidateTokenAsync_ShouldDelegateCallToJwtTokenService()
        {
            // Arrange
            var token = "any_token_string";

            _mockJwtTokenService.Setup(x => x.ValidateToken(token))
                .Returns(true);

            // Act
            await _authService.ValidateTokenAsync(token);

            // Assert
            _mockJwtTokenService.Verify(x => x.ValidateToken(token), Times.Once);
            _mockJwtTokenService.Verify(x => x.ValidateToken(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task ValidateTokenAsync_ShouldReturnFalseForExpiredToken()
        {
            // Arrange
            var expiredToken = "expired_jwt_token";

            _mockJwtTokenService.Setup(x => x.ValidateToken(expiredToken))
                .Returns(false);  // JwtTokenService возвращает false для истекшего токена

            // Act
            var result = await _authService.ValidateTokenAsync(expiredToken);

            // Assert
            Assert.False(result);
            _mockJwtTokenService.Verify(x => x.ValidateToken(expiredToken), Times.Once);
        }

        [Fact]
        public async Task ValidateTokenAsync_ShouldReturnFalseForCorruptedToken()
        {
            // Arrange
            var corruptedToken = "corrupted.token.format!!!";

            _mockJwtTokenService.Setup(x => x.ValidateToken(corruptedToken))
                .Returns(false);

            // Act
            var result = await _authService.ValidateTokenAsync(corruptedToken);

            // Assert
            Assert.False(result);
            _mockJwtTokenService.Verify(x => x.ValidateToken(corruptedToken), Times.Once);
        }

        [Fact]
        public async Task ValidateTokenAsync_ShouldPassTokenParameterCorrectly()
        {
            // Arrange
            var specificToken = "specific_token_value_123";

            _mockJwtTokenService.Setup(x => x.ValidateToken(It.IsAny<string>()))
                .Returns(true);

            // Act
            await _authService.ValidateTokenAsync(specificToken);

            // Assert
            _mockJwtTokenService.Verify(x => x.ValidateToken(specificToken), Times.Once);
            _mockJwtTokenService.Verify(x => x.ValidateToken(It.Is<string>(t => t == specificToken)), Times.Once);
        }

        [Fact]
        public async Task ValidateTokenAsync_ShouldHandleEmptyString()
        {
            // Arrange
            var emptyToken = string.Empty;

            _mockJwtTokenService.Setup(x => x.ValidateToken(emptyToken))
                .Returns(false);

            // Act
            var result = await _authService.ValidateTokenAsync(emptyToken);

            // Assert
            Assert.False(result);
            _mockJwtTokenService.Verify(x => x.ValidateToken(emptyToken), Times.Once);
        }

        [Fact]
        public async Task LogoutAsync_ShouldSuccessfullyClearRefreshToken()
        {
            // Arrange
            var userId = 1;
            var existingUser = new User
            {
                UserId = userId,
                Username = "testuser",
                Email = "test@example.com",
                UserRole = UserRole.User,
                RefreshToken = "existing_refresh_token",
                RefreshTokenExpiry = DateTime.Now.AddDays(5),
                UpdatedAt = DateTime.Now.AddDays(-10)
            };

            _mockUserRepository.Setup(x => x.GetByIdAsync(userId))
                .ReturnsAsync(existingUser);

            User? updatedUser = null;
            _mockUserRepository.Setup(x => x.UpdateAsync(It.IsAny<User>()))
                .Callback<User>(u => updatedUser = u)
                .Returns(Task.CompletedTask);
            _mockUserRepository.Setup(x => x.SaveAsync())
                .Returns(Task.CompletedTask);

            // Act
            var result = await _authService.LogoutAsync(userId);

            // Assert
            Assert.True(result);
            Assert.NotNull(updatedUser);
            Assert.Null(updatedUser.RefreshToken);
            Assert.Null(updatedUser.RefreshTokenExpiry);
            _mockUserRepository.Verify(x => x.UpdateAsync(It.IsAny<User>()), Times.Once);
            _mockUserRepository.Verify(x => x.SaveAsync(), Times.Once);
        }

        [Fact]
        public async Task LogoutAsync_ShouldSetRefreshTokenToNull()
        {
            // Arrange
            var userId = 5;
            var originalRefreshToken = "some_refresh_token_value";
            var existingUser = new User
            {
                UserId = userId,
                Username = "testuser",
                Email = "test@example.com",
                UserRole = UserRole.User,
                RefreshToken = originalRefreshToken,
                RefreshTokenExpiry = DateTime.Now.AddDays(3)
            };

            _mockUserRepository.Setup(x => x.GetByIdAsync(userId))
                .ReturnsAsync(existingUser);

            User? updatedUser = null;
            _mockUserRepository.Setup(x => x.UpdateAsync(It.IsAny<User>()))
                .Callback<User>(u => updatedUser = u)
                .Returns(Task.CompletedTask);
            _mockUserRepository.Setup(x => x.SaveAsync())
                .Returns(Task.CompletedTask);

            // Act
            await _authService.LogoutAsync(userId);

            // Assert
            Assert.NotNull(updatedUser);
            Assert.Null(updatedUser.RefreshToken);
            // Примечание: AuthService изменяет тот же объект, что получен из репозитория,
            // поэтому existingUser.RefreshToken также будет null после вызова метода
            // Это ожидаемое поведение - метод модифицирует объект напрямую
            Assert.Null(existingUser.RefreshToken);
        }

        [Fact]
        public async Task LogoutAsync_ShouldSetRefreshTokenExpiryToNull()
        {
            // Arrange
            var userId = 10;
            var existingUser = new User
            {
                UserId = userId,
                Username = "testuser",
                Email = "test@example.com",
                UserRole = UserRole.Admin,
                RefreshToken = "token",
                RefreshTokenExpiry = DateTime.Now.AddDays(7)
            };

            _mockUserRepository.Setup(x => x.GetByIdAsync(userId))
                .ReturnsAsync(existingUser);

            User? updatedUser = null;
            _mockUserRepository.Setup(x => x.UpdateAsync(It.IsAny<User>()))
                .Callback<User>(u => updatedUser = u)
                .Returns(Task.CompletedTask);
            _mockUserRepository.Setup(x => x.SaveAsync())
                .Returns(Task.CompletedTask);

            // Act
            await _authService.LogoutAsync(userId);

            // Assert
            Assert.NotNull(updatedUser);
            Assert.Null(updatedUser.RefreshTokenExpiry);
        }

        [Fact]
        public async Task LogoutAsync_ShouldReturnTrueOnSuccess()
        {
            // Arrange
            var userId = 1;
            var existingUser = new User
            {
                UserId = userId,
                Username = "testuser",
                Email = "test@example.com",
                UserRole = UserRole.User,
                RefreshToken = "token",
                RefreshTokenExpiry = DateTime.Now.AddDays(5)
            };

            _mockUserRepository.Setup(x => x.GetByIdAsync(userId))
                .ReturnsAsync(existingUser);
            _mockUserRepository.Setup(x => x.UpdateAsync(It.IsAny<User>()))
                .Returns(Task.CompletedTask);
            _mockUserRepository.Setup(x => x.SaveAsync())
                .Returns(Task.CompletedTask);

            // Act
            var result = await _authService.LogoutAsync(userId);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task LogoutAsync_ShouldReturnFalseWhenUserNotFound()
        {
            // Arrange
            var nonExistentUserId = 999;

            _mockUserRepository.Setup(x => x.GetByIdAsync(nonExistentUserId))
                .ReturnsAsync((User?)null);

            // Act
            var result = await _authService.LogoutAsync(nonExistentUserId);

            // Assert
            Assert.False(result);
            _mockUserRepository.Verify(x => x.UpdateAsync(It.IsAny<User>()), Times.Never);
            _mockUserRepository.Verify(x => x.SaveAsync(), Times.Never);
        }

        [Fact]
        public async Task LogoutAsync_ShouldUpdateUpdatedAtTimestamp()
        {
            // Arrange
            var userId = 1;
            var existingUser = new User
            {
                UserId = userId,
                Username = "testuser",
                Email = "test@example.com",
                UserRole = UserRole.User,
                RefreshToken = "token",
                RefreshTokenExpiry = DateTime.Now.AddDays(5),
                UpdatedAt = DateTime.Now.AddDays(-10)  // Старая дата
            };

            _mockUserRepository.Setup(x => x.GetByIdAsync(userId))
                .ReturnsAsync(existingUser);

            User? updatedUser = null;
            var beforeLogout = DateTime.Now;
            _mockUserRepository.Setup(x => x.UpdateAsync(It.IsAny<User>()))
                .Callback<User>(u => updatedUser = u)
                .Returns(Task.CompletedTask);
            _mockUserRepository.Setup(x => x.SaveAsync())
                .Returns(Task.CompletedTask);

            // Act
            await _authService.LogoutAsync(userId);
            var afterLogout = DateTime.Now;

            // Assert
            Assert.NotNull(updatedUser);
            Assert.True(updatedUser.UpdatedAt >= beforeLogout && updatedUser.UpdatedAt <= afterLogout);
        }

        [Fact]
        public async Task LogoutAsync_ShouldClearBothRefreshTokenAndExpiry()
        {
            // Arrange
            var userId = 1;
            var existingUser = new User
            {
                UserId = userId,
                Username = "testuser",
                Email = "test@example.com",
                UserRole = UserRole.User,
                RefreshToken = "existing_token_123",
                RefreshTokenExpiry = DateTime.Now.AddDays(3)
            };

            _mockUserRepository.Setup(x => x.GetByIdAsync(userId))
                .ReturnsAsync(existingUser);

            User? updatedUser = null;
            _mockUserRepository.Setup(x => x.UpdateAsync(It.IsAny<User>()))
                .Callback<User>(u => updatedUser = u)
                .Returns(Task.CompletedTask);
            _mockUserRepository.Setup(x => x.SaveAsync())
                .Returns(Task.CompletedTask);

            // Act
            var result = await _authService.LogoutAsync(userId);

            // Assert
            Assert.True(result);
            Assert.NotNull(updatedUser);
            Assert.Null(updatedUser.RefreshToken);
            Assert.Null(updatedUser.RefreshTokenExpiry);
            // Проверяем что оба поля очищены одновременно
            Assert.True(updatedUser.RefreshToken == null && updatedUser.RefreshTokenExpiry == null);
        }

        [Fact]
        public async Task LogoutAsync_ShouldNotModifyOtherUserFields()
        {
            // Arrange
            var userId = 1;
            var existingUser = new User
            {
                UserId = userId,
                FirstName = "John",
                LastName = "Doe",
                Username = "johndoe",
                Email = "john@example.com",
                UserRole = UserRole.Admin,
                RefreshToken = "token_to_clear",
                RefreshTokenExpiry = DateTime.Now.AddDays(5),
                CreatedAt = DateTime.Now.AddDays(-100),
                UpdatedAt = DateTime.Now.AddDays(-5)
            };

            _mockUserRepository.Setup(x => x.GetByIdAsync(userId))
                .ReturnsAsync(existingUser);

            User? updatedUser = null;
            _mockUserRepository.Setup(x => x.UpdateAsync(It.IsAny<User>()))
                .Callback<User>(u => updatedUser = u)
                .Returns(Task.CompletedTask);
            _mockUserRepository.Setup(x => x.SaveAsync())
                .Returns(Task.CompletedTask);

            // Act
            await _authService.LogoutAsync(userId);

            // Assert
            Assert.NotNull(updatedUser);
            Assert.Equal(existingUser.UserId, updatedUser.UserId);
            Assert.Equal(existingUser.FirstName, updatedUser.FirstName);
            Assert.Equal(existingUser.LastName, updatedUser.LastName);
            Assert.Equal(existingUser.Username, updatedUser.Username);
            Assert.Equal(existingUser.Email, updatedUser.Email);
            Assert.Equal(existingUser.UserRole, updatedUser.UserRole);
            Assert.Equal(existingUser.CreatedAt, updatedUser.CreatedAt);
            // Только RefreshToken, RefreshTokenExpiry и UpdatedAt должны измениться
            Assert.Null(updatedUser.RefreshToken);
            Assert.Null(updatedUser.RefreshTokenExpiry);
        }

        [Fact]
        public async Task LogoutAsync_ShouldHandleUserWithoutRefreshToken()
        {
            // Arrange
            var userId = 1;
            var existingUser = new User
            {
                UserId = userId,
                Username = "testuser",
                Email = "test@example.com",
                UserRole = UserRole.User,
                RefreshToken = null,  // Уже null
                RefreshTokenExpiry = null  // Уже null
            };

            _mockUserRepository.Setup(x => x.GetByIdAsync(userId))
                .ReturnsAsync(existingUser);

            User? updatedUser = null;
            _mockUserRepository.Setup(x => x.UpdateAsync(It.IsAny<User>()))
                .Callback<User>(u => updatedUser = u)
                .Returns(Task.CompletedTask);
            _mockUserRepository.Setup(x => x.SaveAsync())
                .Returns(Task.CompletedTask);

            // Act
            var result = await _authService.LogoutAsync(userId);

            // Assert
            Assert.True(result);
            Assert.NotNull(updatedUser);
            Assert.Null(updatedUser.RefreshToken);
            Assert.Null(updatedUser.RefreshTokenExpiry);
            // Метод должен успешно выполниться даже если токен уже был null
        }
    }
}

