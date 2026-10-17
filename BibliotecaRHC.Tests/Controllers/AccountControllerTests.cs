using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BibliotecaRHC.API.Context;
using BibliotecaRHC.API.Controllers;
using BibliotecaRHC.API.Models;
using BibliotecaRHC.API.Services;
using BibliotecaRHC.Tests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Moq;

namespace BibliotecaRHC.Tests.Controllers;

public class AccountControllerTests
{
    [Fact]
    public async Task AtualizarConta_DevePersistirDadosEHistoricoDePerfil()
    {
        await using var context = CriarContexto();
        var user = new ApplicationUser { Id = 1, UserName = "leitor", Email = "leitor@example.com" };
        var userManager = CriarUserManager(user);
        var tokenService = CriarTokenService();
        var controller = CriarController(context, userManager, tokenService.Object, user);

        var resultado = await controller.AtualizarConta(new UpdateAccountModel
        {
            Nome = "novo-leitor",
            Email = "novo@example.com"
        });

        Assert.IsType<OkObjectResult>(resultado);
        Assert.Equal("novo-leitor", user.UserName);
        Assert.Equal("novo@example.com", user.Email);

        var historico = await context.Set<HistoricoAlteracaoConta>().OrderBy(item => item.CampoAlterado).ToListAsync();
        Assert.Collection(historico,
            item =>
            {
                Assert.Equal("Email", item.CampoAlterado);
                Assert.Equal("leitor@example.com", item.ValorAnterior);
                Assert.Equal("novo@example.com", item.ValorNovo);
                Assert.Equal(1, item.UsuarioId);
                Assert.NotEqual(default, item.DataAlteracaoUtc);
            },
            item =>
            {
                Assert.Equal("Usuário", item.CampoAlterado);
                Assert.Equal("leitor", item.ValorAnterior);
                Assert.Equal("novo-leitor", item.ValorNovo);
            });
    }

    [Fact]
    public async Task AtualizarConta_DeveRegistrarAlteracaoDeSenhaSemSalvarSegredos()
    {
        await using var context = CriarContexto();
        var user = new ApplicationUser { Id = 1, UserName = "leitor", Email = "leitor@example.com" };
        var userManager = CriarUserManager(user);
        userManager.Setup(manager => manager.CheckPasswordAsync(user, "senha-atual")).ReturnsAsync(true);
        userManager.Setup(manager => manager.ChangePasswordAsync(user, "senha-atual", "senha-nova"))
            .ReturnsAsync(IdentityResult.Success);

        var controller = CriarController(context, userManager, CriarTokenService().Object, user);

        var resultado = await controller.AtualizarConta(new UpdateAccountModel
        {
            Nome = "leitor",
            Email = "leitor@example.com",
            SenhaAtual = "senha-atual",
            NovaSenha = "senha-nova"
        });

        Assert.IsType<OkObjectResult>(resultado);
        var registro = await context.Set<HistoricoAlteracaoConta>().SingleAsync();
        Assert.Equal("Senha", registro.CampoAlterado);
        Assert.Null(registro.ValorAnterior);
        Assert.Null(registro.ValorNovo);
    }

    [Fact]
    public async Task AtualizarConta_DeveExigirSenhaAtualAntesDeAlterarSenha()
    {
        await using var context = CriarContexto();
        var user = new ApplicationUser { Id = 1, UserName = "leitor", Email = "leitor@example.com" };
        var userManager = CriarUserManager(user);
        var controller = CriarController(context, userManager, CriarTokenService().Object, user);

        var resultado = await controller.AtualizarConta(new UpdateAccountModel
        {
            Nome = "leitor",
            Email = "leitor@example.com",
            NovaSenha = "senha-nova"
        });

        var badRequest = Assert.IsType<BadRequestObjectResult>(resultado);
        Assert.Empty(await context.Set<HistoricoAlteracaoConta>().ToListAsync());
        userManager.Verify(manager => manager.UpdateAsync(It.IsAny<ApplicationUser>()), Times.Never);
        Assert.NotNull(badRequest.Value);
    }

    private static AppDbContext CriarContexto()
    {
        return TestDbContextFactory.Create(Guid.NewGuid().ToString(), 1);
    }

    private static Mock<UserManager<ApplicationUser>> CriarUserManager(ApplicationUser user)
    {
        var store = new Mock<IUserStore<ApplicationUser>>();
        var manager = new Mock<UserManager<ApplicationUser>>(
            store.Object,
            new Mock<IOptions<IdentityOptions>>().Object,
            new Mock<IPasswordHasher<ApplicationUser>>().Object,
            Array.Empty<IUserValidator<ApplicationUser>>(),
            Array.Empty<IPasswordValidator<ApplicationUser>>(),
            new Mock<ILookupNormalizer>().Object,
            new Mock<IdentityErrorDescriber>().Object,
            new Mock<IServiceProvider>().Object,
            new Mock<ILogger<UserManager<ApplicationUser>>>().Object);

        manager.Setup(item => item.FindByIdAsync(user.Id.ToString())).ReturnsAsync(user);
        manager.Setup(item => item.FindByNameAsync(It.IsAny<string>())).ReturnsAsync((ApplicationUser?)null);
        manager.Setup(item => item.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync((ApplicationUser?)null);
        manager.Setup(item => item.UpdateAsync(user)).ReturnsAsync(IdentityResult.Success);
        manager.Setup(item => item.GetRolesAsync(user)).ReturnsAsync(new List<string>());
        return manager;
    }

    private static Mock<ITokenService> CriarTokenService()
    {
        var tokenService = new Mock<ITokenService>();
        tokenService.Setup(service => service.GenerateAccessToken(
                It.IsAny<IEnumerable<Claim>>(),
                It.IsAny<IConfiguration>()))
            .Returns<IEnumerable<Claim>, IConfiguration>((claims, _) => new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(10),
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes("unit-test-signing-key-1234567890")),
                    SecurityAlgorithms.HmacSha256)));
        return tokenService;
    }

    private static ContaController CriarController(
        AppDbContext context,
        Mock<UserManager<ApplicationUser>> userManager,
        ITokenService tokenService,
        ApplicationUser user)
    {
        var controller = new ContaController(
            context,
            new Mock<IConfiguration>().Object,
            tokenService,
            userManager.Object);
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(
                    [new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())],
                    "test"))
            }
        };
        return controller;
    }
}
