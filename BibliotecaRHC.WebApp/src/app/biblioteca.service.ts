import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Livro } from './livro';
import { catchError, Observable, throwError, BehaviorSubject } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class BibliotecaService {

  private readonly API = 'http://localhost:5145/api/livros';
  
  private idSelecionadoSource = new BehaviorSubject<number | null>(null);
  idSelecionado$ = this.idSelecionadoSource.asObservable();

  constructor(private http: HttpClient) { }

  private getAuthHeaders(): HttpHeaders {
    const token = localStorage.getItem('usuarioLogado');
    return new HttpHeaders({
      'Authorization': `Bearer ${token}`
    });
  }

  listar(): Observable<Livro[]> {
    return this.http.get<Livro[]>(`${this.API}/obter-livros`, {
      headers: this.getAuthHeaders()
    });
  }

  buscarPorCodigo(id: number): Observable<Livro> {
    return this.http.get<Livro>(`${this.API}/obter-livro-por-id/${id}`, {
      headers: this.getAuthHeaders()
    });
  }

  criar(livro: Livro): Observable<Livro> {
    return this.http.post<Livro>(`${this.API}/adicionar-livro`, livro, {
      headers: this.getAuthHeaders()
    });
  }

  editar(livro: Livro): Observable<Livro> {
    return this.http.put<Livro>(`${this.API}/atualizar-livro/${livro.id}`, livro, {
      headers: this.getAuthHeaders()
    });
  }

  excluir(id: number): Observable<Livro> {
    return this.http.delete<Livro>(`${this.API}/remover-livro/${id}`, {
      headers: this.getAuthHeaders()
    }).pipe(
      catchError((error) => {
        console.error('Erro ao excluir:', error);
        return throwError(() => error);
      })
    );
  }

  obterCodigoProximoLivro(): Observable<number> {
    return this.http.get<number>(`${this.API}/obter-codigo-proximo-livro`, {
      headers: this.getAuthHeaders()
    });
  }

  setIdSelecionado(id: number): void {
    this.idSelecionadoSource.next(id);
  }

  limparSelecionado(): void {
    this.idSelecionadoSource.next(null);
  }
}
