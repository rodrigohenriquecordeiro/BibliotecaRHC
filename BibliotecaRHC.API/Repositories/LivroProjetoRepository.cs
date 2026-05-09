using BibliotecaRHC.API.Context;
using BibliotecaRHC.API.Models;

namespace BibliotecaRHC.API.Repositories;

public class LivroProjetoRepository : Repository<LivroProjeto>, ILivroProjetoRepository
{
    public LivroProjetoRepository(AppDbContext context) : base(context) { }
}

public interface ILivroProjetoRepository : IRepository<LivroProjeto> { }
