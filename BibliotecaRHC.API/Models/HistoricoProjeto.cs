namespace BibliotecaRHC.API.Models;

public class HistoricoProjeto
{
    public int Id { get; set; }

    public int ProjetoId { get; set; }

    public DateTime? DataAlteracao { get; set; }

    public ProjetoStatus ProjetoStatus { get; set; }
}
