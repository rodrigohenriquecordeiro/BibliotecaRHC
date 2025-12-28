using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BibliotecaRHC.API.Models;

public class Autor
{
    public int Id { get; set; }
    
    public string? NomeDoAutor { get; set; }

    public ICollection<Livro> Livros { get; set; } = [];

    public ICollection<FraseInesquecivel> FrasesInesqueciveis { get; set; } = [];

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
