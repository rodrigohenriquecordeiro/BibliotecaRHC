using System.ComponentModel.DataAnnotations;

namespace BibliotecaRHC.API.Models;

public class RegisterModel
{
    [Required(ErrorMessage = "Nome de usuário é obrigatório")]
    public string? UserName { get; set; }

    [EmailAddress]
    [Required(ErrorMessage = "Email é obrigatório")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Senha é obrigatório")]
    public string? Password { get; set; }
}
