using System.ComponentModel.DataAnnotations;

namespace BibliotecaRHC.API.Models;

public class UpdateAccountModel
{
    [Required(ErrorMessage = "Nome de usuário é obrigatório")]
    [MinLength(3, ErrorMessage = "Nome de usuário deve ter pelo menos 3 caracteres")]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email é obrigatório")]
    [EmailAddress(ErrorMessage = "Formato de e-mail inválido")]
    public string Email { get; set; } = string.Empty;

    public string? SenhaAtual { get; set; }
    public string? NovaSenha { get; set; }
}
