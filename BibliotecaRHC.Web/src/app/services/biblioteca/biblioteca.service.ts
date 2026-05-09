import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Livro } from '../../models/livro';
import { LivrosPaginados } from '../../models/livrosPaginados';
import { Frase } from '../../models/frase';
import { Projeto } from '../../models/projeto';
import { catchError, Observable, throwError, BehaviorSubject } from 'rxjs';
import { map } from 'rxjs/operators';
import { LivroProjeto } from '../../models/livroProjeto';

@Injectable({ providedIn: 'root' })
export class BibliotecaService {

  private readonly API = 'https://localhost:7254/api/';

  private idSelecionadoSource = new BehaviorSubject<number | null>(null);
  idSelecionado$ = this.idSelecionadoSource.asObservable();

  private idSelecionadoFraseSource = new BehaviorSubject<number | null>(null);
  idSelecionadoFrase$ = this.idSelecionadoFraseSource.asObservable();

  private idSelecionadoProjetoSource = new BehaviorSubject<number | null>(null);
  idSelecionadoProjeto$ = this.idSelecionadoProjetoSource.asObservable();

  private idSelecionadoLivroProjetoSource = new BehaviorSubject<number | null>(null);
  idSelecionadoLivroProjeto$ = this.idSelecionadoLivroProjetoSource.asObservable();

  constructor(private http: HttpClient) { }

  private getAuthHeaders(): HttpHeaders {
    const token = localStorage.getItem('usuarioLogado');
    return new HttpHeaders({
      'Authorization': `Bearer ${token}`
    });
  }

  listar(): Observable<Livro[]> {
    return this.http.get<Livro[]>(`${this.API}livros/obter-livros`, {
      headers: this.getAuthHeaders()
    });
  }

  listarLivros(pagina: number = 1, campo: string = '', valor: string = ''): Observable<LivrosPaginados> {

    if (valor && valor.trim() !== '') {
      const params = new HttpParams()
        .set('campo', campo)
        .set('valor', valor);

      return this.http.get<Livro[]>(`${this.API}livros/filtra-dashboard`, {
        headers: this.getAuthHeaders(),
        params: params
      }).pipe(
        map((livrosFiltrados: Livro[]) => {
          return {
            Livros: livrosFiltrados,
            TotalLivros: livrosFiltrados.length,
            TotalPaginas: 1,
            TemPaginaAnterior: false,
            TemProximaPagina: false
          } as LivrosPaginados;
        })
      );

    } else {
      const params = new HttpParams().set('paginaAtual', pagina.toString());

      return this.http.get<LivrosPaginados>(`${this.API}livros/obter-livros-paginados`, {
        headers: this.getAuthHeaders(),
        params: params
      });
    }
  }

  buscarPorCodigo(id: number): Observable<Livro> {
    return this.http.get<Livro>(`${this.API}livros/obter-livro-por-id/${id}`, {
      headers: this.getAuthHeaders()
    });
  }

  criar(livro: Livro): Observable<Livro> {
    return this.http.post<Livro>(`${this.API}livros/adicionar-livro`, livro, {
      headers: this.getAuthHeaders()
    });
  }

  editar(livro: Livro): Observable<Livro> {
    return this.http.put<Livro>(`${this.API}livros/atualizar-livro/${livro.id}`, livro, {
      headers: this.getAuthHeaders()
    });
  }

  excluir(id: number): Observable<Livro> {
    return this.http.delete<Livro>(`${this.API}livros/remover-livro/${id}`, {
      headers: this.getAuthHeaders()
    }).pipe(
      catchError((error) => {
        console.error('Erro ao excluir:', error);
        return throwError(() => error);
      })
    );
  }

  obterCodigoProximoLivro(): Observable<number> {
    return this.http.get<number>(`${this.API}livros/obter-codigo-proximo-livro`, {
      headers: this.getAuthHeaders()
    });
  }

  setIdSelecionado(id: number): void {
    this.idSelecionadoSource.next(id);
  }

  limparSelecionado(): void {
    this.idSelecionadoSource.next(null);
  }

  listarFrases(): Observable<Frase[]> {
    return this.http.get<Frase[]>(`${this.API}frases/obter-frases`, {
      headers: this.getAuthHeaders()
    }).pipe(
      catchError((error) => {
        console.error('Erro ao listar frases:', error);
        return throwError(() => error);
      })
    );
  }

  criarFrase(frase: Frase): Observable<Frase> {
    return this.http.post<Frase>(`${this.API}frases/adicionar-frase`, frase, {
      headers: this.getAuthHeaders()
    }).pipe(
      catchError((error) => {
        console.error('Erro ao criar frase:', error);
        return throwError(() => error);
      })
    );
  }

  buscarFrasePorCodigo(id: number): Observable<Frase> {
    return this.http.get<Frase>(`${this.API}frases/obter-frase-por-id/${id}`, {
      headers: this.getAuthHeaders()
    });
  }

