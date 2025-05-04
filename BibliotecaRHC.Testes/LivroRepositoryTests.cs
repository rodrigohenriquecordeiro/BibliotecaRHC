using BibliotecaRHC.API.Context;
using BibliotecaRHC.API.Models;
using BibliotecaRHC.API.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaRHC.Testes;

public class LivroRepositoryTests
{
    public List<Livro> ListaDeLivros { get; set; }

    public LivroRepositoryTests()
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
                DataDeAquisicao = DateTime.Now.ToString("dd/MM/yyyy")
            }
        ];
        #endregion
    }

    private AppDbContext CriarContextoComBancoEmMemoria(string nomeBanco)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(nomeBanco)
            .Options;

        return new AppDbContext(options);
    }

    private LivroRepository CriarRepositorio(string? nomeBanco = null, bool comDados = true)
    {
        nomeBanco ??= Guid.NewGuid().ToString();
        var context = CriarContextoComBancoEmMemoria(nomeBanco);

        if (comDados)
        {
            context.Livros!.AddRange(ListaDeLivros);
            context.SaveChanges();
        }

        return new LivroRepository(context);
    }

    [Fact]
    public async Task ObterTodos_DeveRetornarTodosOsLivros()
    {
        // Arrange
        var repository = CriarRepositorio();

        // Act
        var resultado = await repository.ObterTodos();

        // Assert
        Assert.Equal(ListaDeLivros.Count, resultado.Count());
    }

    [Fact]
    public async Task ObterTodos_DeveRetornarVazio()
    {
        // Arrange
        var repository = CriarRepositorio(comDados: false);

        // Act
        var resultado = await repository.ObterTodos();

        // Assert
        Assert.Empty(resultado);
    }

    [Theory]
    [InlineData(1, "Autor 1")]
    [InlineData(2, "Autor 2")]
    [InlineData(3, "Autor 3")]
    public async Task ObterPorId_DeveRetornarLivroCorreto(int id, string autorEsperado)
    {
        // Arrange
        var repository = CriarRepositorio();

        // Act
        var resultado = await repository.ObterPorId(id);

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal(autorEsperado, resultado.Autor);
    }

    [Fact]
    public async Task ObterPorId_DeveLancarExcecao_QuandoNaoEncontrar()
    {
        // Arrange
        var repository = CriarRepositorio(comDados: false);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            await repository.ObterPorId(999);
        });

        Assert.Equal("Livro não encontrado.", exception.Message);
    }

    [Fact]
    public async Task Adicionar_DeveAdicionarUmLivro()
    {
        // Arrange
        var repository = CriarRepositorio(comDados: false);
        var novoLivro = new Livro
        {
            Autor = "Autor Novo",
            NomeDoLivro = "Livro Novo",
            Editora = "Editora Nova",
            AnoDePublicacao = "2023",
            NumeroDePaginas = 150,
            ClassificacaoCatalografica = "Classificação Nova",
            Observacao = "Observação Nova",
            DataDeAquisicao = DateTime.Now.ToString("dd/MM/yyyy")
        };

        // Act
        await repository.Adicionar(novoLivro);
        var livros = await repository.ObterTodos();

        // Assert
        Assert.Single(livros);
        Assert.Equal("Livro Novo", livros.First().NomeDoLivro);
    }

    [Fact]
    public async Task Atualizar_DeveAtualizarUmLivro()
    {
        // Arrange
        var repository = CriarRepositorio();

        // Act
        var livro = await repository.ObterPorId(1);
        livro!.Autor = "Autor Atualizado";

        await repository.Atualizar(livro);
        var livroAtualizado = await repository.ObterPorId(1);

        // Assert            
        Assert.Equal("Autor Atualizado", livroAtualizado!.Autor);
    }

    [Fact]
    public async Task Atualizar_DeveLancarExcecao_QuandoNaoAcharLivroParaAtualizar()
    {
        // Arrange
        var repository = CriarRepositorio(comDados: false);

        // Act
        var livroInexistente = new Livro
        {
            Id = 999,
            Autor = "Autor Inexistente",
            NomeDoLivro = "Livro Inexistente",
            Editora = "Editora Inexistente",
            AnoDePublicacao = "2023",
            NumeroDePaginas = 150,
            ClassificacaoCatalografica = "Classificação Inexistente",
            Observacao = "Observação Inexistente",
            DataDeAquisicao = DateTime.Now.ToString("dd/MM/yyyy")
        };

        // Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            await repository.Atualizar(livroInexistente);
        });
    }

    [Fact]
    public async Task Excluir_DeveExcluirUmLivro()
    {
        // Arrange
        var context = CriarContextoComBancoEmMemoria("DbTeste_Excluir");
        context.Livros!.AddRange(ListaDeLivros);
        await context.SaveChangesAsync();
        var repository = new LivroRepository(context);

        // Act
        await repository.Excluir(1);
        var resultado = await context.Livros.FindAsync(1);

        // Assert
        Assert.Null(resultado);
    }

    [Fact]
    public async Task Excluir_DeveLancarExcecao_QuandoNaoEncontrarLivroParaExcluir()
    {
        // Arrange
        var repository = CriarRepositorio(comDados: false);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            await repository.Excluir(999);
        });

        Assert.Equal("Livro não encontrado.", exception.Message);
    }

    [Fact]
    public async Task ObterCodigoUltimoLivro_DeveRetornarMaiorId()
    {
        // Arrange
        var repository = CriarRepositorio();

        // Act
        var resultado = await repository.ObterCodigoUltimoLivro();

        // Assert
        Assert.Equal(3, resultado);
    }

    [Fact]
    public async Task ObterCodigoUltimoLivro_DeveRetornarZero_QuandoNaoHouverLivros()
    {
        // Arrange
        var repository = CriarRepositorio(comDados: false);

        // Act
        var resultado = await repository.ObterCodigoUltimoLivro();

        // Assert
        Assert.Equal(0, resultado);
    }
}
