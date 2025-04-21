using BibliotecaRHC.Models.Entities;
using BibliotecaRHC.Models.Interfaces;

namespace BibliotecaRHC.Services;

public class LivroService : ILivroService
{
    private readonly ILivroRepository _repository;

    public LivroService(ILivroRepository repository) => _repository = repository;

    public IEnumerable<Livro> ObterLivros() => _repository.ObterTodos();

    public Livro ObterLivroPorId(int id) => _repository.ObterPorId(id);

    public void AdicionarLivro(Livro livro)
    {
        _repository.Adicionar(livro);
    }

    public void AtualizarLivro(Livro livro)
    {
        var existente = _repository.ObterPorId(livro.Id);
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

    public void RemoverLivro(int id) => _repository.Excluir(id);
}

public interface ILivroService
{
    IEnumerable<Livro> ObterLivros();

    Livro ObterLivroPorId(int id);

    void AdicionarLivro(Livro livro);

    void AtualizarLivro(Livro livro);

    void RemoverLivro(int id);
}
