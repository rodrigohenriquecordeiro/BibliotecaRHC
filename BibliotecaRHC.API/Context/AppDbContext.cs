﻿using BibliotecaRHC.API.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

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