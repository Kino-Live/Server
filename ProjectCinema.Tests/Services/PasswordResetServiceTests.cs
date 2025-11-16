using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Moq;
using ProjectCinema.BLL.DTO.Users;
using ProjectCinema.BLL.Interfaces;
using ProjectCinema.BLL.Services;
using ProjectCinema.Entities;
using ProjectCinema.Repositories.Interfaces;
using ProjectCinema.Settings;
using Xunit;

namespace ProjectCinema.Tests.Services
{
    public class PasswordResetServiceTests
    {
        private readonly Mock<IPasswordResetTokenRepository> _tokenRepoMock;
        private readonly Mock<IUserRepository> _userRepoMock;
        private readonly Mock<IEmailSender> _emailSenderMock;
        private readonly Mock<IOptions<PasswordResetOptions>> _optionsMock;

        private PasswordResetService CreateService(int ttlMinutes = 30, string? defaultRedirectUrl = "https://example.com/reset-password")
        {
            _optionsMock.Setup(o => o.Value).Returns(new PasswordResetOptions
            {
                TokenTTLMinutes = ttlMinutes,
                DefaultRedirectUrl = defaultRedirectUrl ?? string.Empty,
                AllowedRedirectHosts = new[] { "example.com" }
            });

            return new PasswordResetService(
                _tokenRepoMock.Object,
                _userRepoMock.Object,
                _emailSenderMock.Object,
                _optionsMock.Object
            );
        }

        // ========================= ValidateTokenAsync =========================

