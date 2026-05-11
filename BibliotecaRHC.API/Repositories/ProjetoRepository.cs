using BibliotecaRHC.API.Context;
using BibliotecaRHC.API.Models;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaRHC.API.Repositories;

public class ProjetoRepository  : Repository<Projeto>, IProjetoRepository
{
    public ProjetoRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<Projeto>> ObterTodosProjetosCompletos()
    {
        return await _context.Projeto
            .Include(p => p.LivroProjetos)
            .Include(h => h.HistoricoProjetos)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Projeto?> ObterPorIdProjeto(int projetoId)
    {
        return await _context.Projeto
            .Include(p => p.LivroProjetos)
            .Include(h => h.HistoricoProjetos)
            .FirstOrDefaultAsync(x => x.Id == projetoId);
    }
}

public interface IProjetoRepository : IRepository<Projeto> 
{
    Task<IEnumerable<Projeto>> ObterTodosProjetosCompletos();

    Task<Projeto?> ObterPorIdProjeto(int projetoId);
}
