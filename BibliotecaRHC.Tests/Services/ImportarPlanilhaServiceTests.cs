using Moq;
using OfficeOpenXml;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using BibliotecaRHC.API.Models;
using BibliotecaRHC.API.Services;

namespace BibliotecaRHC.Tests.Domain;

public class ImportarPlanilhaServiceTests
{
    private readonly Mock<ILivroService> _livroServiceMock;
    private readonly Mock<ILogger<ImportarPlanilhaService>> _loggerMock;
    private readonly ImportarPlanilhaService _service;

    public ImportarPlanilhaServiceTests()
    {
        ExcelPackage.License.SetNonCommercialPersonal("Rodrigo");

        _livroServiceMock = new Mock<ILivroService>();
        _loggerMock = new Mock<ILogger<ImportarPlanilhaService>>();

        _service = new ImportarPlanilhaService(_livroServiceMock.Object, _loggerMock.Object);
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
