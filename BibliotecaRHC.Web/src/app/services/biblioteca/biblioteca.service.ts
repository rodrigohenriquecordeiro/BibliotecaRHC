import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Livro } from '../../models/livro';
import { Frase } from '../../models/frase';
import { catchError, Observable, throwError, BehaviorSubject } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class BibliotecaService {

  private readonly API = 'https://localhost:7254/api/';

  private idSelecionadoSource = new BehaviorSubject<number | null>(null);
  idSelecionado$ = this.idSelecionadoSource.asObservable();

  private idSelecionadoFraseSource = new BehaviorSubject<number | null>(null);
  idSelecionadoFrase$ = this.idSelecionadoFraseSource.asObservable();

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
}
