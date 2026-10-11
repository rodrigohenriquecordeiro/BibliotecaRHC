using BibliotecaRHC.API.Context;
using BibliotecaRHC.API.Models;
using BibliotecaRHC.API.Repositories;

namespace BibliotecaRHC.Tests.Repositories;

public class LivroProjetoRepositoryTests : BaseRepositoryTests<LivroProjeto>
{
    protected override IRepository<LivroProjeto> CriarRepositorio(AppDbContext context) => new LivroProjetoRepository(context);
    
    protected override List<LivroProjeto> ObterListaPadrao()
    {
        return
        [
            new() { Id = 1, Nome = "LP 1", AnoDePublicacao = "2020", Lido = false, ProjetoId = 1 },
            new() { Id = 2, Nome = "LP 2", AnoDePublicacao = "2021", Lido = false, ProjetoId = 1 },
            new() { Id = 3, Nome = "LP 3", AnoDePublicacao = "2022", Lido = false, ProjetoId = 2 }
        ];
    }

    protected override LivroProjeto CriarNovaEntidade()
    {
        return new LivroProjeto { Id = 999, Nome = "Novo LP", AnoDePublicacao = "2023", ProjetoId = 1 };
    }

    protected override void AlterarEntidadeParaUpdate(LivroProjeto entity)
    {
        entity.Nome = "Nome Atualizado";
    }

    protected override int ObterIdValido() => 1;

    protected override int ObterIdInvalido() => 9999;
}
