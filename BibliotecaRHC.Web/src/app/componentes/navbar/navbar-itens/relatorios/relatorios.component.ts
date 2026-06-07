import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RelatoriosService } from '../../../../services/relatorios/relatorios.service';
import { Top10Item, Top10LivroLongo } from '../../../../models/relatorios';

@Component({
  selector: 'app-relatorios',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './relatorios.component.html',
  styleUrls: ['./relatorios.component.css']
})
export class RelatoriosComponent implements OnInit {
  private relatoriosService = inject(RelatoriosService);

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

  ngOnInit(): void {
    this.carregarDados();
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