using Moq;
using OfficeOpenXml;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using BibliotecaRHC.API.Context;
using BibliotecaRHC.API.Models;
using BibliotecaRHC.API.Repositories;
using BibliotecaRHC.API.Services;

namespace BibliotecaRHC.Tests.Domain;

public class ImportarPlanilhaServiceTests
{
    private readonly Mock<ILivroService> _livroServiceMock;
    private readonly Mock<IFrasesInesqueciveisService> _frasesServiceMock;
    private readonly Mock<IProjetoService> _projetoServiceMock;
    private readonly Mock<ILivroProjetoService> _livroProjetoServiceMock;
    private readonly Mock<IHistoricoProjetoRepository> _historicoProjetoRepositoryMock;
    private readonly Mock<ILogger<ImportarPlanilhaService>> _loggerMock;
    private readonly ImportarPlanilhaService _service;

    public ImportarPlanilhaServiceTests()
    {
        ExcelPackage.License.SetNonCommercialPersonal("Rodrigo");

        _livroServiceMock = new Mock<ILivroService>();
        _loggerMock = new Mock<ILogger<ImportarPlanilhaService>>();
        _frasesServiceMock = new Mock<IFrasesInesqueciveisService>();
        _projetoServiceMock = new Mock<IProjetoService>();
        _livroProjetoServiceMock = new Mock<ILivroProjetoService>();
        _historicoProjetoRepositoryMock = new Mock<IHistoricoProjetoRepository>();

        _service = new ImportarPlanilhaService(
            _livroServiceMock.Object,
            _frasesServiceMock.Object,
            _projetoServiceMock.Object,
            _livroProjetoServiceMock.Object,
            _historicoProjetoRepositoryMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task ImportarPlanilhaAsync_DeveImportarLivroComSucesso_QuandoDadosSaoValidos()
    {
        // Arrange
        var arquivoMock = CriarArquivoExcelMock(new List<object[]>
        {
            new object[] { "Livro Teste", "Autor Teste", "Editora Teste", "2024", "100", "A1", "Obs", "01/01/2024" }
        });

        // Act
        var resultado = await _service.ImportarPlanilhaAsync(arquivoMock.Object);

        // Assert
        Assert.Empty(resultado); 

        _livroServiceMock.Verify(x => x.AdicionarLivroAsync(It.Is<Livro>(l =>
            l.NomeDoLivro == "Livro Teste" &&
            l.NumeroDePaginas == 100 &&
            l.AnoDePublicacao == "2024"
        )), Times.Once);
    }

    [Fact]
    public async Task ImportarPlanilhaAsync_DeveRetornarErro_QuandoDataAquisicaoForInvalida()
    {
        // Arrange
        var arquivoMock = CriarArquivoExcelMock(new List<object[]>
        {
            new object[] { "Livro Ruim", "Autor", "Ed", "2024", "10", "A", "Obs", "DataInvalida" }
        });

        // Act
        var resultado = await _service.ImportarPlanilhaAsync(arquivoMock.Object);

        // Assert
        Assert.Single(resultado); 
        Assert.Contains("Data de Aquisição inválida ou vazia", resultado[0]);

        _livroServiceMock.Verify(x => x.AdicionarLivroAsync(It.IsAny<Livro>()), Times.Never);
    }

    [Fact]
    public async Task ImportarPlanilhaAsync_DevePularLinha_QuandoNomeDoLivroEstiverVazio()
    {
        // Arrange
        var arquivoMock = CriarArquivoExcelMock(new List<object[]>
        {
            new object[] { "", "Autor", "Ed", "2024", "10", "A", "Obs", "01/01/2024" }
        });

        // Act
        var resultado = await _service.ImportarPlanilhaAsync(arquivoMock.Object);

        // Assert
        Assert.Empty(resultado);
        _livroServiceMock.Verify(x => x.AdicionarLivroAsync(It.IsAny<Livro>()), Times.Never);
    }

    [Fact]
    public async Task ImportarPlanilhaAsync_DeveTratarTrimNasStrings_EConverterDataExcel()
    {
        // Arrange
        double dataSerialExcel = 45352;

        var arquivoMock = CriarArquivoExcelMock(new List<object[]>
        {
            new object[] { "  Livro Espaços  ", "  Autor  ", "Ed", "2024", "10", "A", "Obs", dataSerialExcel }
        });

        // Act
        await _service.ImportarPlanilhaAsync(arquivoMock.Object);

        // Assert
        _livroServiceMock.Verify(x => x.AdicionarLivroAsync(It.Is<Livro>(l =>
            l.NomeDoLivro == "  Livro Espaços  " && 
            l.Autor == "Autor" && 
            l.DataDeAquisicao != null 
        )), Times.Once);
    }

    [Fact]
    public async Task ImportarPlanilhaAsync_DeveCapturarExcecaoDoService_ERetornarNaLista()
    {
        // Arrange
        var arquivoMock = CriarArquivoExcelMock(new List<object[]>
        {
            new object[] { "Livro Erro", "Autor", "Ed", "2024", "10", "A", "Obs", "01/01/2024" }
        });

        _livroServiceMock.Setup(x => x.AdicionarLivroAsync(It.IsAny<Livro>()))
                         .ThrowsAsync(new Exception("Erro de Banco de Dados"));

        // Act
        var resultado = await _service.ImportarPlanilhaAsync(arquivoMock.Object);

        // Assert
        Assert.Equal(2, resultado.Count);
        Assert.Contains("Erro ao salvar 'Livro Erro'", resultado[0]);
        Assert.Contains("Erro de Banco de Dados", resultado[0]);
    }

    [Fact]
    public async Task ImportarPlanilhaAsync_DeveVincularTodosOsRegistrosAoUsuarioAutenticado()
    {
        const int userId = 42;
        await using var context = TestDbContextFactory.Create(Guid.NewGuid().ToString(), userId);
        var arquivo = CriarPlanilhaCompleta();
        var service = CriarServicoReal(context);

        var erros = await service.ImportarPlanilhaAsync(arquivo.Object);

        Assert.Empty(erros);
        Assert.All(context.Set<Livro>().ToList(), livro => Assert.Equal(userId, livro.UsuarioId));
        Assert.All(context.Set<FraseInesquecivel>().ToList(), frase => Assert.Equal(userId, frase.UsuarioId));
        Assert.All(context.Set<Projeto>().ToList(), projeto => Assert.Equal(userId, projeto.UsuarioId));
        Assert.All(context.Set<LivroProjeto>().ToList(), livro => Assert.Equal(userId, livro.UsuarioId));
        Assert.All(context.Set<HistoricoProjeto>().ToList(), historico => Assert.Equal(userId, historico.UsuarioId));
    }

    [Fact]
    public async Task ImportarPlanilhaAsync_DeveCriarProjetoQuandoAbaNaoPossuiColunaDeNumeracao()
    {
        const int userId = 42;
        await using var context = TestDbContextFactory.Create(Guid.NewGuid().ToString(), userId);
        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("Projetos");
        PreencherLinha(worksheet, 1,
        [
            "Nome do Projeto", "Data da Criação", "Status do Projeto", "Nome do Livro",
            "Autor", "Editora", "Ano de Publicação", "Número de Páginas", "Classificação",
            "Observação", "Data de Aquisição", "Lido", "Ano Ultima Leitura"
        ]);
        PreencherLinha(worksheet, 2,
        [
            "Projeto sem numeração", new DateTime(2024, 1, 1), "Em andamento", "Livro do projeto",
            "Autor", "Editora", "2024", 100, "A1", "", new DateTime(2024, 1, 1), "Não", ""
        ]);

        var erros = await CriarServicoReal(context).ImportarPlanilhaAsync(CriarArquivoExcel(package).Object);

        Assert.Empty(erros);
        var projeto = Assert.Single(context.Set<Projeto>().ToList());
        Assert.Equal("Projeto sem numeração", projeto.Nome);
        Assert.Equal(ProjetoStatus.Andamento, projeto.ProjetoStatus);
        Assert.Equal("Livro do projeto", Assert.Single(context.Set<LivroProjeto>().ToList()).Nome);
    }

    [Fact]
    public async Task ImportarPlanilhaAsync_DeveRejeitarProjetoDeOutroUsuario()
    {
        const int projectOwnerId = 21;
        int projectId;
        await using (var ownerContext = TestDbContextFactory.Create(Guid.NewGuid().ToString(), projectOwnerId))
        {
            var projeto = new Projeto { Nome = "Projeto privado" };
            ownerContext.Set<Projeto>().Add(projeto);
            await ownerContext.SaveChangesAsync();
            projectId = projeto.Id;
        }

        await using var importerContext = TestDbContextFactory.Create(Guid.NewGuid().ToString(), 22);
        var service = CriarServicoReal(importerContext);
        var arquivo = CriarPlanilhaLivroProjeto(projectId);

        var erros = await service.ImportarPlanilhaAsync(arquivo.Object);

        Assert.Contains(erros, erro => erro.Contains($"ProjetoID {projectId} não encontrado."));
        Assert.Empty(importerContext.Set<LivroProjeto>().ToList());
    }

    private static ImportarPlanilhaService CriarServicoReal(AppDbContext context)
    {
        var unitOfWork = new UnityOfWork(context);
        return new ImportarPlanilhaService(
            new LivroService(unitOfWork, NullLogger<LivroService>.Instance),
            new FrasesInesqueciveisService(unitOfWork, NullLogger<FrasesInesqueciveisService>.Instance),
            new ProjetoService(unitOfWork, NullLogger<ProjetoService>.Instance),
            new LivroProjetoService(unitOfWork, NullLogger<LivroProjetoService>.Instance),
            new HistoricoProjetoRepository(context),
            NullLogger<ImportarPlanilhaService>.Instance);
    }

    private static Mock<IFormFile> CriarPlanilhaCompleta()
    {
        using var package = new ExcelPackage();

        var livros = package.Workbook.Worksheets.Add("Livros");
        livros.Cells[1, 1].Value = "N°";
        livros.Cells[1, 2].Value = "Nome do Livro";
        livros.Cells[1, 3].Value = "Autor";
        livros.Cells[1, 4].Value = "Editora";
        livros.Cells[1, 5].Value = "Ano";
        livros.Cells[1, 6].Value = "Páginas";
        livros.Cells[1, 7].Value = "Classificação";
        livros.Cells[1, 8].Value = "Observação";
        livros.Cells[1, 9].Value = "Data de Aquisição";
        livros.Cells[1, 10].Value = "Lido";
        livros.Cells[1, 11].Value = "Ano Última Leitura";
        PreencherLinha(livros, 2, ["", "Livro da estante", "Autor", "Editora", "2024", 100, "A1", "", new DateTime(2024, 1, 1), "Sim", 2024]);

        var frases = package.Workbook.Worksheets.Add("Frases");
        frases.Cells[1, 1].Value = "N°";
        frases.Cells[1, 2].Value = "Frase";
        frases.Cells[1, 3].Value = "Autor";
        frases.Cells[1, 4].Value = "Nome do Livro";
        PreencherLinha(frases, 2, ["", "Frase importada", "Autor", "Livro da estante"]);

        var projetos = package.Workbook.Worksheets.Add("Projetos");
        projetos.Cells[1, 1].Value = "N°";
        projetos.Cells[1, 2].Value = "Nome do Projeto";
        projetos.Cells[1, 3].Value = "Data da Criação";
        projetos.Cells[1, 4].Value = "Status do Projeto";
        projetos.Cells[1, 5].Value = "Nome do Livro";
        projetos.Cells[1, 7].Value = "Ano";
        projetos.Cells[1, 12].Value = "Lido";
        projetos.Cells[1, 13].Value = "Data de Leitura";
        PreencherLinha(projetos, 2, ["", "Projeto importado", new DateTime(2024, 1, 1), "Em andamento", "Livro do projeto", "", "2024", "", "", "", "", "Não", new DateTime(2024, 1, 1)]);

        var livrosProjeto = package.Workbook.Worksheets.Add("LivroProjeto");
        livrosProjeto.Cells[1, 1].Value = "N°";
        livrosProjeto.Cells[1, 2].Value = "Nome do Livro";
        livrosProjeto.Cells[1, 3].Value = "Ano de Publicação";
        livrosProjeto.Cells[1, 4].Value = "Lido";
        livrosProjeto.Cells[1, 5].Value = "Data de Leitura";
        livrosProjeto.Cells[1, 6].Value = "ProjetoID";
        PreencherLinha(livrosProjeto, 2, ["", "Livro associado", "2024", "Não", new DateTime(2024, 1, 1), 1]);

        return CriarArquivoExcel(package);
    }

    private static Mock<IFormFile> CriarPlanilhaLivroProjeto(int projectId)
    {
        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("LivroProjeto");
        worksheet.Cells[1, 1].Value = "N°";
        worksheet.Cells[1, 2].Value = "Nome do Livro";
        worksheet.Cells[1, 3].Value = "Ano de Publicação";
        worksheet.Cells[1, 4].Value = "Lido";
        worksheet.Cells[1, 5].Value = "Data de Leitura";
        worksheet.Cells[1, 6].Value = "ProjetoID";
        PreencherLinha(worksheet, 2, ["", "Livro privado", "2024", "Não", new DateTime(2024, 1, 1), projectId]);
        return CriarArquivoExcel(package);
    }

    private static void PreencherLinha(ExcelWorksheet worksheet, int row, object?[] values)
    {
        for (var column = 0; column < values.Length; column++)
            worksheet.Cells[row, column + 1].Value = values[column];
    }

    private static Mock<IFormFile> CriarArquivoExcel(ExcelPackage package)
    {
        var stream = new MemoryStream(package.GetAsByteArray());
        var fileMock = new Mock<IFormFile>();
        fileMock.Setup(file => file.OpenReadStream()).Returns(stream);
        fileMock.Setup(file => file.FileName).Returns("importacao.xlsx");
        fileMock.Setup(file => file.Length).Returns(stream.Length);
        fileMock.Setup(file => file.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .Callback<Stream, CancellationToken>((target, _) => stream.CopyTo(target))
            .Returns(Task.CompletedTask);
        return fileMock;
    }

    /// <summary>
    /// Método auxiliar para criar um IFormFile fake contendo um Excel real em memória
    /// </summary>
    private Mock<IFormFile> CriarArquivoExcelMock(List<object[]> linhasDeDados)
    {
        var stream = new MemoryStream();

        using (var package = new ExcelPackage(stream))
        {
            var worksheet = package.Workbook.Worksheets.Add("Planilha1");
            worksheet.Cells[1, 1].Value = "ID";
            worksheet.Cells[1, 2].Value = "Nome";
            worksheet.Cells[1, 3].Value = "Autor";
            worksheet.Cells[1, 4].Value = "Editora";
            worksheet.Cells[1, 5].Value = "Ano";
            worksheet.Cells[1, 6].Value = "Paginas";
            worksheet.Cells[1, 7].Value = "Classificacao";
            worksheet.Cells[1, 8].Value = "Observacao";
            worksheet.Cells[1, 9].Value = "Data";

            for (int i = 0; i < linhasDeDados.Count; i++)
            {
                var dados = linhasDeDados[i];
                int linhaExcel = i + 2;

                if (dados.Length > 0) worksheet.Cells[linhaExcel, 2].Value = dados[0]; // Nome
                if (dados.Length > 1) worksheet.Cells[linhaExcel, 3].Value = dados[1]; // Autor
                if (dados.Length > 2) worksheet.Cells[linhaExcel, 4].Value = dados[2]; // Editora
                if (dados.Length > 3) worksheet.Cells[linhaExcel, 5].Value = dados[3]; // Ano
                if (dados.Length > 4) worksheet.Cells[linhaExcel, 6].Value = dados[4]; // Paginas
                if (dados.Length > 5) worksheet.Cells[linhaExcel, 7].Value = dados[5]; // Classificacao
                if (dados.Length > 6) worksheet.Cells[linhaExcel, 8].Value = dados[6]; // Observacao
                if (dados.Length > 7) worksheet.Cells[linhaExcel, 9].Value = dados[7]; // Data Aquisição
            }

            package.Save();
        }

        stream.Position = 0;

        var fileMock = new Mock<IFormFile>();
        fileMock.Setup(f => f.OpenReadStream()).Returns(stream);
        fileMock.Setup(f => f.FileName).Returns("teste.xlsx");
        fileMock.Setup(f => f.Length).Returns(stream.Length);

        fileMock.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .Callback<Stream, CancellationToken>((target, token) =>
            {
                stream.CopyTo(target);
            })
            .Returns(Task.CompletedTask);

        return fileMock;
    }
}
