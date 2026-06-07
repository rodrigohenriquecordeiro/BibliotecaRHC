using BibliotecaRHC.API.Models;
using BibliotecaRHC.API.Repositories;

namespace BibliotecaRHC.API.Services;

public class RelatoriosService(IRelatoriosRepository repository) : IRelatoriosService
{
    public async Task<Relatorios> ObterDadosDoRelatorio() => await repository.ObterQuantidades();
}

public interface IRelatoriosService
{
    Task<Relatorios> ObterDadosDoRelatorio();
}
