import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Livro } from './livro';
import { catchError, Observable, throwError, BehaviorSubject } from 'rxjs';
import { FraseInesquecivel } from './frase-inesquecivel';

@Injectable({ providedIn: 'root' })
export class BibliotecaService {

  private readonly API = 'http://localhost:5145/api/';
  
  private idSelecionadoSource = new BehaviorSubject<number | null>(null);
  idSelecionado$ = this.idSelecionadoSource.asObservable();

  constructor(private http: HttpClient) { }

  private getAuthHeaders(): HttpHeaders {
    const token = localStorage.getItem('usuarioLogado');
    return new HttpHeaders({
      'Authorization': `Bearer ${token}`
    });
  }

  // #region CRUD Livros
  listar(): Observable<Livro[]> {
    return this.http.get<Livro[]>(`${this.API}livros/obter-livros`, {
      headers: this.getAuthHeaders()
    });
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

  // #endregion

  // #region Frases Inesquecíveis
  criarFrase(frase: FraseInesquecivel): Observable<FraseInesquecivel> {
    return this.http.post<FraseInesquecivel>(`${this.API}frases/adicionar-frase`, frase, {
      headers: this.getAuthHeaders()
    });
  }

  // #endregion
}
