using BibliotecaRHC.API.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Moq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace BibliotecaRHC.Tests
{
    public class TokenServiceTests
    {
        [Fact]
        public void GenerateAccessToken_ValidClaims_ReturnsValidJwtSecurityToken()
        {
            // Arrange
            var claims = new List<Claim>
            {
                new (ClaimTypes.NameIdentifier, "123"),
                new (ClaimTypes.Name, "testuser")
            };

            var jwtSettings = new Dictionary<string, string?>
            {
                {"JWT:SecretKey", "ThisIsASecretKeyForJwtToken12345!"},
                {"JWT:TokenValidityInMinutes", "60"},
                {"JWT:ValidIssuer", "MyAppIssuer"},
                {"JWT:ValidAudience", "MyAppAudience"}
            };

            IConfiguration config = new ConfigurationBuilder()
                .AddInMemoryCollection(jwtSettings)
                .Build();

            // Act
            var tokenService = new TokenService();
            var token = tokenService.GenerateAccessToken(claims, config);

            // Assert
            Assert.NotNull(token);
            Assert.IsType<JwtSecurityToken>(token);
            Assert.Equal("MyAppIssuer", token.Issuer);
            Assert.Equal("MyAppAudience", token.Audiences.First());
            Assert.True(token.ValidTo > DateTime.UtcNow);
        }

        [Fact]
        public void GenerateRefreshToken_ShouldNotBeNullOrEmpty()
        {
            // Arrange & Act
            var service = new TokenService();
            var token = service.GenerateRefreshToken();

            // Assert
            Assert.False(string.IsNullOrEmpty(token));
        }

        [Fact]
        public void GenerateRefreshToken_ShouldBeUnique()
        {
            // Arrange & Act
            var service = new TokenService();
            var token1 = service.GenerateRefreshToken();
            var token2 = service.GenerateRefreshToken();

            // Assert
            Assert.NotEqual(token1, token2);
        }

        [Fact]
        public void GenerateRefreshToken_ShouldBeValidBase64()
        {
            // Arrange & Act
            var service = new TokenService();
            var token = service.GenerateRefreshToken();

            // Assert
            var isValidBase64 = IsBase64String(token);
            Assert.True(isValidBase64);
        }

        [Fact]
        public void GetPrincipalFromExpiredToken_ShouldReturnPrincipal_WhenTokenIsExpiredButValid()
        {
            // Arrange
            var secretKey = "super_secret_test_key_1234567890!";
            var mockConfig = new Mock<IConfiguration>();
            mockConfig.Setup(c => c["JWT:SecretKey"]).Returns(secretKey);

            var service = new TokenService();
            var expiredToken = GenerateExpiredToken(secretKey);

            // Act
            var principal = service.GetPrincipalFromExpiredToken(expiredToken, mockConfig.Object);

            // Assert
            Assert.NotNull(principal);
            Assert.Equal("TestUser", principal.Identity?.Name);
        }

        [Fact]
        public void GetPrincipalFromExpiredToken_ShouldThrowException_WhenTokenIsInvalid()
        {
            // Arrange
            var secretKey = "super_secret_test_key_1234567890!";
            var mockConfig = new Mock<IConfiguration>();
            mockConfig.Setup(c => c["JWT:SecretKey"]).Returns(secretKey);

            var service = new TokenService();
            var invalidToken = "this.is.not.a.valid.token";

            // Act & Assert
            Assert.Throws<SecurityTokenMalformedException>(() =>
                service.GetPrincipalFromExpiredToken(invalidToken, mockConfig.Object));
        }

        #region Funções Auxiliares
        public static string GenerateRefreshToken()
        {
            var secureRandomBytes = new byte[128];

            using var randomNumberGenerator = RandomNumberGenerator.Create();
            randomNumberGenerator.GetBytes(secureRandomBytes);

            var refreshToken = Convert.ToBase64String(secureRandomBytes);
            return refreshToken;
        }

        private bool IsBase64String(string base64)
        {
            Span<byte> buffer = new Span<byte>(new byte[base64.Length]);
            return Convert.TryFromBase64String(base64, buffer, out _);
        }

        private string GenerateExpiredToken(string secretKey)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(secretKey);

            var issuedAt = DateTime.UtcNow.AddMinutes(-60);
            var notBefore = issuedAt;
            var expires = DateTime.UtcNow.AddMinutes(-30);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, "TestUser"),
                    new Claim(ClaimTypes.Role, "Admin")
                }),
                NotBefore = notBefore,
                IssuedAt = issuedAt,
                Expires = expires,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        #endregion
    }
}
