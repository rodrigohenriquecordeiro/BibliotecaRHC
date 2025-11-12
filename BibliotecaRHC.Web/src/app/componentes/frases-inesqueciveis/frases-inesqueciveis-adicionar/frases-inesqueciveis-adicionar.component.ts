import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';

@Component({
  selector: 'app-frases-inesqueciveis-adicionar',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule
  ],
  templateUrl: './frases-inesqueciveis-adicionar.component.html',
  styleUrl: './frases-inesqueciveis-adicionar.component.css'
})
export class FrasesInesqueciveisAdicionarComponent implements OnInit {

  form!: FormGroup;
  visible: boolean = false;

  constructor(private fb: FormBuilder) { }

  ngOnInit(): void {
    this.form = this.fb.group({
      autor: ['', Validators.required],
      livro: ['', Validators.required],
      frase: ['', Validators.required]
    });
  }

  close() {
    console.log('Modal fechado.');
    this.visible = false;
  }

  salvar() {
    if (this.form.valid) {
      console.log('Salvando:', this.form.value);
      this.close(); 
    } else {
      console.error('Formulário inválido.');
      this.form.markAllAsTouched();
    }
  }
}
