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

    public async Task<IEnumerable<Livro>> ObterLivrosFiltrados(string campo, string valor)
    {
        var query = _context.Livro!.AsQueryable();

        query = FormataString(campo) switch
        {
            "autor" => query.Where(x => x.Autor!.Contains(FormataString(valor))),
            "nomedolivro" => query.Where(x => x.NomeDoLivro!.Contains(FormataString(valor))),
            "editora" => query.Where(x => x.Editora!.Contains(FormataString(valor))),
            "anodepublicacao" => query.Where(x => x.AnoDePublicacao!.Equals(FormataString(valor))),
            "numerodepaginas" => query.Where(x => x.NumeroDePaginas == int.Parse(valor.Trim())),
            "classificacaocatalografica" => query.Where(x => x.ClassificacaoCatalografica!.Equals(FormataString(valor))),
            "observacao" => query.Where(x => x.Observacao!.Contains(FormataString(valor))),
            "datadeaquisicao" => query.Where(x => x.DataDeAquisicao == DateTime.Parse(valor.Trim())),
            "lido" => query.Where(x => x.Lido == bool.Parse(valor.Trim())),
            "anoultimaleitura" => query.Where(x => x.AnoUltimaLeitura == int.Parse(valor.Trim())),
            _ => query
        };

        return await query.OrderBy(x => x.Id).ToListAsync();
    }

    private static string FormataString(string valor) => valor.Trim().ToLower();
}

public interface ILivroRepository : IRepository<Livro>
{
    Task<int> ObterCodigoUltimoLivroAsync();
    Task<IEnumerable<Livro>> ObterLivrosPaginados(int paginaAtual, int tamanhoPagina);
    Task<IEnumerable<Livro>> ObterLivrosFiltrados(string campo, string valor);
}