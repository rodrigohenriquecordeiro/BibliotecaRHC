import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Livro } from '../../../../livro';
import { BibliotecaService } from '../../../../biblioteca.service';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-minha-estante',
  standalone: true,
  imports: [FormsModule, CommonModule, RouterModule],
  templateUrl: './minha-estante.component.html',
  styleUrl: './minha-estante.component.css'
})
export class MinhaEstanteComponent implements OnInit {
  livros: Livro[] = [];
  idLivrosSelecionados: number[] = [];

  constructor(private service: BibliotecaService) {}

  ngOnInit(): void {
    this.service.listar().subscribe({
      next: livros => {
        this.livros = livros;
      },
      error: err => console.error('MinhaEstanteComponent: Erro ao buscar livros:', err) 
    });
  }

  onSelecionarLivro(event: any, id: number) {
    if (event.target.checked) {
      this.idLivrosSelecionados.push(id);
    } else {
      this.idLivrosSelecionados = this.idLivrosSelecionados.filter(i => i !== id);
    }
  }

  getLivroSelecionado(): Livro | undefined {
    const id = this.idLivrosSelecionados[0];
    return this.livros.find(livro => livro.id === id);
  }

  confirmarApagar() {
    const idParaApagar = this.idLivrosSelecionados[0];

    this.service.excluir(idParaApagar).subscribe({
      next: () => {
        this.livros = this.livros.filter(l => l.id !== idParaApagar);
        this.idLivrosSelecionados = [];
      },
      error: err => console.error('Erro ao excluir o livro:', err)
    });
  }
}
