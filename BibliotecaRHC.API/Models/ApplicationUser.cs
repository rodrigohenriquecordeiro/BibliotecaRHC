using Microsoft.AspNetCore.Identity;

namespace BibliotecaRHC.API.Models;

public class ApplicationUser : IdentityUser<int>
{
    public string? RefreshToken { get; set; }
    public DateTime RefreshTokenExpiryTime { get; set; }
    public DateTime DataCriacao { get; set; }
}
