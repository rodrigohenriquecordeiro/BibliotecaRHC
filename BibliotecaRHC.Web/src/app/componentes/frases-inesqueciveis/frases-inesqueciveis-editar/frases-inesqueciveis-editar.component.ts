import { Component } from '@angular/core';
import { FormGroup, Validators, FormControl, ReactiveFormsModule } from '@angular/forms';
import { BibliotecaService } from '../../../services/biblioteca/biblioteca.service';
import { Frase } from '../../../models/frase';
import { firstValueFrom } from 'rxjs';

@Component({
  selector: 'app-frases-inesqueciveis-editar',
  standalone: true,
  imports: [ReactiveFormsModule ],
  templateUrl: './frases-inesqueciveis-editar.component.html',
  styleUrl: './frases-inesqueciveis-editar.component.css'
})
export class FrasesInesqueciveisEditarComponent {

  idFrase: number | null = null;

  form = new FormGroup({
    id: new FormControl<number | null>(null, Validators.required),
    frase: new FormControl<string | null>(null, Validators.required),
    autor: new FormControl<string | null>(null, Validators.required),
    nomeDoLivro: new FormControl<string | null>(null, Validators.required),
    dataCriacao: new FormControl<string | null>(null, Validators.required)
  });

  constructor(private service: BibliotecaService) { }

  ngOnInit(): void {
    this.service.idSelecionadoFrase$.subscribe(id => {
      this.idFrase = id;
      if (id !== null) {
        console.log('ID da Frase selecionado:', id);
        this.carregarFrase(id);
      }
    });
  }

  async carregarFrase(id: number): Promise<void> {
    try {
      const frase = await firstValueFrom(this.service.buscarFrasePorCodigo(id));
      if (frase) {
        console.log('Frase encontrada:', frase);
        this.preencherFormulario(frase);
      } else {
        console.error('Frase não encontrada com o ID:', id);
      }
    } catch (error) {
      console.error('Erro ao buscar a frase:', error);
    }
  }

  preencherFormulario(frase: Frase): void {
    this.form.patchValue({
      id: frase.id,
      frase: frase.frase,
      autor: frase.autor,
      nomeDoLivro: frase.nomeDoLivro,
      dataCriacao: frase.dataCriacao
    })
  }

  async salvarAlteracoes(): Promise<void> {
    if (this.form.invalid) {
      console.error('Formulário inválido.');
      return;
    }

    try {
      const formValue = this.form.value;

      const frase: Frase = {
        id: formValue.id!,
        frase: formValue.frase!,
        autor: formValue.autor!,
        nomeDoLivro: formValue.nomeDoLivro!,
        dataCriacao: formValue.dataCriacao!
      };

      await firstValueFrom(this.service.editarFrase(frase));

      console.log('Alterações salvas com sucesso!');
      this.form.reset();
    } catch (error) {
      console.error('Erro ao salvar as alterações:', error);
    }
  }

}
