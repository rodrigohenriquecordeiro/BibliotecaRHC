using BibliotecaRHC.API.Context;
using BibliotecaRHC.API.Models;
using BibliotecaRHC.API.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaRHC.Tests;

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
                DataDeAquisicao = DateTime.Now.ToString("dd/MM/yyyy") }
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

    private (LivroRepository repo, AppDbContext context) CriarRepositorioComContexto(string? nomeBanco = null, bool comDados = true)
    {
        nomeBanco ??= Guid.NewGuid().ToString();
        var context = CriarContextoComBancoEmMemoria(nomeBanco);

        if (comDados)
        {
            context.Livros!.AddRange(ListaDeLivros);
            context.SaveChanges();
        }

        var repo = new LivroRepository(context);
        return (repo, context);
    }

    [Fact]
    public async Task ObterTodos_DeveRetornarTodosOsLivros()
    {
        // Arrange
        var repository = CriarRepositorio();

        // Act
        var resultado = await repository.GetAllAsync();

        // Assert
        Assert.Equal(ListaDeLivros.Count, resultado.Count());
    }

    [Fact]
    public async Task ObterTodos_DeveRetornarVazio()
    {
        // Arrange
        var repository = CriarRepositorio(comDados: false);

        // Act 
        var resultado = await repository.GetAllAsync();

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
        var resultado = await repository.GetByIDAsync(id);

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal(autorEsperado, resultado!.Autor);
    }

    [Fact]
    public async Task ObterPorId_DeveRetornarNull_QuandoNaoEncontrar()
    {
        // Arrange
        var repository = CriarRepositorio(comDados: false);

        // Act
        var resultado = await repository.GetByIDAsync(999);

        // Assert
        Assert.Null(resultado);
    }

    [Fact]
    public async Task Adicionar_DeveAdicionarUmLivro()
    {
        // Arrange
        var (repository, context) = CriarRepositorioComContexto(comDados: false);
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
        repository.Add(novoLivro);
        await context.SaveChangesAsync();

        // Assert
        var livros = await repository.GetAllAsync();
        Assert.Single(livros);
    }

    [Fact]
    public async Task Atualizar_DeveAtualizarUmLivro()
    {
        // Arrange
        var repository = CriarRepositorio();
        var livro = await repository.GetByIDAsync(1);
        Assert.NotNull(livro);

        // Act
        livro!.Autor = "Autor Atualizado";
        repository.Update(livro);
        var livroAtualizado = await repository.GetByIDAsync(1);

        // Assert
        Assert.Equal("Autor Atualizado", livroAtualizado!.Autor);
    }

    [Fact]
    public void Atualizar_DeveLancarExcecao_QuandoLivroNaoExiste()
    {
        // Arrange
        var repository = CriarRepositorio(comDados: false);
        var livroInexistente = new Livro { Id = 999, Autor = "Autor Inexistente" };

        // Act
        var exception = Assert.Throws<InvalidOperationException>(() =>
        {
            repository.Update(livroInexistente);
        });

        // Assert
        Assert.Equal("Livro não encontrado para atualizar.", exception.Message);
    }

    [Fact]
    public async Task Remover_DeveRemoverUmLivro()
    {
        // Arrange
        var (repository, context) = CriarRepositorioComContexto();
        var livroParaRemover = await repository.GetByIDAsync(1);
        Assert.NotNull(livroParaRemover);

        // Act
        repository.Remove(livroParaRemover!);
        await context.SaveChangesAsync();

        // Assert
        var livroRemovido = await repository.GetByIDAsync(1);
        Assert.Null(livroRemovido);
        Assert.Equal(ListaDeLivros.Count - 1, (await repository.GetAllAsync()).Count());
    }

    [Fact]
    public async Task ObterCodigoUltimoLivro_DeveRetornarMaiorId()
    {
        // Arrange
        var repository = CriarRepositorio();

        // Act
        var resultado = await repository.ObterCodigoUltimoLivroAsync();

        // Assert
        Assert.Equal(3, resultado);
    }

    [Fact]
    public async Task ObterCodigoUltimoLivro_DeveRetornarZero_QuandoNaoHouverLivros()
    {
        // Arrange
        var repository = CriarRepositorio(comDados: false);

        // Act
        var resultado = await repository.ObterCodigoUltimoLivroAsync();

        // Assert
        Assert.Equal(0, resultado);
    }
}
