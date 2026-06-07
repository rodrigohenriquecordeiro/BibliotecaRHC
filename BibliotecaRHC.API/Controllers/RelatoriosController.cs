using BibliotecaRHC.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecaRHC.API.Controllers;

[ApiController]
[Route("api/relatorios")]
public class RelatoriosController(IRelatoriosService service) : ControllerBase
{
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetDashboard()
    {
        var dados = await service.ObterDadosDoRelatorio();
        return Ok(dados);
    }
}