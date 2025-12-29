using BibliotecaRHC.API.Models;
using BibliotecaRHC.API.Repositories;

namespace BibliotecaRHC.API.Domain;

public class LivroService : ILivroService
{
    private readonly IUnityOfWork _unityOfWork;
    private readonly ILogger<LivroService> _logger;

    public LivroService(IUnityOfWork unityOfWork, ILogger<LivroService> logger)
    {
        _unityOfWork = unityOfWork;
        _logger = logger;
    }

    public async Task<IEnumerable<Livro>> ObterTodosOsLivros()
    {
        var livros = await _unityOfWork.LivroRepository.GetAllAsync();

        if (!livros.Any())
        {
            _logger.LogInformation("Nenhum livro encontrado.");
            return Enumerable.Empty<Livro>();
        }

        return livros;
    }

    public async Task<Livro?> ObterLivroPorId(int id)
    {
        var livro = await _unityOfWork.LivroRepository.GetByIDAsync(id);

        if (livro == null)
        {
            _logger.LogWarning($"Livro com ID {id} não encontrado.");
        }

        return livro;
    }

    public async Task<Livro> AdicionarLivroAsync(Livro livro)
    {
        _unityOfWork.LivroRepository.Add(livro);
        await _unityOfWork.CommitAsync();
        return livro;
    }

    public async Task<Livro?> AtualizarLivro(Livro livro)
    {
        var existente = await _unityOfWork.LivroRepository.GetByIDAsync(livro.Id);

        if (existente == null)
        {
            _logger.LogWarning($"Livro com ID {livro.Id} não encontrado para atualização.");
            return null;
        }

        _unityOfWork.LivroRepository.Update(livro);
        await _unityOfWork.CommitAsync();
        return livro;
    }

    public async Task<Livro?> RemoverLivro(int id)
    {
        var livro = await _unityOfWork.LivroRepository.GetByIDAsync(id);

        if (livro == null)
        {
            _logger.LogWarning($"Livro com ID {id} não encontrado para remoção.");
            return null;
        }

        _unityOfWork.LivroRepository.Remove(livro);
        await _unityOfWork.CommitAsync();
        return livro;
    }

    public async Task<int> GeraCodigoProximoLivro()
    {
        var ultimoCodigo = await _unityOfWork.LivroRepository.ObterCodigoUltimoLivroAsync();
        return ultimoCodigo + 1;
    }
}

public interface ILivroService
{
    Task<IEnumerable<Livro>> ObterTodosOsLivros();
    Task<Livro?> ObterLivroPorId(int id);
    Task<Livro> AdicionarLivroAsync(Livro livro);
    Task<Livro?> AtualizarLivro(Livro livro);
    Task<Livro?> RemoverLivro(int id);
    Task<int> GeraCodigoProximoLivro();
}