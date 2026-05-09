using BibliotecaRHC.API.Models;
using BibliotecaRHC.API.Repositories;

namespace BibliotecaRHC.API.Services;

public class ProjetoService : IProjetoService
{
    private readonly IUnityOfWork _unityOfWork;
    private readonly ILogger<ProjetoService> _logger;

    public ProjetoService(IUnityOfWork unityOfWork, ILogger<ProjetoService> logger)
    {
        _unityOfWork = unityOfWork;
        _logger = logger;
    }

    public async Task<IEnumerable<Projeto>> ObterTodosOsProjetos()
    {
        var projetos = await _unityOfWork.ProjetoRepository.GetAllAsync();

        if (!projetos.Any())
        {
            _logger.LogInformation("Nenhum projeto encontrado.");
            return [];
        }

        return projetos;
    }

    public async Task<Projeto?> ObterProjetoPorId(int id)
    {
        var projeto = await _unityOfWork.ProjetoRepository.GetByIDAsync(id);

        if (projeto == null)
        {
            _logger.LogWarning($"Projeto com ID {id} não encontrado.");
        }

        return projeto;
    }

    public async Task<Projeto> AdicionarProjeto(Projeto projeto)
    {
        _unityOfWork.ProjetoRepository.Add(projeto);
        await _unityOfWork.CommitAsync();
        return projeto;
    }

    public async Task<Projeto?> AtualizarProjeto(Projeto projeto)
    {
        var projetos = await _unityOfWork.ProjetoRepository.GetByIDAsync(projeto.Id);

        if (projetos == null)
        {
            _logger.LogWarning($"Projeto com ID {projeto.Id} não encontrado para atualização.");
            return null;
        }

        _unityOfWork.ProjetoRepository.Update(projeto);
        await _unityOfWork.CommitAsync();
        return projeto;
    }

    public async Task<Projeto?> RemoverProjeto(int id)
    {
        var projeto = await _unityOfWork.ProjetoRepository.GetByIDAsync(id);

        if (projeto == null)
        {
            _logger.LogWarning($"Projeto com ID {id} não encontrada para remoção.");
            return null;
        }

        var livrosNoProjeto = await _unityOfWork.LivroProjetoRepository.FindAsync(lp => lp.Id == id);
        _unityOfWork.LivroProjetoRepository.RemoveRange(livrosNoProjeto);
        _unityOfWork.ProjetoRepository.Remove(projeto);
        await _unityOfWork.CommitAsync();
        return projeto;
    }
}

public interface IProjetoService
{
    Task<IEnumerable<Projeto>> ObterTodosOsProjetos();
    Task<Projeto?> ObterProjetoPorId(int id);
    Task<Projeto> AdicionarProjeto(Projeto projeto);
    Task<Projeto?> AtualizarProjeto(Projeto projeto);
    Task<Projeto?> RemoverProjeto(int id);
}