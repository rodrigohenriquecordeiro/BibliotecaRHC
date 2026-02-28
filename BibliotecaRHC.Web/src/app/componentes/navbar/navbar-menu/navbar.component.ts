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
  placeHolderFiltro: string = 'Selecione um filtro';

  private inscricaoFiltro!: Subscription;

  private readonly placeholders: Record<string, string> = {
    'Livro': 'Digite um livro',
    'Autor': 'Digite um autor',
    'Editora': 'Digite uma editora',
    'Número de Páginas': 'Digite o número de páginas',
    'Ano de Publicação': 'Digite o ano de publicação',
    'Data de Aquisição': 'Digite a data de aquisição',
    'Classificação Catalográfica': 'Digite a classificação catalográfica',
    'Você já leu?': 'Digite SIM ou NÃO',
    'Última leitura': 'Digite o ano da última leitura',
    'Observação': 'Digite uma observação'
  };

  constructor(
    private router: Router,
    private filtroService: FiltroService) { }

  ngOnInit() {
    this.inscricaoFiltro = this.filtroService.filtroAtual$.subscribe(filtro => {
      if (filtro.termo === '' && this.inputBusca) {
        this.inputBusca.nativeElement.value = '';
        this.filtroSelecionado = 'Livro';
        this.placeHolderFiltro = this.placeholders['Livro'];
      }
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
    this.inputBusca.nativeElement.focus();
  }

  logout() {
    localStorage.removeItem('usuarioLogado');
    this.router.navigate(['/login']);
  }

  pesquisar(termo: string) {
    let campoParaAPI = this.filtroSelecionado;

    if (campoParaAPI === 'Livro') {
      campoParaAPI = 'NomeDoLivro';
    }

    this.filtroService.atualizarFiltro(campoParaAPI, termo);
  }
}
