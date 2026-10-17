using System.Security.Claims;
using BibliotecaRHC.API.Context;
using BibliotecaRHC.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaRHC.Tests;

internal static class TestDbContextFactory
{
    public static AppDbContext Create(string databaseName, int userId = 1)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName)
            .EnableSensitiveDataLogging()
            .Options;
        var accessor = new HttpContextAccessor
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(
                    [new Claim(ClaimTypes.NameIdentifier, userId.ToString())],
                    "test"))
            }
        };
        var context = new AppDbContext(options, accessor);

        if (!context.Users.Any(user => user.Id == userId))
        {
            context.Users.Add(new ApplicationUser
            {
                Id = userId,
                UserName = $"user-{userId}",
                NormalizedUserName = $"USER-{userId}",
                Email = $"user-{userId}@example.com",
                NormalizedEmail = $"USER-{userId}@EXAMPLE.COM"
            });
            context.SaveChanges();
        }

        return context;
    }
}
