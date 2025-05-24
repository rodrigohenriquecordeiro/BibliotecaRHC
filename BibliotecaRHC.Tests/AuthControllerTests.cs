using BibliotecaRHC.API.Controllers;
using BibliotecaRHC.API.Models;
using BibliotecaRHC.API.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Moq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BibliotecaRHC.Tests;

public class AuthControllerTests
{
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly Mock<ITokenService> _tokenServiceMock;
    private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
    private readonly Mock<RoleManager<IdentityRole>> _roleManagerMock;

    public AuthControllerTests()
    {
        _configurationMock = new Mock<IConfiguration>();
        _tokenServiceMock = new Mock<ITokenService>();
        _userManagerMock = CreateMockUserManager();
        _roleManagerMock = CreateMockRoleManager();
    }

    [Fact]
    public async Task CreateRole_DeveRetornarSuccess_WhenRoleDoesNotExist()
    {
        // Arrange
        _roleManagerMock.Setup(r => r.RoleExistsAsync("Admin")).ReturnsAsync(false);
        _roleManagerMock.Setup(r => r.CreateAsync(It.IsAny<IdentityRole>()))
                        .ReturnsAsync(IdentityResult.Success);

        var controller = new AuthController(_tokenServiceMock.Object, _userManagerMock.Object, _roleManagerMock.Object, _configurationMock.Object);

        // Act
        var result = await controller.CreateRole("Admin");

        // Assert
        var okResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);
    }

    [Fact]
    public async Task AddUserToRole_ShouldReturnSuccess_WhenUserExists()
    {
        // Arrange
        var user = new ApplicationUser { Email = "test@example.com" };

        _userManagerMock.Setup(m => m.FindByEmailAsync(user.Email)).ReturnsAsync(user);
        _userManagerMock.Setup(m => m.AddToRoleAsync(user, "Admin"))
                        .ReturnsAsync(IdentityResult.Success);

        var controller = new AuthController(_tokenServiceMock.Object, _userManagerMock.Object, _roleManagerMock.Object, _configurationMock.Object);

        // Act
        var result = await controller.AddUserToRole(user.Email, "Admin");

        // Assert
        var okResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);
    }

    [Fact]
    public async Task Register_ShouldReturnSuccess_WhenUserIsCreated()
    {
        // Arrange
        _userManagerMock.Setup(m => m.FindByNameAsync(It.IsAny<string>())).ReturnsAsync((ApplicationUser?)null);
        _userManagerMock.Setup(m => m.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                        .ReturnsAsync(IdentityResult.Success);

        var controller = new AuthController(_tokenServiceMock.Object, _userManagerMock.Object, _roleManagerMock.Object, _configurationMock.Object);

        // Act
        var result = await controller.Register(new RegisterModel
        {
            UserName = "user1",
            Email = "user1@example.com",
            Password = "Test@123"
        });

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, ok.StatusCode);
    }

    [Fact]
    public async Task Login_ShouldReturnToken_WhenCredentialsAreValid()
    {
        // Arrange
        var user = new ApplicationUser { UserName = "user1", Email = "user1@example.com" };

        _userManagerMock.Setup(m => m.FindByEmailAsync(user.Email)).ReturnsAsync(user);
        _userManagerMock.Setup(m => m.CheckPasswordAsync(user, "Test@123")).ReturnsAsync(true);
        _userManagerMock.Setup(m => m.GetRolesAsync(user)).ReturnsAsync(new List<string> { "Admin" });
        _userManagerMock.Setup(m => m.UpdateAsync(user)).ReturnsAsync(IdentityResult.Success);

        _tokenServiceMock.Setup(t => t.GenerateAccessToken(It.IsAny<List<Claim>>(), _configurationMock.Object))
                         .Returns(new JwtSecurityToken());
        _tokenServiceMock.Setup(t => t.GenerateRefreshToken()).Returns("dummy_refresh_token");

        _configurationMock.Setup(c => c["JWT:RefreshTokenValidityInMinutes"]).Returns("30");

        var controller = new AuthController(_tokenServiceMock.Object, _userManagerMock.Object, _roleManagerMock.Object, _configurationMock.Object);

        // Act
        var result = await controller.Login(new LoginModel
        {
            UserEmail = user.Email,
            Password = "Test@123"
        });

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
    }

    [Fact]
    public async Task RefreshToken_ShouldReturnNewTokens_WhenValid()
    {
        // Arrange
        var secretKey = "super_secret_test_key_1234567890!";
        _configurationMock.Setup(c => c["JWT:SecretKey"]).Returns(secretKey);

        var userName = "testuser";
        var claims = new List<Claim> { new Claim(ClaimTypes.Name, userName) };

        var key = Encoding.UTF8.GetBytes(secretKey);
        var validJwtToken = new JwtSecurityToken(
            issuer: "TestIssuer",
            audience: "TestAudience",
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(30),
            signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
        );

        _tokenServiceMock.Setup(ts => ts.GetPrincipalFromExpiredToken(It.IsAny<string>(), _configurationMock.Object))
                         .Returns(new ClaimsPrincipal(new ClaimsIdentity(claims, "mock")));

        _tokenServiceMock.Setup(ts => ts.GenerateAccessToken(It.IsAny<List<Claim>>(), _configurationMock.Object))
                         .Returns(validJwtToken);

        _tokenServiceMock.Setup(ts => ts.GenerateRefreshToken()).Returns("new_refresh_token");

        var user = new ApplicationUser
        {
            UserName = userName,
            RefreshToken = "old_refresh_token",
            RefreshTokenExpiryTime = DateTime.Now.AddMinutes(10)
        };

        _userManagerMock.Setup(um => um.FindByNameAsync(userName)).ReturnsAsync(user);
        _userManagerMock.Setup(um => um.UpdateAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(IdentityResult.Success);

        var controller = new AuthController(_tokenServiceMock.Object, _userManagerMock.Object, _roleManagerMock.Object, _configurationMock.Object);

        var tokenModel = new TokenModel
        {
            AccessToken = "expired_access_token",
            RefreshToken = "old_refresh_token"
        };

        // Act
        var result = await controller.RefreshToken(tokenModel);

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.NotNull(objectResult.Value);

        var accessTokenProperty = objectResult.Value.GetType().GetProperty("accessToken");
        var refreshTokenProperty = objectResult.Value.GetType().GetProperty("refreshToken");

        Assert.NotNull(accessTokenProperty);
        Assert.NotNull(refreshTokenProperty);

        var accessToken = accessTokenProperty.GetValue(objectResult.Value) as string;
        var refreshToken = refreshTokenProperty.GetValue(objectResult.Value) as string;

        Assert.False(string.IsNullOrEmpty(accessToken));
        Assert.False(string.IsNullOrEmpty(refreshToken));
        Assert.Equal("new_refresh_token", refreshToken);
    }

    [Fact]
    public async Task Revoke_ShouldClearRefreshToken_WhenUserExists()
    {
        // Arrange
        var user = new ApplicationUser { UserName = "user1", RefreshToken = "token" };

        _userManagerMock.Setup(m => m.FindByNameAsync("user1")).ReturnsAsync(user);
        _userManagerMock.Setup(m => m.UpdateAsync(user)).ReturnsAsync(IdentityResult.Success);

        var controller = new AuthController(_tokenServiceMock.Object, _userManagerMock.Object, _roleManagerMock.Object, _configurationMock.Object);

        // Act
        var result = await controller.Revoke("user1");

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    #region Funções Auxiliares
    private static Mock<UserManager<ApplicationUser>> CreateMockUserManager()
    {
        var store = new Mock<IUserStore<ApplicationUser>>();
        return new Mock<UserManager<ApplicationUser>>(
            store.Object,
            new Mock<IOptions<IdentityOptions>>().Object,
            new Mock<IPasswordHasher<ApplicationUser>>().Object,
            Array.Empty<IUserValidator<ApplicationUser>>(),
            Array.Empty<IPasswordValidator<ApplicationUser>>(),
            new Mock<ILookupNormalizer>().Object,
            new Mock<IdentityErrorDescriber>().Object,
            new Mock<IServiceProvider>().Object,
            new Mock<ILogger<UserManager<ApplicationUser>>>().Object);
    }

    private static Mock<RoleManager<IdentityRole>> CreateMockRoleManager()
    {
        var store = new Mock<IRoleStore<IdentityRole>>();
        return new Mock<RoleManager<IdentityRole>>(
            store.Object,
            Array.Empty<IRoleValidator<IdentityRole>>(),
            new Mock<ILookupNormalizer>().Object,
            new Mock<IdentityErrorDescriber>().Object,
            new Mock<ILogger<RoleManager<IdentityRole>>>().Object);
    }
    #endregion
}
