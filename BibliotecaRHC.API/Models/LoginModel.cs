using System.ComponentModel.DataAnnotations;

namespace BibliotecaRHC.API.Models;

public class LoginModel
{
    [Required(ErrorMessage ="Email de usuário é obrigatório")]
    public string? UserEmail { get; set; }

    [Required(ErrorMessage = "Senha é obrigatório")]
    public string? Password { get; set; }
}
