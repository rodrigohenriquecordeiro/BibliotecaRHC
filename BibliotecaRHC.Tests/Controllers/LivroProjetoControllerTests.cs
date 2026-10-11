using BibliotecaRHC.API.Controllers;
using BibliotecaRHC.API.Models;
using BibliotecaRHC.API.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace BibliotecaRHC.Tests.Controllers;

public class LivroProjetoControllerTests
{
    [Fact]
    public async Task Post_DeveRetornarCreatedAtAction_ComLivroProjetoCriado()
    {
        var mockService = new Mock<ILivroProjetoService>();
        var entidade = new LivroProjeto { Id = 1, Nome = "LP" };
        mockService.Setup(s => s.AdicionarLivroNoProjeto(It.IsAny<LivroProjeto>())).ReturnsAsync(entidade);

        var controller = new LivroProjetoController(mockService.Object);

        var resultado = await controller.Post(entidade);

        var created = Assert.IsType<CreatedAtActionResult>(resultado);
        var valor = Assert.IsType<LivroProjeto>(created.Value);
        Assert.Equal(1, valor.Id);
    }

    [Fact]
    public async Task GetTodos_DeveRetornarOkComLista()
    {
        var mockService = new Mock<ILivroProjetoService>();
        var lista = new List<LivroProjeto> { new() { Id = 1 } };
        mockService.Setup(s => s.ObterTodosOsLivrosDoProjeto()).ReturnsAsync(lista);

        var controller = new LivroProjetoController(mockService.Object);

        var resultado = await controller.GetTodos();

        var ok = Assert.IsType<OkObjectResult>(resultado);
        var retornado = Assert.IsAssignableFrom<IEnumerable<LivroProjeto>>(ok.Value);
        Assert.Single(retornado);
    }

    [Fact]
    public async Task GetById_DeveRetornarOk_QuandoExiste()
    {
        var mockService = new Mock<ILivroProjetoService>();
        mockService.Setup(s => s.ObterLivroDoProjetoPorId(1)).ReturnsAsync(new LivroProjeto { Id = 1 });

        var controller = new LivroProjetoController(mockService.Object);

        var resultado = await controller.GetById(1);

        var ok = Assert.IsType<OkObjectResult>(resultado);
        var item = Assert.IsType<LivroProjeto>(ok.Value);
        Assert.Equal(1, item.Id);
    }

    [Fact]
    public async Task GetById_DeveRetornarNotFound_QuandoNaoExiste()
    {
        var mockService = new Mock<ILivroProjetoService>();
        mockService.Setup(s => s.ObterLivroDoProjetoPorId(999)).ReturnsAsync((LivroProjeto?)null);

        var controller = new LivroProjetoController(mockService.Object);

        var resultado = await controller.GetById(999);

        Assert.IsType<NotFoundResult>(resultado);
    }

    [Fact]
    public async Task Put_DeveRetornarNoContent_QuandoAtualizadoComSucesso()
    {
        var mockService = new Mock<ILivroProjetoService>();
        var entidade = new LivroProjeto { Id = 1, Nome = "Atual" };
        mockService.Setup(s => s.AtualizarLivroNoProjeto(entidade)).ReturnsAsync(entidade);

        var controller = new LivroProjetoController(mockService.Object);

        var resultado = await controller.Put(1, entidade);

        Assert.IsType<NoContentResult>(resultado);
    }

    [Fact]
    public async Task Put_DeveRetornarBadRequest_QuandoIdDiferente()
    {
        var mockService = new Mock<ILivroProjetoService>();
        var entidade = new LivroProjeto { Id = 1 };
        var controller = new LivroProjetoController(mockService.Object);

        var resultado = await controller.Put(999, entidade);

        Assert.IsType<BadRequestObjectResult>(resultado);
    }

    [Fact]
    public async Task Put_DeveRetornarNotFound_QuandoNaoExiste()
    {
        var mockService = new Mock<ILivroProjetoService>();
        var entidade = new LivroProjeto { Id = 999 };
        mockService.Setup(s => s.AtualizarLivroNoProjeto(It.IsAny<LivroProjeto>())).ReturnsAsync((LivroProjeto?)null);

        var controller = new LivroProjetoController(mockService.Object);

        var resultado = await controller.Put(999, entidade);

        Assert.IsType<NotFoundResult>(resultado);
    }

    [Fact]
    public async Task Delete_DeveRetornarNoContent_QuandoRemovido()
    {
        var mockService = new Mock<ILivroProjetoService>();
        mockService.Setup(s => s.RemoverLivroDoProjeto(1)).ReturnsAsync(new LivroProjeto { Id = 1 });

        var controller = new LivroProjetoController(mockService.Object);

        var resultado = await controller.Delete(1);

        Assert.IsType<NoContentResult>(resultado);
    }

    [Fact]
    public async Task Delete_DeveRetornarNotFound_QuandoNaoExiste()
    {
        var mockService = new Mock<ILivroProjetoService>();
        mockService.Setup(s => s.RemoverLivroDoProjeto(999)).ReturnsAsync((LivroProjeto?)null);

        var controller = new LivroProjetoController(mockService.Object);

        var resultado = await controller.Delete(999);

        Assert.IsType<NotFoundResult>(resultado);
    }
}
