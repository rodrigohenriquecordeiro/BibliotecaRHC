import { Component, inject } from '@angular/core';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common'; 
import { AuthService } from '../services/auth.service'; 
import { HttpErrorResponse } from '@angular/common/http';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css'],
  standalone: true,
  imports: [FormsModule, CommonModule]
})
export class LoginComponent {
  userEmail: string = '';
  password: string = '';
  errorMessage: string | null = null;
  
  private authService = inject(AuthService);
  private router = inject(Router);

  login(): void {
    if (this.userEmail && this.password) {
      const credentials = {
        userEmail: this.userEmail,
        password: this.password
      };

      this.authService.login(credentials).subscribe({
        next: (response) => {
          console.log('Login bem-sucedido!', response);
          this.errorMessage = null;
          this.router.navigate(['/dashboard']); 
        },
        error: (err: HttpErrorResponse) => {
          console.error('Erro no login:', err);
          this.errorMessage = 'E-mail ou senha inválidos. Tente novamente.';
        }
      });
    } else {
      this.errorMessage = 'Por favor, preencha o e-mail e a senha.';
    }
  }
}