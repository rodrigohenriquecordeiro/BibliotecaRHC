import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Projeto } from '../../models/projeto';
import { LivroProjeto } from '../../models/livroProjeto';

@Component({
  selector: 'app-projetos-leitura',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './projeto-leitura.component.html',
  styleUrls: ['./projeto-leitura.component.css']
})
export class ProjetosLeituraComponent {
  projetos: Projeto[] = [
    {
      id: 1, nome: 'Sherlock Holmes', livros: [
        { id: 101, nome: 'Um Estudo em Vermelho', ano: 1887, lido: true, dataLeitura: '2024-01-15' },
        { id: 102, nome: 'O Signo dos Quatro', ano: 1890, lido: false, dataLeitura: null }
      ]
    },
    { id: 2, nome: 'Bíblia King James', livros: [] },
    { id: 3, nome: 'Viagens com Júlio Verne', livros: [] },
    { id: 4, nome: 'Detetive Norueguês Harry Hole', livros: [] },
    { id: 5, nome: 'Presentes da Deh', livros: [] },
    {
      id: 6, nome: 'Clube do Livro', livros: [
        { id: 601, nome: 'História do Olho', ano: 1928, lido: true, dataLeitura: '2026-05-09' }
      ]
    },
  ];

  projetoSelecionado: Projeto | null = null;
  novoProjetoNome: string = '';
  projetoParaEdicao: Projeto | null = null;
  nomeProjetoEdicao: string = '';
  projetoParaExclusao: Projeto | null = null;

  livroParaEdicao: LivroProjeto | null = null;
  livroEmEdicaoNome: string = '';
  livroEmEdicaoAno: number = 0;
  livroParaExclusao: LivroProjeto | null = null;

  novoLivroNome: string = '';
  novoLivroAno: number = new Date().getFullYear();

  abrirModalProjeto(projeto: Projeto) {
    this.projetoSelecionado = projeto;
  }

  prepararNovoProjeto() {
    this.novoProjetoNome = '';
  }

  salvarNovoProjeto() {
    if (this.novoProjetoNome.trim()) {
      const novoId = this.projetos.length > 0 ? Math.max(...this.projetos.map(p => p.id)) + 1 : 1;
      this.projetos.push({
        id: novoId,
        nome: this.novoProjetoNome,
        livros: []
      });
      this.novoProjetoNome = '';
    }
  }

  prepararEdicao(projeto: Projeto) {
    this.projetoParaEdicao = projeto;
    this.nomeProjetoEdicao = projeto.nome;
  }

  salvarEdicao() {
    if (this.projetoParaEdicao && this.nomeProjetoEdicao.trim()) {
      this.projetoParaEdicao.nome = this.nomeProjetoEdicao;
      this.projetoParaEdicao = null;
    }
  }

  prepararExclusao(projeto: Projeto) {
    this.projetoParaExclusao = projeto;
  }

  confirmarExclusao() {
    if (this.projetoParaExclusao) {
      this.projetos = this.projetos.filter(p => p.id !== this.projetoParaExclusao!.id);
      this.projetoParaExclusao = null;
    }
  }

  prepararNovoLivro() {
    this.novoLivroNome = '';
    this.novoLivroAno = new Date().getFullYear();
  }

  adicionarLivro() {
    if (this.projetoSelecionado && this.novoLivroNome.trim()) {
      const novoId = Date.now();

      this.projetoSelecionado.livros.push({
        id: novoId,
        nome: this.novoLivroNome,
        ano: this.novoLivroAno,
        lido: false,
        dataLeitura: null
      });

      this.novoLivroNome = '';
      this.novoLivroAno = new Date().getFullYear();
    }
  }

  prepararEdicaoLivro(livro: LivroProjeto) {
    this.livroParaEdicao = livro;
    this.livroEmEdicaoNome = livro.nome;
    this.livroEmEdicaoAno = livro.ano;
  }

  salvarEdicaoLivro() {
    if (this.livroParaEdicao && this.livroEmEdicaoNome.trim()) {
      this.livroParaEdicao.nome = this.livroEmEdicaoNome;
      this.livroParaEdicao.ano = this.livroEmEdicaoAno;
      this.livroParaEdicao = null;
    }
  }

  prepararExclusaoLivro(livro: LivroProjeto) {
    this.livroParaExclusao = livro;
  }

  confirmarExclusaoLivro() {
    if (this.projetoSelecionado && this.livroParaExclusao) {
      this.projetoSelecionado.livros = this.projetoSelecionado.livros.filter(l => l.id !== this.livroParaExclusao!.id);
      this.livroParaExclusao = null;
    }
  }
}