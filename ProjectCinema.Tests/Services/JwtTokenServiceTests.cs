using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ProjectCinema.BLL.Services;
using ProjectCinema.Enums;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using Xunit;

namespace ProjectCinema.Tests.Services
{
    public class JwtTokenServiceTests
    {
        private readonly IConfiguration _configuration;
        private readonly JwtTokenService _jwtTokenService;

        public JwtTokenServiceTests()
        {
            var configurationBuilder = new ConfigurationBuilder();
            var configurationData = new Dictionary<string, string>
            {
                { "JwtSettings:SecretKey", "ThisIsAVeryLongSecretKeyForJWTTokenGenerationItShouldBeAtLeast32CharactersLong12345" },
                { "JwtSettings:Issuer", "ProjectCinema" },
                { "JwtSettings:Audience", "ProjectCinemaUsers" },
                { "JwtSettings:ExpirationMinutes", "60" }
            };

            configurationBuilder.AddInMemoryCollection(configurationData);
            _configuration = configurationBuilder.Build();

            _jwtTokenService = new JwtTokenService(_configuration);
        }

        [Fact]
        public void GenerateAccessToken_ShouldCreateValidJwtToken()
        {
            var userId = 1;
            var email = "test@example.com";
            var username = "testuser";
            var role = UserRole.User;

            var token = _jwtTokenService.GenerateAccessToken(userId, email, username, role);

            Assert.NotNull(token);
            Assert.NotEmpty(token);
            
            var parts = token.Split('.');
            Assert.Equal(3, parts.Length); 

            var isValid = _jwtTokenService.ValidateToken(token);
            Assert.True(isValid);
        }

        [Fact]
        public void GenerateAccessToken_ShouldContainCorrectClaims()
        {
            var userId = 5;
            var email = "john@example.com";
            var username = "john_doe";
            var role = UserRole.Admin;

            var token = _jwtTokenService.GenerateAccessToken(userId, email, username, role);

            var extractedUserId = _jwtTokenService.GetUserIdFromToken(token);
            var extractedEmail = _jwtTokenService.GetEmailFromToken(token);
            var extractedRole = _jwtTokenService.GetUserRoleFromToken(token);

            Assert.NotNull(extractedUserId);
            Assert.Equal(userId, extractedUserId.Value);
            Assert.Equal(email, extractedEmail);
            Assert.NotNull(extractedRole);
            Assert.Equal(role, extractedRole.Value);
        }

        [Fact]
        public void GenerateAccessToken_ShouldContainUsername()
        {
            var userId = 3;
            var email = "test@example.com";
            var username = "test_username";
            var role = UserRole.User;

            var token = _jwtTokenService.GenerateAccessToken(userId, email, username, role);

            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);
            var usernameClaim = jwtToken.Claims.FirstOrDefault(x => x.Type == "username");

            Assert.NotNull(usernameClaim);
            Assert.Equal(username, usernameClaim.Value);
        }

        [Fact]
        public void GenerateAccessToken_ShouldHaveCorrectExpirationTime()
        {
            var userId = 1;
            var email = "test@example.com";
            var username = "testuser";
            var role = UserRole.User;

            var token = _jwtTokenService.GenerateAccessToken(userId, email, username, role);

            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);

            var expectedExpiration = DateTime.UtcNow.AddMinutes(60);
            var actualExpiration = jwtToken.ValidTo;
            
            var timeDifference = Math.Abs((expectedExpiration - actualExpiration).TotalMinutes);
            Assert.True(timeDifference < 1, $"Expiration time should be approximately 60 minutes from now, but difference is {timeDifference} minutes");
            
