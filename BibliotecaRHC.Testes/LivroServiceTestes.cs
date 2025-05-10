using BibliotecaRHC.API.Domain;
using BibliotecaRHC.API.Models;
using BibliotecaRHC.API.Repositories;
using Microsoft.Extensions.Logging;
using Moq;

namespace BibliotecaRHC.Testes;

public class LivroServiceTests
{
    private readonly Mock<IUnityOfWork> _mockUow;
    private readonly Mock<ILivroRepository> _mockRepo;
    private readonly LivroService _service;

    public LivroServiceTests()
    {
        _mockUow = new Mock<IUnityOfWork>();
        _mockRepo = new Mock<ILivroRepository>();
        _mockUow.Setup(u => u.LivroRepository).Returns(_mockRepo.Object);

        var mockLogger = new Mock<ILogger<LivroService>>();
        _service = new LivroService(_mockUow.Object, mockLogger.Object);
    }

    [Fact]
    public async Task ObterLivros_DeveChamarRepositorio()
    {
        // Arrange
        var livros = new List<Livro> { new() { Id = 1, Autor = "Teste" } };
        _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(livros);

        // Act
        var resultado = await _service.ObterTodosOsLivros();

        // Assert
        Assert.Single(resultado);
        _mockRepo.Verify(r => r.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task ObterLivroPorId_DeveRetornarLivroCorreto()
    {
        // Arrange
        var livroEsperado = new Livro { Id = 1, Autor = "Autor Teste" };
        _mockRepo.Setup(r => r.GetByIDAsync(1)).ReturnsAsync(livroEsperado);

        // Act
        var resultado = await _service.ObterLivroPorId(1);

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal("Autor Teste", resultado!.Autor);
        _mockRepo.Verify(r => r.GetByIDAsync(1), Times.Once);
    }

    [Fact]
    public async Task ObterLivroPorId_DeveRetornarNull_QuandoNaoEncontrado()
    {
        // Arrange
        _mockRepo.Setup(r => r.GetByIDAsync(999)).ReturnsAsync((Livro?)null);

        // Act
        var resultado = await _service.ObterLivroPorId(999);

        // Assert
        Assert.Null(resultado);
        _mockRepo.Verify(r => r.GetByIDAsync(999), Times.Once);
    }

    [Fact]
    public async Task AdicionarLivro_DeveChamarAdd()
    {
        // Arrange
        var livro = new Livro { Id = 1, Autor = "Autor Novo", NomeDoLivro = "Livro Novo" };

        // Act
        _mockRepo.Setup(r => r.Add(It.IsAny<Livro>())).Verifiable();
        await _service.AdicionarLivroAsync(livro);

        // Assert
        _mockRepo.Verify(r => r.Add(It.Is<Livro>(l => l.Id == 1 && l.Autor == "Autor Novo")), Times.Once);
    }

    [Fact]
    public async Task AtualizarLivro_DeveChamarUpdate_QuandoLivroExistir()
    {
        // Arrange
        var livroExistente = new Livro { Id = 1, Autor = "Original" };
        var livroAtualizado = new Livro { Id = 1, Autor = "Atualizado" };

        _mockRepo.Setup(r => r.GetByIDAsync(1)).ReturnsAsync(livroExistente);
        _mockRepo.Setup(r => r.Update(It.IsAny<Livro>())).Verifiable();

        // Act
        await _service.AtualizarLivro(livroAtualizado);

        // Assert
        _mockRepo.Verify(r => r.Update(It.Is<Livro>(l => l.Autor == "Atualizado")), Times.Once);
    }

    [Fact]
    public async Task AtualizarLivro_NaoChamaUpdate_SeLivroNaoExistir()
    {
        // Arrange
        var livro = new Livro { Id = 1, Autor = "Teste" };
        _mockRepo.Setup(r => r.GetByIDAsync(1)).ReturnsAsync((Livro?)null);

        // Act
        await _service.AtualizarLivro(livro);

        // Assert
        _mockRepo.Verify(r => r.Update(It.IsAny<Livro>()), Times.Never);
    }

    [Fact]
    public async Task RemoverLivro_DeveChamarRemove_SeLivroExistir()
    {
        // Arrange
        var livro = new Livro { Id = 1, Autor = "Autor Teste" };
        _mockRepo.Setup(r => r.GetByIDAsync(1)).ReturnsAsync(livro);
        _mockRepo.Setup(r => r.Remove(It.IsAny<Livro>())).Verifiable();

        // Act
        await _service.RemoverLivro(livro.Id);

        // Assert
        _mockRepo.Verify(r => r.Remove(It.Is<Livro>(l => l.Id == 1)), Times.Once);
    }

    [Fact]
    public async Task RemoverLivro_NaoChamaRemove_SeLivroNaoExistir()
    {
        // Arrange
        _mockRepo.Setup(r => r.GetByIDAsync(1)).ReturnsAsync((Livro?)null);

        // Act
        await _service.RemoverLivro(1);

        // Assert
        _mockRepo.Verify(r => r.Remove(It.IsAny<Livro>()), Times.Never);
    }

    [Fact]
    public async Task GeraCodigoProximoLivro_DeveSomarUmAoUltimoId()
    {
        // Arrange
        _mockRepo.Setup(r => r.ObterCodigoUltimoLivroAsync()).ReturnsAsync(5);

        // Act
        var resultado = await _service.GeraCodigoProximoLivro();

        // Assert
        Assert.Equal(6, resultado);
    }

    [Fact]
    public async Task GeraCodigoProximoLivro_DeveRetornarUm_SeNaoExistirLivro()
    {
        // Arrange
        _mockRepo.Setup(r => r.ObterCodigoUltimoLivroAsync()).ReturnsAsync(0);

        // Act
        var resultado = await _service.GeraCodigoProximoLivro();

        // Assert
        Assert.Equal(1, resultado);
    }
}
