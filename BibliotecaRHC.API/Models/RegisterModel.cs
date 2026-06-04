using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BibliotecaRHC.API.Models;

public class RegisterModel
{
    [Required(ErrorMessage = "Nome de usuário é obrigatório")]
    [JsonPropertyName("nome")] 
    public string? Nome { get; set; }

    [EmailAddress(ErrorMessage = "Formato de e-mail inválido")]
    [Required(ErrorMessage = "Email é obrigatório")]
    [JsonPropertyName("email")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Senha é obrigatória")]
    [JsonPropertyName("senha")]
    public string? Senha { get; set; }

    [JsonPropertyName("dataCriacao")]
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
}
