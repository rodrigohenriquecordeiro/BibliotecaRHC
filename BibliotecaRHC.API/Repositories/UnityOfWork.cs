using BibliotecaRHC.API.Context;

namespace BibliotecaRHC.API.Repositories;

public class UnityOfWork : IUnityOfWork
{
    private readonly AppDbContext _context;
    private ILivroRepository? _livroRepository;
    private IFrasesInesqueciveisRepository? _frasesInesqueciveisRepository;
    private IProjetoRepository? _projetoRepository;
    private ILivroProjetoRepository? _livroProjetoRepository;
    private IHistoricoProjetoRepository? _historicoProjetoRepository;

    public UnityOfWork(AppDbContext context)
    {
        _context = context;
    }

    public ILivroRepository LivroRepository => _livroRepository ??= new LivroRepository(_context);
    public IFrasesInesqueciveisRepository FrasesInesqueciveisRepository => _frasesInesqueciveisRepository ??= new FraseInesquecivelRepository(_context);
    public IProjetoRepository ProjetoRepository =>  _projetoRepository ??= new ProjetoRepository(_context);
    public ILivroProjetoRepository LivroProjetoRepository => _livroProjetoRepository ??= new LivroProjetoRepository(_context);
    public IHistoricoProjetoRepository HistoricoProjetoRepository => _historicoProjetoRepository ??= new HistoricoProjetoRepository(_context);

    public async Task<int> CommitAsync() => await _context.SaveChangesAsync();

    public void Dispose()
    {
        _context.Dispose();
        GC.SuppressFinalize(this);
    }

    public void LimparRastreador()
    {
        _context.ChangeTracker.Clear();
    }
}
