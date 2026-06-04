import { Component } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { BibliotecaService } from '../../../services/biblioteca/biblioteca.service';
import { Livro } from '../../../models/livro';
import { firstValueFrom } from 'rxjs';
import { Router, RouterModule } from '@angular/router';

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
    observacao: new FormControl<string | null>(null),
    lido: new FormControl<boolean>(false),
    anoUltimaLeitura: new FormControl<number | null>(null)
  });

  constructor(
    private service: BibliotecaService,
    private router: Router
  ) { }

  async ngOnInit(): Promise<void> {
    try {
      const proximoCodigo = await firstValueFrom(this.service.obterCodigoProximoLivro());
      const codigoLivro = Number(proximoCodigo);

      this.form.get('codigoDoLivro')?.setValue(codigoLivro);
    } catch (error) {
      console.error('Erro ao buscar o código do livro:', error);
    }

    this.ControlaExibicaoCampoLido();
  }

  private ControlaExibicaoCampoLido() {
    this.form.get('lido')?.valueChanges.subscribe(isLido => {
      const anoControl = this.form.get('anoUltimaLeitura');

      if (isLido) {
        anoControl?.setValidators([Validators.required]);
      } else {
        anoControl?.reset();
        anoControl?.clearValidators();
      }

      anoControl?.updateValueAndValidity();
    });
  }

  async cadastrar(): Promise<void> {
    if (this.form.invalid) {
      console.error('Formulário inválido.');

      Object.keys(this.form.controls).forEach(key => {
        const control = this.form.get(key);
        if (control && control.invalid) {
          console.log(`Campo inválido: ${key} | Erros:`, control.errors);
        }
      });

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
        observacao: formValue.observacao || '',
        lido: formValue.lido!,
        anoUltimaLeitura: formValue.anoUltimaLeitura!
      };

      await firstValueFrom(this.service.criar(livro));

      console.log('Livro cadastrado com sucesso!');
      this.form.reset();
      this.router.navigate(['/minha-estante']);
    } catch (error) {
      console.error('Erro ao cadastrar o livro:', error);
    }
  }

}
