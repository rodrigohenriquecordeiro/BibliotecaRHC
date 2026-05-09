export interface LivroProjeto {
  id: number;
  nome: string;
  anoDePublicacao: string;
  lido: boolean;
  dataDeLeitura: string | null;
  projetoId: number;
}