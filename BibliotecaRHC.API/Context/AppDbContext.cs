using BibliotecaRHC.Models;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaRHC.API.Context;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Livro>? Livros { get; set; }
}