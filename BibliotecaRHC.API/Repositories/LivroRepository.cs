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

    public async Task<IEnumerable<Livro>> ObterLivrosPaginados(
        int paginaAtual,
        int tamanhoPagina,
        string campoOrdenacao = "id",
        string direcaoOrdenacao = "desc")
    {
        var livros = await AplicarOrdenacao(_context.Livro!.AsQueryable(), campoOrdenacao, direcaoOrdenacao)
            .Skip((paginaAtual - 1) * tamanhoPagina)
            .Take(tamanhoPagina)
            .ToListAsync();

        return livros;
    }

    public async Task<IEnumerable<Livro>> ObterLivrosFiltrados(
        string campo,
        string valor,
        string campoOrdenacao = "id",
        string direcaoOrdenacao = "desc")
    {
        var query = CriarConsultaFiltrada(campo, valor);
        return await AplicarOrdenacao(query, campoOrdenacao, direcaoOrdenacao).ToListAsync();
    }

    public async Task<int> ContarLivrosFiltrados(string campo, string valor) =>
        await CriarConsultaFiltrada(campo, valor).CountAsync();

    public async Task<IEnumerable<Livro>> ObterLivrosFiltradosPaginados(
        string campo,
        string valor,
        int paginaAtual,
        int tamanhoPagina,
        string campoOrdenacao = "id",
        string direcaoOrdenacao = "desc")
    {
        var query = CriarConsultaFiltrada(campo, valor);
        return await AplicarOrdenacao(query, campoOrdenacao, direcaoOrdenacao)
            .Skip((paginaAtual - 1) * tamanhoPagina)
            .Take(tamanhoPagina)
            .ToListAsync();
    }

    private IQueryable<Livro> CriarConsultaFiltrada(string campo, string valor)
    {
        var query = _context.Livro!.AsQueryable();

        return FormataString(campo) switch
        {
            "autor" => query.Where(x => x.Autor!.Contains(FormataString(valor))),
            "nomedolivro" => query.Where(x => x.NomeDoLivro!.Contains(FormataString(valor))),
            "editora" => query.Where(x => x.Editora!.Contains(FormataString(valor))),
            "anodepublicacao" => query.Where(x => x.AnoDePublicacao!.Equals(FormataString(valor))),
            "numerodepaginas" => query.Where(x => x.NumeroDePaginas == int.Parse(valor.Trim())),
            "classificacaocatalografica" => query.Where(x => x.ClassificacaoCatalografica!.Equals(FormataString(valor))),
            "observacao" => query.Where(x => x.Observacao!.Contains(FormataString(valor))),
            "datadeaquisicao" => query.Where(x => x.DataDeAquisicao == DateTime.Parse(valor.Trim())),
            "lido" => query.Where(x => x.Lido == FormataBool(valor)),
            "anoultimaleitura" => query.Where(x => x.AnoUltimaLeitura == int.Parse(valor.Trim())),
            _ => query
        };
    }

    private static IOrderedQueryable<Livro> AplicarOrdenacao(
        IQueryable<Livro> query,
        string campoOrdenacao,
        string direcaoOrdenacao)
    {
        var campo = campoOrdenacao.Trim().ToLowerInvariant();
        var crescente = direcaoOrdenacao.Equals("asc", StringComparison.OrdinalIgnoreCase);

        return campo switch
        {
            "autor" => crescente ? query.OrderBy(x => x.Autor).ThenBy(x => x.Id) : query.OrderByDescending(x => x.Autor).ThenBy(x => x.Id),
            "nomedolivro" => crescente ? query.OrderBy(x => x.NomeDoLivro).ThenBy(x => x.Id) : query.OrderByDescending(x => x.NomeDoLivro).ThenBy(x => x.Id),
            "editora" => crescente ? query.OrderBy(x => x.Editora).ThenBy(x => x.Id) : query.OrderByDescending(x => x.Editora).ThenBy(x => x.Id),
            "numerodepaginas" => crescente ? query.OrderBy(x => x.NumeroDePaginas).ThenBy(x => x.Id) : query.OrderByDescending(x => x.NumeroDePaginas).ThenBy(x => x.Id),
            "anodepublicacao" => crescente ? query.OrderBy(x => x.AnoDePublicacao).ThenBy(x => x.Id) : query.OrderByDescending(x => x.AnoDePublicacao).ThenBy(x => x.Id),
            "datadeaquisicao" => crescente ? query.OrderBy(x => x.DataDeAquisicao).ThenBy(x => x.Id) : query.OrderByDescending(x => x.DataDeAquisicao).ThenBy(x => x.Id),
            "classificacaocatalografica" => crescente ? query.OrderBy(x => x.ClassificacaoCatalografica).ThenBy(x => x.Id) : query.OrderByDescending(x => x.ClassificacaoCatalografica).ThenBy(x => x.Id),
            "observacao" => crescente ? query.OrderBy(x => x.Observacao).ThenBy(x => x.Id) : query.OrderByDescending(x => x.Observacao).ThenBy(x => x.Id),
            "lido" => crescente ? query.OrderBy(x => x.Lido).ThenBy(x => x.Id) : query.OrderByDescending(x => x.Lido).ThenBy(x => x.Id),
            "anoultimaleitura" => crescente ? query.OrderBy(x => x.AnoUltimaLeitura).ThenBy(x => x.Id) : query.OrderByDescending(x => x.AnoUltimaLeitura).ThenBy(x => x.Id),
            _ => crescente ? query.OrderBy(x => x.Id) : query.OrderByDescending(x => x.Id)
        };
    }

    private static string FormataString(string valor) => valor.Trim().ToLower();

    private static bool FormataBool(string valor) => valor.Trim().ToLower().Equals("sim");
}

public interface ILivroRepository : IRepository<Livro>
{
    Task<int> ObterCodigoUltimoLivroAsync();
    Task<IEnumerable<Livro>> ObterLivrosPaginados(int paginaAtual, int tamanhoPagina, string campoOrdenacao = "id", string direcaoOrdenacao = "desc");
    Task<IEnumerable<Livro>> ObterLivrosFiltrados(string campo, string valor, string campoOrdenacao = "id", string direcaoOrdenacao = "desc");
    Task<int> ContarLivrosFiltrados(string campo, string valor);
    Task<IEnumerable<Livro>> ObterLivrosFiltradosPaginados(string campo, string valor, int paginaAtual, int tamanhoPagina, string campoOrdenacao = "id", string direcaoOrdenacao = "desc");
}