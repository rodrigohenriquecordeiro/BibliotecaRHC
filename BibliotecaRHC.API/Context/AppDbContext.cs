using BibliotecaRHC.API.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaRHC.API.Context;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public DbSet<Livro>? Livro { get; set; }
    public DbSet<FraseInesquecivel>? FraseInesquecivel{ get; set; }
    public DbSet<Projeto> Projeto { get; set; }
    public DbSet<LivroProjeto> LivroProjeto { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
    }
}