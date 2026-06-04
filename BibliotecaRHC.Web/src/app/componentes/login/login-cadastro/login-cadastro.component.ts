import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../../services/auth/auth.service';

@Component({
  selector: 'app-login-cadastro',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './login-cadastro.component.html',
  styleUrls: ['./login-cadastro.component.css']
})
export class LoginCadastroComponent implements OnInit {
  form!: FormGroup;
  erroCadastro: string = '';

  constructor(
    private fb: FormBuilder,
    private router: Router,
    private authService: AuthService
  ) { }

  ngOnInit(): void {
    this.form = this.fb.group({
      nome: ['', [Validators.required, Validators.minLength(3)]],
      email: ['', [Validators.required, Validators.email]],
      senha: ['', [Validators.required, Validators.minLength(6)]],
      confirmarSenha: ['', Validators.required]
    }, {
      validators: this.senhasConferemValidator
    });
  }

  private senhasConferemValidator(group: FormGroup) {
    const senha = group.get('senha')?.value;
    const confirmarSenha = group.get('confirmarSenha')?.value;
    return senha === confirmarSenha ? null : { senhasDiferentes: true };
  }

  cadastrarUsuario() {
    if (this.form.invalid) {
      this.erroCadastro = 'Por favor, preencha todos os campos corretamente.';
      return;
    }

    const formValue = this.form.value;

    const payloadCadastro = {
      nome: String(formValue.nome).trim(),
      email: String(formValue.email).trim(),
      senha: String(formValue.senha),
      dataCriacao: new Date().toISOString()
    };

    this.authService.registrar(payloadCadastro).subscribe({
      next: (response: any) => {
        console.log('Usuário cadastrado com sucesso!', response);

        this.erroCadastro = 'Cadastro realizado com sucesso!';

        setTimeout(() => {
          this.router.navigate(['/minha-estante']);
        }, 1000);
      },
      error: (err) => {
        console.error('Erro detalhado:', err);

        if (err.error && err.error.errors) {
          const listaErros = Object.values(err.error.errors).flat().join(' ');
          this.erroCadastro = listaErros;
        } else if (err.error && err.error.message) {
          this.erroCadastro = err.error.message;
        } else {
          this.erroCadastro = 'A validação do servidor falhou. Certifique-se de usar uma senha com letra Maiúscula, números e caracteres especiais.';
        }
      }
    });
  }
}