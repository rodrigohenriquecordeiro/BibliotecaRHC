import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Router } from '@angular/router';
import { environment } from '../../../environments/environment.development';
import { BehaviorSubject, Observable, tap } from 'rxjs';

export interface DadosConta {
  nome: string;
  email: string;
}

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private API = `${environment.apiUrl}/auth`;
  private readonly contaApi = `${environment.apiUrl}/conta`;
  private readonly usuarioAtualSource = new BehaviorSubject<DadosConta | null>(null);
  readonly usuarioAtual$ = this.usuarioAtualSource.asObservable();

  constructor(private http: HttpClient, private router: Router) { }

  login(credentials: { UserEmail: string; Password: string }) {
    return this.http.post(`${this.API}/login`, credentials);
  }

  registrar(dadosUsuario: any): Observable<any> {
    const headers = new HttpHeaders({ 'Content-Type': 'application/json' });
    return this.http.post(`${this.API}/register`, dadosUsuario, { headers });
  }

  obterConta(): Observable<{ nome: string; email: string }> {
    return this.http.get<DadosConta>(this.contaApi).pipe(
      tap(conta => this.usuarioAtualSource.next(conta))
    );
  }

  atualizarConta(dados: {
    nome: string;
    email: string;
    senhaAtual?: string;
    novaSenha?: string;
  }): Observable<{
    accessToken: string;
    usuario: { nome: string; email: string };
  }> {
    return this.http.put<{
      accessToken: string;
      usuario: { nome: string; email: string };
    }>(this.contaApi, dados).pipe(
      tap(resposta => this.usuarioAtualSource.next(resposta.usuario))
    );
  }
  
  logout() {
    localStorage.removeItem('token');
    this.router.navigate(['/login']);
  }

  saveToken(token: string) {
    localStorage.setItem('token', token);
  }

  getToken(): string | null {
    return localStorage.getItem('token');
  }

  isAuthenticated(): boolean {
    return !!this.getToken();
  }
}