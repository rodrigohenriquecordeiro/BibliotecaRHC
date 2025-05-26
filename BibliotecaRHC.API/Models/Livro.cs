using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BibliotecaRHC.API.Models;

public class Livro
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Obrigatório colocar o Autor")]
    [StringLength(200, ErrorMessage = "Permitido no máximo 200 caracteres")]
    public string? Autor { get; set; }

    [Required(ErrorMessage = "Obrigatório colocar o Livro")]
    [StringLength(300, ErrorMessage = "Permitido no máximo 300 caracteres")]
    public string? NomeDoLivro { get; set; }

    [Required(ErrorMessage = "Obrigatório colocar a Editora")]
    [StringLength(100, ErrorMessage = "Permitido no máximo 100 caracteres")]
    public string? Editora { get; set; }

    [Required(ErrorMessage = "Obrigatório colocar o Ano de Publicação")]
    [StringLength(10, ErrorMessage = "Permitido no máximo 10 caracteres")]
    public string? AnoDePublicacao { get; set; }

    [Required(ErrorMessage = "Obrigatório colocar o Número de Páginas")]
    [Range(minimum: 1, maximum: 10000, ErrorMessage = "Número de páginas excede o permitido")]
    [RegularExpression("([0-9]+)", ErrorMessage = "Número de Páginas somente aceita valores numéricos")]
    public int? NumeroDePaginas { get; set; }

    [Required(ErrorMessage = "Obrigatório colocar a Classificação Catalográfica")]
    [StringLength(100, ErrorMessage = "Permitido no máximo 100 caracteres")]
    public string? ClassificacaoCatalografica { get; set; }

    [Required(AllowEmptyStrings = true)]
    [StringLength(300, ErrorMessage = "Permitido no máximo 300 caracteres")]
    public string? Observacao { get; set; }

    [Required(ErrorMessage = "Obrigatório colocar a Data de Aquisição")]
    public string? DataDeAquisicao { get; set; }

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
