import { Autor } from "./autor";
import { Livro } from "./livro";

export interface Frase {
  id: number;
  frase: string;
  autor: Autor;
  nomeDoLivro: Livro;
  dataCriacao: string; 
}