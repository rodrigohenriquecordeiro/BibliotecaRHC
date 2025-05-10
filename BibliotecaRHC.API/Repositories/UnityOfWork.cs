using BibliotecaRHC.API.Context;

namespace BibliotecaRHC.API.Repositories;

public class UnityOfWork : IUnityOfWork
{
    public AppDbContext _context;
    private ILivroRepository? _livroRepository;

    public UnityOfWork(AppDbContext context)
    {
        _context = context;
    }

    public ILivroRepository LivroRepository
    {
        get
        {
            return _livroRepository ??= new LivroRepository(_context);
        }
    }

    public void Commit()
    {
        _context.SaveChanges();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
