
import { LivroProjeto } from "./livroProjeto";

export interface Projeto {
  id: number;
  nome: string;
  livros: LivroProjeto[];
}