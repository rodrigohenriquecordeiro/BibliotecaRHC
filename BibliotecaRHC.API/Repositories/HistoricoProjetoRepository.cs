using BibliotecaRHC.API.Context;
using BibliotecaRHC.API.Models;

namespace BibliotecaRHC.API.Repositories;

public class HistoricoProjetoRepository : Repository<HistoricoProjeto>, IHistoricoProjetoRepository
{
    public HistoricoProjetoRepository(AppDbContext context) : base(context) { }

    public async Task AtualizarHistorico(Projeto projeto)
    {
        var historico = new HistoricoProjeto
        {
            Id = 0,
            ProjetoId = projeto.Id,
            DataAlteracao = DateTime.Now,
            ProjetoStatus = projeto.ProjetoStatus
        };

        await _context.HistoricoProjeto.AddAsync(historico);
        await _context.SaveChangesAsync();
    }
}

public interface IHistoricoProjetoRepository : IRepository<HistoricoProjeto> 
{
    Task AtualizarHistorico(Projeto projeto);
}
