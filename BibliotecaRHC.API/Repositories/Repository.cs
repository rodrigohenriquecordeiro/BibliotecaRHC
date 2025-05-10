using BibliotecaRHC.API.Context;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaRHC.API.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly AppDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(AppDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<IEnumerable<T>> GetAllAsync() => await _dbSet.AsNoTracking().ToListAsync();

    public async Task<T?> GetByIDAsync(int id) => await _dbSet.FindAsync(id);

    public void Add(T entity) => _dbSet.Add(entity);

    public void Update(T entity) => _context.Entry(entity).State = EntityState.Modified;

    public void Remove(T entity) => _dbSet.Remove(entity);
}
