using BibliotecaRHC.API.Models;
using BibliotecaRHC.API.Repositories;
using BibliotecaRHC.Tests;

namespace BibliotecaRHC.Tests.Context;

public class AppDbContextUserIsolationTests
{
    [Fact]
    public async Task LivrosEConsultasDeRepositorioFicamRestritosAoUsuarioConectado()
    {
        var databaseName = Guid.NewGuid().ToString();
        await using (var firstUserContext = TestDbContextFactory.Create(databaseName, 10))
        {
            firstUserContext.Set<Livro>().Add(CriarLivro("Livro A"));
            await firstUserContext.SaveChangesAsync();
        }

        int livroDoOutroUsuarioId;
        await using (var secondUserContext = TestDbContextFactory.Create(databaseName, 20))
        {
            var livroDoOutroUsuario = CriarLivro("Livro B");
            secondUserContext.Set<Livro>().Add(livroDoOutroUsuario);
            await secondUserContext.SaveChangesAsync();
            livroDoOutroUsuarioId = livroDoOutroUsuario.Id;
        }

        await using var contextUserA = TestDbContextFactory.Create(databaseName, 10);
        var repository = new LivroRepository(contextUserA);

        var livrosVisiveis = await repository.GetAllAsync();
        var livroDeOutroUsuario = await repository.GetByIDAsync(livroDoOutroUsuarioId);

        Assert.Single(livrosVisiveis);
        Assert.Equal("Livro A", livrosVisiveis.Single().NomeDoLivro);
        Assert.Null(livroDeOutroUsuario);
    }

    private static Livro CriarLivro(string nome) => new()
    {
        NomeDoLivro = nome,
        Autor = "Autor",
        Editora = "Editora",
        AnoDePublicacao = "2026",
        NumeroDePaginas = 100,
        ClassificacaoCatalografica = "A1",
        Observacao = string.Empty,
        DataDeAquisicao = DateTime.UtcNow
    };
}
