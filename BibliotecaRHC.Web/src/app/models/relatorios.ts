export interface Top10Item {
  descricao: string;
  quantidade: number;
}

export interface Top10LivroLongo {
  nomeDoLivro: string;
  numeroDePaginas: string;
}

export interface Relatorios {
  totalLivros: number;
  livrosLidos: number;
  livrosNaoLidos: number;
  totalEditoras: number;
  totalAutores: number;
  projetosLeituraAndamento: number;
  topClassificacoes: Top10Item[];
  topAutores: Top10Item[];
  topEditoras: Top10Item[];
  topAnosPublicacao: Top10Item[];
  topAnosAquisicao: Top10Item[];
  topLivrosLongos: Top10LivroLongo[];
}