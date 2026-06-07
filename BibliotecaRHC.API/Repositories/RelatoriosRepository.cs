using BibliotecaRHC.API.Context;
using BibliotecaRHC.API.Models;
using Microsoft.EntityFrameworkCore;
using static BibliotecaRHC.API.Models.Relatorios;

namespace BibliotecaRHC.API.Repositories;

public class RelatoriosRepository : Repository<Relatorios>, IRelatoriosRepository
{
    public RelatoriosRepository(AppDbContext context) : base(context) { }

    public async Task<Relatorios> ObterQuantidades()
    {
        var totalLivros = await _context.Set<Livro>().CountAsync();
        var lidos = await _context.Set<Livro>().CountAsync(l => l.Lido);

        return new Relatorios
        {
            TotalLivros = totalLivros,
            LivrosLidos = lidos,
            LivrosNaoLidos = totalLivros - lidos,
            TotalEditoras = await _context.Set<Livro>().Select(l => l.Editora).Distinct().CountAsync(),
            TotalAutores = await _context.Set<Livro>().Select(l => l.Autor).Distinct().CountAsync(),
            ProjetosLeituraAndamento = await _context.Set<Projeto>().CountAsync(p => p.ProjetoStatus == ProjetoStatus.Andamento),

            TopClassificacoes = await _context.Set<Livro>()
                .Where(l => !string.IsNullOrEmpty(l.ClassificacaoCatalografica))
                .GroupBy(l => l.ClassificacaoCatalografica!)
                .OrderByDescending(g => g.Count()).Take(10)
                .Select(g => new Top10Item { Descricao = g.Key, Quantidade = g.Count() }).ToListAsync(),

            TopAutores = await _context.Set<Livro>()
                .GroupBy(l => l.Autor!)
                .OrderByDescending(g => g.Count()).Take(10)
                .Select(g => new Top10Item { Descricao = g.Key, Quantidade = g.Count() }).ToListAsync(),

            TopEditoras = await _context.Set<Livro>()
                .GroupBy(l => l.Editora!)
                .OrderByDescending(g => g.Count()).Take(10)
                .Select(g => new Top10Item { Descricao = g.Key, Quantidade = g.Count() }).ToListAsync(),

            TopAnosPublicacao = await _context.Set<Livro>()
                .GroupBy(l => l.AnoDePublicacao!) 
                .OrderByDescending(g => g.Count())
                .Take(10)
                .Select(g => new Top10Item
                {
                    Descricao = g.Key.ToString(),
                    Quantidade = g.Count()
                })
                .ToListAsync(),

            TopAnosAquisicao = await _context.Set<Livro>()
                .GroupBy(l => l.DataDeAquisicao!.Value.Year) 
                .OrderByDescending(g => g.Count())
                .Take(10)
                .Select(g => new Top10Item
                {
                    Descricao = g.Key.ToString(),
                    Quantidade = g.Count()
                })
                .ToListAsync(),

            TopLivrosLongos = await _context.Set<Livro>()
                .OrderByDescending(l => l.NumeroDePaginas!).Take(10)
                .Select(l => new Top10LivroLongo { NomeDoLivro = l.NomeDoLivro!, NumeroDePaginas = (int)l.NumeroDePaginas! }).ToListAsync()
        };
    }
}

public interface IRelatoriosRepository : IRepository<Relatorios>
{
    Task<Relatorios> ObterQuantidades();
}
