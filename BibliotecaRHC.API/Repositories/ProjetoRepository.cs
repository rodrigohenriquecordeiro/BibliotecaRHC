using BibliotecaRHC.API.Context;
using BibliotecaRHC.API.Models;

namespace BibliotecaRHC.API.Repositories;

public class ProjetoRepository  : Repository<Projeto>, IProjetoRepository
{
    public ProjetoRepository(AppDbContext context) : base(context) { }
}

public interface IProjetoRepository : IRepository<Projeto> { }
