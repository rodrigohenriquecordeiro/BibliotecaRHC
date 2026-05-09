
import { LivroProjeto } from "./livroProjeto";

export interface Projeto {
  id: number;
  nome: string;
  livroProjetos: LivroProjeto[];
  dataCriacao: Date;
}