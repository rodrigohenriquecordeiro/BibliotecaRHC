using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using BibliotecaRHC.API.Controllers;
using BibliotecaRHC.API.Services;

namespace BibliotecaRHC.Tests.Controllers;

public class ImportarPlanilhaControllerTests
{
    private readonly Mock<IImportarPlanilha> _serviceMock;
    private readonly ImportarPlanilhaController _controller;

    public ImportarPlanilhaControllerTests()
    {
        _serviceMock = new Mock<IImportarPlanilha>();
        _controller = new ImportarPlanilhaController(_serviceMock.Object);
    }

    [Fact]
    public async Task ImportarExcel_DeveRetornarBadRequest_QuandoArquivoForNulo()
    {
        // Arrange
        IFormFile? arquivoNulo = null;

        // Act
        var resultado = await _controller.ImportarExcel(arquivoNulo!);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(resultado);
        Assert.Equal("Envie um arquivo válido.", badRequestResult.Value);
    }

    [Fact]
    public async Task ImportarExcel_DeveRetornarBadRequest_QuandoArquivoEstiverVazio()
    {
        // Arrange
        var arquivoMock = new Mock<IFormFile>();
        arquivoMock.Setup(f => f.Length).Returns(0); 

        // Act
        var resultado = await _controller.ImportarExcel(arquivoMock.Object);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(resultado);
        Assert.Equal("Envie um arquivo válido.", badRequestResult.Value);
    }

    [Fact]
    public async Task ImportarExcel_DeveRetornarBadRequest_QuandoExtensaoNaoForXlsx()
    {
        // Arrange
        var arquivoMock = new Mock<IFormFile>();
        arquivoMock.Setup(f => f.Length).Returns(100);
        arquivoMock.Setup(f => f.FileName).Returns("livros.txt"); 

        // Act
        var resultado = await _controller.ImportarExcel(arquivoMock.Object);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(resultado);
        Assert.Equal("Apenas arquivos .xlsx são permitidos.", badRequestResult.Value);
    }

    [Fact]
    public async Task ImportarExcel_DeveRetornarOk_QuandoServicoNaoRetornarErros()
    {
        // Arrange
        var arquivoMock = new Mock<IFormFile>();
        arquivoMock.Setup(f => f.Length).Returns(100);
        arquivoMock.Setup(f => f.FileName).Returns("livros.xlsx");

        _serviceMock.Setup(s => s.ImportarPlanilhaAsync(It.IsAny<IFormFile>()))
                    .ReturnsAsync(new List<string>());

        // Act
        var resultado = await _controller.ImportarExcel(arquivoMock.Object);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(resultado);

        Assert.Equal(200, okResult.StatusCode);

        var valorRetorno = okResult.Value;
        var mensagem = valorRetorno?.GetType().GetProperty("Mensagem")?.GetValue(valorRetorno, null) as string;

        Assert.Equal("Todos os livros foram importados com sucesso!", mensagem);
    }

    [Fact]
    public async Task ImportarExcel_DeveRetornarBadRequest_QuandoServicoRetornarListaDeErros()
    {
        // Arrange
        var arquivoMock = new Mock<IFormFile>();
        arquivoMock.Setup(f => f.Length).Returns(100);
        arquivoMock.Setup(f => f.FileName).Returns("livros.xlsx");

        var listaDeErros = new List<string> { "Erro na linha 2", "Erro na linha 5" };

        _serviceMock.Setup(s => s.ImportarPlanilhaAsync(It.IsAny<IFormFile>()))
                    .ReturnsAsync(listaDeErros);

        // Act
        var resultado = await _controller.ImportarExcel(arquivoMock.Object);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(resultado);
        Assert.Equal(400, badRequestResult.StatusCode);

        var valorRetorno = badRequestResult.Value;

        var mensagem = valorRetorno?.GetType().GetProperty("Mensagem")?.GetValue(valorRetorno, null) as string;
        var errosRetornados = valorRetorno?.GetType().GetProperty("Erros")?.GetValue(valorRetorno, null) as List<string>;

        Assert.Equal("A importação foi concluída, mas algumas linhas falharam.", mensagem);
        Assert.Equal(2, errosRetornados?.Count);
        Assert.Contains("Erro na linha 2", errosRetornados!);
    }
}