        [Fact]
        public async Task ValidateToken_ValidToken_ReturnsTrue()
        {
            
            var token = "test-token-123";
            var expectedHash = ComputeSha256HexLower(token);

            _tokenRepoMock.Setup(r => r.FindByTokenHashAsync(expectedHash))
                .ReturnsAsync(new PasswordResetToken
                {
                    ExpiresAt = DateTime.UtcNow.AddMinutes(5),
                    UsedAt = null
                });

            var service = CreateService();

            
            var result = await service.ValidateTokenAsync(token);

            // Assert
            Assert.True(result);
            _tokenRepoMock.Verify(r => r.FindByTokenHashAsync(expectedHash), Times.Once);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task ValidateToken_NullOrEmptyOrWhitespace_ReturnsFalse_AndDoesNotCallRepo(string? token)
        {
            
            var service = CreateService();

            // Act
            var result = await service.ValidateTokenAsync(token!);

            // Assert
            Assert.False(result);
            _tokenRepoMock.Verify(r => r.FindByTokenHashAsync(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task ValidateToken_NotFound_ReturnsFalse()
        {
            // Arrange
            var token = "not-found-token";
            var expectedHash = ComputeSha256HexLower(token);
            _tokenRepoMock.Setup(r => r.FindByTokenHashAsync(expectedHash))
                .ReturnsAsync((PasswordResetToken?)null);

            var service = CreateService();

            // Act
            var result = await service.ValidateTokenAsync(token);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task ValidateToken_Expired_ReturnsFalse()
        {
            // Arrange
            var token = "expired-token";
            var expectedHash = ComputeSha256HexLower(token);
            _tokenRepoMock.Setup(r => r.FindByTokenHashAsync(expectedHash))
                .ReturnsAsync(new PasswordResetToken
                {
                    ExpiresAt = DateTime.UtcNow.AddMinutes(-1),
                    UsedAt = null
                });

            var service = CreateService();

            // Act
            var result = await service.ValidateTokenAsync(token);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task ValidateToken_AlreadyUsed_ReturnsFalse()
        {
            // Arrange
            var token = "used-token";
            var expectedHash = ComputeSha256HexLower(token);
            _tokenRepoMock.Setup(r => r.FindByTokenHashAsync(expectedHash))
                .ReturnsAsync(new PasswordResetToken
                {
                    ExpiresAt = DateTime.UtcNow.AddMinutes(10),
                    UsedAt = DateTime.UtcNow
                });

            var service = CreateService();

            // Act
            var result = await service.ValidateTokenAsync(token);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task ValidateToken_HashComputedCorrectly_CallsRepoWithSha256LowerHex()
        {
            // Arrange
            var token = "hash-me";
            var expectedHash = ComputeSha256HexLower(token);
            PasswordResetToken tokenEntity = new PasswordResetToken
            {
                ExpiresAt = DateTime.UtcNow.AddMinutes(1),
                UsedAt = null
            };
            _tokenRepoMock.Setup(r => r.FindByTokenHashAsync(expectedHash)).ReturnsAsync(tokenEntity);

            var service = CreateService();

            // Act
            var result = await service.ValidateTokenAsync(token);

            // Assert
            Assert.True(result);
            _tokenRepoMock.Verify(r => r.FindByTokenHashAsync(expectedHash), Times.Once);
        }

        private static string ComputeSha256HexLower(string input)
        {
            var bytes = Encoding.UTF8.GetBytes(input);
            var hash = SHA256.HashData(bytes);
            var hex = Convert.ToHexString(hash).ToLowerInvariant();
            return hex;
        }

        // ========================= ConfirmPasswordResetAsync =========================

        [Fact]
        public async Task ConfirmPasswordReset_Success_UpdatesUser_InvalidatesRefresh_MarksTokenUsed_ReturnsTrue()
        {
            var token = "confirm-ok";
            var expectedHash = ComputeSha256HexLower(token);

            var originalHash = new byte[] { 1, 2, 3 };
            var originalSalt = new byte[] { 4, 5, 6 };

            var user = new User
            {
                UserId = 91,
                Email = "u@example.com",
                PasswordHash = originalHash,
                PasswordSalt = originalSalt,
                RefreshToken = "old-refresh",
                RefreshTokenExpiry = DateTime.UtcNow.AddDays(1),
                UpdatedAt = DateTime.UtcNow.AddDays(-1)
            };

            var tokenEntity = new PasswordResetToken
            {
                PasswordResetTokenId = 100,
                ExpiresAt = DateTime.UtcNow.AddMinutes(10),
                UsedAt = null,
                User = user
            };

            _tokenRepoMock.Setup(r => r.FindByTokenHashAsync(expectedHash)).ReturnsAsync(tokenEntity);
            _userRepoMock.Setup(r => r.UpdateAsync(It.IsAny<User>())).Returns(Task.CompletedTask);
            _userRepoMock.Setup(r => r.SaveAsync()).Returns(Task.CompletedTask);
            _tokenRepoMock.Setup(r => r.MarkAsUsedAsync(100)).ReturnsAsync(true);

            var service = CreateService();

            var dto = new PasswordResetConfirmDTO { Token = token, NewPassword = "NewPassword123!", ConfirmPassword = "NewPassword123!" };
            var result = await service.ConfirmPasswordResetAsync(dto);

            Assert.True(result);

            _tokenRepoMock.Verify(r => r.FindByTokenHashAsync(expectedHash), Times.Once);
            _userRepoMock.Verify(r => r.UpdateAsync(It.Is<User>(u => u.UserId == user.UserId)), Times.Once);
            _userRepoMock.Verify(r => r.SaveAsync(), Times.Once);
            _tokenRepoMock.Verify(r => r.MarkAsUsedAsync(100), Times.Once);

            Assert.NotNull(user.PasswordHash);
            Assert.NotNull(user.PasswordSalt);
            Assert.False(user.PasswordHash.SequenceEqual(originalHash));
            Assert.False(user.PasswordSalt.SequenceEqual(originalSalt));
            Assert.Null(user.RefreshToken);
            Assert.Null(user.RefreshTokenExpiry);
            Assert.True(user.UpdatedAt <= DateTime.UtcNow.AddSeconds(2));
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public async Task ConfirmPasswordReset_EmptyOrWhitespaceToken_ReturnsFalse_AndDoesNotCallRepositories(string token)
        {
            var service = CreateService();
            var dto = new PasswordResetConfirmDTO { Token = token, NewPassword = "NewPassword123!", ConfirmPassword = "NewPassword123!" };

            var result = await service.ConfirmPasswordResetAsync(dto);

            Assert.False(result);
            _tokenRepoMock.Verify(r => r.FindByTokenHashAsync(It.IsAny<string>()), Times.Never);
            _userRepoMock.Verify(r => r.UpdateAsync(It.IsAny<User>()), Times.Never);
            _userRepoMock.Verify(r => r.SaveAsync(), Times.Never);
            _tokenRepoMock.Verify(r => r.MarkAsUsedAsync(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task ConfirmPasswordReset_TokenNotFound_ReturnsFalse()
        {
            var token = "no-token";
            var expectedHash = ComputeSha256HexLower(token);
            _tokenRepoMock.Setup(r => r.FindByTokenHashAsync(expectedHash))
                .ReturnsAsync((PasswordResetToken?)null);

            var service = CreateService();
            var dto = new PasswordResetConfirmDTO { Token = token, NewPassword = "A1b2c3d4!", ConfirmPassword = "A1b2c3d4!" };

            var result = await service.ConfirmPasswordResetAsync(dto);
            Assert.False(result);
        }

        [Fact]
        public async Task ConfirmPasswordReset_TokenExpired_ReturnsFalse()
        {
            var token = "expired";
            var expectedHash = ComputeSha256HexLower(token);
            _tokenRepoMock.Setup(r => r.FindByTokenHashAsync(expectedHash))
                .ReturnsAsync(new PasswordResetToken
                {
                    ExpiresAt = DateTime.UtcNow.AddMinutes(-1),
                    UsedAt = null,
                    User = new User { UserId = 1 }
                });

            var service = CreateService();
            var dto = new PasswordResetConfirmDTO { Token = token, NewPassword = "A1b2c3d4!", ConfirmPassword = "A1b2c3d4!" };

            var result = await service.ConfirmPasswordResetAsync(dto);
            Assert.False(result);
        }

        [Fact]
        public async Task ConfirmPasswordReset_TokenAlreadyUsed_ReturnsFalse()
        {
            var token = "used";
            var expectedHash = ComputeSha256HexLower(token);
            _tokenRepoMock.Setup(r => r.FindByTokenHashAsync(expectedHash))
                .ReturnsAsync(new PasswordResetToken
                {
                    ExpiresAt = DateTime.UtcNow.AddMinutes(5),
                    UsedAt = DateTime.UtcNow,
                    User = new User { UserId = 1 }
                });

            var service = CreateService();
            var dto = new PasswordResetConfirmDTO { Token = token, NewPassword = "A1b2c3d4!", ConfirmPassword = "A1b2c3d4!" };

            var result = await service.ConfirmPasswordResetAsync(dto);
            Assert.False(result);
        }

        [Fact]
        public async Task ConfirmPasswordReset_TokenHasNoUser_ReturnsFalse()
        {
            var token = "no-user";
            var expectedHash = ComputeSha256HexLower(token);
            _tokenRepoMock.Setup(r => r.FindByTokenHashAsync(expectedHash))
                .ReturnsAsync(new PasswordResetToken
                {
                    ExpiresAt = DateTime.UtcNow.AddMinutes(5),
                    UsedAt = null,
                    User = null
                });

            var service = CreateService();
            var dto = new PasswordResetConfirmDTO { Token = token, NewPassword = "A1b2c3d4!", ConfirmPassword = "A1b2c3d4!" };

            var result = await service.ConfirmPasswordResetAsync(dto);
            Assert.False(result);
        }

        [Fact]
        public async Task ConfirmPasswordReset_Order_UpdateAndSave_Before_MarkAsUsed()
        {
            var token = "order-ok";
            var expectedHash = ComputeSha256HexLower(token);

            var user = new User { UserId = 501, Email = "u@example.com", PasswordHash = new byte[] { 1 }, PasswordSalt = new byte[] { 2 } };
            var tokenEntity = new PasswordResetToken
            {
                PasswordResetTokenId = 777,
                ExpiresAt = DateTime.UtcNow.AddMinutes(5),
                UsedAt = null,
                User = user
            };

            _tokenRepoMock.Setup(r => r.FindByTokenHashAsync(expectedHash)).ReturnsAsync(tokenEntity);

            var sequence = new MockSequence();
            _userRepoMock.InSequence(sequence).Setup(r => r.UpdateAsync(It.Is<User>(u => u.UserId == user.UserId))).Returns(Task.CompletedTask);
            _userRepoMock.InSequence(sequence).Setup(r => r.SaveAsync()).Returns(Task.CompletedTask);
            _tokenRepoMock.InSequence(sequence).Setup(r => r.MarkAsUsedAsync(777)).ReturnsAsync(true);

            var service = CreateService();
            var dto = new PasswordResetConfirmDTO { Token = token, NewPassword = "NewPassword123!", ConfirmPassword = "NewPassword123!" };

            var result = await service.ConfirmPasswordResetAsync(dto);

            Assert.True(result);
            _userRepoMock.Verify(r => r.UpdateAsync(It.Is<User>(u => u.UserId == user.UserId)), Times.Once);
            _userRepoMock.Verify(r => r.SaveAsync(), Times.Once);
            _tokenRepoMock.Verify(r => r.MarkAsUsedAsync(777), Times.Once);
        }

        [Fact]
        public async Task ConfirmPasswordReset_SaveThrows_ExceptionPropagates_And_NotMarkAsUsed()
        {
            var token = "save-throws";
            var expectedHash = ComputeSha256HexLower(token);

            var user = new User { UserId = 601, Email = "u@example.com", PasswordHash = new byte[] { 1 }, PasswordSalt = new byte[] { 2 } };
            var tokenEntity = new PasswordResetToken
            {
                PasswordResetTokenId = 888,
                ExpiresAt = DateTime.UtcNow.AddMinutes(5),
                UsedAt = null,
                User = user
            };

            _tokenRepoMock.Setup(r => r.FindByTokenHashAsync(expectedHash)).ReturnsAsync(tokenEntity);
            _userRepoMock.Setup(r => r.UpdateAsync(It.IsAny<User>())).Returns(Task.CompletedTask);
            _userRepoMock.Setup(r => r.SaveAsync()).ThrowsAsync(new Exception("db failure"));

            var service = CreateService();
            var dto = new PasswordResetConfirmDTO { Token = token, NewPassword = "NewPassword123!", ConfirmPassword = "NewPassword123!" };

            await Assert.ThrowsAsync<Exception>(async () => await service.ConfirmPasswordResetAsync(dto));

            _tokenRepoMock.Verify(r => r.MarkAsUsedAsync(It.IsAny<int>()), Times.Never);
        }
        public PasswordResetServiceTests()
        {
            _tokenRepoMock = new Mock<IPasswordResetTokenRepository>(MockBehavior.Strict);
            _userRepoMock = new Mock<IUserRepository>(MockBehavior.Strict);
            _emailSenderMock = new Mock<IEmailSender>(MockBehavior.Strict);
            _optionsMock = new Mock<IOptions<PasswordResetOptions>>(MockBehavior.Strict);
        }

        [Fact]
        public async Task RequestPasswordReset_ExistingUser_CreatesToken_And_SendsEmail()
        {
            // Arrange
            var email = "user@example.com";
            var requestIp = "192.168.1.10";
            var userAgent = "Mozilla/5.0";
            var user = new User { UserId = 42, Email = email };

            _userRepoMock.Setup(r => r.GetByEmailAsync(email)).ReturnsAsync(user);

            PasswordResetToken? capturedToken = null;
            _tokenRepoMock
                .Setup(r => r.CreateAsync(It.IsAny<PasswordResetToken>()))
                .Callback<PasswordResetToken>(t => capturedToken = t)
                .ReturnsAsync((PasswordResetToken t) => t);

            _emailSenderMock
                .Setup(s => s.SendAsync(
                    email,
                    It.Is<string>(subj => subj.Contains("Password Reset Request", StringComparison.OrdinalIgnoreCase)),
                    It.IsAny<string>(),
                    false,
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var service = CreateService(ttlMinutes: 30);

            var dto = new PasswordResetRequestDTO { Email = email, RedirectUrl = null };

            // Act
            await service.RequestPasswordResetAsync(dto, requestIp, userAgent);

            
            _userRepoMock.Verify(r => r.GetByEmailAsync(email), Times.Once);
            _tokenRepoMock.Verify(r => r.CreateAsync(It.IsAny<PasswordResetToken>()), Times.Once);
            _emailSenderMock.VerifyAll();

            
            Assert.NotNull(capturedToken);
            Assert.Equal(user.UserId, capturedToken!.UserId);
            Assert.False(string.IsNullOrWhiteSpace(capturedToken.TokenHash));
            Assert.Equal(64, capturedToken.TokenHash.Length);
            Assert.Matches(new Regex("^[0-9a-f]{64}$"), capturedToken.TokenHash);
            Assert.Null(capturedToken.UsedAt);
            Assert.Equal(requestIp, capturedToken.RequestIp);
            Assert.Equal(userAgent, capturedToken.UserAgent);
            Assert.True(capturedToken.CreatedAt <= DateTime.UtcNow.AddSeconds(2));
            Assert.InRange(
                capturedToken.ExpiresAt,
                DateTime.UtcNow.AddMinutes(30).AddSeconds(-5),
                DateTime.UtcNow.AddMinutes(30).AddSeconds(5)
            );

            
            _emailSenderMock.Verify(s => s.SendAsync(
                email,
                It.IsAny<string>(),
                It.Is<string>(body => body.Contains("token=") && body.Contains("30")),
                false,
                It.IsAny<CancellationToken>()), Times.Once);

            
            _tokenRepoMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task RequestPasswordReset_NonExistingUser_NoToken_NoEmail_NoErrors()
        {
            
            var email = "absent@example.com";
            _userRepoMock.Setup(r => r.GetByEmailAsync(email)).ReturnsAsync((User?)null);

            var service = CreateService(ttlMinutes: 30);
            var dto = new PasswordResetRequestDTO { Email = email, RedirectUrl = null };

            
            await service.RequestPasswordResetAsync(dto, requestIp: "127.0.0.1", userAgent: "UA");

            
            _userRepoMock.Verify(r => r.GetByEmailAsync(email), Times.Once);
            _tokenRepoMock.Verify(r => r.CreateAsync(It.IsAny<PasswordResetToken>()), Times.Never);
            _emailSenderMock.Verify(s => s.SendAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()), Times.Never);

            
            _tokenRepoMock.VerifyNoOtherCalls();
            _emailSenderMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task RequestPasswordReset_EmailSenderThrows_DoesNotFail_TokenCreated_And_SendCalledOnce_AfterCreate()
        {
            // Arrange
            var email = "user@example.com";
            var user = new User { UserId = 7, Email = email };
            _userRepoMock.Setup(r => r.GetByEmailAsync(email)).ReturnsAsync(user);

            PasswordResetToken? capturedToken = null;
            string? capturedBody = null;

            var sequence = new MockSequence();
            _tokenRepoMock.InSequence(sequence)
                .Setup(r => r.CreateAsync(It.IsAny<PasswordResetToken>()))
                .Callback<PasswordResetToken>(t => capturedToken = t)
                .ReturnsAsync((PasswordResetToken t) => t);

            _emailSenderMock.InSequence(sequence)
                .Setup(s => s.SendAsync(
                    email,
                    It.IsAny<string>(),
                    It.Is<string>(b => true),
                    false,
                    It.IsAny<CancellationToken>()))
                .Callback<string, string, string, bool, CancellationToken>((_, __, body, ___, ____) => capturedBody = body)
                .ThrowsAsync(new Exception("SMTP failure"));

            var service = CreateService(ttlMinutes: 30);

            
            await service.RequestPasswordResetAsync(
                new PasswordResetRequestDTO { Email = email },
                requestIp: null,
                userAgent: null);

            
            Assert.NotNull(capturedToken);
            Assert.NotNull(capturedBody);
            _userRepoMock.Verify(r => r.GetByEmailAsync(email), Times.Once);
            _tokenRepoMock.Verify(r => r.CreateAsync(It.IsAny<PasswordResetToken>()), Times.Exactly(1)); // one in normal setup + one in sequence
            _emailSenderMock.Verify(s => s.SendAsync(
                email,
                It.IsAny<string>(),
                It.IsAny<string>(),
                false,
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task RequestPasswordReset_UsesProvidedRedirectUrl_WhenWhitelisted()
        {
            
            var email = "user@example.com";
            var user = new User { UserId = 11, Email = email };
            _userRepoMock.Setup(r => r.GetByEmailAsync(email)).ReturnsAsync(user);

            PasswordResetToken? capturedToken = null;
            _tokenRepoMock
                .Setup(r => r.CreateAsync(It.IsAny<PasswordResetToken>()))
                .Callback<PasswordResetToken>(t => capturedToken = t)
                .ReturnsAsync((PasswordResetToken t) => t);

            string? capturedBody = null;
            _emailSenderMock
                .Setup(s => s.SendAsync(
                    email,
                    It.IsAny<string>(),
                    It.Is<string>(b => true),
                    false,
                    It.IsAny<CancellationToken>()))
                .Callback<string, string, string, bool, CancellationToken>((_, __, body, ___, ____) => capturedBody = body)
                .Returns(Task.CompletedTask);

            var service = CreateService(ttlMinutes: 30, defaultRedirectUrl: "https://default.example/reset");
            var redirectUrl = "https://example.com/app/reset";

            
            await service.RequestPasswordResetAsync(
                new PasswordResetRequestDTO { Email = email, RedirectUrl = redirectUrl },
                requestIp: "1.2.3.4",
                userAgent: "UA");

            
            Assert.NotNull(capturedBody);
            Assert.Contains("https://example.com/app/reset", capturedBody);
            Assert.Contains("token=", capturedBody);
            Assert.DoesNotContain("https://default.example/reset", capturedBody);
        }

        [Fact]
        public async Task RequestPasswordReset_UsesDefaultRedirectUrl_WhenProvidedRedirectIsNotWhitelisted()
        {
            
            var email = "user@example.com";
            var user = new User { UserId = 21, Email = email };
            _userRepoMock.Setup(r => r.GetByEmailAsync(email)).ReturnsAsync(user);

            PasswordResetToken? capturedToken = null;
            _tokenRepoMock
                .Setup(r => r.CreateAsync(It.IsAny<PasswordResetToken>()))
                .Callback<PasswordResetToken>(t => capturedToken = t)
                .ReturnsAsync((PasswordResetToken t) => t);

            string? capturedBody = null;
            _emailSenderMock
                .Setup(s => s.SendAsync(
                    email,
                    It.IsAny<string>(),
                    It.Is<string>(b => true),
                    false,
                    It.IsAny<CancellationToken>()))
                .Callback<string, string, string, bool, CancellationToken>((_, __, body, ___, ____) => capturedBody = body)
                .Returns(Task.CompletedTask);

            var service = CreateService(ttlMinutes: 30, defaultRedirectUrl: "https://default.example/reset");
            var redirectUrl = "https://not-allowed.com/app/reset";

            
            await service.RequestPasswordResetAsync(
                new PasswordResetRequestDTO { Email = email, RedirectUrl = redirectUrl },
                requestIp: "1.2.3.4",
                userAgent: "UA");

            
            Assert.NotNull(capturedBody);
            Assert.Contains("https://default.example/reset", capturedBody);
            Assert.Contains("token=", capturedBody);
            Assert.DoesNotContain("https://not-allowed.com/app/reset", capturedBody);
        }

        [Fact]
        public async Task RequestPasswordReset_TokenInUrl_IsUriSafeEncoded()
        {
            
            var email = "user@example.com";
            var user = new User { UserId = 31, Email = email };
            _userRepoMock.Setup(r => r.GetByEmailAsync(email)).ReturnsAsync(user);

            PasswordResetToken? capturedToken = null;
            _tokenRepoMock
                .Setup(r => r.CreateAsync(It.IsAny<PasswordResetToken>()))
                .Callback<PasswordResetToken>(t => capturedToken = t)
                .ReturnsAsync((PasswordResetToken t) => t);

            string? capturedBody = null;
            _emailSenderMock
                .Setup(s => s.SendAsync(
                    email,
                    It.IsAny<string>(),
                    It.Is<string>(b => true),
                    false,
                    It.IsAny<CancellationToken>()))
                .Callback<string, string, string, bool, CancellationToken>((_, __, body, ___, ____) => capturedBody = body)
                .Returns(Task.CompletedTask);

            var service = CreateService(ttlMinutes: 30, defaultRedirectUrl: "https://default.example/reset");

            
            await service.RequestPasswordResetAsync(
                new PasswordResetRequestDTO { Email = email },
                requestIp: "10.0.0.1",
                userAgent: "UA");

            
            Assert.NotNull(capturedBody);
            var lines = capturedBody!.Split('\n');
            var urlLine = lines.FirstOrDefault(l => l.StartsWith("http", StringComparison.OrdinalIgnoreCase)) ?? capturedBody!;
            var uri = new Uri(urlLine.Trim());
            var query = System.Web.HttpUtility.ParseQueryString(uri.Query);
            var token = query.Get("token");
            Assert.False(string.IsNullOrWhiteSpace(token));
            
            var unescaped = Uri.UnescapeDataString(token!);
            var reEscaped = Uri.EscapeDataString(unescaped);
            Assert.Equal(token, reEscaped);
            
            Assert.DoesNotContain('+', token);
            Assert.DoesNotContain('/', token);
            Assert.DoesNotContain('=', token);
        }

        [Fact]
        public async Task RequestPasswordReset_RedirectUrlWithQuery_UsesAmpersandForToken()
        {
            
            var email = "user@example.com";
            var user = new User { UserId = 41, Email = email };
            _userRepoMock.Setup(r => r.GetByEmailAsync(email)).ReturnsAsync(user);

            _tokenRepoMock
                .Setup(r => r.CreateAsync(It.IsAny<PasswordResetToken>()))
                .ReturnsAsync((PasswordResetToken t) => t);

            string? capturedBody = null;
            _emailSenderMock
                .Setup(s => s.SendAsync(
                    email,
                    It.IsAny<string>(),
                    It.Is<string>(b => true),
                    false,
                    It.IsAny<CancellationToken>()))
                .Callback<string, string, string, bool, CancellationToken>((_, __, body, ___, ____) => capturedBody = body)
                .Returns(Task.CompletedTask);

            var service = CreateService(defaultRedirectUrl: "https://default.example/reset");
            var redirectWithQuery = "https://example.com/app/reset?ref=mail";

            
            await service.RequestPasswordResetAsync(
                new PasswordResetRequestDTO { Email = email, RedirectUrl = redirectWithQuery },
                requestIp: null,
                userAgent: null);

            
            Assert.NotNull(capturedBody);
            Assert.Contains("https://example.com/app/reset?ref=mail&token=", capturedBody);
        }

        [Fact]
        public async Task RequestPasswordReset_RedirectUrlWithoutQuery_UsesQuestionMarkForToken()
        {
            
            var email = "user@example.com";
            var user = new User { UserId = 51, Email = email };
            _userRepoMock.Setup(r => r.GetByEmailAsync(email)).ReturnsAsync(user);

            _tokenRepoMock
                .Setup(r => r.CreateAsync(It.IsAny<PasswordResetToken>()))
                .ReturnsAsync((PasswordResetToken t) => t);

            string? capturedBody = null;
            _emailSenderMock
                .Setup(s => s.SendAsync(
                    email,
                    It.IsAny<string>(),
                    It.Is<string>(b => true),
                    false,
                    It.IsAny<CancellationToken>()))
                .Callback<string, string, string, bool, CancellationToken>((_, __, body, ___, ____) => capturedBody = body)
                .Returns(Task.CompletedTask);

            var service = CreateService(defaultRedirectUrl: "https://default.example/reset");
            var redirectNoQuery = "https://example.com/app/reset";

            
            await service.RequestPasswordResetAsync(
                new PasswordResetRequestDTO { Email = email, RedirectUrl = redirectNoQuery },
                requestIp: null,
                userAgent: null);

            
            Assert.NotNull(capturedBody);
            Assert.Contains("https://example.com/app/reset?token=", capturedBody);
        }

        [Fact]
        public async Task RequestPasswordReset_EmptyDefaultRedirect_ThrowsInvalidOperationException()
        {   
            var email = "user@example.com";
            var user = new User { UserId = 61, Email = email };
            _userRepoMock.Setup(r => r.GetByEmailAsync(email)).ReturnsAsync(user);

            _optionsMock.Setup(o => o.Value).Returns(new PasswordResetOptions
            {
                TokenTTLMinutes = 30,
                DefaultRedirectUrl = string.Empty,
                AllowedRedirectHosts = new[] { "example.com" }
            });

            _tokenRepoMock
                .Setup(r => r.CreateAsync(It.IsAny<PasswordResetToken>()))
                .ReturnsAsync((PasswordResetToken t) => t);

            var service = new PasswordResetService(
                _tokenRepoMock.Object,
                _userRepoMock.Object,
                _emailSenderMock.Object,
                _optionsMock.Object);

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await service.RequestPasswordResetAsync(
                    new PasswordResetRequestDTO { Email = email, RedirectUrl = null },
                    requestIp: null,
                    userAgent: null));

            // Assert interactions: token was created, email was not sent
            _tokenRepoMock.Verify(r => r.CreateAsync(It.IsAny<PasswordResetToken>()), Times.Once);
            _emailSenderMock.Verify(s => s.SendAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task RequestPasswordReset_NullRequestIp_PersistsNull()
        {
            var email = "user@example.com";
            var user = new User { UserId = 71, Email = email };
            _userRepoMock.Setup(r => r.GetByEmailAsync(email)).ReturnsAsync(user);

            PasswordResetToken? capturedToken = null;
            _tokenRepoMock
                .Setup(r => r.CreateAsync(It.IsAny<PasswordResetToken>()))
                .Callback<PasswordResetToken>(t => capturedToken = t)
                .ReturnsAsync((PasswordResetToken t) => t);

            _emailSenderMock
                .Setup(s => s.SendAsync(
                    email,
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    false,
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var service = CreateService();

            await service.RequestPasswordResetAsync(
                new PasswordResetRequestDTO { Email = email },
                requestIp: null,
                userAgent: "UA");

            Assert.NotNull(capturedToken);
            Assert.Null(capturedToken!.RequestIp);
        }

        [Fact]
        public async Task RequestPasswordReset_EmptyUserAgent_PersistsAsIs()
        {
            var email = "user@example.com";
            var user = new User { UserId = 81, Email = email };
            _userRepoMock.Setup(r => r.GetByEmailAsync(email)).ReturnsAsync(user);

            PasswordResetToken? capturedToken = null;
            _tokenRepoMock
                .Setup(r => r.CreateAsync(It.IsAny<PasswordResetToken>()))
                .Callback<PasswordResetToken>(t => capturedToken = t)
                .ReturnsAsync((PasswordResetToken t) => t);

            _emailSenderMock
                .Setup(s => s.SendAsync(
                    email,
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    false,
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var service = CreateService();

            await service.RequestPasswordResetAsync(
                new PasswordResetRequestDTO { Email = email },
                requestIp: "127.0.0.1",
                userAgent: string.Empty);

            Assert.NotNull(capturedToken);
            Assert.Equal(string.Empty, capturedToken!.UserAgent);
        }
    }
}


