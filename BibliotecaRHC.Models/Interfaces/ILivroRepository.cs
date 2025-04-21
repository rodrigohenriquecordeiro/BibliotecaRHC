using BibliotecaRHC.Models.Entities;

namespace BibliotecaRHC.Models.Interfaces;

public interface ILivroRepository
{
    Task<IEnumerable<Livro>> ObterTodos(); 

    Task<Livro> ObterPorId(int id);

    Task Adicionar(Livro livro);

    Task Atualizar(Livro livro);

    Task Excluir(int id);
}
