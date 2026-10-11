using OfficeOpenXml;
using BibliotecaRHC.API.Models;
using BibliotecaRHC.API.Repositories;
using System.Globalization;

namespace BibliotecaRHC.API.Services;

public class ImportarPlanilhaService : IImportarPlanilha
{
    private readonly ILivroService _livroService;
    private readonly IFrasesInesqueciveisService _frasesService;
    private readonly IProjetoService _projetoService;
    private readonly ILivroProjetoService _livroProjetoService;
    private readonly IHistoricoProjetoRepository _historicoProjetoRepository;
    private readonly ILogger<ImportarPlanilhaService> _logger;

    public ImportarPlanilhaService(ILivroService livroService,
                                   IFrasesInesqueciveisService frasesService,
                                   IProjetoService projetoService,
                                   ILivroProjetoService livroProjetoService,
                                   IHistoricoProjetoRepository historicoProjetoRepository,
                                   ILogger<ImportarPlanilhaService> logger)
    {
        _livroService = livroService;
        _frasesService = frasesService;
        _projetoService = projetoService;
        _livroProjetoService = livroProjetoService;
        _historicoProjetoRepository = historicoProjetoRepository;
        _logger = logger;
    }

    private static int FindColumnIndex(Dictionary<string,int> headerMap, string[] keys, int defaultIndex)
    {
        foreach(var k in keys)
        {
            var match = headerMap.Keys.FirstOrDefault(h => h.Contains(k.Trim().ToLowerInvariant()));
            if (match != null) return headerMap[match];
        }

        return defaultIndex;
    }


