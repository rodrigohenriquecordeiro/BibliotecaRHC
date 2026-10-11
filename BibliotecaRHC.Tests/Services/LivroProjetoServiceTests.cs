using BibliotecaRHC.API.Models;
using BibliotecaRHC.API.Repositories;
using BibliotecaRHC.API.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace BibliotecaRHC.Tests.Services;

public class LivroProjetoServiceTests
{
    private readonly Mock<IUnityOfWork> _mockUow;
    private readonly Mock<ILivroProjetoRepository> _mockRepo;
    private readonly LivroProjetoService _service;

    public LivroProjetoServiceTests()
    {
        _mockUow = new Mock<IUnityOfWork>();
        _mockRepo = new Mock<ILivroProjetoRepository>();
        _mockUow.Setup(u => u.LivroProjetoRepository).Returns(_mockRepo.Object);

        var mockLogger = new Mock<ILogger<LivroProjetoService>>();
        _service = new LivroProjetoService(_mockUow.Object, mockLogger.Object);
    }

    [Fact]
    public async Task ObterTodosOsLivrosDoProjeto_DeveRetornarListaQuandoExistir()
    {
        var lista = new List<LivroProjeto> { new() { Id = 1, Nome = "LP1" } };
        _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(lista);

        var resultado = await _service.ObterTodosOsLivrosDoProjeto();

        Assert.Single(resultado);
        _mockRepo.Verify(r => r.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task ObterTodosOsLivrosDoProjeto_DeveRetornarVazio_QuandoSemDados()
    {
        _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<LivroProjeto>());

        var resultado = await _service.ObterTodosOsLivrosDoProjeto();

        Assert.Empty(resultado);
    }

    [Fact]
    public async Task ObterLivroDoProjetoPorId_DeveRetornarEntidade_QuandoExiste()
    {
        var entidade = new LivroProjeto { Id = 1, Nome = "LP1" };
        _mockRepo.Setup(r => r.GetByIDAsync(1)).ReturnsAsync(entidade);

        var resultado = await _service.ObterLivroDoProjetoPorId(1);

        Assert.NotNull(resultado);
        Assert.Equal(1, resultado!.Id);
    }

    [Fact]
    public async Task ObterLivroDoProjetoPorId_DeveRetornarNull_QuandoNaoExiste()
    {
        _mockRepo.Setup(r => r.GetByIDAsync(999)).ReturnsAsync((LivroProjeto?)null);

        var resultado = await _service.ObterLivroDoProjetoPorId(999);

        Assert.Null(resultado);
    }

    [Fact]
    public async Task AdicionarLivroNoProjeto_DeveChamarAddECommit()
    {
        var entidade = new LivroProjeto { Id = 10, Nome = "Novo" };
        _mockRepo.Setup(r => r.Add(It.IsAny<LivroProjeto>())).Verifiable();
        _mockUow.Setup(u => u.CommitAsync()).ReturnsAsync(1);

        var resultado = await _service.AdicionarLivroNoProjeto(entidade);

        Assert.Equal(entidade, resultado);
        _mockRepo.Verify(r => r.Add(It.IsAny<LivroProjeto>()), Times.Once);
        _mockUow.Verify(u => u.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task AtualizarLivroNoProjeto_DeveAtualizar_QuandoExiste()
    {
        var entidade = new LivroProjeto { Id = 1, Nome = "Existente" };
        _mockRepo.Setup(r => r.GetByIDAsync(1)).ReturnsAsync(entidade);
        _mockRepo.Setup(r => r.Update(It.IsAny<LivroProjeto>())).Verifiable();
        _mockUow.Setup(u => u.CommitAsync()).ReturnsAsync(1);

        var resultado = await _service.AtualizarLivroNoProjeto(entidade);

        Assert.NotNull(resultado);
        _mockRepo.Verify(r => r.Update(It.IsAny<LivroProjeto>()), Times.Once);
        _mockUow.Verify(u => u.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task AtualizarLivroNoProjeto_DeveRetornarNull_QuandoNaoExiste()
    {
        var entidade = new LivroProjeto { Id = 999, Nome = "NaoExiste" };
        _mockRepo.Setup(r => r.GetByIDAsync(999)).ReturnsAsync((LivroProjeto?)null);

        var resultado = await _service.AtualizarLivroNoProjeto(entidade);

        Assert.Null(resultado);
        _mockRepo.Verify(r => r.Update(It.IsAny<LivroProjeto>()), Times.Never);
    }

    [Fact]
    public async Task RemoverLivroDoProjeto_DeveRemover_QuandoExiste()
    {
        var entidade = new LivroProjeto { Id = 1, Nome = "Existente" };
        _mockRepo.Setup(r => r.GetByIDAsync(1)).ReturnsAsync(entidade);
        _mockRepo.Setup(r => r.Remove(It.IsAny<LivroProjeto>())).Verifiable();
        _mockUow.Setup(u => u.CommitAsync()).ReturnsAsync(1);

        var resultado = await _service.RemoverLivroDoProjeto(1);

        Assert.NotNull(resultado);
        _mockRepo.Verify(r => r.Remove(It.IsAny<LivroProjeto>()), Times.Once);
        _mockUow.Verify(u => u.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task RemoverLivroDoProjeto_DeveRetornarNull_QuandoNaoExiste()
    {
        _mockRepo.Setup(r => r.GetByIDAsync(999)).ReturnsAsync((LivroProjeto?)null);

        var resultado = await _service.RemoverLivroDoProjeto(999);

        Assert.Null(resultado);
    }
}
