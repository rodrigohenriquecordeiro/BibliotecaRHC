using BibliotecaRHC.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;

namespace BibliotecaRHC.API.Controllers;

[ApiController]
[Authorize]
[Route("api/importar-planilha")]
public class ImportarPlanilhaController : ControllerBase
{
    private readonly IImportarPlanilha _service;

    public ImportarPlanilhaController(IImportarPlanilha service)
    {
        _service = service;
    }

    [HttpGet("modelo")]
    public IActionResult Modelo()
    {
        using var package = new ExcelPackage();
                
        var wsLivros = package.Workbook.Worksheets.Add("Livros");
        wsLivros.Cells[1, 1].Value = "N°";
        wsLivros.Cells[1, 2].Value = "Nome do Livro";
        wsLivros.Cells[1, 3].Value = "Autor";
        wsLivros.Cells[1, 4].Value = "Editora";
        wsLivros.Cells[1, 5].Value = "Ano de Publicação";
        wsLivros.Cells[1, 6].Value = "Número de Páginas";
        wsLivros.Cells[1, 7].Value = "Classificação";
        wsLivros.Cells[1, 8].Value = "Observação";
        wsLivros.Cells[1, 9].Value = "Data de Aquisição";
        wsLivros.Cells[1, 10].Value = "Lido";
        wsLivros.Cells[1, 11].Value = "Ano Ultima Leitura";
                
        var wsFrases = package.Workbook.Worksheets.Add("Frases");
        wsFrases.Cells[1, 1].Value = "N°";
        wsFrases.Cells[1, 2].Value = "Frase";
        wsFrases.Cells[1, 3].Value = "Autor";
        wsFrases.Cells[1, 4].Value = "Nome do Livro";
                
        var wsProjetos = package.Workbook.Worksheets.Add("Projetos");
        wsProjetos.Cells[1, 1].Value = "N°";
        wsProjetos.Cells[1, 2].Value = "Nome do Projeto";
        wsProjetos.Cells[1, 3].Value = "Data da Criação";
        wsProjetos.Cells[1, 4].Value = "Status do Projeto";
                
        wsProjetos.Cells[1, 5].Value = "Nome do Livro";
        wsProjetos.Cells[1, 6].Value = "Autor";
        wsProjetos.Cells[1, 7].Value = "Editora";
        wsProjetos.Cells[1, 8].Value = "Ano de Publicação";
        wsProjetos.Cells[1, 9].Value = "Número de Páginas";
        wsProjetos.Cells[1, 10].Value = "Classificação";
        wsProjetos.Cells[1, 11].Value = "Observação";
        wsProjetos.Cells[1, 12].Value = "Data de Aquisição";
        wsProjetos.Cells[1, 13].Value = "Lido";
        wsProjetos.Cells[1, 14].Value = "Ano Ultima Leitura";
                
        var wsLivroProjeto = package.Workbook.Worksheets.Add("LivroProjeto");
        wsLivroProjeto.Cells[1, 1].Value = "N°";
        wsLivroProjeto.Cells[1, 2].Value = "Nome do Livro";
        wsLivroProjeto.Cells[1, 3].Value = "Ano de Publicação";
        wsLivroProjeto.Cells[1, 4].Value = "Lido";
        wsLivroProjeto.Cells[1, 5].Value = "Data de Leitura";
        wsLivroProjeto.Cells[1, 6].Value = "ProjetoID";

        var bytes = package.GetAsByteArray();

        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "modelo-importacao.xlsx");
    }

    [HttpPost("importar-excel")]
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
