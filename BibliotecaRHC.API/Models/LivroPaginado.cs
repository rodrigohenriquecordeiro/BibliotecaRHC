namespace BibliotecaRHC.API.Models;

public class LivroPaginado
{
    public IEnumerable<Livro> Livros { get; set; } = [];
    public int TotalLivros{ get; set; }
    public int TotalPaginas { get; set; }
    public bool TemPaginaAnterior { get; set; }
    public bool TemProximaPagina { get; set; }
}
