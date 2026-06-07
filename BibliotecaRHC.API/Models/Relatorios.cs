namespace BibliotecaRHC.API.Models;

public class Relatorios
{
    public int Id { get; set; }
    public int TotalLivros { get; set; }
    public int LivrosLidos { get; set; }
    public int LivrosNaoLidos { get; set; }
    public int TotalEditoras { get; set; }
    public int TotalAutores { get; set; }
    public int ProjetosLeituraAndamento { get; set; }
    public IEnumerable<Top10Item> TopClassificacoes { get; set; } = [];
    public IEnumerable<Top10Item> TopAutores { get; set; } = [];
    public IEnumerable<Top10Item> TopEditoras { get; set; } = [];
    public IEnumerable<Top10Item> TopAnosPublicacao { get; set; } = [];
    public IEnumerable<Top10Item> TopAnosAquisicao { get; set; } = [];
    public IEnumerable<Top10LivroLongo> TopLivrosLongos { get; set; } = [];
}

public class Top10Item
{
    public string Descricao { get; set; } = string.Empty;
    public int Quantidade { get; set; }
}

public class Top10LivroLongo
{
    public string NomeDoLivro { get; set; } = string.Empty;
    public int NumeroDePaginas { get; set; }
}