            Assert.True(jwtToken.ValidTo > DateTime.UtcNow);
        }

        [Fact]
        public void GenerateAccessToken_ShouldContainCorrectIssuer()
        {
            var userId = 1;
            var email = "test@example.com";
            var username = "testuser";
            var role = UserRole.User;

            var token = _jwtTokenService.GenerateAccessToken(userId, email, username, role);

            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);

            Assert.Equal("ProjectCinema", jwtToken.Issuer);
        }

        [Fact]
        public void GenerateAccessToken_ShouldContainCorrectAudience()
        {
            var userId = 1;
            var email = "test@example.com";
            var username = "testuser";
            var role = UserRole.User;

            var token = _jwtTokenService.GenerateAccessToken(userId, email, username, role);

            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);

            Assert.Contains("ProjectCinemaUsers", jwtToken.Audiences);
        }

        [Fact]
        public void GenerateAccessToken_ShouldGenerateDifferentTokensForDifferentUsers()
        {
            var userId1 = 1;
            var userId2 = 2;

            var token1 = _jwtTokenService.GenerateAccessToken(userId1, "user1@test.com", "user1", UserRole.User);
            var token2 = _jwtTokenService.GenerateAccessToken(userId2, "user2@test.com", "user2", UserRole.User);

            Assert.NotEqual(token1, token2);
        }

        [Fact]
        public void GenerateRefreshToken_ShouldGenerateNonEmptyString()
        {
            var token = _jwtTokenService.GenerateRefreshToken();

            Assert.NotNull(token);
            Assert.NotEmpty(token);
        }

        [Fact]
        public void GenerateRefreshToken_ShouldGenerateDifferentTokensOnEachCall()
        {
            var token1 = _jwtTokenService.GenerateRefreshToken();
            var token2 = _jwtTokenService.GenerateRefreshToken();
            var token3 = _jwtTokenService.GenerateRefreshToken();

            Assert.NotEqual(token1, token2);
            Assert.NotEqual(token2, token3);
            Assert.NotEqual(token1, token3);
        }

        [Fact]
        public void GenerateRefreshToken_ShouldBeInBase64Format()
        {
            var token = _jwtTokenService.GenerateRefreshToken();

            byte[] decodedBytes;
            try
            {
                decodedBytes = Convert.FromBase64String(token);
            }
            catch (FormatException)
            {
                Assert.True(false, "Token is not in valid Base64 format");
                return;
            }

            Assert.NotNull(decodedBytes);
        }

        [Fact]
        public void GenerateRefreshToken_ShouldBe32BytesLong()
        {
            var token = _jwtTokenService.GenerateRefreshToken();

            var decodedBytes = Convert.FromBase64String(token);

            Assert.Equal(32, decodedBytes.Length);
        }

        [Fact]
        public void ValidateToken_ShouldReturnTrueForValidToken()
        {
            var token = _jwtTokenService.GenerateAccessToken(1, "test@example.com", "testuser", UserRole.User);

            var isValid = _jwtTokenService.ValidateToken(token);

            Assert.True(isValid);
        }

        [Fact]
        public void ValidateToken_ShouldReturnFalseForExpiredToken()
        {
            var shortExpConfig = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "JwtSettings:SecretKey", "ThisIsAVeryLongSecretKeyForJWTTokenGenerationItShouldBeAtLeast32CharactersLong12345" },
                    { "JwtSettings:Issuer", "ProjectCinema" },
                    { "JwtSettings:Audience", "ProjectCinemaUsers" },
                    { "JwtSettings:ExpirationMinutes", "1" }
                })
                .Build();

            var shortExpService = new JwtTokenService(shortExpConfig);
            

            var token = shortExpService.GenerateAccessToken(1, "test@example.com", "testuser", UserRole.User);
            Assert.True(_jwtTokenService.ValidateToken(token));
            
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("ThisIsAVeryLongSecretKeyForJWTTokenGenerationItShouldBeAtLeast32CharactersLong12345");
            var claims = new ClaimsIdentity(new[]
            {
                new Claim("userId", "1"),
                new Claim("email", "test@example.com"),
                new Claim("username", "testuser"),
                new Claim("role", "User")
            });
            
            var expiredTokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claims,
                Expires = DateTime.UtcNow.AddMinutes(-5),
                NotBefore = DateTime.UtcNow.AddMinutes(-10),
                Issuer = "ProjectCinema",
                Audience = "ProjectCinemaUsers",
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            
            var expiredToken = tokenHandler.CreateToken(expiredTokenDescriptor);
            var expiredTokenString = tokenHandler.WriteToken(expiredToken);

            var isValid = _jwtTokenService.ValidateToken(expiredTokenString);

            Assert.False(isValid);
        }

        [Fact]
        public void ValidateToken_ShouldReturnFalseForTokenWithWrongSignature()
        {
            var wrongKeyConfig = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "JwtSettings:SecretKey", "DifferentSecretKeyThatWillMakeTheSignatureInvalid123456789" },
                    { "JwtSettings:Issuer", "ProjectCinema" },
                    { "JwtSettings:Audience", "ProjectCinemaUsers" },
                    { "JwtSettings:ExpirationMinutes", "60" }
                })
                .Build();

            var wrongKeyService = new JwtTokenService(wrongKeyConfig);
            var tokenWithWrongSignature = wrongKeyService.GenerateAccessToken(1, "test@example.com", "testuser", UserRole.User);

            var isValid = _jwtTokenService.ValidateToken(tokenWithWrongSignature);

            Assert.False(isValid);
        }

        [Fact]
        public void ValidateToken_ShouldReturnFalseForTokenWithWrongIssuer()
        {
            var wrongIssuerConfig = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "JwtSettings:SecretKey", "ThisIsAVeryLongSecretKeyForJWTTokenGenerationItShouldBeAtLeast32CharactersLong12345" },
                    { "JwtSettings:Issuer", "WrongIssuer" },
                    { "JwtSettings:Audience", "ProjectCinemaUsers" },
                    { "JwtSettings:ExpirationMinutes", "60" }
                })
                .Build();

            var wrongIssuerService = new JwtTokenService(wrongIssuerConfig);
            var tokenWithWrongIssuer = wrongIssuerService.GenerateAccessToken(1, "test@example.com", "testuser", UserRole.User);

            var isValid = _jwtTokenService.ValidateToken(tokenWithWrongIssuer);

            Assert.False(isValid);
        }

        [Fact]
        public void ValidateToken_ShouldReturnFalseForTokenWithWrongAudience()
        {
            var wrongAudienceConfig = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "JwtSettings:SecretKey", "ThisIsAVeryLongSecretKeyForJWTTokenGenerationItShouldBeAtLeast32CharactersLong12345" },
                    { "JwtSettings:Issuer", "ProjectCinema" },
                    { "JwtSettings:Audience", "WrongAudience" },
                    { "JwtSettings:ExpirationMinutes", "60" }
                })
                .Build();

            var wrongAudienceService = new JwtTokenService(wrongAudienceConfig);
            var tokenWithWrongAudience = wrongAudienceService.GenerateAccessToken(1, "test@example.com", "testuser", UserRole.User);

            var isValid = _jwtTokenService.ValidateToken(tokenWithWrongAudience);

            Assert.False(isValid);
        }

        [Fact]
        public void ValidateToken_ShouldReturnFalseForCorruptedToken()
        {
            var corruptedToken = "ThisIsNotAValidJWTToken.AtAll.!!!";

            var isValid = _jwtTokenService.ValidateToken(corruptedToken);

            Assert.False(isValid);
        }

        [Fact]
        public void ValidateToken_ShouldReturnFalseForEmptyString()
        {
            var isValid = _jwtTokenService.ValidateToken("");

            Assert.False(isValid);
        }

        [Fact]
        public void ValidateToken_ShouldReturnFalseForNull()
        {
            var isValid = _jwtTokenService.ValidateToken(null!);

            Assert.False(isValid);
        }

        [Fact]
        public void GetUserIdFromToken_ShouldExtractUserIdFromValidToken()
        {
            var expectedUserId = 42;
            var token = _jwtTokenService.GenerateAccessToken(expectedUserId, "test@example.com", "testuser", UserRole.User);

            var extractedUserId = _jwtTokenService.GetUserIdFromToken(token);

            Assert.NotNull(extractedUserId);
            Assert.Equal(expectedUserId, extractedUserId.Value);
        }

        [Fact]
        public void GetUserIdFromToken_ShouldReturnNullForInvalidToken()
        {
            var invalidToken = "invalid.token.format";

            var extractedUserId = _jwtTokenService.GetUserIdFromToken(invalidToken);

            Assert.Null(extractedUserId);
        }

        [Fact]
        public void GetUserIdFromToken_ShouldReturnNullForExpiredToken()
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("ThisIsAVeryLongSecretKeyForJWTTokenGenerationItShouldBeAtLeast32CharactersLong12345");
            var claims = new ClaimsIdentity(new[]
            {
                new Claim("userId", "100")
            });

            var expiredTokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claims,
                Expires = DateTime.UtcNow.AddMinutes(-5),
                NotBefore = DateTime.UtcNow.AddMinutes(-10),
                Issuer = "ProjectCinema",
                Audience = "ProjectCinemaUsers",
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var expiredToken = tokenHandler.CreateToken(expiredTokenDescriptor);
            var expiredTokenString = tokenHandler.WriteToken(expiredToken);


            var extractedUserId = _jwtTokenService.GetUserIdFromToken(expiredTokenString);

            Assert.Null(extractedUserId);
        }

        [Fact]
        public void GetUserIdFromToken_ShouldReturnNullForCorruptedToken()
        {
            var corruptedToken = "ThisIsNotAValidJWTToken.AtAll.!!!";

            var extractedUserId = _jwtTokenService.GetUserIdFromToken(corruptedToken);

            Assert.Null(extractedUserId);
        }

        [Fact]
        public void GetUserIdFromToken_ShouldReturnNullForTokenWithoutUserId()
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("ThisIsAVeryLongSecretKeyForJWTTokenGenerationItShouldBeAtLeast32CharactersLong12345");
            var claims = new ClaimsIdentity(new[]
            {
                new Claim("email", "test@example.com"),
                new Claim("role", "User")
            });

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claims,
                Expires = DateTime.UtcNow.AddMinutes(60),
                Issuer = "ProjectCinema",
                Audience = "ProjectCinemaUsers",
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            var extractedUserId = _jwtTokenService.GetUserIdFromToken(tokenString);

            Assert.Null(extractedUserId);
        }

        [Fact]
        public void GetUserRoleFromToken_ShouldExtractRoleFromValidToken()
        {
            var expectedRole = UserRole.Admin;
            var token = _jwtTokenService.GenerateAccessToken(1, "admin@example.com", "adminuser", expectedRole);

            var extractedRole = _jwtTokenService.GetUserRoleFromToken(token);

            Assert.NotNull(extractedRole);
            Assert.Equal(expectedRole, extractedRole.Value);
        }

        [Fact]
        public void GetUserRoleFromToken_ShouldReturnCorrectRole()
        {
            var userRole = UserRole.User;
            var adminRole = UserRole.Admin;

            var userToken = _jwtTokenService.GenerateAccessToken(1, "user@example.com", "user1", userRole);
            var adminToken = _jwtTokenService.GenerateAccessToken(2, "admin@example.com", "admin1", adminRole);

            var extractedUserRole = _jwtTokenService.GetUserRoleFromToken(userToken);
            var extractedAdminRole = _jwtTokenService.GetUserRoleFromToken(adminToken);

            Assert.NotNull(extractedUserRole);
            Assert.Equal(userRole, extractedUserRole.Value);
            
            Assert.NotNull(extractedAdminRole);
            Assert.Equal(adminRole, extractedAdminRole.Value);
        }

        [Fact]
        public void GetUserRoleFromToken_ShouldReturnNullForInvalidToken()
        {
            var invalidToken = "invalid.token.format";

            var extractedRole = _jwtTokenService.GetUserRoleFromToken(invalidToken);

            Assert.Null(extractedRole);
        }

        [Fact]
        public void GetUserRoleFromToken_ShouldReturnNullForExpiredToken()
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("ThisIsAVeryLongSecretKeyForJWTTokenGenerationItShouldBeAtLeast32CharactersLong12345");
            var claims = new ClaimsIdentity(new[]
            {
                new Claim("userId", "1"),
                new Claim("email", "test@example.com"),
                new Claim("role", "Admin")
            });

            var expiredTokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claims,
                Expires = DateTime.UtcNow.AddMinutes(-5),
                NotBefore = DateTime.UtcNow.AddMinutes(-10),
                Issuer = "ProjectCinema",
                Audience = "ProjectCinemaUsers",
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var expiredToken = tokenHandler.CreateToken(expiredTokenDescriptor);
            var expiredTokenString = tokenHandler.WriteToken(expiredToken);

            var extractedRole = _jwtTokenService.GetUserRoleFromToken(expiredTokenString);

            Assert.Null(extractedRole);
        }

        [Fact]
        public void GetUserRoleFromToken_ShouldReturnNullForTokenWithoutRole()
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("ThisIsAVeryLongSecretKeyForJWTTokenGenerationItShouldBeAtLeast32CharactersLong12345");
            var claims = new ClaimsIdentity(new[]
            {
                new Claim("userId", "1"),
                new Claim("email", "test@example.com")
            });

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claims,
                Expires = DateTime.UtcNow.AddMinutes(60),
                Issuer = "ProjectCinema",
                Audience = "ProjectCinemaUsers",
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            var extractedRole = _jwtTokenService.GetUserRoleFromToken(tokenString);

            Assert.Null(extractedRole);
        }

        [Fact]
        public void GetEmailFromToken_ShouldExtractEmailFromValidToken()
        {
            var expectedEmail = "john@example.com";
            var token = _jwtTokenService.GenerateAccessToken(1, expectedEmail, "john_doe", UserRole.User);

            var extractedEmail = _jwtTokenService.GetEmailFromToken(token);

            Assert.NotNull(extractedEmail);
            Assert.Equal(expectedEmail, extractedEmail);
        }

        [Fact]
        public void GetEmailFromToken_ShouldReturnNullForInvalidToken()
        {
            var invalidToken = "invalid.token.format";

            var extractedEmail = _jwtTokenService.GetEmailFromToken(invalidToken);

            Assert.Null(extractedEmail);
        }

        [Fact]
        public void GetEmailFromToken_ShouldReturnNullForExpiredToken()
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("ThisIsAVeryLongSecretKeyForJWTTokenGenerationItShouldBeAtLeast32CharactersLong12345");
            var claims = new ClaimsIdentity(new[]
            {
                new Claim("userId", "1"),
                new Claim("email", "expired@example.com"),
                new Claim("role", "User")
            });

            var expiredTokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claims,
                Expires = DateTime.UtcNow.AddMinutes(-5),
                NotBefore = DateTime.UtcNow.AddMinutes(-10),
                Issuer = "ProjectCinema",
                Audience = "ProjectCinemaUsers",
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var expiredToken = tokenHandler.CreateToken(expiredTokenDescriptor);
            var expiredTokenString = tokenHandler.WriteToken(expiredToken);

            var extractedEmail = _jwtTokenService.GetEmailFromToken(expiredTokenString);

            Assert.Null(extractedEmail);
        }

        [Fact]
        public void GetEmailFromToken_ShouldReturnNullForTokenWithoutEmail()
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("ThisIsAVeryLongSecretKeyForJWTTokenGenerationItShouldBeAtLeast32CharactersLong12345");
            var claims = new ClaimsIdentity(new[]
            {
                new Claim("userId", "1"),
                new Claim("role", "User")
            });

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claims,
                Expires = DateTime.UtcNow.AddMinutes(60),
                Issuer = "ProjectCinema",
                Audience = "ProjectCinemaUsers",
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            var extractedEmail = _jwtTokenService.GetEmailFromToken(tokenString);

            Assert.Null(extractedEmail);
        }

        [Fact]
        public void GetEmailFromToken_ShouldReturnNullForCorruptedToken()
        {
            var corruptedToken = "ThisIsNotAValidJWTToken.AtAll.!!!";

            var extractedEmail = _jwtTokenService.GetEmailFromToken(corruptedToken);

            Assert.Null(extractedEmail);
        }

        [Fact]
        public void IsTokenExpired_ShouldReturnFalseForValidToken()
        {
            var token = _jwtTokenService.GenerateAccessToken(1, "test@example.com", "testuser", UserRole.User);

            var isExpired = _jwtTokenService.IsTokenExpired(token);

            Assert.False(isExpired);
        }

        [Fact]
        public void IsTokenExpired_ShouldReturnTrueForExpiredToken()
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("ThisIsAVeryLongSecretKeyForJWTTokenGenerationItShouldBeAtLeast32CharactersLong12345");
            var claims = new ClaimsIdentity(new[]
            {
                new Claim("userId", "1"),
                new Claim("email", "test@example.com"),
                new Claim("role", "User")
            });

            var expiredTokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claims,
                Expires = DateTime.UtcNow.AddMinutes(-5),
                NotBefore = DateTime.UtcNow.AddMinutes(-10),
                Issuer = "ProjectCinema",
                Audience = "ProjectCinemaUsers",
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var expiredToken = tokenHandler.CreateToken(expiredTokenDescriptor);
            var expiredTokenString = tokenHandler.WriteToken(expiredToken);

            var isExpired = _jwtTokenService.IsTokenExpired(expiredTokenString);

            Assert.True(isExpired);
        }

        [Fact]
        public void IsTokenExpired_ShouldReturnTrueForInvalidTokenFormat()
        {
            var invalidToken = "invalid.token.format";

            var isExpired = _jwtTokenService.IsTokenExpired(invalidToken);

            Assert.True(isExpired);
        }

        [Fact]
        public void IsTokenExpired_ShouldReturnTrueForCorruptedToken()
        {
            var corruptedToken = "ThisIsNotAValidJWTToken.AtAll.!!!";

            var isExpired = _jwtTokenService.IsTokenExpired(corruptedToken);

            Assert.True(isExpired);
        }

        [Fact]
        public void IsTokenExpired_ShouldReturnTrueForEmptyString()
        {
            var emptyToken = string.Empty;

            var isExpired = _jwtTokenService.IsTokenExpired(emptyToken);

            Assert.True(isExpired);
        }

        [Fact]
        public void IsTokenExpired_ShouldReturnTrueForNullToken()
        {
            string? nullToken = null;

            var isExpired = _jwtTokenService.IsTokenExpired(nullToken!);

            Assert.True(isExpired);
        }

        [Fact]
        public void IsTokenExpired_ShouldReturnFalseForTokenExpiringInFuture()
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("ThisIsAVeryLongSecretKeyForJWTTokenGenerationItShouldBeAtLeast32CharactersLong12345");
            var claims = new ClaimsIdentity(new[]
            {
                new Claim("userId", "1"),
                new Claim("email", "test@example.com"),
                new Claim("role", "User")
            });

            var futureTokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claims,
                Expires = DateTime.UtcNow.AddHours(1),
                Issuer = "ProjectCinema",
                Audience = "ProjectCinemaUsers",
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var futureToken = tokenHandler.CreateToken(futureTokenDescriptor);
            var futureTokenString = tokenHandler.WriteToken(futureToken);

            var isExpired = _jwtTokenService.IsTokenExpired(futureTokenString);

            Assert.False(isExpired);
        }

        [Fact]
        public void IsTokenExpired_ShouldReturnTrueForTokenExpiringJustNow()
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("ThisIsAVeryLongSecretKeyForJWTTokenGenerationItShouldBeAtLeast32CharactersLong12345");
            var claims = new ClaimsIdentity(new[]
            {
                new Claim("userId", "1"),
                new Claim("email", "test@example.com"),
                new Claim("role", "User")
            });

            var justExpiredTokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claims,
                Expires = DateTime.UtcNow.AddSeconds(-1),
                NotBefore = DateTime.UtcNow.AddMinutes(-5),
                Issuer = "ProjectCinema",
                Audience = "ProjectCinemaUsers",
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var justExpiredToken = tokenHandler.CreateToken(justExpiredTokenDescriptor);
            var justExpiredTokenString = tokenHandler.WriteToken(justExpiredToken);

            var isExpired = _jwtTokenService.IsTokenExpired(justExpiredTokenString);

            Assert.True(isExpired);
        }

        [Fact]
        public void IsTokenExpired_ShouldReturnTrueForTokenWithWrongStructure()
        {
            var malformedToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.InvalidPayload.Signature";

            var isExpired = _jwtTokenService.IsTokenExpired(malformedToken);

            Assert.True(isExpired);
        }
    }
}

