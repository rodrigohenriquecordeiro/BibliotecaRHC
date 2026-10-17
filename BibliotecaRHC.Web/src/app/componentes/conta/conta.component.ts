import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { AuthService } from '../../services/auth/auth.service';

@Component({
  selector: 'app-conta',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './conta.component.html',
  styleUrl: './conta.component.css'
})
export class ContaComponent implements OnInit {
  form!: FormGroup;
  carregando = true;
  salvando = false;
  mensagemErro = '';
  mensagemSucesso = '';

  constructor(
    private readonly formBuilder: FormBuilder,
    private readonly authService: AuthService
  ) { }

  ngOnInit(): void {
    this.form = this.formBuilder.group({
      nome: ['', [Validators.required, Validators.minLength(3)]],
      email: ['', [Validators.required, Validators.email]],
      senhaAtual: [''],
      novaSenha: ['', Validators.minLength(6)],
      confirmarSenha: ['']
    }, { validators: this.validarSenhas });

    this.authService.obterConta().subscribe({
      next: conta => {
        this.form.patchValue({ nome: conta.nome, email: conta.email });
        this.carregando = false;
      },
      error: erro => {
        console.error('Erro ao carregar os dados da conta:', erro);
        this.mensagemErro = erro.error?.message || 'Não foi possível carregar os dados da conta.';
        this.carregando = false;
      }
    });
  }

  salvar(): void {
    this.mensagemErro = '';
    this.mensagemSucesso = '';
    this.form.markAllAsTouched();

    if (this.form.invalid) {
      return;
    }

    const dados = this.form.getRawValue();
    this.salvando = true;

    this.authService.atualizarConta({
      nome: String(dados.nome).trim(),
      email: String(dados.email).trim(),
      senhaAtual: dados.novaSenha ? dados.senhaAtual : undefined,
      novaSenha: dados.novaSenha || undefined
    }).subscribe({
      next: resposta => {
        if (resposta.accessToken) {
          this.authService.saveToken(resposta.accessToken);
        }

        this.form.patchValue({
          nome: resposta.usuario.nome,
          email: resposta.usuario.email,
          senhaAtual: '',
          novaSenha: '',
          confirmarSenha: ''
        });
        this.mensagemSucesso = 'Dados da conta atualizados com sucesso.';
        this.salvando = false;
      },
      error: erro => {
        console.error('Erro ao atualizar os dados da conta:', erro);
        this.mensagemErro = erro.error?.message || 'Não foi possível atualizar os dados da conta.';
        this.salvando = false;
      }
    });
  }

  private validarSenhas(group: FormGroup) {
    const senhaAtual = group.get('senhaAtual')?.value;
    const novaSenha = group.get('novaSenha')?.value;
    const confirmarSenha = group.get('confirmarSenha')?.value;

    if (novaSenha && !senhaAtual) {
      return { senhaAtualObrigatoria: true };
    }

    if (novaSenha !== confirmarSenha) {
      return { senhasDiferentes: true };
    }

    return null;
  }
}
