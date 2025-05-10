namespace BibliotecaRHC.API.Repositories;

public interface IUnityOfWork
{
    ILivroRepository LivroRepository { get; }

    void Commit();
}
