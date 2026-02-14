using OfficeOpenXml;
using BibliotecaRHC.API.Models;
using System.Globalization; 

namespace BibliotecaRHC.API.Domain;

public class ImportarPlanilhaService : IImportarPlanilha
{
    private readonly ILivroService _livroService;
    private readonly ILogger<ImportarPlanilhaService> _logger;

    public ImportarPlanilhaService(ILivroService livroService, ILogger<ImportarPlanilhaService> logger)
    {
        _livroService = livroService;
        _logger = logger;
    }

    public async Task<List<string>> ImportarPlanilhaAsync(IFormFile arquivo)
    {
        var erros = new List<string>();

        using var stream = new MemoryStream();
        await arquivo.CopyToAsync(stream);

        using var package = new ExcelPackage(stream);
        ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
        int rowCount = worksheet.Dimension.End.Row;

        for (int row = 2; row <= rowCount; row++)
        {
            var nomeLivro = worksheet.Cells[row, 2].Value?.ToString();
            if (string.IsNullOrWhiteSpace(nomeLivro)) 
                continue;

            try
            {
                DateTime? dataAquisicao = ObterDataFormatada(worksheet.Cells[row, 9].Value);

                if (dataAquisicao == null)
                {
                    erros.Add($"Linha {row}: Data de Aquisição inválida ou vazia.");
                    continue;
                }

                Livro livro = new()
                {
                    NomeDoLivro = nomeLivro,
                    Autor = FormatarTexto(worksheet.Cells[row, 3].Value?.ToString()),
                    Editora = FormatarTexto(worksheet.Cells[row, 4].Value?.ToString()),
                    AnoDePublicacao = FormatarTexto(worksheet.Cells[row, 5].Value), 
                    NumeroDePaginas = int.TryParse(worksheet.Cells[row, 6].Value?.ToString(), out int pags) ? pags : 0,
                    ClassificacaoCatalografica = FormatarTexto(worksheet.Cells[row, 7].Value?.ToString()),
                    Observacao = FormatarTexto(worksheet.Cells[row, 8].Value?.ToString()),
                    DataDeAquisicao = dataAquisicao 
                };

                await _livroService.AdicionarLivroAsync(livro);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Erro ao importar linha {Linha}", row);
                erros.Add($"Linha {row}: Erro ao salvar '{nomeLivro}' - {e.Message}");
                erros.Add($">--> Inner Excepetion: {e.InnerException}");
            }
        }

        return erros;
    }

    #region Métodos para Formatar

    /// <summary>
    /// Recebe o valor bruto da célula (object), converte para string e remove espaços em branco das pontas.
    /// Retorna null se a célula for nula.
    /// </summary>
    private static string? FormatarTexto(object? valorCelula)
    {
        if (valorCelula == null) 
            return string.Empty;
        
        return valorCelula.ToString()?.Trim();
    }

    /// <summary>
    /// Tenta extrair uma Data válida (DateTime)
    /// </summary>
    private static DateTime? ObterDataFormatada(object? valorCelula)
    {
        if (valorCelula == null) 
            return null;

        if (valorCelula is DateTime data)
            return data;

        if (valorCelula is double numeroSerial)
        {
            try
            {
                return DateTime.FromOADate(numeroSerial);
            }
            catch { return null; }
        }

        string textoData = valorCelula.ToString()!.Trim();

        if (DateTime.TryParse(textoData, new CultureInfo("pt-BR"), DateTimeStyles.None, out DateTime dataConvertida))
            return dataConvertida;

        return null; 
    }

    #endregion
}

public interface IImportarPlanilha
{
    Task<List<string>> ImportarPlanilhaAsync(IFormFile arquivo);
}
