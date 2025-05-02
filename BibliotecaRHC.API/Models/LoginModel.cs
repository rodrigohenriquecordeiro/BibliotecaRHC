using System.ComponentModel.DataAnnotations;

namespace BibliotecaRHC.API.Models;

public class LoginModel
{
    [Required(ErrorMessage ="Nome de usuário é obrigatório")]
    public string? UserName { get; set; }

    [Required(ErrorMessage = "Senha é obrigatório")]
    public string? Password { get; set; }
}