  editarFrase(frase: Frase): Observable<Frase> {
    return this.http.put<Frase>(`${this.API}frases/atualizar-frase/${frase.id}`, frase, {
      headers: this.getAuthHeaders()
    });
  }

  excluirFrase(id: number): Observable<Frase> {
    return this.http.delete<Frase>(`${this.API}frases/remover-frase/${id}`, {
      headers: this.getAuthHeaders()
    }).pipe(
      catchError((error) => {
        console.error('Erro ao excluir:', error);
        return throwError(() => error);
      })
    );
  }

  oterFraseAleatoria(): Observable<Frase> {
    return this.http.get<Frase>(`${this.API}frases/obter-frase-aleatoria`, {
      headers: this.getAuthHeaders()
    });
  }

  setIdSelecionadoFrase(id: number): void {
    this.idSelecionadoFraseSource.next(id);
  }

  limparFraseSelecionado(): void {
    this.idSelecionadoFraseSource.next(null);
  }

  importarPlanilha(arquivo: File): Observable<any> {
    const formData = new FormData();
    formData.append('arquivo', arquivo);
    return this.http.post(`${this.API}importar-planilha/importar-excel`, formData);
  }

  listaProjetos(): Observable<Projeto[]> {
    return this.http.get<Projeto[]>(`${this.API}projetos/obter-projetos`, {
      headers: this.getAuthHeaders()
    }).pipe(
      catchError((error) => {
        console.error('Erro ao listar projetos:', error);
        return throwError(() => error);
      })
    );
  }

  criarProjeto(projeto: Projeto): Observable<Projeto> {
    return this.http.post<Projeto>(`${this.API}projetos/adicionar-projeto`, projeto, {
      headers: this.getAuthHeaders()
    }).pipe(
      catchError((error) => {
        console.error('Erro ao criar projeto:', error);
        return throwError(() => error);
      })
    );
  }

  buscarProjetoPorCodigo(id: number): Observable<Projeto> {
    return this.http.get<Projeto>(`${this.API}projetos/obter-projeto-por-id/${id}`, {
      headers: this.getAuthHeaders()
    });
  }

  editarProjeto(projeto: Projeto): Observable<Projeto> {
    return this.http.put<Projeto>(`${this.API}projetos/atualizar-projeto/${projeto.id}`, projeto, {
      headers: this.getAuthHeaders()
    });
  }

  excluirProjeto(id: number): Observable<Projeto> {
    return this.http.delete<Projeto>(`${this.API}projetos/remover-projeto/${id}`, {
      headers: this.getAuthHeaders()
    }).pipe(
      catchError((error) => {
        console.error('Erro ao excluir projeto:', error);
        return throwError(() => error);
      })
    );
  }

  setIdSelecionadoProjeto(id: number): void {
    this.idSelecionadoProjetoSource.next(id);
  }

  limparProjetoSelecionado(): void {
    this.idSelecionadoProjetoSource.next(null);
  }

  listaLivrosNoProjetos(): Observable<LivroProjeto[]> {
    return this.http.get<LivroProjeto[]>(`${this.API}livro-projeto/obter-livro-projeto`, {
      headers: this.getAuthHeaders()
    }).pipe(
      catchError((error) => {
        console.error('Erro ao listar livros no projeto:', error);
        return throwError(() => error);
      })
    );
  }

  criarLivroNoProjeto(livroNoProjeto: LivroProjeto): Observable<LivroProjeto> {
    return this.http.post<LivroProjeto>(`${this.API}livro-projeto/adicionar-livro-projeto`, livroNoProjeto, {
      headers: this.getAuthHeaders()
    }).pipe(
      catchError((error) => {
        console.error('Erro ao criar livro no projeto:', error);
        return throwError(() => error);
      })
    );
  }

  buscarLivroNoProjetoPorCodigo(id: number): Observable<LivroProjeto> {
    return this.http.get<LivroProjeto>(`${this.API}livro-projeto/obter-livro-projeto-por-id/${id}`, {
      headers: this.getAuthHeaders()
    });
  }

  editarLivroNoProjeto(livroNoProjeto: LivroProjeto): Observable<LivroProjeto> {
    return this.http.put<LivroProjeto>(`${this.API}livro-projeto/atualizar-livro-projeto/${livroNoProjeto.id}`, livroNoProjeto, {
      headers: this.getAuthHeaders()
    });
  }

  excluirLivroNoProjeto(id: number): Observable<LivroProjeto> {
    return this.http.delete<LivroProjeto>(`${this.API}livro-projeto/remover-livro-projeto/${id}`, {
      headers: this.getAuthHeaders()
    }).pipe(
      catchError((error) => {
        console.error('Erro ao excluir livro no projeto:', error);
        return throwError(() => error);
      })
    );
  }

  setIdSelecionadoLivroProjeto(id: number): void {
    this.idSelecionadoLivroProjetoSource.next(id);
  }

  limparLivroProjetoSelecionado(): void {
    this.idSelecionadoLivroProjetoSource.next(null);
  }

}
