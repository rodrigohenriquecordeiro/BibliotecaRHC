using BibliotecaRHC.API.Context;

namespace BibliotecaRHC.API.Repositories;

public class UnityOfWork : IUnityOfWork
{
    private readonly AppDbContext _context;
    private ILivroRepository? _livroRepository;

    public UnityOfWork(AppDbContext context)
    {
        _context = context;
    }

    public ILivroRepository LivroRepository => _livroRepository ??= new LivroRepository(_context);

    public async Task<int> CommitAsync() => await _context.SaveChangesAsync();

    public void Dispose()
    {
        _context.Dispose();
        GC.SuppressFinalize(this);
    }
}
