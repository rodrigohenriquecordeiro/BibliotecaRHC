using BibliotecaRHC.API.Context;
using BibliotecaRHC.API.Models;

namespace BibliotecaRHC.API.Repositories;

public class FrasesInesqueciveisRepository : Repository<FrasesInesqueciveis>, IFrasesInesqueciveisRepository
{
    public FrasesInesqueciveisRepository(AppDbContext context) : base(context) { }
}

public interface IFrasesInesqueciveisRepository : IRepository<FrasesInesqueciveis>
{
}
