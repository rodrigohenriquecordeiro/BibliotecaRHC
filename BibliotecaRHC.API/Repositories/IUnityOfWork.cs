namespace BibliotecaRHC.API.Repositories;

public interface IUnityOfWork : IDisposable
{
    ILivroRepository LivroRepository { get; }
    IFrasesInesqueciveisRepository FrasesInesqueciveisRepository { get; }

    Task<int> CommitAsync();
}
