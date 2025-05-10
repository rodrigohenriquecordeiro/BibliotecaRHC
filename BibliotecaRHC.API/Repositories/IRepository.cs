using System.Linq.Expressions;

namespace BibliotecaRHC.API.Repositories;

public interface IRepository<T>
{
    IEnumerable<T> GetAll();
    T? GetByID(Expression<Func<T, bool>> predicate);
    T Create(T entity);
    T Update(T entity);
    T Delete(T entity);
}
