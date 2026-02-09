using BibliotecaRHC.API.Context;
using BibliotecaRHC.API.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaRHC.Tests.Repositories;

public abstract class BaseRepositoryTests<T> where T : class
{
    protected abstract IRepository<T> CriarRepositorio(AppDbContext context);

    protected abstract List<T> ObterListaPadrao();

    protected abstract T CriarNovaEntidade();

    protected abstract void AlterarEntidadeParaUpdate(T entity);

    protected abstract int ObterIdValido();

    protected abstract int ObterIdInvalido();

    protected AppDbContext CriarContextoComBancoEmMemoria(string nomeBanco)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(nomeBanco)
            .EnableSensitiveDataLogging()
            .Options;

        return new AppDbContext(options);
    }

    protected (IRepository<T> repo, AppDbContext context) CriarRepositorioComContexto(string? nomeBanco = null, bool comDados = true)
    {
        nomeBanco ??= Guid.NewGuid().ToString();
        var context = CriarContextoComBancoEmMemoria(nomeBanco);

        if (comDados)
        {
            var dados = ObterListaPadrao();
            context.Set<T>().AddRange(dados);
            context.SaveChanges();

            context.ChangeTracker.Clear();
        }

        var repo = CriarRepositorio(context);
        return (repo, context);
    }

    [Fact]
    public async Task GetAllAsync_DeveRetornarTodos()
    {
        // Arrange
        var (repo, _) = CriarRepositorioComContexto();
        var listaPadrao = ObterListaPadrao();

        // Act
        var resultado = await repo.GetAllAsync();

        // Assert
        Assert.Equal(listaPadrao.Count, resultado.Count());
    }

    [Fact]
    public async Task GetAllAsync_DeveRetornarVazio_QuandoSemDados()
    {
        // Arrange
        var (repo, _) = CriarRepositorioComContexto(comDados: false);

        // Act
        var resultado = await repo.GetAllAsync();

        // Assert
        Assert.Empty(resultado);
    }

    [Fact]
    public async Task GetByIdAsync_DeveRetornarEntidade_QuandoIdExiste()
    {
        // Arrange
        var (repo, _) = CriarRepositorioComContexto();
        var id = ObterIdValido();

        // Act
        var resultado = await repo.GetByIDAsync(id);

        // Assert
        Assert.NotNull(resultado);
    }

    [Fact]
    public async Task GetByIdAsync_DeveRetornarNull_QuandoIdNaoExiste()
    {
        // Arrange
        var (repo, _) = CriarRepositorioComContexto();
        var id = ObterIdInvalido();

        // Act
        var resultado = await repo.GetByIDAsync(id);

        // Assert
        Assert.Null(resultado);
    }

    [Fact]
    public async Task Add_DeveSalvarNoBanco()
    {
        // Arrange
        var (repo, context) = CriarRepositorioComContexto(comDados: false);
        var novaEntidade = CriarNovaEntidade();

        // Act
        repo.Add(novaEntidade);
        await context.SaveChangesAsync();

        // Assert
        Assert.Equal(1, await context.Set<T>().CountAsync());
    }

    [Fact]
    public async Task Update_DeveAtualizarEntidade()
    {
        // Arrange
        var (repo, context) = CriarRepositorioComContexto();
        var id = ObterIdValido();

        var entidade = await context.Set<T>().FindAsync(id);
        context.Entry(entidade!).State = EntityState.Detached;

        AlterarEntidadeParaUpdate(entidade!);

        // Act
        repo.Update(entidade!);

        // Assert
        var entidadeAtualizada = await context.Set<T>().FindAsync(id);
        Assert.NotNull(entidadeAtualizada);
    }

    [Fact]
    public void Update_DeveLancarException_QuandoNaoExiste()
    {
        // Arrange
        var (repo, _) = CriarRepositorioComContexto(comDados: false);
        var entidade = CriarNovaEntidade();

        // Act & Assert
        var ex = Assert.Throws<InvalidOperationException>(() => repo.Update(entidade));
        Assert.Contains("não encontrado para atualizar", ex.Message);
    }

    [Fact]
    public async Task Remove_DeveExcluirEntidade()
    {
        // Arrange
        var (repo, context) = CriarRepositorioComContexto();
        var id = ObterIdValido();
        var entidade = await repo.GetByIDAsync(id);

        // Act
        repo.Remove(entidade!);

        // Assert
        var buscaNovamente = await context.Set<T>().FindAsync(id);
        Assert.Null(buscaNovamente);
    }
}
