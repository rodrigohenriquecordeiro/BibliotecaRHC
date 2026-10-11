using BibliotecaRHC.API.Context;
using BibliotecaRHC.API.Models;
using BibliotecaRHC.API.Repositories;

namespace BibliotecaRHC.Tests.Repositories;

public class FraseInesquecivelRepositoryTests : BaseRepositoryTests<FraseInesquecivel>
{
    protected override IRepository<FraseInesquecivel> CriarRepositorio(AppDbContext context) => new FraseInesquecivelRepository(context);
    
    protected override List<FraseInesquecivel> ObterListaPadrao()
    {
        return
        [
            new() { Id = 1, Frase = "Frase 1", Autor = "Autor 1", NomeDoLivro = "Livro A", DataCriacao = DateTime.Now },
            new() { Id = 2, Frase = "Frase 2", Autor = "Autor 2", NomeDoLivro = "Livro B", DataCriacao = DateTime.Now },
            new() { Id = 3, Frase = "Frase 3", Autor = "Autor 3", NomeDoLivro = "Livro C", DataCriacao = DateTime.Now }
        ];
    }

    protected override FraseInesquecivel CriarNovaEntidade()
    {
        return new FraseInesquecivel { Id = 999, Frase = "Nova", Autor = "Autor Novo", NomeDoLivro = "Livro Novo" };
    }

    protected override void AlterarEntidadeParaUpdate(FraseInesquecivel entity) => entity.Autor = "Autor Atualizado";
    
    protected override int ObterIdValido() => 1;

    protected override int ObterIdInvalido() => 9999;
}
