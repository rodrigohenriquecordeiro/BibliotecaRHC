using BibliotecaRHC.Models;
using BibliotecaRHC.Data.Repositories;

namespace BibliotecaRHC.Services;

public class LivroService : ILivroService
{
    private readonly ILivroRepository _repository;

    public LivroService(ILivroRepository repository) => _repository = repository;

    public async Task<IEnumerable<Livro>> ObterLivros() => await _repository.ObterTodos();

    public async Task<Livro> ObterLivroPorId(int id) => await _repository.ObterPorId(id);

    public async Task AdicionarLivro(Livro livro) => await _repository.Adicionar(livro);

    public async Task AtualizarLivro(Livro livro)
    {
        var existente = await _repository.ObterPorId(livro.Id);
        if (existente != null)
        {
            existente.Autor = livro.Autor;
            existente.NomeDoLivro = livro.NomeDoLivro;
            existente.AnoDePublicacao = livro.AnoDePublicacao;
            existente.NumeroDePaginas = livro.NumeroDePaginas;
            existente.ClassificacaoCatalografica = livro.ClassificacaoCatalografica;
            existente.Observacao = livro.Observacao;
            existente.DataDeAquisicao = livro.DataDeAquisicao;

            await _repository.Atualizar(livro);
        }
    }

    public async Task RemoverLivro(int id) => await _repository.Excluir(id);
}

public interface ILivroService
{
    Task<IEnumerable<Livro>> ObterLivros();

    Task<Livro> ObterLivroPorId(int id);

    Task AdicionarLivro(Livro livro);

    Task AtualizarLivro(Livro livro);

    Task RemoverLivro(int id);
}
