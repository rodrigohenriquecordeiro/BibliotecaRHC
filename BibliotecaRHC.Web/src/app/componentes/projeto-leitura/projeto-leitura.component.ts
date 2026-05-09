import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

interface LivroProjeto {
  id: number;
  nome: string;
  ano: number;
  lido: boolean;
  dataLeitura: string | null;
}

interface Projeto {
  id: number;
  nome: string;
  livros: LivroProjeto[];
}

@Component({
  selector: 'app-projetos-leitura',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './projeto-leitura.component.html',
  styleUrls: ['./projeto-leitura.component.css']
})
export class ProjetosLeituraComponent {
  projetos: Projeto[] = [
    { id: 1, nome: 'Sherlock Holmes', livros: [
      { id: 101, nome: 'Um Estudo em Vermelho', ano: 1887, lido: true, dataLeitura: '2024-01-15' },
      { id: 102, nome: 'O Signo dos Quatro', ano: 1890, lido: false, dataLeitura: null }
    ]},
    { id: 2, nome: 'Bíblia King James', livros: [] },
    { id: 3, nome: 'Viagens com Júlio Verne', livros: [] },
    { id: 4, nome: 'Detetive Norueguês Harry Hole', livros: [] },
    { id: 5, nome: 'Presentes da Deh', livros: [] },
    { id: 6, nome: 'Clube do Livro', livros: [ 
      { id: 601, nome: 'História do Olho', ano: 1928, lido: true, dataLeitura: '2026-05-09' }
    ]},
  ];

  projetoSelecionado: Projeto | null = null;

  abrirModalProjeto(projeto: Projeto) {
    this.projetoSelecionado = projeto;
  }
}