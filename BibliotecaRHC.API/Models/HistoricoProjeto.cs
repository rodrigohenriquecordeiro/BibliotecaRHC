using System.Text.Json.Serialization;

namespace BibliotecaRHC.API.Models;

public class HistoricoProjeto : IEntidadeDoUsuario
{
    public int Id { get; set; }
    public int UsuarioId { get; set; }

    public int ProjetoId { get; set; }

    [JsonIgnore]
    public Projeto? Projeto { get; set; }

    public DateTime? DataAlteracao { get; set; }

    public ProjetoStatus ProjetoStatus { get; set; }
}
