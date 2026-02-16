import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Livro } from '../../../../models/livro';
import { BibliotecaService } from '../../../../services/biblioteca/biblioteca.service';
import { Router, RouterModule } from '@angular/router';
import { Frase } from '../../../../models/frase';
import { SimNaoPipe } from '../../../../pipes/sim-nao.pipe';

@Component({
  selector: 'app-minha-estante',
  standalone: true,
  imports: [FormsModule, CommonModule, RouterModule, SimNaoPipe],
  templateUrl: './minha-estante.component.html',
  styleUrl: './minha-estante.component.css'
})
export class MinhaEstanteComponent implements OnInit {
  livros: Livro[] = [];
  idLivrosSelecionados: number[] = [];
  fraseDestaque: Frase | undefined;
  paginaAtual: number = 1;
  totalPaginas: number = 0;
  temProximaPagina: boolean = false;
  temPaginaAnterior: boolean = false;

  constructor(
    private service: BibliotecaService,
    private router: Router
  ) { }

  ngOnInit(): void {
    this.carregarLivros(1);
    this.GerarFraseAleatoria();
  }

  carregarLivros(pagina: number): void {
    console.log('Tentando carregar a página:', pagina);

    if (pagina < 1 || (this.totalPaginas > 0 && pagina > this.totalPaginas)) {
      return;
    }

    this.service.listarPaginados(pagina).subscribe({
      next: (dados: any) => { 
        this.livros = dados.livros || dados.Livros;
        this.totalPaginas = dados.totalPaginas || dados.TotalPaginas;
        this.temPaginaAnterior = dados.temPaginaAnterior || dados.TemPaginaAnterior;
        this.temProximaPagina = dados.temProximaPagina || dados.TemProximaPagina;
        
        this.paginaAtual = pagina;
        this.idLivrosSelecionados = [];
      },
      error: err => console.error('Erro ao buscar livros:', err)
    });
  }

  get paginas(): number[] {
    return Array(this.totalPaginas).fill(0).map((x, i) => i + 1);
  }

  get paginasVisiveis(): (number | string)[] {
    const total = this.totalPaginas;
    const atual = this.paginaAtual;
    const delta = 2; 

    if (total <= 7) {
      return Array.from({ length: total }, (_, i) => i + 1);
    }

    const lista: (number | string)[] = [];
    const rangeInicio = Math.max(2, atual - delta);
    const rangeFim = Math.min(total - 1, atual + delta);

    lista.push(1);

    if (rangeInicio > 2) {
      lista.push('...');
    }

    for (let i = rangeInicio; i <= rangeFim; i++) {
      lista.push(i);
    }

    if (rangeFim < total - 1) {
      lista.push('...');
    }

    lista.push(total);
    return lista;
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

  private GerarFraseAleatoria(): void {
    this.service.oterFraseAleatoria().subscribe({
      next: (listaFrases) => {
        this.fraseDestaque = listaFrases;
      },
      error: (err) => {
        console.error('Falha ao carregar frases no dashboard:', err);
      }
    });
  }

  gerarIdAleatorio(tamanhoMaximo: number): number {
    return Math.floor(Math.random() * tamanhoMaximo) + 1;
  }
}
