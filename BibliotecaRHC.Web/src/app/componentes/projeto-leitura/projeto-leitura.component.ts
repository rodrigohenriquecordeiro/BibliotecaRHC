import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Projeto } from '../../models/projeto';
import { LivroProjeto } from '../../models/livroProjeto';
import { BibliotecaService } from '../../services/biblioteca/biblioteca.service';
import { ProjetoStatus } from '../../models/projetoStatus';

declare var bootstrap: any;
@Component({
  selector: 'app-projetos-leitura',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './projeto-leitura.component.html',
  styleUrls: ['./projeto-leitura.component.css']
})
export class ProjetosLeituraComponent implements OnInit {
  projetos: Projeto[] = [];
  idProjetoSelecionado: number[] = [];

  projetoSelecionado: Projeto | null = null;
  novoProjetoNome: string = '';
  novoProjetoData: Date = new Date();
  novoProjetoStatus: ProjetoStatus = ProjetoStatus.NaoIniciado;

  projetoParaEdicao: Projeto | null = null;
  nomeProjetoEdicao: string = '';
  dataProjetoEdicao: Date = new Date();
  statusProjetoEdicao: ProjetoStatus = ProjetoStatus.NaoIniciado;

  projetoParaExclusao: Projeto | null = null;

  livroParaEdicao: LivroProjeto | null = null;
  livroEmEdicaoNome: string = '';
  livroEmEdicaoAno: string = '';
  livroParaExclusao: LivroProjeto | null = null;

  novoLivroNome: string = '';
  novoLivroAno: string = '';

  constructor(
    private service: BibliotecaService,
  ) { }

  ngOnInit(): void {
    this.carregarProjetos();
  }

  carregarProjetos(): void {
    this.service.listaProjetos().subscribe({
      next: (listaProjetos) => {
        listaProjetos.forEach(projeto => {
          if (projeto.livroProjetos) {
            projeto.livroProjetos.forEach(livro => {
              if (livro.dataDeLeitura) {
                livro.dataDeLeitura = livro.dataDeLeitura.split('T')[0];
              }
            });
          }
        });
        this.projetos = listaProjetos;
      },
      error: (err) => console.error('Falha ao carregar projetos:', err)
    });
  }

  abrirModalProjeto(projeto: Projeto) {
    this.projetoSelecionado = projeto;
  }

  prepararNovoProjeto() {
    this.novoProjetoNome = '';
    this.novoProjetoData = new Date().toISOString().split('T')[0] as any;;
    this.novoProjetoStatus = ProjetoStatus.NaoIniciado;
  }

  obterDescricaoStatus(status: number): string {
    switch (Number(status)) {
      case 0: return 'Não Iniciado';
      case 1: return 'Andamento';
      case 2: return 'Finalizado';
      case 3: return 'Abandonado';
      default: return 'Não Iniciado';
    }
  }

  salvarNovoProjeto() {
    if (this.novoProjetoNome.trim() && this.novoProjetoData) {

      const novoProjeto: any = {
        id: 0,
        nome: this.novoProjetoNome,
        dataCriacao: this.novoProjetoData,
        livros: [],
        projetoStatus: this.novoProjetoStatus
      };

      this.service.criarProjeto(novoProjeto).subscribe({
        next: (projetoCriado) => {
          this.projetos.push(projetoCriado);
          this.novoProjetoNome = '';
          this.novoProjetoData = projetoCriado.dataCriacao;
          this.novoProjetoStatus = ProjetoStatus.NaoIniciado;

          const modalNovoEl = document.getElementById('modalNovoProjeto');
          if (modalNovoEl) {
            const modalNovo = bootstrap.Modal.getInstance(modalNovoEl);
            if (modalNovo) modalNovo.hide();
          }

          this.abrirModalProjeto(projetoCriado);

          const modalProjEl = document.getElementById('modalProjeto');
          if (modalProjEl) {
            const modalProj = new bootstrap.Modal(modalProjEl);
            modalProj.show();
          }
        },
        error: (err) => console.error('Erro ao criar projeto:', err)
      });
    }
  }

  prepararEdicao(projeto: Projeto) {
    this.projetoParaEdicao = projeto;
    this.nomeProjetoEdicao = projeto.nome;
    this.statusProjetoEdicao = projeto.projetoStatus ?? ProjetoStatus.NaoIniciado;

    if (projeto.dataCriacao) {
      const dateObj = new Date(projeto.dataCriacao);
      this.dataProjetoEdicao = dateObj.toISOString().split('T')[0] as any;
    }

    this.statusProjetoEdicao = projeto.projetoStatus ?? ProjetoStatus.NaoIniciado;
  }

  salvarEdicao() {
    if (this.projetoParaEdicao && this.nomeProjetoEdicao.trim()) {
      const projetoAtualizado = {
        ...this.projetoParaEdicao,
        nome: this.nomeProjetoEdicao,
        dataCriacao: this.dataProjetoEdicao,
        projetoStatus: this.statusProjetoEdicao
      };

      this.service.editarProjeto(projetoAtualizado).subscribe({
        next: (res) => {
          this.projetoParaEdicao!.nome = this.nomeProjetoEdicao;
          this.projetoParaEdicao!.dataCriacao = this.dataProjetoEdicao;
          this.projetoParaEdicao!.projetoStatus = this.statusProjetoEdicao;
          this.projetoParaEdicao = null;
        },
        error: (err) => console.error('Erro ao editar projeto:', err)
      });
    }
  }

