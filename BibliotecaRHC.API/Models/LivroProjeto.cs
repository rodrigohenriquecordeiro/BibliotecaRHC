using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BibliotecaRHC.API.Models;

public class LivroProjeto
{
    public int Id { get; set; }

    [StringLength(300, ErrorMessage = "Permitido no máximo 300 caracteres")]
    public string? Nome { get; set; }

    [StringLength(20, ErrorMessage = "Permitido no máximo 20 caracteres")]
    public string? AnoDePublicacao { get; set; }

    public bool Lido { get; set; } = false;

    public DateTime DataDeLeitura { get; set; }

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
