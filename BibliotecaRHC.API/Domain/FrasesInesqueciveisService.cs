using BibliotecaRHC.API.Models;
using BibliotecaRHC.API.Repositories;

namespace BibliotecaRHC.API.Domain
{
    public class FrasesInesqueciveisService: IFrasesInesqueciveisService
    {
        private readonly IUnityOfWork _unityOfWork;
        private readonly ILogger<FrasesInesqueciveisService> _logger;

        public FrasesInesqueciveisService(IUnityOfWork unityOfWork, ILogger<FrasesInesqueciveisService> logger)
        {
            _unityOfWork = unityOfWork;
            _logger = logger;
        }

        public async Task<IEnumerable<FrasesInesqueciveis>> ObterTodasAsFrases()
        {
            var frases = await _unityOfWork.FrasesInesqueciveisRepository.GetAllAsync();

            if (!frases.Any())
            {
                _logger.LogInformation("Nenhuma frase encontrada.");
                return [];
            }

            return frases;
        }

        public async Task<FrasesInesqueciveis?> ObterFrasePorId(int id)
        {
            var frase = await _unityOfWork.FrasesInesqueciveisRepository.GetByIDAsync(id);

            if (frase == null)
            {
                _logger.LogWarning($"Frase com ID {id} não encontrada.");
            }

            return frase;
        }

        public async Task<FrasesInesqueciveis> AdicionarFrase(FrasesInesqueciveis frase)
        {
            _unityOfWork.FrasesInesqueciveisRepository.Add(frase);
            await _unityOfWork.CommitAsync();
            return frase;
        }

        public async Task<FrasesInesqueciveis?> AtualizarFrase(FrasesInesqueciveis frase)
        {
            var fraseQueSeraAlterada = await _unityOfWork.FrasesInesqueciveisRepository.GetByIDAsync(frase.Id);

            if (fraseQueSeraAlterada == null)
            {
                _logger.LogWarning($"Frase com ID {frase.Id} não encontrada para atualização.");
                return null;
            }

            _unityOfWork.FrasesInesqueciveisRepository.Update(frase);
            await _unityOfWork.CommitAsync();
            return frase;
        }
        public async Task<FrasesInesqueciveis?> RemoverFrase(int id)
        {
            var fraseInesquecivel = await _unityOfWork.FrasesInesqueciveisRepository.GetByIDAsync(id);

            if (fraseInesquecivel == null)
            {
                _logger.LogWarning($"Frase com ID {id} não encontrada para remoção.");
                return null;
            }

            _unityOfWork.FrasesInesqueciveisRepository.Remove(fraseInesquecivel);
            await _unityOfWork.CommitAsync();
            return fraseInesquecivel;
        }

        public async Task<FrasesInesqueciveis?> ObterFraseAleatoria()
        {
            var todasFrasesAleatorias = await _unityOfWork.FrasesInesqueciveisRepository.GetAllAsync();

            if (!todasFrasesAleatorias.Any())
            {
                FrasesInesqueciveis frase = new()
                {
                    Id = 1,
                    Frase = "Um leitor vive mil vidas antes de morrer. O homem que nunca lê vive apenas uma.",
                    Autor = "George R. R. Martin",
                    DataCriacao = DateTime.Now,
                };

                return frase;
            }

            var fraseAleatoriaConvertida = todasFrasesAleatorias.ToList();
            int fraseAleatoria = new Random().Next(0, fraseAleatoriaConvertida.Count);

            return fraseAleatoriaConvertida[fraseAleatoria];
        }
    }

    public interface IFrasesInesqueciveisService
    {
        Task<IEnumerable<FrasesInesqueciveis>> ObterTodasAsFrases();
        Task<FrasesInesqueciveis?> ObterFrasePorId(int id);
        Task<FrasesInesqueciveis> AdicionarFrase(FrasesInesqueciveis frase);
        Task<FrasesInesqueciveis?> AtualizarFrase(FrasesInesqueciveis frase);
        Task<FrasesInesqueciveis?> RemoverFrase(int id);
        Task<FrasesInesqueciveis?> ObterFraseAleatoria();
    }
}
