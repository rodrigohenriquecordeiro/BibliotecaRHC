using BibliotecaRHC.API.Context;
using BibliotecaRHC.API.Models;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaRHC.API.Repositories;

public class ProjetoRepository  : Repository<Projeto>, IProjetoRepository
{
    public ProjetoRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<Projeto>> GetAllWithLivros()
    {
        return await _context.Projeto
            .Include(p => p.LivroProjetos) 
            .ToListAsync();
    }
}

public interface IProjetoRepository : IRepository<Projeto> 
{
    Task<IEnumerable<Projeto>> GetAllWithLivros();
}
