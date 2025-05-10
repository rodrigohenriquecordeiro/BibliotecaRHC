using BibliotecaRHC.API.Context;
using BibliotecaRHC.API.Models;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaRHC.API.Repositories;

public class LivroRepository : Repository<Livro>, ILivroRepository
{
    IUnityOfWork _unityOfWork;

    public LivroRepository(AppDbContext context, IUnityOfWork unityOfWork) : base(context)
    {
        _unityOfWork = unityOfWork;
    }

    public async Task<int> ObterCodigoUltimoLivro()
    {
        var ultimoLivro = await _context.Livros!.OrderByDescending(l => l.Id).FirstOrDefaultAsync();

        if (ultimoLivro != null)
        {
            Console.WriteLine($"O último livro cadastrado tem o ID: {ultimoLivro.Id}");
            return ultimoLivro.Id;
        }
        else
        {
            Console.WriteLine("Nenhum livro cadastrado.");
            return 0;
        }
    }
}

public interface ILivroRepository : IRepository<Livro>
{
    Task<int> ObterCodigoUltimoLivro();
}