import { Component, AfterViewInit, ViewChild, ElementRef } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FiltroService } from '../../../services/filtro/filtro.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [RouterModule, CommonModule],
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.css'
})
export class NavbarComponent implements AfterViewInit {

  @ViewChild('inputBusca') inputBusca!: ElementRef;

  filtroSelecionado: string = 'Livro';
  placeHolderFiltro: string = 'Selecione um filtro ou digite um livro';
  termoBusca: string = '';

  private inscricaoFiltro!: Subscription;

  private readonly placeholders: Record<string, string> = {
    'Livro': 'Digite um livro',
    'Autor': 'Digite um autor',
    'Editora': 'Digite uma editora',
    'Número de Páginas': 'Digite o número de páginas',
    'Ano de Publicação': 'Digite o ano de publicação',
    'Data de Aquisição': 'Digite a data de aquisição (dd/mm/aaaa)',
    'Classificação Catalográfica': 'Digite a classificação catalográfica',
    'Você já leu?': 'Digite sim ou não',
    'Última leitura': 'Digite o ano da última leitura',
    'Observação': 'Digite uma observação'
  };

  private readonly camposParaAPI: Record<string, string> = {
    'Livro': 'nomedolivro',
    'Autor': 'autor',
    'Editora': 'editora',
    'Número de Páginas': 'numerodepaginas',
    'Ano de Publicação': 'anodepublicacao',
    'Data de Aquisição': 'datadeaquisicao',
    'Classificação Catalográfica': 'classificacaocatalografica',
    'Você já leu?': 'lido',
    'Última leitura': 'anoultimaleitura',
    'Observação': 'observacao'
  };

  private readonly filtrosParaCampoAPI: Record<string, string> = Object.fromEntries(
    Object.entries(this.camposParaAPI).map(([filtro, campoAPI]) => [campoAPI, filtro])
  );

  constructor(
    private router: Router,
    private filtroService: FiltroService) { }

  ngOnInit() {
    this.inscricaoFiltro = this.filtroService.filtroAtual$.subscribe(filtro => {
      this.termoBusca = filtro.termo;
      this.filtroSelecionado = filtro.termo === ''
        ? 'Livro'
        : this.filtrosParaCampoAPI[filtro.tipo] || 'Livro';
      this.placeHolderFiltro = this.placeholders[this.filtroSelecionado] || 'Selecione um filtro';
    });
  }

  ngOnDestroy() {
    if (this.inscricaoFiltro) this.inscricaoFiltro.unsubscribe();
  }

  ngAfterViewInit() {
    this.inputBusca?.nativeElement?.focus();
  }

  mudarFiltro(novoValor: string) {
    this.filtroSelecionado = novoValor;
    this.placeHolderFiltro = this.placeholders[novoValor] || 'Selecione um filtro';
    this.termoBusca = '';
    this.inputBusca?.nativeElement?.focus();
  }

  logout() {
    localStorage.removeItem('usuarioLogado');
    this.router.navigate(['/login']);
  }

  pesquisar(termo: string) {
    const campoAPI = this.camposParaAPI[this.filtroSelecionado];
    this.filtroService.atualizarFiltro(campoAPI, termo);
  }
}
