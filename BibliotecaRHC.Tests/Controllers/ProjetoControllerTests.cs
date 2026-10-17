using BibliotecaRHC.API.Controllers;
using BibliotecaRHC.API.Models;
using BibliotecaRHC.API.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Text.Json;

namespace BibliotecaRHC.Tests.Controllers;

public class ProjetoControllerTests
{
    [Fact]
    public async Task Post_DeveRetornarCreatedAtAction_ComProjetoCriado()
    {
        var mockService = new Mock<IProjetoService>();
        var projeto = new Projeto { Id = 1, Nome = "P1" };
        mockService.Setup(s => s.AdicionarProjeto(It.IsAny<Projeto>())).ReturnsAsync(projeto);

        var controller = new ProjetoController(mockService.Object);

        var resultado = await controller.Post(projeto);

        var created = Assert.IsType<CreatedAtActionResult>(resultado);
        var valor = Assert.IsType<Projeto>(created.Value);
        Assert.Equal(1, valor.Id);
    }

    [Fact]
    public async Task GetTodos_DeveRetornarOkComLista()
    {
        var mockService = new Mock<IProjetoService>();
        var lista = new List<Projeto> { new() { Id = 1 } };
        mockService.Setup(s => s.ObterTodosOsProjetos()).ReturnsAsync(lista);

        var controller = new ProjetoController(mockService.Object);

        var resultado = await controller.GetTodos();

        var ok = Assert.IsType<OkObjectResult>(resultado);
        var retornado = Assert.IsAssignableFrom<IEnumerable<Projeto>>(ok.Value);
        Assert.Single(retornado);
    }

    [Fact]
    public void ProjetoComLivrosEHistorico_DeveSerializarSemCiclo()
    {
        var projeto = new Projeto { Id = 1, Nome = "Projeto de leitura" };
        projeto.LivroProjetos.Add(new LivroProjeto { Id = 2, ProjetoId = projeto.Id, Projeto = projeto });
        projeto.HistoricoProjetos.Add(new HistoricoProjeto { Id = 3, ProjetoId = projeto.Id, Projeto = projeto });

        var json = JsonSerializer.Serialize(projeto);

        Assert.Contains("\"LivroProjetos\"", json);
        Assert.Contains("\"HistoricoProjetos\"", json);
        Assert.DoesNotContain("\"Projeto\":", json);
    }

    [Fact]
    public async Task GetById_DeveRetornarOk_QuandoExiste()
    {
        var mockService = new Mock<IProjetoService>();
        mockService.Setup(s => s.ObterProjetoPorId(1)).ReturnsAsync(new Projeto { Id = 1 });

        var controller = new ProjetoController(mockService.Object);

        var resultado = await controller.GetById(1);

        var ok = Assert.IsType<OkObjectResult>(resultado);
        var item = Assert.IsType<Projeto>(ok.Value);
        Assert.Equal(1, item.Id);
    }

    [Fact]
    public async Task GetById_DeveRetornarNotFound_QuandoNaoExiste()
    {
        var mockService = new Mock<IProjetoService>();
        mockService.Setup(s => s.ObterProjetoPorId(999)).ReturnsAsync((Projeto?)null);

        var controller = new ProjetoController(mockService.Object);

        var resultado = await controller.GetById(999);

        Assert.IsType<NotFoundResult>(resultado);
    }

    [Fact]
    public async Task Put_DeveRetornarNoContent_QuandoAtualizadoComSucesso()
    {
        var mockService = new Mock<IProjetoService>();
        var projeto = new Projeto { Id = 1, Nome = "Atual" };
        mockService.Setup(s => s.AtualizarProjeto(projeto)).ReturnsAsync(projeto);

        var controller = new ProjetoController(mockService.Object);

        var resultado = await controller.Put(1, projeto);

        Assert.IsType<NoContentResult>(resultado);
    }

    [Fact]
    public async Task Put_DeveRetornarBadRequest_QuandoIdDiferente()
    {
        var mockService = new Mock<IProjetoService>();
        var projeto = new Projeto { Id = 1 };
        var controller = new ProjetoController(mockService.Object);

        var resultado = await controller.Put(999, projeto);

        Assert.IsType<BadRequestObjectResult>(resultado);
    }

    [Fact]
    public async Task Put_DeveRetornarNotFound_QuandoNaoExiste()
    {
        var mockService = new Mock<IProjetoService>();
        var projeto = new Projeto { Id = 999 };
        mockService.Setup(s => s.AtualizarProjeto(It.IsAny<Projeto>())).ReturnsAsync((Projeto?)null);

        var controller = new ProjetoController(mockService.Object);

        var resultado = await controller.Put(999, projeto);

        Assert.IsType<NotFoundResult>(resultado);
    }

    [Fact]
    public async Task Delete_DeveRetornarNoContent_QuandoRemovido()
    {
        var mockService = new Mock<IProjetoService>();
        mockService.Setup(s => s.RemoverProjeto(1)).ReturnsAsync(new Projeto { Id = 1 });

        var controller = new ProjetoController(mockService.Object);

        var resultado = await controller.Delete(1);

        Assert.IsType<NoContentResult>(resultado);
    }

    [Fact]
    public async Task Delete_DeveRetornarNotFound_QuandoNaoExiste()
    {
        var mockService = new Mock<IProjetoService>();
        mockService.Setup(s => s.RemoverProjeto(999)).ReturnsAsync((Projeto?)null);

        var controller = new ProjetoController(mockService.Object);

        var resultado = await controller.Delete(999);

        Assert.IsType<NotFoundResult>(resultado);
    }
}
