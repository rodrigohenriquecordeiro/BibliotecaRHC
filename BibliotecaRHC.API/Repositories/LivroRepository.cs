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

    public async Task<IEnumerable<Livro>> ObterLivrosPaginados(int paginaAtual, int tamanhoPagina)
    {
        var livros = await _context.Livro!
            .OrderBy(l => l.Id)
            .Skip((paginaAtual - 1) * tamanhoPagina)
            .Take(tamanhoPagina)
            .ToListAsync();

        return livros;
    }
}

public interface ILivroRepository : IRepository<Livro>
{
    Task<int> ObterCodigoUltimoLivroAsync();
    Task<IEnumerable<Livro>> ObterLivrosPaginados(int paginaAtual, int tamanhoPagina);
}