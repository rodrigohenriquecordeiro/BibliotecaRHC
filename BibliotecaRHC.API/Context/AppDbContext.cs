using BibliotecaRHC.API.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaRHC.API.Context;

public class AppDbContext : IdentityDbContext<ApplicationUser, IdentityRole<int>, int>
{
    public DbSet<Livro>? Livro { get; set; }
    public DbSet<FraseInesquecivel>? FraseInesquecivel{ get; set; }
    public DbSet<Projeto> Projeto { get; set; }
    public DbSet<LivroProjeto> LivroProjeto { get; set; }
    public DbSet<HistoricoProjeto> HistoricoProjeto { get; set; }
    public DbSet<HistoricoAlteracaoConta> HistoricoAlteracaoConta { get; set; }

    private readonly IHttpContextAccessor? _httpContextAccessor;
    private int? CurrentUserId =>
        int.TryParse(
            _httpContextAccessor?.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier),
            out var userId)
            ? userId
            : null;

    public AppDbContext(
        DbContextOptions<AppDbContext> options,
        IHttpContextAccessor? httpContextAccessor = null) : base(options)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<HistoricoAlteracaoConta>()
            .ToTable("HistoricoAlteracaoConta")
            .Property(history => history.CampoAlterado)
            .HasMaxLength(100);

        builder.Entity<HistoricoAlteracaoConta>().Property(history => history.Id).HasColumnOrder(0);
        builder.Entity<HistoricoAlteracaoConta>().Property(history => history.UsuarioId).HasColumnOrder(1);
        builder.Entity<HistoricoAlteracaoConta>()
            .HasIndex(history => new { history.UsuarioId, history.DataAlteracaoUtc });

        builder.Entity<HistoricoAlteracaoConta>()
            .HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(history => history.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Livro>()
            .HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(livro => livro.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.Entity<Projeto>()
            .HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(projeto => projeto.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.Entity<FraseInesquecivel>()
            .HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(frase => frase.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.Entity<LivroProjeto>()
            .HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(livroProjeto => livroProjeto.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.Entity<HistoricoProjeto>()
            .HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(historico => historico.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Projeto>()
            .Property(projeto => projeto.Id).HasColumnOrder(0);
        builder.Entity<Projeto>().Property(projeto => projeto.UsuarioId).HasColumnOrder(1);
        builder.Entity<Livro>().Property(livro => livro.Id).HasColumnOrder(0);
        builder.Entity<Livro>().Property(livro => livro.UsuarioId).HasColumnOrder(1);
        builder.Entity<FraseInesquecivel>().Property(frase => frase.Id).HasColumnOrder(0);
        builder.Entity<FraseInesquecivel>().Property(frase => frase.UsuarioId).HasColumnOrder(1);
        builder.Entity<LivroProjeto>().Property(livroProjeto => livroProjeto.Id).HasColumnOrder(0);
        builder.Entity<LivroProjeto>().Property(livroProjeto => livroProjeto.UsuarioId).HasColumnOrder(1);
        builder.Entity<HistoricoProjeto>().Property(historico => historico.Id).HasColumnOrder(0);
        builder.Entity<HistoricoProjeto>().Property(historico => historico.UsuarioId).HasColumnOrder(1);

        builder.Entity<Projeto>()
            .HasAlternateKey(projeto => new { projeto.Id, projeto.UsuarioId });
        builder.Entity<LivroProjeto>()
            .HasOne(livroProjeto => livroProjeto.Projeto)
            .WithMany(projeto => projeto.LivroProjetos)
            .HasForeignKey(livroProjeto => new { livroProjeto.ProjetoId, livroProjeto.UsuarioId })
            .HasPrincipalKey(projeto => new { projeto.Id, projeto.UsuarioId })
            .OnDelete(DeleteBehavior.Cascade);
        builder.Entity<HistoricoProjeto>()
            .HasOne(historico => historico.Projeto)
            .WithMany(projeto => projeto.HistoricoProjetos)
            .HasForeignKey(historico => new { historico.ProjetoId, historico.UsuarioId })
            .HasPrincipalKey(projeto => new { projeto.Id, projeto.UsuarioId })
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Livro>().HasQueryFilter(livro => livro.UsuarioId == CurrentUserId);
        builder.Entity<Projeto>().HasQueryFilter(projeto => projeto.UsuarioId == CurrentUserId);
        builder.Entity<FraseInesquecivel>().HasQueryFilter(frase => frase.UsuarioId == CurrentUserId);
        builder.Entity<LivroProjeto>().HasQueryFilter(livroProjeto => livroProjeto.UsuarioId == CurrentUserId);
        builder.Entity<HistoricoProjeto>().HasQueryFilter(historico => historico.UsuarioId == CurrentUserId);
        builder.Entity<HistoricoAlteracaoConta>().HasQueryFilter(historico => historico.UsuarioId == CurrentUserId);
    }

    public override int SaveChanges()
    {
        AplicarProprietarioAtual();
        return base.SaveChanges();
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        AplicarProprietarioAtual();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        AplicarProprietarioAtual();
        return base.SaveChangesAsync(cancellationToken);
    }

    public override Task<int> SaveChangesAsync(
        bool acceptAllChangesOnSuccess,
        CancellationToken cancellationToken = default)
    {
        AplicarProprietarioAtual();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    private void AplicarProprietarioAtual()
    {
        var userId = CurrentUserId;
        if (userId is null)
            return;

        foreach (var entry in ChangeTracker.Entries<IEntidadeDoUsuario>())
        {
            if (entry.State is EntityState.Added or EntityState.Modified)
                entry.Entity.UsuarioId = userId.Value;
        }
    }
}