    public async Task<List<string>> ImportarPlanilhaAsync(IFormFile arquivo)
    {
        var erros = new List<string>();

        using var stream = new MemoryStream();
        await arquivo.CopyToAsync(stream);

        using var package = new ExcelPackage(stream);

        foreach (var worksheet in package.Workbook.Worksheets)
        {
            if (worksheet == null || worksheet.Dimension == null)
                continue;

            var sheetName = (worksheet.Name ?? string.Empty).Trim().ToLowerInvariant();
            int rowCount = worksheet.Dimension.End.Row;

            // mapear cabeçalhos e detectar se existe coluna de número (N°, Nº) na coluna A
            int maxColGlobal = worksheet.Dimension.End.Column;
            var headerMapGlobal = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            for (int c = 1; c <= maxColGlobal; c++)
            {
                var h = FormatarTexto(worksheet.Cells[1, c].Value);
                if (string.IsNullOrWhiteSpace(h)) continue;
                headerMapGlobal[h.Trim().ToLowerInvariant()] = c;
            }

            int colOffset = 0;
            string[] numeroKeys = new[] { "nº", "n°", "n", "no", "número", "numero", "#" };
            if (headerMapGlobal.Keys.Any(k => numeroKeys.Any(nk => k.Contains(nk))))
            {
                colOffset = 1; // planilha possui coluna 'Nº' antes dos dados
            }

            if (sheetName.Contains("frase"))
            {
                int colFrase = 1 + colOffset;
                int colAutor = 2 + colOffset;
                int colNomeLivro = 3 + colOffset;

                for (int row = 2; row <= rowCount; row++)
                {
                    var textoFrase = worksheet.Cells[row, colFrase].Value?.ToString();
                    if (string.IsNullOrWhiteSpace(textoFrase)) continue;

                    try
                    {
                        var frase = new FraseInesquecivel
                        {
                            Frase = FormatarTexto(worksheet.Cells[row, colFrase].Value),
                            Autor = FormatarTexto(worksheet.Cells[row, colAutor].Value),
                            NomeDoLivro = FormatarTexto(worksheet.Cells[row, colNomeLivro].Value)
                        };

                        await _frasesService.AdicionarFrase(frase);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, "Erro ao importar frase na linha {Linha}", row);
                        erros.Add($"Linha (Frases) {row}: Erro ao salvar frase - {e.Message}");
                    }
                }
            }
            else if (sheetName.Contains("livroprojet") || sheetName.Contains("livro projeto") || sheetName.Contains("livroprojeto"))
            {              
                int maxCol = worksheet.Dimension.End.Column;
                var headerMap = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
                for (int c = 1; c <= maxCol; c++)
                {
                    var h = FormatarTexto(worksheet.Cells[1, c].Value);
                    if (string.IsNullOrWhiteSpace(h)) continue;
                    headerMap[h.Trim().ToLowerInvariant()] = c;
                }

                int colNome = FindColumnIndex(headerMap, ["nome do livro", "nome do livro"], 2);
                int colAno = FindColumnIndex(headerMap, ["ano de publicação", "ano"], 3);
                int colLido = FindColumnIndex(headerMap, ["lido"], 4);
                int colDataLeitura = FindColumnIndex(headerMap, ["data de leitura", "data de leitura"], 5);
                int colProjetoId = FindColumnIndex(headerMap, ["projetoid", "projeto id", "projetoid"], 6);

                for (int row = 2; row <= rowCount; row++)
                {
                    var nomeLivro = worksheet.Cells[row, colNome].Value?.ToString();
                    if (string.IsNullOrWhiteSpace(nomeLivro)) continue;

                    try
                    {
                        var lp = new LivroProjeto
                        {
                            Nome = FormatarTexto(worksheet.Cells[row, colNome].Value),
                            AnoDePublicacao = FormatarTexto(worksheet.Cells[row, colAno].Value),
                            Lido = FormatarBooleano(worksheet.Cells[row, colLido].Value?.ToString()),
                            DataDeLeitura = ObterDataFormatada(worksheet.Cells[row, colDataLeitura].Value)
                        };

                        var projetoIdObj = worksheet.Cells[row, colProjetoId].Value;
                        if (projetoIdObj != null && int.TryParse(projetoIdObj.ToString(), out int projetoId))
                        {
                            var projetoExistente = await _projetoService.ObterProjetoPorId(projetoId);
                            if (projetoExistente == null)
                            {
                                erros.Add($"Linha (LivroProjeto) {row}: ProjetoID {projetoId} não encontrado.");
                                continue;
                            }

                            lp.ProjetoId = projetoId;
                            await _livroProjetoService.AdicionarLivroNoProjeto(lp);
                            await _historicoProjetoRepository.AtualizarHistorico(projetoExistente);
                        }
                        else
                        {
                            var livro = new Livro
                            {
                                NomeDoLivro = lp.Nome,
                                AnoDePublicacao = lp.AnoDePublicacao,
                                DataDeAquisicao = lp.DataDeLeitura ?? DateTime.Now,
                                Lido = lp.Lido ?? false
                            };

                            await _livroService.AdicionarLivroAsync(livro);
                        }
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, "Erro ao importar LivroProjeto na linha {Linha}", row);
                        erros.Add($"Linha (LivroProjeto) {row}: Erro ao salvar livro-projeto - {e.Message}");
                    }
                }

            }
            else if (sheetName.Contains("projet"))
            {
                var projetos = new Dictionary<string, Projeto>();

                int colNomeProjeto = 1 + colOffset;
                int colDataCriacao = 2 + colOffset;
                int colStatusProjeto = 3 + colOffset;
                int colLivroStart = 4 + colOffset;

                for (int row = 2; row <= rowCount; row++)
                {
                    var nomeProjeto = FormatarTexto(worksheet.Cells[row, colNomeProjeto].Value);
                    if (string.IsNullOrWhiteSpace(nomeProjeto)) continue;

                    try
                    {
                        if (!projetos.ContainsKey(nomeProjeto))
                        {
                            var projeto = new Projeto
                            {
                                Nome = nomeProjeto,
                                DataCriacao = ObterDataFormatada(worksheet.Cells[row, colDataCriacao].Value) ?? DateTime.Now,
                                ProjetoStatus = ParseProjetoStatus(worksheet.Cells[row, colStatusProjeto].Value),
                                LivroProjetos = new List<LivroProjeto>(),
                                HistoricoProjetos = new List<HistoricoProjeto>()
                            };

                            projetos[nomeProjeto] = projeto;
                        }

                        var nomeLivro = FormatarTexto(worksheet.Cells[row, colLivroStart].Value);
                        if (string.IsNullOrWhiteSpace(nomeLivro))
                            continue;

                        var livroProjeto = new LivroProjeto
                        {
                            Nome = nomeLivro,
                            AnoDePublicacao = FormatarTexto(worksheet.Cells[row, colLivroStart + 2].Value),
                            Lido = FormatarBooleano(worksheet.Cells[row, colLivroStart + 7].Value?.ToString()),
                            DataDeLeitura = ObterDataFormatada(worksheet.Cells[row, colLivroStart + 8].Value),
                            ProjetoId = 0
                        };

                        projetos[nomeProjeto].LivroProjetos.Add(livroProjeto);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, "Erro ao importar projeto na linha {Linha}", row);
                        erros.Add($"Linha (Projetos) {row}: Erro ao salvar projeto/livro - {e.Message}");
                    }
                }

