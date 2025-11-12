import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Livro } from '../../../../models/livro';
import { BibliotecaService } from '../../../../services/biblioteca/biblioteca.service';
import { Router, RouterModule } from '@angular/router';

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

  constructor(
    private service: BibliotecaService,
    private router: Router
  ) {}

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
    if (this.idLivrosSelecionados.length === 0) {
      console.warn('Nenhum livro selecionado para exclusão.');
      return;
    }

    const idsParaApagar = [...this.idLivrosSelecionados];
    
    const apagarLivro = (id: number) => {
        return new Promise<void>((resolve, reject) => {
            this.service.excluir(id).subscribe({
                next: () => resolve(),
                error: (err) => reject(err)
            });
        });
    };

    Promise.all(idsParaApagar.map(apagarLivro))
        .then(() => {
            this.livros = this.livros.filter(l => !idsParaApagar.includes(l.id));
            this.idLivrosSelecionados = [];
            console.log(`${idsParaApagar.length} livro(s) apagado(s) com sucesso.`);
        })
        .catch(err => {
          console.error('Erro ao excluir um ou mais livros:', err);
        });
  }

  editarLivro() {
    const idParaEditar = this.idLivrosSelecionados[0];
    if (!idParaEditar) {
      console.warn('Nenhum livro selecionado para edição.');
      return;
    }
    
    this.service.setIdSelecionado(idParaEditar);
    this.router.navigate(['/editar']);
  }
}