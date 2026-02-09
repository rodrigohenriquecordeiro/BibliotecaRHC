using BibliotecaRHC.API.Controllers;
using BibliotecaRHC.API.Domain;
using BibliotecaRHC.API.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace BibliotecaRHC.Tests.Controllers;

public class FraseInesquecivelControllerTests
{
    private readonly Mock<IFrasesInesqueciveisService> _serviceMock;
    private readonly FraseInesquecivelController _controller;

    public FraseInesquecivelControllerTests()
    {
        _serviceMock = new Mock<IFrasesInesqueciveisService>();
        _controller = new FraseInesquecivelController(_serviceMock.Object);
    }

    #region GET Methods

    [Fact]
    public async Task GetTodos_DeveRetornarOk_ComListaDeFrases()
    {
        // Arrange
        var listaMock = new List<FraseInesquecivel> { new() { Id = 1, Frase = "Teste" } };
        _serviceMock.Setup(s => s.ObterTodasAsFrases()).ReturnsAsync(listaMock);

        // Act
        var result = await _controller.GetTodos();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var retorno = Assert.IsAssignableFrom<IEnumerable<FraseInesquecivel>>(okResult.Value);
        Assert.Single(retorno);
    }

    [Fact]
    public async Task GetById_DeveRetornarOk_QuandoEncontrado()
    {
        // Arrange
        var fraseMock = new FraseInesquecivel { Id = 1, Frase = "Teste" };
        _serviceMock.Setup(s => s.ObterFrasePorId(1)).ReturnsAsync(fraseMock);

        // Act
        var result = await _controller.GetById(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var retorno = Assert.IsType<FraseInesquecivel>(okResult.Value);
        Assert.Equal(1, retorno.Id);
    }

    [Fact]
    public async Task GetById_DeveRetornarNotFound_QuandoNaoEncontrado()
    {
        // Arrange
        _serviceMock.Setup(s => s.ObterFrasePorId(99)).ReturnsAsync((FraseInesquecivel?)null);

        // Act
        var result = await _controller.GetById(99);

        // Assert
        Assert.IsType<NotFoundResult>(result); 
    }

    [Fact]
    public async Task GetFraseAleatoria_DeveRetornarOk_QuandoEncontrada()
    {
        // Arrange
        var fraseMock = new FraseInesquecivel { Id = 5, Frase = "Aleatoria" };
        _serviceMock.Setup(s => s.ObterFraseAleatoria()).ReturnsAsync(fraseMock);

        // Act
        var result = await _controller.GetFraseAleatoria();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(fraseMock, okResult.Value);
    }

    #endregion

    #region POST Method

    [Fact]
    public async Task Post_DeveRetornarCreated_QuandoSucesso()
    {
        // Arrange
        var novaFrase = new FraseInesquecivel { Frase = "Nova" };
        var fraseCriada = new FraseInesquecivel { Id = 10, Frase = "Nova" };

        _serviceMock.Setup(s => s.AdicionarFrase(novaFrase)).ReturnsAsync(fraseCriada);

        // Act
        var result = await _controller.Post(novaFrase);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result); 
        Assert.Equal(nameof(FraseInesquecivelController.GetById), createdResult.ActionName);
        Assert.Equal(10, createdResult.RouteValues!["id"]); 
        Assert.Equal(fraseCriada, createdResult.Value);
    }

    #endregion

    #region PUT Method

    [Fact]
    public async Task Put_DeveRetornarBadRequest_QuandoIdsNaoBatem()
    {
        // Arrange
        var frase = new FraseInesquecivel { Id = 1 };
        int idDiferente = 2;

        // Act
        var result = await _controller.Put(idDiferente, frase);

        // Assert
        var badRequest = Assert.IsType<BadRequestObjectResult>(result); 
        Assert.Equal("ID da frase não corresponde ao ID fornecido na URL.", badRequest.Value);
        _serviceMock.Verify(s => s.AtualizarFrase(It.IsAny<FraseInesquecivel>()), Times.Never);
    }

    [Fact]
    public async Task Put_DeveRetornarNoContent_QuandoAtualizadoComSucesso()
    {
        // Arrange
        var frase = new FraseInesquecivel { Id = 1, Frase = "Atualizada" };
        _serviceMock.Setup(s => s.AtualizarFrase(frase)).ReturnsAsync(frase);

        // Act
        var result = await _controller.Put(1, frase);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Put_DeveRetornarNotFound_QuandoFraseNaoExiste()
    {
        // Arrange
        var frase = new FraseInesquecivel { Id = 99 };
        _serviceMock.Setup(s => s.AtualizarFrase(frase)).ReturnsAsync((FraseInesquecivel?)null);

        // Act
        var result = await _controller.Put(99, frase);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    #endregion

    #region DELETE Method

    [Fact]
    public async Task Delete_DeveRetornarNoContent_QuandoRemovidoComSucesso()
    {
        // Arrange
        var fraseRemovida = new FraseInesquecivel { Id = 1 };
        _serviceMock.Setup(s => s.RemoverFrase(1)).ReturnsAsync(fraseRemovida);

        // Act
        var result = await _controller.Delete(1);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Delete_DeveRetornarNotFound_QuandoIdNaoExiste()
    {
        // Arrange
        _serviceMock.Setup(s => s.RemoverFrase(99)).ReturnsAsync((FraseInesquecivel?)null);

        // Act
        var result = await _controller.Delete(99);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    #endregion
}