  prepararExclusao(projeto: Projeto) {
    this.projetoParaExclusao = projeto;
  }

  confirmarExclusao() {
    if (this.projetoParaExclusao) {
      this.service.excluirProjeto(this.projetoParaExclusao.id).subscribe({
        next: () => {
          this.projetos = this.projetos.filter(p => p.id !== this.projetoParaExclusao!.id);
          this.projetoParaExclusao = null;
        },
        error: (err) => console.error('Erro ao excluir projeto:', err)
      });
    }
  }

  prepararNovoLivro() {
    this.novoLivroNome = '';
    this.novoLivroAno = '';
    this.novoProjetoStatus = ProjetoStatus.NaoIniciado;
  }

  adicionarLivro() {
    if (this.projetoSelecionado && this.novoLivroNome.trim()) {

      const novoLivro: LivroProjeto = {
        id: 0,
        nome: this.novoLivroNome,
        anoDePublicacao: this.novoLivroAno ? this.novoLivroAno.toString() : '',
        lido: false,
        dataDeLeitura: null,
        projetoId: this.projetoSelecionado.id
      };

      this.service.criarLivroNoProjeto(novoLivro).subscribe({
        next: (livroCriado) => {
          if (!this.projetoSelecionado!.livroProjetos) {
            this.projetoSelecionado!.livroProjetos = [];
          }

          this.projetoSelecionado!.livroProjetos.push(livroCriado);

          const index = this.projetos.findIndex(p => p.id === this.projetoSelecionado!.id);
          if (index !== -1) {
            this.projetos[index].livroProjetos = [...this.projetoSelecionado!.livroProjetos];
          }

          this.prepararNovoLivro();

          const modalAddEl = document.getElementById('modalAdicionarLivro');
          const modalProjEl = document.getElementById('modalProjeto');

          if (modalAddEl && modalProjEl) {
            const modalAdd = bootstrap.Modal.getInstance(modalAddEl);
            modalAdd?.hide();

            const modalProj = new bootstrap.Modal(modalProjEl);
            modalProj.show();
          }
        },
        error: (err) => {
          console.error('Erro ao persistir novo livro:', err);
          alert('Erro ao salvar o livro. Verifique a conexão com o servidor.');
        }
      });
    }
  }

  prepararEdicaoLivro(livro: LivroProjeto) {
    this.livroParaEdicao = livro;
    this.livroEmEdicaoNome = livro.nome;
    this.livroEmEdicaoAno = livro.anoDePublicacao;
  }

  salvarEdicaoLivro() {
    if (this.livroParaEdicao && this.livroEmEdicaoNome.trim()) {
      const livroAtualizado: LivroProjeto = {
        ...this.livroParaEdicao,
        nome: this.livroEmEdicaoNome,
        anoDePublicacao: this.livroEmEdicaoAno ? this.livroEmEdicaoAno.toString() : ''
      };

      this.service.editarLivroNoProjeto(livroAtualizado).subscribe({
        next: (res) => {
          this.livroParaEdicao!.nome = this.livroEmEdicaoNome;
          this.livroParaEdicao!.anoDePublicacao = this.livroEmEdicaoAno;
          this.livroParaEdicao = null;
        },
        error: (err) => console.error('Erro ao atualizar livro:', err)
      });
    }
  }

  prepararExclusaoLivro(livro: LivroProjeto) {
    this.livroParaExclusao = livro;
  }

  confirmarExclusaoLivro() {
    if (this.projetoSelecionado && this.livroParaExclusao) {
      this.service.excluirLivroNoProjeto(this.livroParaExclusao.id).subscribe({
        next: () => {
          this.projetoSelecionado!.livroProjetos = this.projetoSelecionado!.livroProjetos.filter(
            l => l.id !== this.livroParaExclusao!.id
          );

          const index = this.projetos.findIndex(p => p.id === this.projetoSelecionado!.id);
          if (index !== -1) {
            this.projetos[index].livroProjetos = [...this.projetoSelecionado!.livroProjetos];
          }

          this.livroParaExclusao = null;
        }
      });
    }
  }

  alternarStatusLeitura(livro: LivroProjeto) {
    if (!livro.lido) {
      livro.dataDeLeitura = null;
    } else if (!livro.dataDeLeitura) {
      livro.dataDeLeitura = new Date().toISOString().split('T')[0];
    }

    this.persistirAlteracaoLivro(livro);
  }

  salvarDataLeitura(livro: LivroProjeto) {
    if (livro.lido) {
      this.persistirAlteracaoLivro(livro);
    }
  }

  persistirAlteracaoLivro(livro: LivroProjeto) {
    this.service.editarLivroNoProjeto(livro).subscribe({
      next: () => {
        console.log('Livro atualizado com sucesso');
      },
      error: (err) => {
        console.error('Erro ao atualizar livro:', err);
        alert('Não foi possível salvar a alteração.');
      }
    });
  }
}