using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ProjectCinema.BLL.DTO.Users;
using ProjectCinema.BLL.Interfaces;
using ProjectCinema.Controllers;
using ProjectCinema.Enums;

namespace ProjectCinema.Tests.Controllers
{
    public class AuthControllerTests
    {
        private readonly Mock<IAuthService> _mockAuthService;
        private readonly Mock<IJwtTokenService> _mockJwtTokenService;
        private readonly AuthController _authController;

        public AuthControllerTests()
        {
            _mockAuthService = new Mock<IAuthService>();
            _mockJwtTokenService = new Mock<IJwtTokenService>();
            _authController = new AuthController(_mockAuthService.Object, _mockJwtTokenService.Object);
            
            // Setup default HttpContext for controller
            var httpContext = new DefaultHttpContext();
            _authController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
        }

        [Fact]
        public async Task RegisterAsync_ShouldReturn200OKOnSuccessfulRegistration()
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

            var authResponse = new AuthResponseDTO
            {
                Token = "access_token",
                RefreshToken = "refresh_token",
                UserId = 1,
                Email = "john@example.com",
                Username = "johndoe",
                Role = UserRole.User,
                ExpiresAt = DateTime.Now.AddHours(1)
            };

            _mockAuthService.Setup(x => x.RegisterAsync(registerDto))
                .ReturnsAsync(authResponse);

