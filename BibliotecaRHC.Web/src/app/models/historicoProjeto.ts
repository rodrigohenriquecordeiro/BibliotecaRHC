import { ProjetoStatus } from "./projetoStatus";

export interface HistoricoProjeto {
    id: Number;
    projetoId: Number;
    dataAlteracao: Date;
    projetoStatus: ProjetoStatus
}