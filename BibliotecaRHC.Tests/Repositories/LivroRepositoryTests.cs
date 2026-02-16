using BibliotecaRHC.API.Context;
using BibliotecaRHC.API.Models;
using BibliotecaRHC.API.Repositories;

namespace BibliotecaRHC.Tests.Repositories;

public class LivroRepositoryTests : BaseRepositoryTests<Livro>
{
    protected override IRepository<Livro> CriarRepositorio(AppDbContext context)
    {
        return new LivroRepository(context);
    }

    protected override List<Livro> ObterListaPadrao()
    {
        #region Lista de Livros
        return
        [
            new() 
            { 
                Id = 1, 
                Autor = "Autor 1", 
                NomeDoLivro = "Livro 1", 
                Editora = "Editora 1", 
                AnoDePublicacao = "2020", 
                NumeroDePaginas = 100, 
                ClassificacaoCatalografica = "A1", 
                Observacao = "Obs 1", 
                DataDeAquisicao = DateTime.Now,
                Lido = false
            },
            new() 
            { 
                Id = 2, 
                Autor = "Autor 2", 
                NomeDoLivro = "Livro 2", 
                Editora = "Editora 2", 
                AnoDePublicacao = "2021", 
                NumeroDePaginas = 200, 
                ClassificacaoCatalografica = "A2", 
                Observacao = "Obs 2", 
                DataDeAquisicao = DateTime.Now,
                Lido = false
            },
            new() 
            { 
                Id = 3, 
                Autor = "Autor 3", 
                NomeDoLivro = "Livro 3", 
                Editora = "Editora 3", 
                AnoDePublicacao = "2022", 
                NumeroDePaginas = 300, 
                ClassificacaoCatalografica = "A3", 
                Observacao = "Obs 3", 
                DataDeAquisicao = DateTime.Now,
                Lido = false
            }
        ];
        #endregion
    }

    protected override Livro CriarNovaEntidade()
    {
        return new Livro
        {
            Id = 999,
            Autor = "Autor Novo",
            NomeDoLivro = "Livro Novo",
            Editora = "Editora Nova",
            AnoDePublicacao = "2023",
            NumeroDePaginas = 150,
            ClassificacaoCatalografica = "New",
            Observacao = "New Obs",
            DataDeAquisicao = DateTime.Now,
            Lido = false
        };
    }

    protected override void AlterarEntidadeParaUpdate(Livro entity)
    {
        entity.Autor = "Autor Atualizado pelo Teste";
    }

    protected override int ObterIdValido() => 1;

    protected override int ObterIdInvalido() => 9999;

    [Fact]
    public async Task ObterCodigoUltimoLivro_DeveRetornarMaiorId()
    {
        // Arrange
        var (repo, _) = CriarRepositorioComContexto();

        var livroRepo = repo as LivroRepository;

        // Act
        var resultado = await livroRepo!.ObterCodigoUltimoLivroAsync();

        // Assert
        Assert.Equal(3, resultado);
    }

    [Theory]
    [InlineData(1, "Autor 1")]
    [InlineData(2, "Autor 2")]
    public async Task ObterPorId_DeveRetornarDadosCorretos(int id, string autorEsperado)
    {
        var (repo, _) = CriarRepositorioComContexto();
        var livro = await repo.GetByIDAsync(id);
        Assert.Equal(autorEsperado, livro!.Autor);
    }
}
