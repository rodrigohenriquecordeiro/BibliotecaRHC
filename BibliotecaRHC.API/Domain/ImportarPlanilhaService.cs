using BibliotecaRHC.API.Models;
using OfficeOpenXml;

namespace BibliotecaRHC.API.Domain;

public class ImportarPlanilhaService : IImportarPlanilha
{
    private readonly ILivroService _livro;
    private readonly ILogger<ImportarPlanilhaService> _logger;

    public ImportarPlanilhaService(ILivroService livro, ILogger<ImportarPlanilhaService> logger)
    {
        _livro = livro;
        _logger = logger;
    }

    public async Task ImportarPlanilhaAsync(IFormFile arquivo)
    {
        try
        {
            using var stream = new MemoryStream();
            await arquivo.CopyToAsync(stream);

            using var package = new ExcelPackage(stream);
            ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
            int colCount = worksheet.Dimension.End.Column;
            int rowCount = worksheet.Dimension.End.Row;

            for (int row = 2; row <= rowCount; row++)
            {
                if (!string.IsNullOrEmpty(worksheet.Cells[row, 2].Value?.ToString()))
                {
                    Livro livro = new()
                    {
                        NomeDoLivro = worksheet.Cells[row, 2].Value?.ToString(),
                        Autor = worksheet.Cells[row, 3].Value?.ToString(),
                        Editora = worksheet.Cells[row, 4].Value?.ToString(),
                        AnoDePublicacao = worksheet.Cells[row, 5].Value?.ToString(),
                        NumeroDePaginas = int.Parse(worksheet.Cells[row, 6].Value?.ToString()!),
                        ClassificacaoCatalografica = worksheet.Cells[row, 7].Value?.ToString(),
                        Observacao = worksheet.Cells[row, 8].Value?.ToString() ?? string.Empty,
                        DataDeAquisicao = worksheet.Cells[row, 9].Value?.ToString()
                    };

                    await _livro.AdicionarLivroAsync(livro);
                }
            }
        }
        catch (Exception e)
        {
            _logger.LogError("Erro ao importar planilha: {Message}", e.Message);
            throw;
        }
    }
}

public interface IImportarPlanilha
{
    Task ImportarPlanilhaAsync(IFormFile arquivo);
}
