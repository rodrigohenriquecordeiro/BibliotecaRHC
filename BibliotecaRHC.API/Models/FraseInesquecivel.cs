using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BibliotecaRHC.API.Models;

public class FraseInesquecivel
{
    public int Id { get; set; }
    
    public string? Frase { get; set; }

    public int? AutorId { get; set; }

    public Autor? Autor { get; set; }

    public int? LivroId { get; set; }

    public Livro? Livro { get; set; }

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
