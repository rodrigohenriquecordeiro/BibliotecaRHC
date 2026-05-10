
import { LivroProjeto } from "./livroProjeto";
import { ProjetoStatus } from "./projetoStatus";

export interface Projeto {
  id: number;
  nome: string;
  livroProjetos: LivroProjeto[];
  dataCriacao: Date;
  projetoStatus: ProjetoStatus;
}