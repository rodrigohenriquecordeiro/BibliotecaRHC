import { Component, AfterViewInit, ViewChild, ElementRef } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';

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

  constructor(private router: Router) {}

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
}
