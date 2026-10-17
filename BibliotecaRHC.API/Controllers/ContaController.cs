using System.IdentityModel.Tokens.Jwt;
using System.Globalization;
using System.Security.Claims;
using BibliotecaRHC.API.Context;
using BibliotecaRHC.API.Models;
using BibliotecaRHC.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaRHC.API.Controllers;

[ApiController]
[Authorize]
[Route("api/conta")]
public class ContaController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly ITokenService _tokenService;
    private readonly UserManager<ApplicationUser> _userManager;

    public ContaController(
        AppDbContext context,
        IConfiguration configuration,
        ITokenService tokenService,
        UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _configuration = configuration;
        _tokenService = tokenService;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> GetConta()
    {
        var user = await ObterUsuarioAutenticado();
        if (user is null)
            return NotFound(new { message = "Usuário não encontrado." });

        return Ok(new { nome = user.UserName, email = user.Email });
    }

    [HttpPut]
    public async Task<IActionResult> AtualizarConta([FromBody] UpdateAccountModel model)
    {
        var user = await ObterUsuarioAutenticado();
        if (user is null)
            return NotFound(new { message = "Usuário não encontrado." });

        var nome = model.Nome.Trim();
        var email = model.Email.Trim();
        var nomeExistente = await _userManager.FindByNameAsync(nome);
        if (nomeExistente is not null && nomeExistente.Id != user.Id)
            return Conflict(new { message = "Esse nome de usuário já está em uso." });

        var emailExistente = await _userManager.FindByEmailAsync(email);
        if (emailExistente is not null && emailExistente.Id != user.Id)
            return Conflict(new { message = "Esse e-mail já está em uso." });

        var alterandoSenha = !string.IsNullOrWhiteSpace(model.NovaSenha);
        if (alterandoSenha)
        {
            if (string.IsNullOrWhiteSpace(model.SenhaAtual))
                return BadRequest(new { message = "Informe a senha atual para cadastrar uma nova senha." });

            if (!await _userManager.CheckPasswordAsync(user, model.SenhaAtual))
                return BadRequest(new { message = "A senha atual está incorreta." });

            foreach (var validator in _userManager.PasswordValidators)
            {
                var validacaoSenha = await validator.ValidateAsync(_userManager, user, model.NovaSenha);
                if (!validacaoSenha.Succeeded)
                    return BadRequest(new { message = string.Join(" ", validacaoSenha.Errors.Select(error => error.Description)) });
            }
        }

        await using var transaction = _context.Database.IsRelational()
            ? await _context.Database.BeginTransactionAsync()
            : null;

        var historico = new List<HistoricoAlteracaoConta>();
        if (user.UserName != nome)
        {
            historico.Add(CriarHistorico(user.Id, "Usuário", user.UserName, nome));
            user.UserName = nome;
        }

        if (user.Email != email)
        {
            historico.Add(CriarHistorico(user.Id, "Email", user.Email, email));
            user.Email = email;
        }

        var resultadoAtualizacao = await _userManager.UpdateAsync(user);
        if (!resultadoAtualizacao.Succeeded)
            return BadRequest(new { message = string.Join(" ", resultadoAtualizacao.Errors.Select(error => error.Description)) });

        if (alterandoSenha)
        {
            var resultadoSenha = await _userManager.ChangePasswordAsync(user, model.SenhaAtual!, model.NovaSenha!);
            if (!resultadoSenha.Succeeded)
                return BadRequest(new { message = string.Join(" ", resultadoSenha.Errors.Select(error => error.Description)) });

            historico.Add(CriarHistorico(user.Id, "Senha", null, null));
        }

        if (historico.Count > 0)
        {
            _context.Set<HistoricoAlteracaoConta>().AddRange(historico);
            await _context.SaveChangesAsync();
        }

        if (transaction is not null)
            await transaction.CommitAsync();

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString(CultureInfo.InvariantCulture)),
            new(ClaimTypes.Name, user.UserName!),
            new(ClaimTypes.Email, user.Email!),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
        var roles = await _userManager.GetRolesAsync(user);
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var accessToken = new JwtSecurityTokenHandler().WriteToken(
            _tokenService.GenerateAccessToken(claims, _configuration));

        return Ok(new
        {
            accessToken,
            usuario = new { nome = user.UserName, email = user.Email }
        });
    }

    private async Task<ApplicationUser?> ObterUsuarioAutenticado()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (int.TryParse(userId, NumberStyles.None, CultureInfo.InvariantCulture, out var numericUserId))
            return await _userManager.FindByIdAsync(numericUserId.ToString(CultureInfo.InvariantCulture));

        var userName = User.Identity?.Name;
        return string.IsNullOrWhiteSpace(userName)
            ? null
            : await _userManager.FindByNameAsync(userName);
    }

    private static HistoricoAlteracaoConta CriarHistorico(
        int userId,
        string campo,
        string? valorAnterior,
        string? valorNovo) =>
        new()
        {
            UsuarioId = userId,
            DataAlteracaoUtc = DateTime.UtcNow,
            CampoAlterado = campo,
            ValorAnterior = valorAnterior,
            ValorNovo = valorNovo
        };
}
