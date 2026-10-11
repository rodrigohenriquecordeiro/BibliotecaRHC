using BibliotecaRHC.API.Controllers;
using BibliotecaRHC.API.Models;
using BibliotecaRHC.API.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace BibliotecaRHC.Tests.Controllers;

public class RelatoriosControllerTests
{
    [Fact]
    public async Task GetDashboard_DeveRetornarOkComDados()
    {
        var mockService = new Mock<IRelatoriosService>();
        var rel = new Relatorios { TotalLivros = 2 };
        mockService.Setup(s => s.ObterDadosDoRelatorio()).ReturnsAsync(rel);

        var controller = new RelatoriosController(mockService.Object);

        var resultado = await controller.GetDashboard();

        var ok = Assert.IsType<OkObjectResult>(resultado);
        var valor = Assert.IsType<Relatorios>(ok.Value);
        Assert.Equal(2, valor.TotalLivros);
    }
}
