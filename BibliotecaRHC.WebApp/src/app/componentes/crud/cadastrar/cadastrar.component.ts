import { Component } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { BibliotecaService } from '../../../biblioteca.service';
import { Livro } from '../../../livro';
import { map, Observable } from 'rxjs';
import { Data } from '@angular/router';

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

  constructor(private service: BibliotecaService) { }

  ngOnInit(): void {
    this.ultimoCodigoDoLivro().subscribe({
      next: (codigo) => {
        this.form.patchValue({ codigoDoLivro: codigo + 1 });
      },
      error: (err) => console.error('Erro ao obter o último código do livro:', err)
    });
  }

  cadastrar(): void {
    const formValue = this.form.value;

    const livro: Livro = {
      id: formValue.codigoDoLivro!,
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
      next: () => this.form.reset(),
      error: (err) => console.error('Erro ao cadastrar o livro:', err)
    });
  }

  ultimoCodigoDoLivro(): Observable<number> {
    return this.service.listar().pipe(
      map(livros => livros.reduce((acc, livro) => Math.max(acc, livro.id), 0))
    );
  }
  
}
