import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RelatoriosService } from '../../../../services/relatorios/relatorios.service';
import { Top10Item, Top10LivroLongo } from '../../../../models/relatorios';
import { Router } from '@angular/router';
import { FiltroService } from '../../../../services/filtro/filtro.service';
import { PageHeaderComponent } from '../../../shared/page-header/page-header.component';
import { RankingRow, ReportRankingComponent } from '../../../shared/report-ranking/report-ranking.component';

@Component({
  selector: 'app-relatorios',
  standalone: true,
  imports: [CommonModule, PageHeaderComponent, ReportRankingComponent],
  templateUrl: './relatorios.component.html',
  styleUrls: ['./relatorios.component.css']
})
export class RelatoriosComponent implements OnInit {
  private relatoriosService = inject(RelatoriosService);
  private router = inject(Router);
  private filtroService = inject(FiltroService);

  totalLivros = signal<number>(0);
  livrosLidos = signal<number>(0);
  livrosNaoLidos = signal<number>(0);
  totalEditoras = signal<number>(0);
  totalAutores = signal<number>(0);
  projetosLeituraAndamento = signal<number>(0);

  topClassificacoes = signal<Top10Item[]>([]);
  topAutores = signal<Top10Item[]>([]);
  topEditoras = signal<Top10Item[]>([]);
  topAnosPublicacao = signal<Top10Item[]>([]);
  topAnosAquisicao = signal<Top10Item[]>([]);
  topLivrosLongos = signal<Top10LivroLongo[]>([]);

  autoresRanking = computed(() => this.mapearItens(this.topAutores()));
  editorasRanking = computed(() => this.mapearItens(this.topEditoras()));
  classificacoesRanking = computed(() => this.mapearItens(this.topClassificacoes()));
  anosPublicacaoRanking = computed(() => this.mapearItens(this.topAnosPublicacao()));
  anosAquisicaoRanking = computed(() => this.mapearItens(this.topAnosAquisicao()));
  livrosLongosRanking = computed(() => this.topLivrosLongos().map(livro => ({
    label: livro.nomeDoLivro,
    value: livro.numeroDePaginas
  })));

  ngOnInit(): void {
    this.carregarDados();
  }

  abrirMinhaEstante(filtroLeitura?: 'sim' | 'não'): void {
    if (filtroLeitura) {
      this.filtroService.atualizarFiltro('lido', filtroLeitura);
    } else {
      this.filtroService.atualizarFiltro('Livro', '');
    }

    this.router.navigate(['/minha-estante']);
  }

  abrirProjetosLeitura(): void {
    this.router.navigate(['/projeto-leitura']);
  }

  private mapearItens(itens: Top10Item[]): RankingRow[] {
    return itens.map(item => ({ label: item.descricao, value: item.quantidade }));
  }

  private carregarDados(): void {
    this.relatoriosService.getDadosDashboard().subscribe(dados => {
      this.totalLivros.set(dados.totalLivros);
      this.livrosLidos.set(dados.livrosLidos);
      this.livrosNaoLidos.set(dados.livrosNaoLidos);
      this.totalEditoras.set(dados.totalEditoras);
      this.totalAutores.set(dados.totalAutores);
      this.projetosLeituraAndamento.set(dados.projetosLeituraAndamento);
      this.topClassificacoes.set(dados.topClassificacoes);
      this.topAutores.set(dados.topAutores);
      this.topEditoras.set(dados.topEditoras);
      this.topAnosPublicacao.set(dados.topAnosPublicacao);
      this.topAnosAquisicao.set(dados.topAnosAquisicao);
      this.topLivrosLongos.set(dados.topLivrosLongos);
    });
  }
}