namespace BibliotecaRHC.API.Repositories;

public interface IUnityOfWork : IDisposable
{
    ILivroRepository LivroRepository { get; }

    Task<int> CommitAsync();
}
