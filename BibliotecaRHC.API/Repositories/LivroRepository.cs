using BibliotecaRHC.API.Context;
using BibliotecaRHC.API.Models;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaRHC.API.Repositories;

public class LivroRepository : Repository<Livro>, ILivroRepository
{
    public LivroRepository(AppDbContext context) : base(context) { }

    public async Task<int> ObterCodigoUltimoLivroAsync()
    {
        var ultimoLivro = await _context.Livros!
            .OrderByDescending(l => l.Id)
            .FirstOrDefaultAsync();

        return ultimoLivro?.Id ?? 0;
    }
}

public interface ILivroRepository : IRepository<Livro>
{
    Task<int> ObterCodigoUltimoLivroAsync();
}