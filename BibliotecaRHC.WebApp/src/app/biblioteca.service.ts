import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Livro } from './livro';
import { catchError, Observable, throwError } from 'rxjs';
import { BehaviorSubject } from 'rxjs';

@Injectable({providedIn: 'root'})

export class BibliotecaService {

  private readonly API = 'http://localhost:5145/api/livros';
  
  private idSelecionadoSource = new BehaviorSubject<number | null>(null);
  idSelecionado$ = this.idSelecionadoSource.asObservable();

  constructor(private http: HttpClient) { }

  listar(): Observable<Livro[]> {
    return this.http.get<Livro[]>(`${this.API}/obter-livros`);
  }

  buscarPorCodigo(id: number) {
    return this.http.get<Livro>(`${this.API}/obter-livro-por-id/${id}`);
  }

  criar(livro: Livro): Observable<Livro> {
    return this.http.post<Livro>(`${this.API}/adicionar-livro`, livro);
  }

  editar(livro: Livro) {
    return this.http.put<Livro>(`${this.API}/atualizar-livro/${livro.id}`, livro);
  }

  excluir(id: number) {
    const url = `${this.API}/${id}`;
    console.log(`Tentando excluir: ${url}`);
    return this.http.delete<Livro>(url).pipe(
      catchError((error) => {
        console.error('Erro ao excluir:', error);
        return throwError(error);
      })
    );
  }
  
  obterCodigoProximoLivro(): Observable<Number> {
    return this.http.get<Number>(`${this.API}/obter-codigo-proximo-livro`);
  }

  setIdSelecionado(id: number): void {
    this.idSelecionadoSource.next(id);
  }

  limparSelecionado(): void {
    this.idSelecionadoSource.next(null);
  }

}
