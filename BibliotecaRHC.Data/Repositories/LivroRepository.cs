using BibliotecaRHC.Models;
using BibliotecaRHC.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaRHC.Data.Repositories;

public class LivroRepository : ILivroRepository
{
    private readonly List<Livro> _livros = new();

    private readonly AppDbContext _context;

    public LivroRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Livro>> ObterTodos()
    {
        var livro = await _context.Set<Livro>().ToListAsync();
        return livro ?? throw new InvalidOperationException("Nenhum livro foi cadastrado ainda!");
    }

    public async Task<Livro> ObterPorId(int id)
    {
        var livro = await _context.Set<Livro>().FindAsync(id);
        return livro ?? throw new InvalidOperationException("Livro não encontrado.");
    }

    public async Task Adicionar(Livro livro)
    {
        await Task.Run(() =>
        {
            _context.Livros?.Add(livro);
            _context.SaveChanges();
        });
    }

    public async Task Atualizar(Livro livro)
    {
        if (_context.Livros == null)
        {
            throw new InvalidOperationException("O conjunto de Livros no contexto é nulo.");
        }

        var existente = await _context.Livros.FindAsync(livro.Id);
        if (existente != null)
        {
            existente.Autor = livro.Autor;
            existente.NomeDoLivro = livro.NomeDoLivro;
            existente.AnoDePublicacao = livro.AnoDePublicacao;
            existente.NumeroDePaginas = livro.NumeroDePaginas;
            existente.ClassificacaoCatalografica = livro.ClassificacaoCatalografica;
            existente.Observacao = livro.Observacao;
            existente.DataDeAquisicao = livro.DataDeAquisicao;

            await _context.SaveChangesAsync();
        }
        else
        {
            throw new InvalidOperationException("Livro não encontrado.");
        }
    }

    public async Task Excluir(int id)
    {
        if (_context.Livros == null)
        {
            throw new InvalidOperationException("O conjunto de Livros no contexto é nulo.");
        }

        var livro = await _context.Livros.FindAsync(id);
        if (livro != null)
        {
            _context.Livros.Remove(livro);
            await _context.SaveChangesAsync();
        }
        else
        {
            throw new InvalidOperationException("Livro não encontrado.");
        }
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

public interface ILivroRepository
{
    Task<IEnumerable<Livro>> ObterTodos();

    Task<Livro> ObterPorId(int id);

    Task Adicionar(Livro livro);

    Task Atualizar(Livro livro);

    Task Excluir(int id);

    Task<int> ObterCodigoUltimoLivro();
}