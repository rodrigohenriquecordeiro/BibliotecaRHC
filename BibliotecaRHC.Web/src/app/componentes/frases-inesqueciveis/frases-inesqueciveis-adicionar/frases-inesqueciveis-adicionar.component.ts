import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormGroup, Validators, FormControl } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { BibliotecaService } from '../../../services/biblioteca/biblioteca.service';
import { Frase } from '../../../models/frase';
import { firstValueFrom } from 'rxjs';
import e from 'express';

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
export class FrasesInesqueciveisAdicionarComponent implements OnInit {

  form = new FormGroup({
    frase: new FormControl<string | null>(null, Validators.required),
    autor: new FormControl<string | null>(null, Validators.required),
    nomeLivro: new FormControl<string | null>(null, Validators.required),
  });

  listaAutoresUnicos: string[] = [];
  mostrarInputAutor: boolean = false;
  autorSelecionado: string = ''; 

  listaLivrosUnicos: string[] = [];
  mostrarInputLivro: boolean = false;
  livroSelecionado: string = ''; 

  constructor(
    private service: BibliotecaService,
    private router: Router
  ) { }

  ngOnInit(): void {
    this.service.listar().subscribe({
      next: (todosOsLivros) => {
        const autoresDuplicados = todosOsLivros.map(livro => livro.autor).filter(a => !!a);
        this.listaAutoresUnicos = [...new Set(autoresDuplicados)].sort();

        const livrosDuplicados = todosOsLivros.map(livro => livro.nomeDoLivro).filter(a => !!a);
        this.listaLivrosUnicos = [...new Set(livrosDuplicados)].sort();
      },
      error: (err) => {
        console.error('Falha ao carregar os autores', err);
      }
    });
  }

  onAutorChange(event: Event) {
    const selectElement = event.target as HTMLSelectElement;
    const valor = selectElement.value;

    this.autorSelecionado = valor;

    if (valor === 'novo') {
      this.mostrarInputAutor = true;
      this.form.get('autor')?.setValue(''); 
      this.form.get('autor')?.enable();
    } else {
      this.mostrarInputAutor = false;
      this.form.get('autor')?.setValue(valor); 
    }
  }

  onLivroChange(event: Event) {
    const selectElement = event.target as HTMLSelectElement;
    const valor = selectElement.value;

    this.livroSelecionado = valor;

    if (valor === 'novo') {
      this.mostrarInputLivro = true;
      this.form.get('nomeLivro')?.setValue(''); 
      this.form.get('nomeLivro')?.enable();
    } else {
      this.mostrarInputLivro = false;
      this.form.get('nomeLivro')?.setValue(valor); 
    }
  }

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

      // Reseta visualmente
      this.autorSelecionado = '';
      this.mostrarInputAutor = false;

      this.livroSelecionado = '';
      this.mostrarInputLivro = false;

      this.router.navigate(['/frases-inesqueciveis-dashboard']);
    } catch (error) {
      console.error('Erro ao cadastrar frase:', error);
    }
  }
}
