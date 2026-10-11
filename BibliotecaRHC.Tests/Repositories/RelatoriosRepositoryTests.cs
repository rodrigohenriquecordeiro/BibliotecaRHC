using BibliotecaRHC.API.Context;
using BibliotecaRHC.API.Models;
using BibliotecaRHC.API.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaRHC.Tests.Repositories;

public class RelatoriosRepositoryTests
{
    private AppDbContext CriarContexto(string nomeBanco)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(nomeBanco)
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public async Task ObterQuantidades_DeveCalcularIndicadoresCorretamente()
    {
        // Arrange
        var nomeBanco = Guid.NewGuid().ToString();
        await using var context = CriarContexto(nomeBanco);

        // Livros
        context.Set<Livro>().AddRange(
            new Livro { Id = 1, Autor = "A1", Editora = "E1", NomeDoLivro = "L1", Observacao = string.Empty, Lido = true, ClassificacaoCatalografica = "C1", AnoDePublicacao = "2020", DataDeAquisicao = DateTime.Now, NumeroDePaginas = 100 },
            new Livro { Id = 2, Autor = "A2", Editora = "E2", NomeDoLivro = "L2", Observacao = string.Empty, Lido = false, ClassificacaoCatalografica = "C2", AnoDePublicacao = "2021", DataDeAquisicao = DateTime.Now, NumeroDePaginas = 200 },
            new Livro { Id = 3, Autor = "A1", Editora = "E1", NomeDoLivro = "L3", Observacao = string.Empty, Lido = false, ClassificacaoCatalografica = "C1", AnoDePublicacao = "2020", DataDeAquisicao = DateTime.Now, NumeroDePaginas = 300 }
        );

        // Projeto em andamento
        context.Set<Projeto>().Add(new Projeto { Id = 1, Nome = "P1", ProjetoStatus = ProjetoStatus.Andamento });

        await context.SaveChangesAsync();

        var repo = new RelatoriosRepository(context);

        // Act
        var resultado = await repo.ObterQuantidades();

        // Assert
        Assert.Equal(3, resultado.TotalLivros);
        Assert.Equal(1, resultado.LivrosLidos);
        Assert.Equal(2, resultado.LivrosNaoLidos);
        Assert.Equal(2, resultado.TotalEditoras);
        Assert.Equal(2, resultado.TotalAutores);
        Assert.Equal(1, resultado.ProjetosLeituraAndamento);
        Assert.NotNull(resultado.TopAutores);
        Assert.NotNull(resultado.TopEditoras);
        Assert.NotNull(resultado.TopClassificacoes);
        Assert.NotNull(resultado.TopLivrosLongos);
    }
}
