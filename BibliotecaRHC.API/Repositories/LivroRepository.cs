using BibliotecaRHC.API.Context;
using BibliotecaRHC.API.Models;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaRHC.API.Repositories;

public class LivroRepository : Repository<Livro>, ILivroRepository
{
    public LivroRepository(AppDbContext context) : base(context) { }

    public async Task<int> ObterCodigoUltimoLivroAsync()
    {
        var ultimoLivro = await _context.Livro!
            .OrderByDescending(l => l.Id)
            .FirstOrDefaultAsync();

        return ultimoLivro?.Id ?? 0;
    }

    public async Task<IEnumerable<Livro>> ObterLivroCompleto()
    {
        return await _context.Livro! 
            .AsNoTracking() 
            .Include(l => l.Autor)
            .Include(l => l.FrasesInesqueciveis)
            .ToListAsync();
    }

    public async Task<IEnumerable<Livro>> ObterLivroPorID(int livroID)
    {
        return await _context.Livro!
            .AsNoTracking()
            .Include(l => l.Autor)
            .Include(l => l.FrasesInesqueciveis)
            .Where(l => l.Id == livroID)
            .ToListAsync();
    }
}

public interface ILivroRepository : IRepository<Livro>
{
    Task<int> ObterCodigoUltimoLivroAsync();

    Task<IEnumerable<Livro>> ObterLivroCompleto();

    Task<IEnumerable<Livro>> ObterLivroPorID(int livroId);
}