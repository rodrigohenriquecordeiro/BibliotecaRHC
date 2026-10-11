using BibliotecaRHC.API.Models;
using BibliotecaRHC.API.Repositories;
using BibliotecaRHC.API.Services;
using Moq;

namespace BibliotecaRHC.Tests.Services;

public class RelatoriosServiceTests
{
    [Fact]
    public async Task ObterDadosDoRelatorio_DeveRetornarDadosDoRepositorio()
    {
        var mockRepo = new Mock<IRelatoriosRepository>();
        var rel = new Relatorios { TotalLivros = 5 };
        mockRepo.Setup(r => r.ObterQuantidades()).ReturnsAsync(rel);

        var service = new RelatoriosService(mockRepo.Object);

        var resultado = await service.ObterDadosDoRelatorio();

        Assert.NotNull(resultado);
        Assert.Equal(5, resultado.TotalLivros);
        mockRepo.Verify(r => r.ObterQuantidades(), Times.Once);
    }
}
