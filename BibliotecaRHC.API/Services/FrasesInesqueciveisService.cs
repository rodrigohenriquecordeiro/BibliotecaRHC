using BibliotecaRHC.API.Models;
using BibliotecaRHC.API.Repositories;

namespace BibliotecaRHC.API.Services
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

        public async Task<IEnumerable<FraseInesquecivel>> ObterTodasAsFrases()
        {
            var frases = await _unityOfWork.FrasesInesqueciveisRepository.GetAllAsync();

            if (!frases.Any())
            {
                _logger.LogInformation("Nenhuma frase encontrada.");
                return [];
            }

            return frases;
        }

        public async Task<FraseInesquecivel?> ObterFrasePorId(int id)
        {
            var frase = await _unityOfWork.FrasesInesqueciveisRepository.GetByIDAsync(id);

            if (frase == null)
            {
                _logger.LogWarning($"Frase com ID {id} não encontrada.");
            }

            return frase;
        }

        public async Task<FraseInesquecivel> AdicionarFrase(FraseInesquecivel frase)
        {
            FormataFrase(frase);

            _unityOfWork.FrasesInesqueciveisRepository.Add(frase);
            await _unityOfWork.CommitAsync();
            return frase;
        }

        private static void FormataFrase(FraseInesquecivel frase)
        {
            frase.Frase = frase.Frase!.Replace("\"", "").Trim();
            if (!string.IsNullOrEmpty(frase.Frase))
            {
                if (frase.Frase[0] == '\'')
                    frase.Frase = frase.Frase[1..].TrimStart();

                if (frase.Frase.Length > 0 && frase.Frase[^1] == '\'')
                    frase.Frase = frase.Frase[..^1].TrimEnd();
            }
        }

        public async Task<FraseInesquecivel?> AtualizarFrase(FraseInesquecivel frase)
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
        public async Task<FraseInesquecivel?> RemoverFrase(int id)
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

        public async Task<FraseInesquecivel?> ObterFraseAleatoria()
        {
            var todasFrasesAleatorias = await _unityOfWork.FrasesInesqueciveisRepository.GetAllAsync();

            if (!todasFrasesAleatorias.Any())
            {
                FraseInesquecivel frase = new()
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
        Task<IEnumerable<FraseInesquecivel>> ObterTodasAsFrases();
        Task<FraseInesquecivel?> ObterFrasePorId(int id);
        Task<FraseInesquecivel> AdicionarFrase(FraseInesquecivel frase);
        Task<FraseInesquecivel?> AtualizarFrase(FraseInesquecivel frase);
        Task<FraseInesquecivel?> RemoverFrase(int id);
        Task<FraseInesquecivel?> ObterFraseAleatoria();
    }
}
