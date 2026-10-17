using BibliotecaRHC.API.Context;
using BibliotecaRHC.API.Models;
using BibliotecaRHC.API.Repositories;
using BibliotecaRHC.Tests;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaRHC.Tests.Repositories;

public class HistoricoProjetoRepositoryTests : BaseRepositoryTests<HistoricoProjeto>
{
    protected override IRepository<HistoricoProjeto> CriarRepositorio(AppDbContext context) => new HistoricoProjetoRepository(context);
    
    protected override List<HistoricoProjeto> ObterListaPadrao()
    {
        return
        [
            new() { Id = 1, ProjetoId = 1, DataAlteracao = DateTime.Now, ProjetoStatus = ProjetoStatus.Andamento },
            new() { Id = 2, ProjetoId = 1, DataAlteracao = DateTime.Now, ProjetoStatus = ProjetoStatus.Finalizado },
            new() { Id = 3, ProjetoId = 2, DataAlteracao = DateTime.Now, ProjetoStatus = ProjetoStatus.Andamento }
        ];
    }

    protected override HistoricoProjeto CriarNovaEntidade()
    {
        return new HistoricoProjeto { Id = 999, ProjetoId = 5, DataAlteracao = DateTime.Now, ProjetoStatus = ProjetoStatus.Andamento };
    }

    protected override void AlterarEntidadeParaUpdate(HistoricoProjeto entity) => entity.ProjetoStatus = ProjetoStatus.Finalizado;
    
    protected override int ObterIdValido() => 1;

    protected override int ObterIdInvalido() => 9999;

    [Fact]
    public async Task AtualizarHistorico_DeveAdicionarHistoricoParaProjeto()
    {
        // Arrange
        var nomeBanco = Guid.NewGuid().ToString();
        await using var context = TestDbContextFactory.Create(nomeBanco);
        var projeto = new Projeto { Id = 10, Nome = "Projeto Teste", DataCriacao = DateTime.Now, ProjetoStatus = ProjetoStatus.Andamento };
        context.Set<Projeto>().Add(projeto);
        await context.SaveChangesAsync();

        var repo = new HistoricoProjetoRepository(context);

        // Act
        await repo.AtualizarHistorico(projeto);

        // Assert
        var historicos = await context.Set<HistoricoProjeto>().Where(h => h.ProjetoId == projeto.Id).ToListAsync();
        Assert.Single(historicos);
        Assert.Equal(projeto.ProjetoStatus, historicos.First().ProjetoStatus);
    }
}
