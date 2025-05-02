using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using BibliotecaRHC.API.Models;

namespace BibliotecaRHC.API.Context;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public DbSet<Livro>? Livros { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
    }
}