import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Livro } from '../../../../livro';
import { BibliotecaService } from '../../../../biblioteca.service';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-minha-estante',
  imports: [FormsModule, CommonModule, RouterModule],
  templateUrl: './minha-estante.component.html',
  styleUrl: './minha-estante.component.css'
})
export class MinhaEstanteComponent {
  livros: Livro[] = []
  selectedLivros: number[] = [];
  livroSelecionado: Livro | null = null;

  constructor(private service: BibliotecaService) {}

  ngOnInit(): void {
    this.service.listar().subscribe((livros) => {
      this.livros = livros
    });
  }

  onSelecionarLivro(event: any, id: number) {
    if (event.target.checked) {
      this.service.setIdSelecionado(id);
    } 
  }

  confirmarApagar() {
    if (this.livroSelecionado) {
      this.service.excluir(this.livroSelecionado.id).subscribe(() => {
        this.livros = this.livros.filter(livro => livro.id !== this.livroSelecionado?.id);
        this.selectedLivros = [];
        this.livroSelecionado = null;
      });
    }
  }

}
