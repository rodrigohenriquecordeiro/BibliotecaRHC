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
                    NumeroDePaginas = FormatarInteiro(worksheet.Cells[row, 6].Value?.ToString()),
                    ClassificacaoCatalografica = FormatarTexto(worksheet.Cells[row, 7].Value?.ToString()),
                    Observacao = FormatarTexto(worksheet.Cells[row, 8].Value?.ToString()),
                    DataDeAquisicao = dataAquisicao,
                    Lido = FormatarBooleano(worksheet.Cells[row, 10].Value?.ToString()),
                    AnoUltimaLeitura = FormatarInteiro(worksheet.Cells[row, 11].Value?.ToString())
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

    /// <summary>
    /// Converte valores diversos (Sim, Yes, True, 1, boolean) para bool.
    /// Padrão é false.
    /// </summary>
    private static bool FormatarBooleano(object? valorCelula)
    {
        if (valorCelula == null) 
            return false;

        if (valorCelula is bool valorBool)
            return valorBool;

        string texto = valorCelula.ToString()?.Trim() ?? string.Empty;
        string[] valoresVerdadeiros = { "Sim", "S", "Yes", "Y", "True", "1" };

        return valoresVerdadeiros.Contains(texto, StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Converte valor da célula para int?. Retorna null se vazio ou inválido.
    /// Trata double do Excel e strings numéricas.
    /// </summary>
    private static int? FormatarInteiro(object? valorCelula)
    {
        if (valorCelula == null) 
            return null;

        if (valorCelula is double numeroDouble)
        {
            try
            {
                return Convert.ToInt32(numeroDouble);
            }
            catch
            {
                return null;
            }
        }

        if (valorCelula is int numeroInt)
            return numeroInt;

        string texto = valorCelula.ToString()?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(texto)) 
            return null;

        if (int.TryParse(texto, out int resultado))
            return resultado;

        return null; 
    }

    #endregion
}

public interface IImportarPlanilha
{
    Task<List<string>> ImportarPlanilhaAsync(IFormFile arquivo);
}
