using BibliotecaRHC.Models.Entities;
using BibliotecaRHC.Models.Interfaces;

namespace BibliotecaRHC.Data.Repositories;

public class LivroRepository : ILivroRepository
{
    private readonly List<Livro> _livros = [];

    public IEnumerable<Livro> ObterTodos() => _livros;

    public Livro ObterPorId(int id) 
    {
        return _livros.FirstOrDefault(p => p.Id == id) ?? throw new InvalidOperationException("Livro não encontrado.");
    }

    public void Adicionar(Livro livro) => _livros.Add(livro);

    public void Atualizar(Livro livro)
    {
        var existente = ObterPorId(livro.Id);
        if (existente != null)
        {
            existente.Autor = livro.Autor;
            existente.NomeDoLivro = livro.NomeDoLivro;
            existente.AnoDePublicacao = livro.AnoDePublicacao;
            existente.NumeroDePaginas = livro.NumeroDePaginas;
            existente.ClassificacaoCatalografica = livro.ClassificacaoCatalografica;
            existente.Observacao = livro.Observacao;
            existente.DataDeAquisicao = livro.DataDeAquisicao;
        }
    }

    public void Excluir(int id) => _livros.RemoveAll(p => p.Id == id);
}
