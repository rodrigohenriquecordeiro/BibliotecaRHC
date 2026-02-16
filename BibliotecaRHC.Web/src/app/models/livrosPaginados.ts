import { Livro } from './livro'; 

export interface LivrosPaginados {
    Livros: Livro[];
    TotalLivros: Number;
    TotalPaginas: Number;
    TemPaginaAnterior: Boolean;
    TemProximaPagina: Boolean;
} 
  