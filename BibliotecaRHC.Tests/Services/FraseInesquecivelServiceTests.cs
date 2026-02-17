using BibliotecaRHC.API.Models;
using BibliotecaRHC.API.Repositories;
using BibliotecaRHC.API.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace BibliotecaRHC.Tests.Services;

public class FrasesInesqueciveisServiceTests
{
    private readonly Mock<IUnityOfWork> _mockUow;
    private readonly Mock<ILogger<FrasesInesqueciveisService>> _mockLogger;
    private readonly Mock<IFrasesInesqueciveisRepository> _MockRepo; 

    // O Service sendo testado
    private readonly FrasesInesqueciveisService _service;

    public FrasesInesqueciveisServiceTests()
    {
        _mockUow = new Mock<IUnityOfWork>();
        _mockLogger = new Mock<ILogger<FrasesInesqueciveisService>>();
        _MockRepo = new Mock<IFrasesInesqueciveisRepository>(); 

        _mockUow.Setup(u => u.FrasesInesqueciveisRepository).Returns(_MockRepo.Object);
        
        _service = new FrasesInesqueciveisService(_mockUow.Object, _mockLogger.Object);
    }

    #region ObterTodasAsFrases

    [Fact]
    public async Task ObterTodasAsFrases_DeveRetornarLista_QuandoExistemFrases()
    {
        // Arrange
        var listaFrases = new List<FraseInesquecivel>
        {
            new() { Id = 1, Frase = "Teste 1" },
            new() { Id = 2, Frase = "Teste 2" }
        };

        _MockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(listaFrases);

        // Act
        var resultado = await _service.ObterTodasAsFrases();

        // Assert
        Assert.NotEmpty(resultado);
        Assert.Equal(2, resultado.Count());
    }

    [Fact]
    public async Task ObterTodasAsFrases_DeveRetornarVazio_QuandoNaoExistemFrases()
    {
        // Arrange
        _MockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<FraseInesquecivel>());

        // Act
        var resultado = await _service.ObterTodasAsFrases();

        // Assert
        Assert.Empty(resultado);
    }

    #endregion

    #region ObterFrasePorId

    [Fact]
    public async Task ObterFrasePorId_DeveRetornarFrase_QuandoEncontrada()
    {
        // Arrange
        var fraseEsperada = new FraseInesquecivel { Id = 1, Frase = "Achou!" };
        _MockRepo.Setup(r => r.GetByIDAsync(1)).ReturnsAsync(fraseEsperada);

        // Act
        var resultado = await _service.ObterFrasePorId(1);

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal(fraseEsperada.Frase, resultado!.Frase);
    }

    [Fact]
    public async Task ObterFrasePorId_DeveRetornarNull_QuandoNaoEncontrada()
    {
        // Arrange
        _MockRepo.Setup(r => r.GetByIDAsync(99)).ReturnsAsync((FraseInesquecivel?)null);

        // Act
        var resultado = await _service.ObterFrasePorId(99);

        // Assert
        Assert.Null(resultado);
    }

    #endregion

    #region AdicionarFrase e Formatação Logic

    [Theory]
    [InlineData("\"Frase com aspas duplas\"", "Frase com aspas duplas")]
    [InlineData("'Frase com aspas simples'", "Frase com aspas simples")]
    [InlineData("' Frase com espacos '", "Frase com espacos")] 
    public async Task AdicionarFrase_DeveLimparFormatacao_ESalvar(string entrada, string esperado)
    {
        // Arrange
        var novaFrase = new FraseInesquecivel { Frase = entrada, Autor = "Autor Teste" };

        // Act
        var resultado = await _service.AdicionarFrase(novaFrase);

        // Assert
        _MockRepo.Verify(r => r.Add(It.Is<FraseInesquecivel>(f => f.Frase == esperado)), Times.Once);
        _mockUow.Verify(u => u.CommitAsync(), Times.Once);
    }

    #endregion

    #region AtualizarFrase

    [Fact]
    public async Task AtualizarFrase_DeveAtualizar_QuandoFraseExiste()
    {
        // Arrange
        var fraseExistente = new FraseInesquecivel { Id = 1, Frase = "Antiga", Autor = "Antigo" };
        var fraseAtualizadaInput = new FraseInesquecivel { Id = 1, Frase = "Nova", Autor = "Novo" };

        _MockRepo.Setup(r => r.GetByIDAsync(1)).ReturnsAsync(fraseExistente);

        // Act
        var resultado = await _service.AtualizarFrase(fraseAtualizadaInput);

        // Assert
        Assert.NotNull(resultado);
        _MockRepo.Verify(r => r.Update(fraseAtualizadaInput), Times.Once);
        _mockUow.Verify(u => u.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task AtualizarFrase_DeveRetornarNull_QuandoFraseNaoExiste()
    {
        // Arrange
        var fraseParaAtualizar = new FraseInesquecivel { Id = 99 };
        _MockRepo.Setup(r => r.GetByIDAsync(99)).ReturnsAsync((FraseInesquecivel?)null);

        // Act
        var resultado = await _service.AtualizarFrase(fraseParaAtualizar);

        // Assert
        Assert.Null(resultado);
        _MockRepo.Verify(r => r.Update(It.IsAny<FraseInesquecivel>()), Times.Never);
        _mockUow.Verify(u => u.CommitAsync(), Times.Never); 
    }

    #endregion

    #region RemoverFrase

    [Fact]
    public async Task RemoverFrase_DeveRemover_QuandoFraseExiste()
    {
        // Arrange
        var fraseExistente = new FraseInesquecivel { Id = 1 };
        _MockRepo.Setup(r => r.GetByIDAsync(1)).ReturnsAsync(fraseExistente);

        // Act
        var resultado = await _service.RemoverFrase(1);

        // Assert
        Assert.NotNull(resultado);
        _MockRepo.Verify(r => r.Remove(fraseExistente), Times.Once);
        _mockUow.Verify(u => u.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task RemoverFrase_DeveRetornarNull_QuandoFraseNaoExiste()
    {
        // Arrange
        _MockRepo.Setup(r => r.GetByIDAsync(99)).ReturnsAsync((FraseInesquecivel?)null);

        // Act
        var resultado = await _service.RemoverFrase(99);

        // Assert
        Assert.Null(resultado);
        _MockRepo.Verify(r => r.Remove(It.IsAny<FraseInesquecivel>()), Times.Never);
        _mockUow.Verify(u => u.CommitAsync(), Times.Never);
    }

    #endregion

    #region ObterFraseAleatoria

    [Fact]
    public async Task ObterFraseAleatoria_DeveRetornarFrasePadrao_QuandoBancoVazio()
    {
        // Arrange
        _MockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<FraseInesquecivel>());

        // Act
        var resultado = await _service.ObterFraseAleatoria();

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal("George R. R. Martin", resultado!.Autor); 
    }

    [Fact]
    public async Task ObterFraseAleatoria_DeveRetornarUmaDasFrasesDoBanco_QuandoExistemDados()
    {
        // Arrange
        var lista = new List<FraseInesquecivel>
        {
            new() { Id = 1, Frase = "A" },
            new() { Id = 2, Frase = "B" },
            new() { Id = 3, Frase = "C" }
        };
        _MockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(lista);

        // Act
        var resultado = await _service.ObterFraseAleatoria();

        // Assert
        Assert.NotNull(resultado);
        Assert.Contains(lista, f => f.Frase == resultado!.Frase); 
    }

    #endregion
}
