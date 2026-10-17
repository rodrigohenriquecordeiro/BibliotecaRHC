using BibliotecaRHC.API.Models;
using BibliotecaRHC.API.Repositories;

namespace BibliotecaRHC.API.Services;

public class LivroProjetoService : ILivroProjetoService
{
    private readonly IUnityOfWork _unityOfWork;
    private readonly ILogger<LivroProjetoService> _logger;

    public LivroProjetoService(IUnityOfWork unityOfWork, ILogger<LivroProjetoService> logger)
    {
        _unityOfWork = unityOfWork;
        _logger = logger;
    }

    public async Task<IEnumerable<LivroProjeto>> ObterTodosOsLivrosDoProjeto()
    {
        var livrosDosProjetos = await _unityOfWork.LivroProjetoRepository.GetAllAsync();

        if (!livrosDosProjetos.Any())
        {
            _logger.LogInformation("Nenhum Livro no Projeto encontrado.");
            return [];
        }

        return livrosDosProjetos;
    }

    public async Task<LivroProjeto?> ObterLivroDoProjetoPorId(int id)
    {
        var livroDoProjeto = await _unityOfWork.LivroProjetoRepository.GetByIDAsync(id);

        if (livroDoProjeto == null)
        {
            _logger.LogWarning($"Livro do Projeto com ID {id} não encontrado.");
        }

        return livroDoProjeto;
    }

    public async Task<LivroProjeto?> AdicionarLivroNoProjeto(LivroProjeto livroNoProjeto)
    {
        var projeto = await _unityOfWork.ProjetoRepository.ObterPorIdProjeto(livroNoProjeto.ProjetoId);
        if (projeto == null)
        {
            _logger.LogWarning("Projeto com ID {ProjetoId} não encontrado para o usuário atual.", livroNoProjeto.ProjetoId);
            return null;
        }

        _unityOfWork.LivroProjetoRepository.Add(livroNoProjeto);
        await _unityOfWork.CommitAsync();
        return livroNoProjeto;
    }

    public async Task<LivroProjeto?> AtualizarLivroNoProjeto(LivroProjeto livroNoProjeto)
    {
        var livroExistente = await _unityOfWork.LivroProjetoRepository.GetByIDAsync(livroNoProjeto.Id);

        if (livroExistente == null)
        {
            _logger.LogWarning($"Livro do Projeto com ID {livroNoProjeto.Id} não encontrado para atualização.");
            return null;
        }

        var projeto = await _unityOfWork.ProjetoRepository.ObterPorIdProjeto(livroNoProjeto.ProjetoId);
        if (projeto == null)
        {
            _logger.LogWarning("Projeto com ID {ProjetoId} não encontrado para o usuário atual.", livroNoProjeto.ProjetoId);
            return null;
        }

        _unityOfWork.LivroProjetoRepository.Update(livroNoProjeto);
        await _unityOfWork.CommitAsync();
        return livroNoProjeto;
    }

    public async Task<LivroProjeto?> RemoverLivroDoProjeto(int id)
    {
        var livroNoProjeto = await _unityOfWork.LivroProjetoRepository.GetByIDAsync(id);

        if (livroNoProjeto == null)
        {
            _logger.LogWarning($"Livro no Projeto com ID {id} não encontrado para remoção.");
            return null;
        }

        _unityOfWork.LivroProjetoRepository.Remove(livroNoProjeto);
        await _unityOfWork.CommitAsync();
        return livroNoProjeto;
    }
}

public interface ILivroProjetoService
{
    Task<IEnumerable<LivroProjeto>> ObterTodosOsLivrosDoProjeto();
    Task<LivroProjeto?> ObterLivroDoProjetoPorId(int id);
    Task<LivroProjeto?> AdicionarLivroNoProjeto(LivroProjeto projeto);
    Task<LivroProjeto?> AtualizarLivroNoProjeto(LivroProjeto projeto);
    Task<LivroProjeto?> RemoverLivroDoProjeto(int id);
}