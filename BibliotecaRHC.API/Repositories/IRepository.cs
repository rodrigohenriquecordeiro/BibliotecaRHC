namespace BibliotecaRHC.API.Repositories;

public interface IRepository<T> where T : class
{
    Task<IEnumerable<T>> GetAllAsync();
    Task<T?> GetByIDAsync(int id);
    void Add(T entity);
    void Update(T entity);
    void Remove(T entity);
}