            // Act
            var result = await _authController.RegisterAsync(registerDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        public async Task RegisterAsync_ShouldReturnAuthResponseDTOInResponse()
        {
            // Arrange
            var registerDto = new RegisterDTO
            {
                FirstName = "Jane",
                LastName = "Smith",
                Username = "janesmith",
                Email = "jane@example.com",
                Password = "SecurePass123!",
                ConfirmPassword = "SecurePass123!",
                DateOfBirth = new DateOnly(1990, 1, 1)
            };

            var expectedAuthResponse = new AuthResponseDTO
            {
                Token = "access_token_result",
                RefreshToken = "refresh_token_result",
                UserId = 5,
                Email = "jane@example.com",
                Username = "janesmith",
                Role = UserRole.User,
                ExpiresAt = DateTime.Now.AddHours(1)
            };

            _mockAuthService.Setup(x => x.RegisterAsync(registerDto))
                .ReturnsAsync(expectedAuthResponse);

            // Act
            var result = await _authController.RegisterAsync(registerDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedAuthResponse = Assert.IsType<AuthResponseDTO>(okResult.Value);
            Assert.Equal(expectedAuthResponse.Token, returnedAuthResponse.Token);
            Assert.Equal(expectedAuthResponse.RefreshToken, returnedAuthResponse.RefreshToken);
            Assert.Equal(expectedAuthResponse.UserId, returnedAuthResponse.UserId);
            Assert.Equal(expectedAuthResponse.Email, returnedAuthResponse.Email);
            Assert.Equal(expectedAuthResponse.Username, returnedAuthResponse.Username);
            Assert.Equal(expectedAuthResponse.Role, returnedAuthResponse.Role);
        }

        [Fact]
        public async Task RegisterAsync_ShouldReturn400BadRequestWhenEmailExists()
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

            _mockAuthService.Setup(x => x.RegisterAsync(registerDto))
                .ThrowsAsync(new InvalidOperationException("User with this email already exists"));

            // Act
            var result = await _authController.RegisterAsync(registerDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal(400, badRequestResult.StatusCode);
            
            var errorObject = badRequestResult.Value;
            var errorProperty = errorObject?.GetType().GetProperty("error");
            var errorMessage = errorProperty?.GetValue(errorObject)?.ToString();
            Assert.Equal("User with this email already exists", errorMessage);
        }

        [Fact]
        public async Task RegisterAsync_ShouldReturn400BadRequestWhenUsernameExists()
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

            _mockAuthService.Setup(x => x.RegisterAsync(registerDto))
                .ThrowsAsync(new InvalidOperationException("User with this username already exists"));

            // Act
            var result = await _authController.RegisterAsync(registerDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal(400, badRequestResult.StatusCode);
            
            var errorObject = badRequestResult.Value;
            var errorProperty = errorObject?.GetType().GetProperty("error");
            var errorMessage = errorProperty?.GetValue(errorObject)?.ToString();
            Assert.Equal("User with this username already exists", errorMessage);
        }

        [Fact]
        public async Task RegisterAsync_ShouldReturn400BadRequestForInvalidData()
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

            _mockAuthService.Setup(x => x.RegisterAsync(registerDto))
                .ThrowsAsync(new Exception("Database connection error"));

            // Act
            var result = await _authController.RegisterAsync(registerDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal(400, badRequestResult.StatusCode);
            
            var errorObject = badRequestResult.Value;
            var errorProperty = errorObject?.GetType().GetProperty("error");
            var errorMessage = errorProperty?.GetValue(errorObject)?.ToString();
            Assert.Equal("Database connection error", errorMessage);
        }

        [Fact]
        public async Task RegisterAsync_ShouldCallRegisterAsyncInService()
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

            var authResponse = new AuthResponseDTO
            {
                Token = "token",
                RefreshToken = "refresh",
                UserId = 1,
                Email = "john@example.com",
                Username = "johndoe",
                Role = UserRole.User,
                ExpiresAt = DateTime.Now.AddHours(1)
            };

            _mockAuthService.Setup(x => x.RegisterAsync(registerDto))
                .ReturnsAsync(authResponse);

            // Act
            await _authController.RegisterAsync(registerDto);

            // Assert
            _mockAuthService.Verify(x => x.RegisterAsync(registerDto), Times.Once);
        }

        [Fact]
        public async Task RegisterAsync_ShouldPassCorrectParametersToService()
        {
            // Arrange
            var registerDto = new RegisterDTO
            {
                FirstName = "Test",
                LastName = "User",
                Username = "testuser",
                Email = "test@example.com",
                Password = "TestPass123!",
                ConfirmPassword = "TestPass123!",
                PhoneNumber = "+1234567890",
                DateOfBirth = new DateOnly(1995, 5, 15)
            };

            var authResponse = new AuthResponseDTO
            {
                Token = "token",
                RefreshToken = "refresh",
                UserId = 1,
                Email = "test@example.com",
                Username = "testuser",
                Role = UserRole.User,
                ExpiresAt = DateTime.Now.AddHours(1)
            };

            _mockAuthService.Setup(x => x.RegisterAsync(It.IsAny<RegisterDTO>()))
                .ReturnsAsync(authResponse);

            // Act
            await _authController.RegisterAsync(registerDto);

            // Assert
            _mockAuthService.Verify(x => x.RegisterAsync(
                It.Is<RegisterDTO>(dto => 
                    dto.FirstName == registerDto.FirstName &&
                    dto.LastName == registerDto.LastName &&
                    dto.Username == registerDto.Username &&
                    dto.Email == registerDto.Email &&
                    dto.Password == registerDto.Password &&
                    dto.ConfirmPassword == registerDto.ConfirmPassword &&
                    dto.PhoneNumber == registerDto.PhoneNumber &&
                    dto.DateOfBirth == registerDto.DateOfBirth
                )), Times.Once);
        }

        [Fact]
        public async Task RegisterAsync_ShouldReturnBadRequestForAnyException()
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

            _mockAuthService.Setup(x => x.RegisterAsync(registerDto))
                .ThrowsAsync(new ArgumentException("Invalid argument"));

            // Act
            var result = await _authController.RegisterAsync(registerDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal(400, badRequestResult.StatusCode);
        }

        [Fact]
        public async Task LoginAsync_ShouldReturn200OKOnSuccessfulLogin()
        {
            // Arrange
            var loginDto = new LoginUserDTO
            {
                UsernameOrEmail = "john@example.com",
                Password = "SecurePass123!"
            };

            var authResponse = new AuthResponseDTO
            {
                Token = "access_token",
                RefreshToken = "refresh_token",
                UserId = 1,
                Email = "john@example.com",
                Username = "johndoe",
                Role = UserRole.User,
                ExpiresAt = DateTime.Now.AddHours(1)
            };

            _mockAuthService.Setup(x => x.LoginAsync(loginDto))
                .ReturnsAsync(authResponse);

            // Act
            var result = await _authController.LoginAsync(loginDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        public async Task LoginAsync_ShouldReturnAuthResponseDTOWithTokens()
        {
            // Arrange
            var loginDto = new LoginUserDTO
            {
                UsernameOrEmail = "jane@example.com",
                Password = "MyPassword123!"
            };

            var expectedAuthResponse = new AuthResponseDTO
            {
                Token = "access_token_result",
                RefreshToken = "refresh_token_result",
                UserId = 5,
                Email = "jane@example.com",
                Username = "janesmith",
                Role = UserRole.Admin,
                ExpiresAt = DateTime.Now.AddHours(1)
            };

            _mockAuthService.Setup(x => x.LoginAsync(loginDto))
                .ReturnsAsync(expectedAuthResponse);

            // Act
            var result = await _authController.LoginAsync(loginDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedAuthResponse = Assert.IsType<AuthResponseDTO>(okResult.Value);
            Assert.Equal(expectedAuthResponse.Token, returnedAuthResponse.Token);
            Assert.Equal(expectedAuthResponse.RefreshToken, returnedAuthResponse.RefreshToken);
            Assert.Equal(expectedAuthResponse.UserId, returnedAuthResponse.UserId);
            Assert.Equal(expectedAuthResponse.Email, returnedAuthResponse.Email);
            Assert.Equal(expectedAuthResponse.Username, returnedAuthResponse.Username);
            Assert.Equal(expectedAuthResponse.Role, returnedAuthResponse.Role);
            Assert.NotNull(returnedAuthResponse.Token);
            Assert.NotNull(returnedAuthResponse.RefreshToken);
        }

        [Fact]
        public async Task LoginAsync_ShouldReturn401UnauthorizedForInvalidCredentials()
        {
            // Arrange
            var loginDto = new LoginUserDTO
            {
                UsernameOrEmail = "nonexistent@example.com",
                Password = "WrongPassword123!"
            };

            _mockAuthService.Setup(x => x.LoginAsync(loginDto))
                .ThrowsAsync(new UnauthorizedAccessException("Invalid credentials"));

            // Act
            var result = await _authController.LoginAsync(loginDto);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result.Result);
            Assert.Equal(401, unauthorizedResult.StatusCode);
            
            var errorObject = unauthorizedResult.Value;
            var errorProperty = errorObject?.GetType().GetProperty("error");
            var errorMessage = errorProperty?.GetValue(errorObject)?.ToString();
            Assert.Equal("Invalid credentials", errorMessage);
        }

        [Fact]
        public async Task LoginAsync_ShouldReturn401UnauthorizedForWrongPassword()
        {
            // Arrange
            var loginDto = new LoginUserDTO
            {
                UsernameOrEmail = "john@example.com",
                Password = "WrongPassword123!"
            };

            _mockAuthService.Setup(x => x.LoginAsync(loginDto))
                .ThrowsAsync(new UnauthorizedAccessException("Invalid credentials"));

            // Act
            var result = await _authController.LoginAsync(loginDto);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result.Result);
            Assert.Equal(401, unauthorizedResult.StatusCode);
        }

        [Fact]
        public async Task LoginAsync_ShouldReturn401UnauthorizedForNonExistentUser()
        {
            // Arrange
            var loginDto = new LoginUserDTO
            {
                UsernameOrEmail = "unknown@example.com",
                Password = "AnyPassword123!"
            };

            _mockAuthService.Setup(x => x.LoginAsync(loginDto))
                .ThrowsAsync(new UnauthorizedAccessException("Invalid credentials"));

            // Act
            var result = await _authController.LoginAsync(loginDto);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result.Result);
            Assert.Equal(401, unauthorizedResult.StatusCode);
        }

        [Fact]
        public async Task LoginAsync_ShouldReturn400BadRequestForOtherErrors()
        {
            // Arrange
            var loginDto = new LoginUserDTO
            {
                UsernameOrEmail = "john@example.com",
                Password = "SecurePass123!"
            };

            _mockAuthService.Setup(x => x.LoginAsync(loginDto))
                .ThrowsAsync(new Exception("Database connection error"));

            // Act
            var result = await _authController.LoginAsync(loginDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal(400, badRequestResult.StatusCode);
            
            var errorObject = badRequestResult.Value;
            var errorProperty = errorObject?.GetType().GetProperty("error");
            var errorMessage = errorProperty?.GetValue(errorObject)?.ToString();
            Assert.Equal("Database connection error", errorMessage);
        }

        [Fact]
        public async Task LoginAsync_ShouldCallLoginAsyncInService()
        {
            // Arrange
            var loginDto = new LoginUserDTO
            {
                UsernameOrEmail = "john@example.com",
                Password = "SecurePass123!"
            };

            var authResponse = new AuthResponseDTO
            {
                Token = "token",
                RefreshToken = "refresh",
                UserId = 1,
                Email = "john@example.com",
                Username = "johndoe",
                Role = UserRole.User,
                ExpiresAt = DateTime.Now.AddHours(1)
            };

            _mockAuthService.Setup(x => x.LoginAsync(loginDto))
                .ReturnsAsync(authResponse);

            // Act
            await _authController.LoginAsync(loginDto);

            // Assert
            _mockAuthService.Verify(x => x.LoginAsync(loginDto), Times.Once);
        }

        [Fact]
        public async Task LoginAsync_ShouldPassCorrectParametersToService()
        {
            // Arrange
            var loginDto = new LoginUserDTO
            {
                UsernameOrEmail = "testuser",
                Password = "TestPassword123!"
            };

            var authResponse = new AuthResponseDTO
            {
                Token = "token",
                RefreshToken = "refresh",
                UserId = 1,
                Email = "test@example.com",
                Username = "testuser",
                Role = UserRole.User,
                ExpiresAt = DateTime.Now.AddHours(1)
            };

            _mockAuthService.Setup(x => x.LoginAsync(It.IsAny<LoginUserDTO>()))
                .ReturnsAsync(authResponse);

            // Act
            await _authController.LoginAsync(loginDto);

            // Assert
            _mockAuthService.Verify(x => x.LoginAsync(
                It.Is<LoginUserDTO>(dto => 
                    dto.UsernameOrEmail == loginDto.UsernameOrEmail &&
                    dto.Password == loginDto.Password
                )), Times.Once);
        }

        [Fact]
        public async Task LoginAsync_ShouldReturnBadRequestForArgumentException()
        {
            // Arrange
            var loginDto = new LoginUserDTO
            {
                UsernameOrEmail = "john@example.com",
                Password = "SecurePass123!"
            };

            _mockAuthService.Setup(x => x.LoginAsync(loginDto))
                .ThrowsAsync(new ArgumentException("Invalid argument"));

            // Act
            var result = await _authController.LoginAsync(loginDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal(400, badRequestResult.StatusCode);
        }

        [Fact]
        public async Task LoginAsync_ShouldHandleLoginByEmail()
        {
            // Arrange
            var loginDto = new LoginUserDTO
            {
                UsernameOrEmail = "user@example.com",  // Email
                Password = "Password123!"
            };

            var authResponse = new AuthResponseDTO
            {
                Token = "token",
                RefreshToken = "refresh",
                UserId = 1,
                Email = "user@example.com",
                Username = "testuser",
                Role = UserRole.User,
                ExpiresAt = DateTime.Now.AddHours(1)
            };

            _mockAuthService.Setup(x => x.LoginAsync(loginDto))
                .ReturnsAsync(authResponse);

            // Act
            var result = await _authController.LoginAsync(loginDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(200, okResult.StatusCode);
            _mockAuthService.Verify(x => x.LoginAsync(loginDto), Times.Once);
        }

        [Fact]
        public async Task LoginAsync_ShouldHandleLoginByUsername()
        {
            // Arrange
            var loginDto = new LoginUserDTO
            {
                UsernameOrEmail = "testuser",  // Username
                Password = "Password123!"
            };

            var authResponse = new AuthResponseDTO
            {
                Token = "token",
                RefreshToken = "refresh",
                UserId = 1,
                Email = "user@example.com",
                Username = "testuser",
                Role = UserRole.User,
                ExpiresAt = DateTime.Now.AddHours(1)
            };

            _mockAuthService.Setup(x => x.LoginAsync(loginDto))
                .ReturnsAsync(authResponse);

            // Act
            var result = await _authController.LoginAsync(loginDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(200, okResult.StatusCode);
            _mockAuthService.Verify(x => x.LoginAsync(loginDto), Times.Once);
        }

        [Fact]
        public async Task RefreshTokenAsync_ShouldReturn200OKOnSuccessfulRefresh()
        {
            // Arrange
            var refreshTokenDto = new RefreshTokenDTO
            {
                RefreshToken = "valid_refresh_token_123"
            };

            var authResponse = new AuthResponseDTO
            {
                Token = "new_access_token",
                RefreshToken = "new_refresh_token",
                UserId = 1,
                Email = "john@example.com",
                Username = "johndoe",
                Role = UserRole.User,
                ExpiresAt = DateTime.Now.AddHours(1)
            };

            _mockAuthService.Setup(x => x.RefreshTokenAsync(refreshTokenDto.RefreshToken))
                .ReturnsAsync(authResponse);

            // Act
            var result = await _authController.RefreshTokenAsync(refreshTokenDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        public async Task RefreshTokenAsync_ShouldReturnNewAuthResponseDTO()
        {
            // Arrange
            var refreshTokenDto = new RefreshTokenDTO
            {
                RefreshToken = "valid_refresh_token"
            };

            var expectedAuthResponse = new AuthResponseDTO
            {
                Token = "new_access_token_result",
                RefreshToken = "new_refresh_token_result",
                UserId = 5,
                Email = "jane@example.com",
                Username = "janesmith",
                Role = UserRole.Admin,
                ExpiresAt = DateTime.Now.AddHours(1)
            };

            _mockAuthService.Setup(x => x.RefreshTokenAsync(refreshTokenDto.RefreshToken))
                .ReturnsAsync(expectedAuthResponse);

            // Act
            var result = await _authController.RefreshTokenAsync(refreshTokenDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedAuthResponse = Assert.IsType<AuthResponseDTO>(okResult.Value);
            Assert.Equal(expectedAuthResponse.Token, returnedAuthResponse.Token);
            Assert.Equal(expectedAuthResponse.RefreshToken, returnedAuthResponse.RefreshToken);
            Assert.Equal(expectedAuthResponse.UserId, returnedAuthResponse.UserId);
            Assert.Equal(expectedAuthResponse.Email, returnedAuthResponse.Email);
            Assert.Equal(expectedAuthResponse.Username, returnedAuthResponse.Username);
            Assert.Equal(expectedAuthResponse.Role, returnedAuthResponse.Role);
            Assert.NotNull(returnedAuthResponse.Token);
            Assert.NotNull(returnedAuthResponse.RefreshToken);
        }

        [Fact]
        public async Task RefreshTokenAsync_ShouldReturn401UnauthorizedForInvalidRefreshToken()
        {
            // Arrange
            var refreshTokenDto = new RefreshTokenDTO
            {
                RefreshToken = "invalid_refresh_token"
            };

            _mockAuthService.Setup(x => x.RefreshTokenAsync(refreshTokenDto.RefreshToken))
                .ThrowsAsync(new UnauthorizedAccessException("Invalid refresh token"));

            // Act
            var result = await _authController.RefreshTokenAsync(refreshTokenDto);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result.Result);
            Assert.Equal(401, unauthorizedResult.StatusCode);
            
            var errorObject = unauthorizedResult.Value;
            var errorProperty = errorObject?.GetType().GetProperty("error");
            var errorMessage = errorProperty?.GetValue(errorObject)?.ToString();
            Assert.Equal("Invalid refresh token", errorMessage);
        }

        [Fact]
        public async Task RefreshTokenAsync_ShouldReturn401UnauthorizedForExpiredRefreshToken()
        {
            // Arrange
            var refreshTokenDto = new RefreshTokenDTO
            {
                RefreshToken = "expired_refresh_token"
            };

            _mockAuthService.Setup(x => x.RefreshTokenAsync(refreshTokenDto.RefreshToken))
                .ThrowsAsync(new UnauthorizedAccessException("Refresh token expired"));

            // Act
            var result = await _authController.RefreshTokenAsync(refreshTokenDto);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result.Result);
            Assert.Equal(401, unauthorizedResult.StatusCode);
            
            var errorObject = unauthorizedResult.Value;
            var errorProperty = errorObject?.GetType().GetProperty("error");
            var errorMessage = errorProperty?.GetValue(errorObject)?.ToString();
            Assert.Equal("Refresh token expired", errorMessage);
        }

        [Fact]
        public async Task RefreshTokenAsync_ShouldReturn400BadRequestForOtherErrors()
        {
            // Arrange
            var refreshTokenDto = new RefreshTokenDTO
            {
                RefreshToken = "some_refresh_token"
            };

            _mockAuthService.Setup(x => x.RefreshTokenAsync(refreshTokenDto.RefreshToken))
                .ThrowsAsync(new Exception("Database connection error"));

            // Act
            var result = await _authController.RefreshTokenAsync(refreshTokenDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal(400, badRequestResult.StatusCode);
            
            var errorObject = badRequestResult.Value;
            var errorProperty = errorObject?.GetType().GetProperty("error");
            var errorMessage = errorProperty?.GetValue(errorObject)?.ToString();
            Assert.Equal("Database connection error", errorMessage);
        }

        [Fact]
        public async Task RefreshTokenAsync_ShouldCallRefreshTokenAsyncInService()
        {
            // Arrange
            var refreshTokenDto = new RefreshTokenDTO
            {
                RefreshToken = "valid_refresh_token_456"
            };

            var authResponse = new AuthResponseDTO
            {
                Token = "token",
                RefreshToken = "refresh",
                UserId = 1,
                Email = "john@example.com",
                Username = "johndoe",
                Role = UserRole.User,
                ExpiresAt = DateTime.Now.AddHours(1)
            };

            _mockAuthService.Setup(x => x.RefreshTokenAsync(refreshTokenDto.RefreshToken))
                .ReturnsAsync(authResponse);

            // Act
            await _authController.RefreshTokenAsync(refreshTokenDto);

            // Assert
            _mockAuthService.Verify(x => x.RefreshTokenAsync(refreshTokenDto.RefreshToken), Times.Once);
        }

        [Fact]
        public async Task RefreshTokenAsync_ShouldPassCorrectRefreshTokenToService()
        {
            // Arrange
            var refreshTokenDto = new RefreshTokenDTO
            {
                RefreshToken = "specific_refresh_token_789"
            };

            var authResponse = new AuthResponseDTO
            {
                Token = "token",
                RefreshToken = "refresh",
                UserId = 1,
                Email = "test@example.com",
                Username = "testuser",
                Role = UserRole.User,
                ExpiresAt = DateTime.Now.AddHours(1)
            };

            _mockAuthService.Setup(x => x.RefreshTokenAsync(It.IsAny<string>()))
                .ReturnsAsync(authResponse);

            // Act
            await _authController.RefreshTokenAsync(refreshTokenDto);

            // Assert
            _mockAuthService.Verify(x => x.RefreshTokenAsync(
                It.Is<string>(token => token == refreshTokenDto.RefreshToken)), Times.Once);
        }

        [Fact]
        public async Task RefreshTokenAsync_ShouldReturnBadRequestForArgumentException()
        {
            // Arrange
            var refreshTokenDto = new RefreshTokenDTO
            {
                RefreshToken = "token"
            };

            _mockAuthService.Setup(x => x.RefreshTokenAsync(refreshTokenDto.RefreshToken))
                .ThrowsAsync(new ArgumentException("Invalid argument"));

            // Act
            var result = await _authController.RefreshTokenAsync(refreshTokenDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal(400, badRequestResult.StatusCode);
        }

        [Fact]
        public async Task RefreshTokenAsync_ShouldReturnNewTokensInResponse()
        {
            // Arrange
            var refreshTokenDto = new RefreshTokenDTO
            {
                RefreshToken = "old_refresh_token"
            };

            var authResponse = new AuthResponseDTO
            {
                Token = "completely_new_access_token",
                RefreshToken = "completely_new_refresh_token",
                UserId = 10,
                Email = "user@example.com",
                Username = "testuser",
                Role = UserRole.User,
                ExpiresAt = DateTime.Now.AddHours(1)
            };

            _mockAuthService.Setup(x => x.RefreshTokenAsync(refreshTokenDto.RefreshToken))
                .ReturnsAsync(authResponse);

            // Act
            var result = await _authController.RefreshTokenAsync(refreshTokenDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedAuthResponse = Assert.IsType<AuthResponseDTO>(okResult.Value);
            Assert.Equal("completely_new_access_token", returnedAuthResponse.Token);
            Assert.Equal("completely_new_refresh_token", returnedAuthResponse.RefreshToken);
            Assert.NotEqual(refreshTokenDto.RefreshToken, returnedAuthResponse.RefreshToken);
        }

        [Fact]
        public async Task LogoutAsync_ShouldReturn200OKOnSuccessfulLogout()
        {
            // Arrange
            var token = "valid_access_token";
            var userId = 1;

            _authController.ControllerContext.HttpContext.Request.Headers["Authorization"] = $"Bearer {token}";

            _mockJwtTokenService.Setup(x => x.GetUserIdFromToken(token))
                .Returns(userId);

            _mockAuthService.Setup(x => x.LogoutAsync(userId))
                .ReturnsAsync(true);

            // Act
            var result = await _authController.LogoutAsync();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
            
            var responseObject = okResult.Value;
            var messageProperty = responseObject?.GetType().GetProperty("message");
            var message = messageProperty?.GetValue(responseObject)?.ToString();
            Assert.Equal("Logged out successfully", message);
        }

        [Fact]
        public async Task LogoutAsync_ShouldReturn401UnauthorizedWhenTokenNotProvided()
        {
            // Arrange
            // Не устанавливаем заголовок Authorization
            _authController.ControllerContext.HttpContext.Request.Headers.Clear();

            // Act
            var result = await _authController.LogoutAsync();

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal(401, unauthorizedResult.StatusCode);
            
            var errorObject = unauthorizedResult.Value;
            var errorProperty = errorObject?.GetType().GetProperty("error");
            var errorMessage = errorProperty?.GetValue(errorObject)?.ToString();
            Assert.Equal("Token not provided", errorMessage);
        }

        [Fact]
        public async Task LogoutAsync_ShouldReturn401UnauthorizedWhenTokenIsEmpty()
        {
            // Arrange
            _authController.ControllerContext.HttpContext.Request.Headers["Authorization"] = "";

            // Act
            var result = await _authController.LogoutAsync();

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal(401, unauthorizedResult.StatusCode);
            
            var errorObject = unauthorizedResult.Value;
            var errorProperty = errorObject?.GetType().GetProperty("error");
            var errorMessage = errorProperty?.GetValue(errorObject)?.ToString();
            Assert.Equal("Token not provided", errorMessage);
        }

        [Fact]
        public async Task LogoutAsync_ShouldReturn401UnauthorizedForInvalidToken()
        {
            // Arrange
            var invalidToken = "invalid_token_string";
            _authController.ControllerContext.HttpContext.Request.Headers["Authorization"] = $"Bearer {invalidToken}";

            _mockJwtTokenService.Setup(x => x.GetUserIdFromToken(invalidToken))
                .Returns((int?)null);

            // Act
            var result = await _authController.LogoutAsync();

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal(401, unauthorizedResult.StatusCode);
            
            var errorObject = unauthorizedResult.Value;
            var errorProperty = errorObject?.GetType().GetProperty("error");
            var errorMessage = errorProperty?.GetValue(errorObject)?.ToString();
            Assert.Equal("Invalid token", errorMessage);
            
            _mockAuthService.Verify(x => x.LogoutAsync(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task LogoutAsync_ShouldReturn400BadRequestWhenLogoutFailed()
        {
            // Arrange
            var token = "valid_access_token";
            var userId = 1;

            _authController.ControllerContext.HttpContext.Request.Headers["Authorization"] = $"Bearer {token}";

            _mockJwtTokenService.Setup(x => x.GetUserIdFromToken(token))
                .Returns(userId);

            _mockAuthService.Setup(x => x.LogoutAsync(userId))
                .ReturnsAsync(false);  // Logout failed

            // Act
            var result = await _authController.LogoutAsync();

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
            
            var errorObject = badRequestResult.Value;
            var errorProperty = errorObject?.GetType().GetProperty("error");
            var errorMessage = errorProperty?.GetValue(errorObject)?.ToString();
            Assert.Equal("Logout failed", errorMessage);
        }

        [Fact]
        public async Task LogoutAsync_ShouldExtractTokenFromAuthorizationHeader()
        {
            // Arrange
            var token = "extracted_token_from_header";
            var userId = 5;

            _authController.ControllerContext.HttpContext.Request.Headers["Authorization"] = $"Bearer {token}";

            _mockJwtTokenService.Setup(x => x.GetUserIdFromToken(token))
                .Returns(userId);

            _mockAuthService.Setup(x => x.LogoutAsync(userId))
                .ReturnsAsync(true);

            // Act
            await _authController.LogoutAsync();

            // Assert
            _mockJwtTokenService.Verify(x => x.GetUserIdFromToken(token), Times.Once);
        }

        [Fact]
        public async Task LogoutAsync_ShouldRemoveBearerPrefixFromToken()
        {
            // Arrange
            var token = "actual_token_value";
            var userId = 1;

            // Устанавливаем заголовок с префиксом "Bearer "
            _authController.ControllerContext.HttpContext.Request.Headers["Authorization"] = $"Bearer {token}";

            _mockJwtTokenService.Setup(x => x.GetUserIdFromToken(token))
                .Returns(userId);

            _mockAuthService.Setup(x => x.LogoutAsync(userId))
                .ReturnsAsync(true);

            // Act
            await _authController.LogoutAsync();

            // Assert
            // Контроллер должен вызвать Replace("Bearer ", ""), поэтому проверяем что передается токен без префикса
            _mockJwtTokenService.Verify(x => x.GetUserIdFromToken(token), Times.Once);
            _mockJwtTokenService.Verify(x => x.GetUserIdFromToken($"Bearer {token}"), Times.Never);
        }

        [Fact]
        public async Task LogoutAsync_ShouldCallLogoutAsyncInService()
        {
            // Arrange
            var token = "valid_token";
            var userId = 10;

            _authController.ControllerContext.HttpContext.Request.Headers["Authorization"] = $"Bearer {token}";

            _mockJwtTokenService.Setup(x => x.GetUserIdFromToken(token))
                .Returns(userId);

            _mockAuthService.Setup(x => x.LogoutAsync(userId))
                .ReturnsAsync(true);

            // Act
            await _authController.LogoutAsync();

            // Assert
            _mockAuthService.Verify(x => x.LogoutAsync(userId), Times.Once);
        }

        [Fact]
        public async Task LogoutAsync_ShouldReturn400BadRequestForExceptions()
        {
            // Arrange
            var token = "valid_token";
            var userId = 1;

            _authController.ControllerContext.HttpContext.Request.Headers["Authorization"] = $"Bearer {token}";

            _mockJwtTokenService.Setup(x => x.GetUserIdFromToken(token))
                .Returns(userId);

            _mockAuthService.Setup(x => x.LogoutAsync(userId))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _authController.LogoutAsync();

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
            
            var errorObject = badRequestResult.Value;
            var errorProperty = errorObject?.GetType().GetProperty("error");
            var errorMessage = errorProperty?.GetValue(errorObject)?.ToString();
            Assert.Equal("Database error", errorMessage);
        }

        [Fact]
        public async Task LogoutAsync_ShouldReturnSuccessMessageOnSuccessfulLogout()
        {
            // Arrange
            var token = "valid_token";
            var userId = 1;

            _authController.ControllerContext.HttpContext.Request.Headers["Authorization"] = $"Bearer {token}";

            _mockJwtTokenService.Setup(x => x.GetUserIdFromToken(token))
                .Returns(userId);

            _mockAuthService.Setup(x => x.LogoutAsync(userId))
                .ReturnsAsync(true);

            // Act
            var result = await _authController.LogoutAsync();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var responseObject = okResult.Value;
            var messageProperty = responseObject?.GetType().GetProperty("message");
            var message = messageProperty?.GetValue(responseObject)?.ToString();
            Assert.Equal("Logged out successfully", message);
        }

        [Fact]
        public async Task LogoutAsync_ShouldHandleTokenWithoutBearerPrefix()
        {
            // Arrange
            var token = "token_without_bearer_prefix";
            var userId = 1;

            // Устанавливаем заголовок без префикса "Bearer "
            _authController.ControllerContext.HttpContext.Request.Headers["Authorization"] = token;

            // Контроллер все равно вызовет Replace("Bearer ", ""), поэтому токен останется как есть
            _mockJwtTokenService.Setup(x => x.GetUserIdFromToken(token))
                .Returns(userId);

            _mockAuthService.Setup(x => x.LogoutAsync(userId))
                .ReturnsAsync(true);

            // Act
            var result = await _authController.LogoutAsync();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
        }
    }
}

