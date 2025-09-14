import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../services/auth/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  providers: [AuthService],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent {
  email: string = '';
  senha: string = '';
  erroLogin: string = '';

  constructor(private router: Router, private authService: AuthService) { }

  onLogin() {
    const loginData = {
      UserEmail: this.email,
      Password: this.senha
    };

    this.authService.login(loginData)
      .subscribe({
        next: (response: any) => {
          const token = response.token; 
          console.log('Token recebido:', token);
          this.authService.saveToken(token);
          console.log('Usuário logado com sucesso.');
          this.router.navigate(['/minha-estante']);
        },
        error: (err) => {
          console.error('Erro no login:', err);
          this.erroLogin = 'Credenciais inválidas. Tente novamente.';
        }
      });
  }
}
