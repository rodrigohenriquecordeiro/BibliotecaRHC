
namespace BibliotecaRHC.API.Repositories;

public interface IUnityOfWork : IDisposable
{
    ILivroRepository LivroRepository { get; }
    IFrasesInesqueciveisRepository FrasesInesqueciveisRepository { get; }
    IProjetoRepository ProjetoRepository { get; }
    ILivroProjetoRepository LivroProjetoRepository { get; }
    IHistoricoProjetoRepository HistoricoProjetoRepository { get; }

    Task<int> CommitAsync();
    void LimparRastreador();
}
