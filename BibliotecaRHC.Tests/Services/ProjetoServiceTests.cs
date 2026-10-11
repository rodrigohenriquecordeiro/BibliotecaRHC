using BibliotecaRHC.API.Models;
using BibliotecaRHC.API.Repositories;
using BibliotecaRHC.API.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace BibliotecaRHC.Tests.Services;

public class ProjetoServiceTests
{
    private readonly Mock<IUnityOfWork> _mockUow;
    private readonly Mock<IProjetoRepository> _mockProjetoRepo;
    private readonly Mock<IHistoricoProjetoRepository> _mockHistoricoRepo;
    private readonly ProjetoService _service;

    public ProjetoServiceTests()
    {
        _mockUow = new Mock<IUnityOfWork>();
        _mockProjetoRepo = new Mock<IProjetoRepository>();
        _mockHistoricoRepo = new Mock<IHistoricoProjetoRepository>();

        _mockUow.Setup(u => u.ProjetoRepository).Returns(_mockProjetoRepo.Object);
        _mockUow.Setup(u => u.HistoricoProjetoRepository).Returns(_mockHistoricoRepo.Object);

        var mockLogger = new Mock<ILogger<ProjetoService>>();
        _service = new ProjetoService(_mockUow.Object, mockLogger.Object);
    }

    [Fact]
    public async Task ObterTodosOsProjetos_DeveRetornarListaQuandoExistir()
    {
        var lista = new List<Projeto> { new() { Id = 1, Nome = "P1" } };
        _mockProjetoRepo.Setup(r => r.ObterTodosProjetosCompletos()).ReturnsAsync(lista);

        var resultado = await _service.ObterTodosOsProjetos();

        Assert.Single(resultado);
    }

    [Fact]
    public async Task ObterTodosOsProjetos_DeveRetornarVazio_QuandoSemDados()
    {
        _mockProjetoRepo.Setup(r => r.ObterTodosProjetosCompletos()).ReturnsAsync(new List<Projeto>());

        var resultado = await _service.ObterTodosOsProjetos();

        Assert.Empty(resultado);
    }

    [Fact]
    public async Task ObterProjetoPorId_DeveRetornarProjeto_QuandoExiste()
    {
        var projeto = new Projeto { Id = 1, Nome = "P1" };
        _mockProjetoRepo.Setup(r => r.ObterPorIdProjeto(1)).ReturnsAsync(projeto);

        var resultado = await _service.ObterProjetoPorId(1);

        Assert.NotNull(resultado);
        Assert.Equal(1, resultado!.Id);
    }

    [Fact]
    public async Task ObterProjetoPorId_DeveRetornarNull_QuandoNaoExiste()
    {
        _mockProjetoRepo.Setup(r => r.ObterPorIdProjeto(999)).ReturnsAsync((Projeto?)null);

        var resultado = await _service.ObterProjetoPorId(999);

        Assert.Null(resultado);
    }

    [Fact]
    public async Task AdicionarProjeto_DeveAdicionarHistoricoECommit()
    {
        var projeto = new Projeto { Id = 0, Nome = "Novo" };
        _mockProjetoRepo.Setup(r => r.Add(It.IsAny<Projeto>())).Verifiable();
        _mockUow.Setup(u => u.CommitAsync()).ReturnsAsync(1);

        var resultado = await _service.AdicionarProjeto(projeto);

        Assert.NotNull(resultado.DataCriacao);
        Assert.NotEmpty(resultado.HistoricoProjetos);
        _mockProjetoRepo.Verify(r => r.Add(It.IsAny<Projeto>()), Times.Once);
        _mockUow.Verify(u => u.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task AtualizarProjeto_DeveAtualizarEChamarHistoricoECommit_QuandoExiste()
    {
        var projetoNoBanco = new Projeto { Id = 1, Nome = "P1", LivroProjetos = [new() { Id = 1, Nome = "L1" }], HistoricoProjetos = [] };
        var projetoAtualizado = new Projeto { Id = 1, Nome = "P1-Atualizado", LivroProjetos = [new() { Id = 1, Nome = "L1-Atualizado" }], HistoricoProjetos = [] };

        _mockProjetoRepo.Setup(r => r.ObterPorIdProjeto(1)).ReturnsAsync(projetoNoBanco);
        _mockHistoricoRepo.Setup(r => r.AtualizarHistorico(It.IsAny<Projeto>())).Returns(Task.CompletedTask).Verifiable();
        _mockUow.Setup(u => u.CommitAsync()).ReturnsAsync(1);

        var resultado = await _service.AtualizarProjeto(projetoAtualizado);

        Assert.NotNull(resultado);
        _mockHistoricoRepo.Verify(r => r.AtualizarHistorico(It.IsAny<Projeto>()), Times.Once);
        _mockUow.Verify(u => u.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task AtualizarProjeto_DeveRetornarNull_QuandoNaoExistir()
    {
        var projeto = new Projeto { Id = 999, Nome = "NaoExiste" };
        _mockProjetoRepo.Setup(r => r.ObterPorIdProjeto(999)).ReturnsAsync((Projeto?)null);

        var resultado = await _service.AtualizarProjeto(projeto);

        Assert.Null(resultado);
    }

    [Fact]
    public async Task RemoverProjeto_DeveRemoverECommit_QuandoExiste()
    {
        var projeto = new Projeto { Id = 1, Nome = "P1" };
        _mockProjetoRepo.Setup(r => r.ObterPorIdProjeto(1)).ReturnsAsync(projeto);
        _mockProjetoRepo.Setup(r => r.Remove(It.IsAny<Projeto>())).Verifiable();
        _mockUow.Setup(u => u.CommitAsync()).ReturnsAsync(1);

        var resultado = await _service.RemoverProjeto(1);

        Assert.NotNull(resultado);
        _mockProjetoRepo.Verify(r => r.Remove(It.IsAny<Projeto>()), Times.Once);
        _mockUow.Verify(u => u.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task RemoverProjeto_DeveRetornarNull_QuandoNaoExiste()
    {
        _mockProjetoRepo.Setup(r => r.ObterPorIdProjeto(999)).ReturnsAsync((Projeto?)null);

        var resultado = await _service.RemoverProjeto(999);

        Assert.Null(resultado);
    }
}
