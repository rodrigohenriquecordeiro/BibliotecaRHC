import { Component } from '@angular/core';
import { BibliotecaService } from '../../../services/biblioteca/biblioteca.service';
import { Livro } from '../../../models/livro';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { firstValueFrom } from 'rxjs';
import { ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-editar',
  imports: [CommonModule,
    ReactiveFormsModule],
  templateUrl: './editar.component.html',
  styleUrl: './editar.component.css'
})

export class EditarComponent {

  idLivro: number | null = null;

  form = new FormGroup({
    codigoDoLivro: new FormControl<number | null>(null, Validators.required),
    nomeDoLivro: new FormControl<string | null>(null, Validators.required),
    autor: new FormControl<string | null>(null, Validators.required),
    editora: new FormControl<string | null>(null, Validators.required),
    numeroDePaginas: new FormControl<number | null>(null, Validators.required),
    anoDePublicacao: new FormControl<number | null>(null, Validators.required),
    dataDeAquisicao: new FormControl<string | null>(null, Validators.required),
    classificacaoCatalografica: new FormControl<string | null>(null, Validators.required),
    observacao: new FormControl<string | null>(null)
  });

  constructor(private service: BibliotecaService) {}

  ngOnInit(): void {
    this.service.idSelecionado$.subscribe(id => {
      this.idLivro = id;
      if (id !== null) {
        console.log('ID do livro selecionado:', id);
        this.carregarLivro(id);
      }
    });
  }
  
  async carregarLivro(id: number): Promise<void> {
    try {
      const livro = await firstValueFrom(this.service.buscarPorCodigo(id));
      if (livro) {
        console.log('Livro encontrado:', livro);
        this.preencherFormulario(livro);
      } else {
        console.error('Livro não encontrado com o ID:', id);
      }
    } catch (error) {
      console.error('Erro ao buscar o livro:', error);
    }
  }

  preencherFormulario(livro: Livro): void {
  this.form.patchValue({
    codigoDoLivro: livro.id,
    nomeDoLivro: livro.nomeDoLivro,
    autor: livro.autor,
    editora: livro.editora,
    numeroDePaginas: Number(livro.numeroDePaginas),
    anoDePublicacao: Number(livro.anoDePublicacao),
    dataDeAquisicao: livro.dataDeAquisicao,
    classificacaoCatalografica: livro.classificacaoCatalografica,
    observacao: livro.observacao
  });
}
  
  async salvarAlteracoes(): Promise<void> {
    if (this.form.invalid) {
      console.error('Formulário inválido.');
      return;
    }

    try {
      const formValue = this.form.value;

      const livro: Livro = {
        id: this.idLivro!, 
        nomeDoLivro: formValue.nomeDoLivro!,
        autor: formValue.autor!,
        editora: formValue.editora!,
        numeroDePaginas: formValue.numeroDePaginas!.toString(),
        anoDePublicacao: formValue.anoDePublicacao!.toString(),
        dataDeAquisicao: formValue.dataDeAquisicao!,
        classificacaoCatalografica: formValue.classificacaoCatalografica!,
        observacao: formValue.observacao || ''
      };

      await firstValueFrom(this.service.editar(livro));

      console.log('Alterações salvas com sucesso!');
      this.form.reset();
    } catch (error) {
      console.error('Erro ao salvar as alterações:', error);
    }
  }
}