                try
                {
                    var existentes = (await _projetoService.ObterTodosOsProjetos()).ToList();

                    foreach (var projeto in projetos.Values)
                    {
                        try
                        {
                            var existente = existentes.FirstOrDefault(p => string.Equals(p.Nome?.Trim(), projeto.Nome?.Trim(), StringComparison.OrdinalIgnoreCase));

                            if (existente != null)
                            {
                                if (projeto.LivroProjetos != null && projeto.LivroProjetos.Any())
                                {
                                    foreach (var lp in projeto.LivroProjetos)
                                    {
                                        lp.ProjetoId = existente.Id;
                                        await _livroProjetoService.AdicionarLivroNoProjeto(lp);
                                    }

                                    await _historicoProjetoRepository.AtualizarHistorico(existente);
                                }
                            }
                            else
                            {
                                await _projetoService.AdicionarProjeto(projeto);
                            }
                        }
                        catch (Exception e)
                        {
                            _logger.LogError(e, "Erro ao salvar projeto {Projeto}", projeto.Nome);
                            erros.Add($"Projeto '{projeto.Nome}': Erro ao salvar - {e.Message}");
                        }
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Erro ao recuperar projetos existentes");
                    erros.Add($"Erro ao processar projetos: {e.Message}");
                }
            }
            else
            {
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
                            Autor = FormatarTexto(worksheet.Cells[row, 3].Value),
                            Editora = FormatarTexto(worksheet.Cells[row, 4].Value),
                            AnoDePublicacao = FormatarTexto(worksheet.Cells[row, 5].Value),
                            NumeroDePaginas = FormatarInteiro(worksheet.Cells[row, 6].Value),
                            ClassificacaoCatalografica = FormatarTexto(worksheet.Cells[row, 7].Value),
                            Observacao = FormatarTexto(worksheet.Cells[row, 8].Value),
                            DataDeAquisicao = dataAquisicao,
                            Lido = FormatarBooleano(worksheet.Cells[row, 10].Value?.ToString()),
                            AnoUltimaLeitura = FormatarInteiro(worksheet.Cells[row, 11].Value)
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
            }
        }

        return erros;
    }

    #region Métodos para Formatar

    private static ProjetoStatus ParseProjetoStatus(object? valorCelula)
    {
        if (valorCelula == null) return ProjetoStatus.NaoIniciado;

        var texto = valorCelula.ToString()?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(texto)) return ProjetoStatus.NaoIniciado;
                
        texto = texto.ToLowerInvariant();

        if (texto.Contains("andamento") || texto.Contains("em andamento") || texto.Contains("andam"))
            return ProjetoStatus.Andamento;
        if (texto.Contains("finaliz") || texto.Contains("conclu") || texto.Contains("feito"))
            return ProjetoStatus.Finalizado;
        if (texto.Contains("aband") || texto.Contains("abandon"))
            return ProjetoStatus.Abandonado;

        return ProjetoStatus.NaoIniciado;
    }

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
