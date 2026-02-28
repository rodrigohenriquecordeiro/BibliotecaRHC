import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Livro } from '../../../../models/livro';
import { BibliotecaService } from '../../../../services/biblioteca/biblioteca.service';
import { Router, RouterModule } from '@angular/router';
import { Frase } from '../../../../models/frase';
import { SimNaoPipe } from '../../../../pipes/sim-nao.pipe';
import { Subscription } from 'rxjs';
import { FiltroService } from '../../../../services/filtro/filtro.service'
import { LivrosPaginados } from '../../../../models/livrosPaginados';

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

  private inscricaoFiltro!: Subscription;
  campoFiltroAtual: string = 'nomeDoLivro';
  valorFiltroAtual: string = '';

  constructor(
    private service: BibliotecaService,
    private router: Router,
    private filtroService: FiltroService
  ) { }

  ngOnInit(): void {
    this.inscricaoFiltro = this.filtroService.filtroAtual$.subscribe(filtro => {
      this.campoFiltroAtual = filtro.tipo;
      this.valorFiltroAtual = filtro.termo;
      this.carregarLivros(1);
    });

    this.GerarFraseAleatoria();
  }

  ngOnDestroy(): void {
    if (this.inscricaoFiltro) {
      this.inscricaoFiltro.unsubscribe();
    }
  }

  carregarLivros(pagina: number): void {
    if (pagina < 1 || (this.totalPaginas > 0 && pagina > this.totalPaginas)) {
      return;
    }

    this.idLivrosSelecionados = [];

    this.service.listarLivros(pagina, this.campoFiltroAtual, this.valorFiltroAtual).subscribe({
      next: (dados: any) => {
        this.livros = dados.Livros || dados.livros || [];
        this.totalPaginas = Number(dados.TotalPaginas || dados.totalPaginas || 0);
        this.temPaginaAnterior = Boolean(dados.TemPaginaAnterior || dados.temPaginaAnterior);
        this.temProximaPagina = Boolean(dados.TemProximaPagina || dados.temProximaPagina);

        this.paginaAtual = pagina;
      },
      error: (err) => {
        console.error('Erro ao buscar livros:', err);
        this.livros = [];
      }
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
    if (this.idLivrosSelecionados?.length === 0) {
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

  get temFiltroAtivo(): boolean {
    return this.valorFiltroAtual !== undefined && this.valorFiltroAtual.trim() !== '';
  }

  limparFiltro() {
    this.filtroService.atualizarFiltro('Livro', '');
  }
}
