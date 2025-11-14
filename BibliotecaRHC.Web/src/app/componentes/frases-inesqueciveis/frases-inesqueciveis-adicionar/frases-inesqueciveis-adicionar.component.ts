import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormGroup, Validators, FormControl } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { BibliotecaService } from '../../../services/biblioteca/biblioteca.service';
import { Frase } from '../../../models/frase';
import { firstValueFrom } from 'rxjs';

@Component({
  selector: 'app-frases-inesqueciveis-adicionar',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterModule
  ],
  templateUrl: './frases-inesqueciveis-adicionar.component.html',
  styleUrl: './frases-inesqueciveis-adicionar.component.css'
})

export class FrasesInesqueciveisAdicionarComponent {

  form = new FormGroup({
    frase: new FormControl<string | null>(null, Validators.required),
    autor: new FormControl<string | null>(null, Validators.required),
    nomeLivro: new FormControl<string | null>(null, Validators.required),
  });

  constructor(
    private service: BibliotecaService,
    private router: Router
  ) { }

  ngOnInit(): void { }

  async salvar(): Promise<void> {
    this.form.markAllAsTouched();

    if (this.form.invalid) {
      console.error('Formulário inválido.');
      return;
    }

    const formValue = this.form.value;
    const dataFormatada = new Date().toISOString().split('T')[0];

    try {
      const frase: Frase = {
        id: 0,
        frase: formValue.frase!,
        autor: formValue.autor!,
        nomeDoLivro: formValue.nomeLivro!,
        dataCriacao: dataFormatada!
      };

      await firstValueFrom(this.service.criarFrase(frase));
      this.form.reset();
      this.router.navigate(['/frases-inesqueciveis-dashboard']);
    } catch (error) {
      console.error('Erro ao cadastrar frase:', error);
    }
  }
} 
