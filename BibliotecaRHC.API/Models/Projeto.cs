using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BibliotecaRHC.API.Models;

public class Projeto
{
    public int Id { get; set; }

    [StringLength(100, ErrorMessage = "Permitido no máximo 100 caracteres")]
    public string? Nome { get; set; }

    public ICollection<LivroProjeto> LivroProjetos { get; set; } = [];

    public DateTime? DataCriacao { get; set; }

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
