using BibliotecaRHC.API.Models;
using BibliotecaRHC.API.Repositories;
using Microsoft.EntityFrameworkCore;

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
        var projetos = await _unityOfWork.ProjetoRepository.ObterTodosProjetosCompletos();

        if (!projetos.Any())
        {
            _logger.LogInformation("Nenhum projeto encontrado.");
            return [];
        }

        return projetos;
    }

    public async Task<Projeto?> ObterProjetoPorId(int projetoId)
    {
        var projeto = await _unityOfWork.ProjetoRepository.ObterPorIdProjeto(projetoId);

        if (projeto == null)
        {
            _logger.LogWarning($"Projeto com ID {projetoId} não encontrado.");
        }

        return projeto;
    }

    public async Task<Projeto> AdicionarProjeto(Projeto projeto)
    {
        projeto.DataCriacao = DateTime.Now;
        projeto.HistoricoProjetos ??= [];
        projeto.HistoricoProjetos.Add(new HistoricoProjeto
        {
            DataAlteracao = DateTime.Now,
            ProjetoStatus = ProjetoStatus.NaoIniciado
        });

        _unityOfWork.ProjetoRepository.Add(projeto);
        await _unityOfWork.CommitAsync();

        return projeto;
    }

    public async Task<Projeto?> AtualizarProjeto(Projeto projeto)
    {
        var projetoNoBanco = await _unityOfWork.ProjetoRepository.ObterPorIdProjeto(projeto.Id);

        if (projetoNoBanco == null) return null;

        projetoNoBanco.Nome = projeto.Nome;
        projetoNoBanco.DataCriacao = projeto.DataCriacao;
        projetoNoBanco.ProjetoStatus = projeto.ProjetoStatus;

        await _unityOfWork.HistoricoProjetoRepository.AtualizarHistorico(projetoNoBanco);
        await _unityOfWork.CommitAsync();

        return projetoNoBanco;
    }

    public async Task<Projeto?> RemoverProjeto(int id)
    {
        var projeto = await _unityOfWork.ProjetoRepository.ObterPorIdProjeto(id);

        if (projeto == null)
        {
            _logger.LogWarning($"Projeto com ID {id} não encontrada para remoção.");
            return null;
        }

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