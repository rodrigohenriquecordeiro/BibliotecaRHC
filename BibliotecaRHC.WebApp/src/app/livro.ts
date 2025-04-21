export interface Livro {
    id: number;
    escritor: string;
    livro: string;
    editora: string;
    numeroDePaginas: number;
    anoDePublicacao: number;
    dataDeAquisicao: Date;
    classificacaoCatalografica: string;
    observacao: string;
}