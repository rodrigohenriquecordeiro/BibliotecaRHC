using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BibliotecaRHC.API.Models;

public class FrasesInesqueciveis
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Obrigatório colocar a Frase")]
    [StringLength(65000, ErrorMessage = "Número de caracteres excede o permitido")]
    public string? Frase { get; set; }

    [Required(ErrorMessage = "Obrigatório colocar o Autor")]
    [StringLength(200, ErrorMessage = "Permitido no máximo 200 caracteres")]
    public string? Autor { get; set; }

    [Required(ErrorMessage = "Obrigatório colocar o Livro")]
    [StringLength(300, ErrorMessage = "Permitido no máximo 300 caracteres")]
    public string? NomeDoLivro { get; set; }

    public DateTime DataCriacao { get; set; } = DateTime.Now;

    public void ValidaClasse()
    {
        ValidationContext context = new(this, serviceProvider: null, items: null);
        List<ValidationResult> results = [];
        bool isValid = Validator.TryValidateObject(this, context, results, true);

        if (isValid == false)
        {
            StringBuilder sbrErrors = new();
            foreach (var validationResult in results)
            {
                sbrErrors.AppendLine(validationResult.ErrorMessage);
            }
            throw new ValidationException(sbrErrors.ToString());
        }
    }
}
