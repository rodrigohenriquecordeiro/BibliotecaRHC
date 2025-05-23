using BibliotecaRHC.API.Controllers;
using BibliotecaRHC.API.Domain;
using BibliotecaRHC.API.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace BibliotecaRHC.Tests;

public class LivroControllerTests
{
    public List<Livro> ListaDeLivros { get; set; }
    public LivroControllerTests()
    {
        #region Lista de Livros
        ListaDeLivros =
        [
            new()
            {
                Id = 1,
                Autor = "Autor 1",
                NomeDoLivro = "Livro 1",
                Editora = "Editora 1",
                AnoDePublicacao = "2020",
                NumeroDePaginas = 100,
                ClassificacaoCatalografica = "Classificação 1",
                Observacao = "Observação 1",
                DataDeAquisicao = DateTime.Now.ToString("dd/MM/yyyy")
            },
            new()
            {
                Id = 2,
                Autor = "Autor 2",
                NomeDoLivro = "Livro 2",
                Editora = "Editora 2",
                AnoDePublicacao = "2021",
                NumeroDePaginas = 200,
                ClassificacaoCatalografica = "Classificação 2",
                Observacao = "Observação 2",
                DataDeAquisicao = DateTime.Now.ToString("dd/MM/yyyy")
            },
            new()
            {
                Id = 3,
                Autor = "Autor 3",
                NomeDoLivro = "Livro 3",
                Editora = "Editora 3",
                AnoDePublicacao = "2022",
                NumeroDePaginas = 300,
                ClassificacaoCatalografica = "Classificação 3",
                Observacao = "Observação 3",
                DataDeAquisicao = DateTime.Now.ToString("dd/MM/yyyy") }
        ];
        #endregion
    }

    [Fact]
    public async Task Post_DeveRetornarCreatedAtAction_ComLivroCriado()
    {
        // Arrange
        var mockService = new Mock<ILivroService>();

        mockService.Setup(s => s.AdicionarLivroAsync(It.IsAny<Livro>()))
                   .ReturnsAsync(ListaDeLivros[0]);

        var controller = new LivrosController(mockService.Object);

        // Act
        var resultado = await controller.Post(ListaDeLivros[0]);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(resultado);
        Assert.NotNull(createdResult.Value);
        var livro = Assert.IsType<Livro>(createdResult.Value);
        Assert.Equal(1, livro.Id);
    }

    [Fact]
    public async Task GetTodos_DeveRetornarOkResultComListaDeLivros()
    {
        // Arrange
        var mockService = new Mock<ILivroService>();

        mockService.Setup(s => s.ObterTodosOsLivros())
                   .ReturnsAsync(ListaDeLivros);

        var controller = new LivrosController(mockService.Object);

        // Act
        var resultado = await controller.GetTodos();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(resultado);
        var livrosRetornados = Assert.IsAssignableFrom<IEnumerable<Livro>>(okResult.Value);
        Assert.Equal(3, ((List<Livro>)livrosRetornados).Count);
    }

    [Fact]
    public async Task GetById_DeveRetornarOkResultComLivroExistente()
    {
        // Arrange
        var mockService = new Mock<ILivroService>();
        mockService.Setup(s => s.ObterLivroPorId(1))
                   .ReturnsAsync(ListaDeLivros[0]);

        var controller = new LivrosController(mockService.Object);

        // Act
        var resultado = await controller.GetById(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(resultado);
        var livroRetornado = Assert.IsType<Livro>(okResult.Value);
        Assert.Equal(1, livroRetornado.Id);
    }

    [Fact]
    public async Task GetById_DeveRetornarNotFound_QuandoLivroNaoExistir()
    {
        // Arrange
        var mockService = new Mock<ILivroService>();
        mockService.Setup(s => s.ObterLivroPorId(999))
                   .ReturnsAsync((Livro?)null);

        var controller = new LivrosController(mockService.Object);

        // Act
        var resultado = await controller.GetById(999);

        // Assert
        Assert.IsType<NotFoundResult>(resultado);
    } 

    [Fact]
    public async Task Put_DeveRetornarNoContent_QuandoLivroAtualizadoComSucesso()
    {
        // Arrange
        var mockService = new Mock<ILivroService>();
        ListaDeLivros[0].NomeDoLivro = "Livro Atualizado";
        mockService.Setup(s => s.AtualizarLivro(ListaDeLivros[0]))
                   .ReturnsAsync(ListaDeLivros[0]);

        var controller = new LivrosController(mockService.Object);

        // Act
        var resultado = await controller.Put(1, ListaDeLivros[0]);

        // Assert
        Assert.IsType<NoContentResult>(resultado);
    }

    [Fact]
    public async Task Put_DeveRetornarNotFound_QuandoLivroNaoExistir()
    {
        // Arrange
        var mockService = new Mock<ILivroService>();
        var livroParaAtualizar = new Livro { Id = 999, NomeDoLivro = "Livro Atualizado" };

        mockService.Setup(s => s.AtualizarLivro(It.IsAny<Livro>()))
                   .ReturnsAsync((Livro?)null);

        var controller = new LivrosController(mockService.Object);

        // Act
        var resultado = await controller.Put(999, livroParaAtualizar); 

        // Assert
        Assert.IsType<NotFoundResult>(resultado);
    }

    [Fact]
    public async Task Put_DeveRetornarBadRequest_QuandoIdNaoCorresponder()
    {
        // Arrange
        var mockService = new Mock<ILivroService>();
        var livroParaAtualizar = new Livro { Id = 1, NomeDoLivro = "Livro Atualizado" };
        var controller = new LivrosController(mockService.Object);

        // Act
        var resultado = await controller.Put(999, livroParaAtualizar);

        // Assert
        Assert.IsType<BadRequestObjectResult>(resultado);
    }

    [Fact]
    public async Task Delete_DeveRetornarNoContent_QuandoLivroRemovidoComSucesso()
    {
        // Arrange
        var mockService = new Mock<ILivroService>();
        mockService.Setup(s => s.RemoverLivro(1))
                   .ReturnsAsync(ListaDeLivros[0]);

        var controller = new LivrosController(mockService.Object);

        // Act
        var resultado = await controller.Delete(1);

        // Assert
        Assert.IsType<NoContentResult>(resultado);
    }

    [Fact]
    public async Task Delete_DeveRetornarNotFound_QuandoLivroNaoExistir()
    {
        // Arrange
        var mockService = new Mock<ILivroService>();
        mockService.Setup(s => s.RemoverLivro(999))
                   .ReturnsAsync((Livro?)null);

        var controller = new LivrosController(mockService.Object);

        // Act
        var resultado = await controller.Delete(999);

        // Assert
        Assert.IsType<NotFoundResult>(resultado);
    }

    [Fact]
    public async Task Get_DeveRetornarCodigoProximoLivro_QuandoExistir()
    {
        // Arrange
        var mockService = new Mock<ILivroService>();
        mockService.Setup(s => s.GeraCodigoProximoLivro())
                   .ReturnsAsync(4);

        var controller = new LivrosController(mockService.Object);
        
        // Act
        var resultado = await controller.GetCodigoProximoLivro();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(resultado);
        var proximoId = Assert.IsType<int>(okResult.Value);
        Assert.Equal(4, proximoId);
    }
}
