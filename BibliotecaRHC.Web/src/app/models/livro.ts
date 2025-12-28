import { Autor } from "./autor";

export interface Livro {
    id: number;
    nomeDoLivro: string;
    autor: Autor;
    editora: string;
    anoDePublicacao: string;
    numeroDePaginas: string;
    classificacaoCatalografica: string;
    observacao: string;
    dataDeAquisicao: string;
}