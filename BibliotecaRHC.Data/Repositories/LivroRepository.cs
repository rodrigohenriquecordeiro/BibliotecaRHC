using BibliotecaRHC.Models.Entities;
using BibliotecaRHC.Models.Interfaces;

namespace BibliotecaRHC.Data.Repositories;

public class LivroRepository : ILivroRepository
{
    private readonly List<Livro> _livros = new();

    public async Task<IEnumerable<Livro>> ObterTodos() => await Task.FromResult(_livros);

    public async Task<Livro> ObterPorId(int id)
    {
        var livro = _livros.FirstOrDefault(p => p.Id == id);
        return await Task.FromResult(livro ?? throw new InvalidOperationException("Livro não encontrado."));
    }

    public async Task Adicionar(Livro livro)
    {
        await Task.Run(() => _livros.Add(livro));
    }

    public async Task Atualizar(Livro livro)
    {
        var existente = _livros.FirstOrDefault(p => p.Id == livro.Id);
        if (existente != null)
        {
            await Task.Run(() =>
            {
                existente.Autor = livro.Autor;
                existente.NomeDoLivro = livro.NomeDoLivro;
                existente.AnoDePublicacao = livro.AnoDePublicacao;
                existente.NumeroDePaginas = livro.NumeroDePaginas;
                existente.ClassificacaoCatalografica = livro.ClassificacaoCatalografica;
                existente.Observacao = livro.Observacao;
                existente.DataDeAquisicao = livro.DataDeAquisicao;
            });
        }
    }

    public async Task Excluir(int id) => await Task.Run(() => _livros.RemoveAll(p => p.Id == id));
}
