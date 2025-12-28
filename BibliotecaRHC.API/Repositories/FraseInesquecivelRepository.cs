using BibliotecaRHC.API.Context;
using BibliotecaRHC.API.Models;

namespace BibliotecaRHC.API.Repositories;

public class FraseInesquecivelRepository : Repository<FraseInesquecivel>, IFrasesInesqueciveisRepository
{
    public FraseInesquecivelRepository(AppDbContext context) : base(context) { }
}

public interface IFrasesInesqueciveisRepository : IRepository<FraseInesquecivel>
{
}
