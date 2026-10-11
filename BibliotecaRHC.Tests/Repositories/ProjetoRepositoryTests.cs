using BibliotecaRHC.API.Context;
using BibliotecaRHC.API.Models;
using BibliotecaRHC.API.Repositories;

namespace BibliotecaRHC.Tests.Repositories;

public class ProjetoRepositoryTests : BaseRepositoryTests<Projeto>
{
    protected override IRepository<Projeto> CriarRepositorio(AppDbContext context)
    {
        return new ProjetoRepository(context);
    }

    protected override List<Projeto> ObterListaPadrao()
    {
        return
        [
            new Projeto { Id = 1, Nome = "P1", ProjetoStatus = ProjetoStatus.Andamento, LivroProjetos = [new LivroProjeto { Id = 1, Nome = "LP1", ProjetoId = 1 }], HistoricoProjetos = [new HistoricoProjeto { Id = 1, ProjetoId = 1, ProjetoStatus = ProjetoStatus.Andamento }] },
            new Projeto { Id = 2, Nome = "P2", ProjetoStatus = ProjetoStatus.Finalizado, LivroProjetos = [new LivroProjeto { Id = 2, Nome = "LP2", ProjetoId = 2 }], HistoricoProjetos = [new HistoricoProjeto { Id = 2, ProjetoId = 2, ProjetoStatus = ProjetoStatus.Finalizado }] },
            new Projeto { Id = 3, Nome = "P3", ProjetoStatus = ProjetoStatus.Andamento, LivroProjetos = [new LivroProjeto { Id = 3, Nome = "LP3", ProjetoId = 3 }], HistoricoProjetos = [new HistoricoProjeto { Id = 3, ProjetoId = 3, ProjetoStatus = ProjetoStatus.Andamento }] }
        ];
    }

    protected override Projeto CriarNovaEntidade()
    {
        return new Projeto { Id = 999, Nome = "Projeto Novo", ProjetoStatus = ProjetoStatus.NaoIniciado };
    }

    protected override void AlterarEntidadeParaUpdate(Projeto entity) => entity.Nome = "Projeto Atualizado";
    
    protected override int ObterIdValido() => 1;

    protected override int ObterIdInvalido() => 9999;

    [Fact]
    public async Task ObterTodosProjetosCompletos_DeveRetornarProjetosComColecoes()
    {
        // Arrange
        var (repo, context) = CriarRepositorioComContexto();

        var projetoRepo = repo as ProjetoRepository;

        // Act
        var resultado = await projetoRepo!.ObterTodosProjetosCompletos();

        // Assert
        Assert.All(resultado, p => Assert.NotNull(p.LivroProjetos));
        Assert.All(resultado, p => Assert.NotNull(p.HistoricoProjetos));
    }

    [Fact]
    public async Task ObterPorIdProjeto_DeveRetornarProjetoCompleto()
    {
        // Arrange
        var (repo, context) = CriarRepositorioComContexto();
        var projetoRepo = repo as ProjetoRepository;

        // Act
        var projeto = await projetoRepo!.ObterPorIdProjeto(1);

        // Assert
        Assert.NotNull(projeto);
        Assert.NotNull(projeto!.LivroProjetos);
        Assert.NotNull(projeto.HistoricoProjetos);
    }
}
