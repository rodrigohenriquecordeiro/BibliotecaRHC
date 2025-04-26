import { Component } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { BibliotecaService } from '../../../biblioteca.service';
import { Livro } from '../../../livro';
import { map } from 'rxjs';

@Component({
  selector: 'app-form',
  imports: [ReactiveFormsModule],
  templateUrl: './cadastrar.component.html',
  styleUrl: './cadastrar.component.css'
})
export class CadastrarComponent {

  form = new FormGroup({
    codigoDoLivro: new FormControl<number | null>(null, Validators.required),
    autor: new FormControl<string | null>(null, Validators.required),
    nomeDoLivro: new FormControl<string | null>(null, Validators.required),
    editora: new FormControl<string | null>(null, Validators.required),
    numeroDePaginas: new FormControl<number | null>(null, Validators.required),
    anoDePublicacao: new FormControl<number | null>(null, Validators.required),
    dataDeAquisicao: new FormControl<string | null>(null, Validators.required),
    classificacaoCatalografica: new FormControl<string | null>(null, Validators.required),
    observacao: new FormControl<string | null>(null)
  });

  codigoUltimoLivro: number | null = null; 
  codigoProximoLivro: number | null = null; 

  constructor(private service: BibliotecaService) { }

  ngOnInit(): void {
    this.obterProximoCodigoLivro();
  }

  obterProximoCodigoLivro(): void {
    this.service.obterCodigoUltimoLivro().pipe(
      map((codigo: number | Number) => Number(codigo) + 1)
    ).subscribe({
      next: (codigo: number) => {
        this.codigoUltimoLivro = codigo;
        this.codigoProximoLivro = codigo;
        this.form.get('codigoDoLivro')?.setValue(codigo);
      },
      error: (err) => console.error('Erro ao obter o código do último livro:', err)
    });
  }

  cadastrar(): void {
    if (this.codigoProximoLivro === null) {
      console.error('O código do próximo livro não foi carregado.');
      return;
    }

    const formValue = this.form.value;

    const livro: Livro = {
      id: this.codigoProximoLivro, 
      autor: formValue.autor!,
      nomeDoLivro: formValue.nomeDoLivro!,
      editora: formValue.editora!,
      numeroDePaginas: formValue.numeroDePaginas!,
      anoDePublicacao: formValue.anoDePublicacao!,
      dataDeAquisicao: new Date(formValue.dataDeAquisicao! + 'T00:00:00'),
      classificacaoCatalografica: formValue.classificacaoCatalografica!,
      observacao: formValue.observacao || ''
    };

    this.service.criar(livro).subscribe({
      next: () => {
        this.form.reset();
      },
      error: (err) => console.error('Erro ao cadastrar o livro:', err)
    });
  }
}
