import { Component } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { BibliotecaService } from '../../../services/biblioteca/biblioteca.service';
import { Livro } from '../../../models/livro';
import { firstValueFrom } from 'rxjs';

@Component({
  selector: 'app-form',
  imports: [ReactiveFormsModule],
  templateUrl: './cadastrar.component.html',
  styleUrl: './cadastrar.component.css'
})

export class CadastrarComponent {

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

  constructor(private service: BibliotecaService) { }

  async ngOnInit(): Promise<void> {
    try {
      const proximoCodigo = await firstValueFrom(this.service.obterCodigoProximoLivro());
      const codigoLivro = Number(proximoCodigo);
  
      this.form.get('codigoDoLivro')?.setValue(codigoLivro);
    } catch (error) {
      console.error('Erro ao buscar o código do livro:', error);
    }
  }

  async cadastrar(): Promise<void> {
    if (this.form.invalid) {
      console.error('Formulário inválido.');
      return;
    }

    try {
      const proximoCodigo = await firstValueFrom(this.service.obterCodigoProximoLivro());
      const codigoLivro = Number(proximoCodigo);
      
      this.form.get('codigoDoLivro')?.setValue(codigoLivro);
      const formValue = this.form.value;

      let dataFormatada: string | null = null;
      if (formValue.dataDeAquisicao) {
        const data = new Date(formValue.dataDeAquisicao);
        dataFormatada = data.toISOString().split('T')[0];
      }

      const livro: Livro = {
        id: codigoLivro,
        nomeDoLivro: formValue.nomeDoLivro!,
        autor: formValue.autor!,
        editora: formValue.editora!,
        numeroDePaginas: formValue.numeroDePaginas!.toString(),
        anoDePublicacao: formValue.anoDePublicacao!.toString(),
        dataDeAquisicao: dataFormatada!,
        classificacaoCatalografica: formValue.classificacaoCatalografica!,
        observacao: formValue.observacao || ''
      };

      await firstValueFrom(this.service.criar(livro));

      console.log('Livro cadastrado com sucesso!');
      this.form.reset();
    } catch (error) {
      console.error('Erro ao cadastrar o livro:', error);
    }
  }

}
