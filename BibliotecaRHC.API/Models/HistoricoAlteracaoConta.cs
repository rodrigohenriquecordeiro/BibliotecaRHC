namespace BibliotecaRHC.API.Models;

public class HistoricoAlteracaoConta : IEntidadeDoUsuario
{
    public int Id { get; set; }
    public int UsuarioId { get; set; }
    public DateTime DataAlteracaoUtc { get; set; }
    public string CampoAlterado { get; set; } = string.Empty;
    public string? ValorAnterior { get; set; }
    public string? ValorNovo { get; set; }
}
