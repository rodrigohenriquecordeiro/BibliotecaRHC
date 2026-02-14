using BibliotecaRHC.API.Domain;
using Microsoft.AspNetCore.Mvc;

namespace BibliotecaRHC.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ImportarPlanilhaController : ControllerBase
{
    private readonly IImportarPlanilha _service;

    public ImportarPlanilhaController(IImportarPlanilha service)
    {
        _service = service;
    }

    [HttpPost("importar")]
    public async Task<IActionResult> ImportarExcel(IFormFile arquivo)
    {
        if (arquivo == null || arquivo.Length == 0)
            return BadRequest("Envie um arquivo válido.");

        if (!arquivo.FileName.EndsWith(".xlsx"))
            return BadRequest("Apenas arquivos .xlsx são permitidos.");

        List<string> erros = await _service.ImportarPlanilhaAsync(arquivo);

        if (erros.Count > 0)
        {
            return BadRequest(new
            {
                Mensagem = "A importação foi concluída, mas algumas linhas falharam.",
                Erros = erros
            });
        }

        return Ok(new { Mensagem = "Todos os livros foram importados com sucesso!" });
    }
}
