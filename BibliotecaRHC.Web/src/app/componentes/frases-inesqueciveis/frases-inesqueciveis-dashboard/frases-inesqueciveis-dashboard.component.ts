import { Component, ViewChild, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FrasesInesqueciveisAdicionarComponent } from '../frases-inesqueciveis-adicionar/frases-inesqueciveis-adicionar.component';
import { BibliotecaService } from '../../../services/biblioteca/biblioteca.service';
import { Frase } from '../../../models/frase';
import { Router, RouterModule } from '@angular/router';

@Component({
  selector: 'app-frases-inesqueciveis-dashboard',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './frases-inesqueciveis-dashboard.component.html',
  styleUrl: './frases-inesqueciveis-dashboard.component.css'
})
export class FrasesInesqueciveisDashboardComponent implements OnInit {
  frases: Frase[] = [];
  idFrasesSelecionadas: number[] = [];

  constructor(
    private service: BibliotecaService,
    private router: Router
  ) { }

  @ViewChild(FrasesInesqueciveisAdicionarComponent)
  adicionarModal!: FrasesInesqueciveisAdicionarComponent;

  ngOnInit(): void {
    this.carregarFrases();
  }

  carregarFrases(): void {
    this.service.listarFrases().subscribe({
      next: (listaFrases) => {
        this.frases = listaFrases;
      },
      error: (err) => {
        console.error('Falha ao carregar frases no dashboard:', err);
      }
    });
  }

  onEditar(id: number): void {
    this.service.setIdSelecionadoFrase(id);
    this.router.navigate(['/frases-inesqueciveis-editar']);
  }

  onApagar(id: number): void {
    this.idFrasesSelecionadas = [id];
  }

  confirmarApagar() {
    if (this.idFrasesSelecionadas.length === 0) {
      console.warn('Nenhuma frase selecionada para exclusão.');
      return;
    }

    const idsParaApagar = [...this.idFrasesSelecionadas];

    const apagarLivro = (id: number) => {
      return new Promise<void>((resolve, reject) => {
        this.service.excluirFrase(id).subscribe({
          next: () => resolve(),
          error: (err) => reject(err)
        });
      });
    };

    Promise.all(idsParaApagar.map(apagarLivro))
      .then(() => {
        this.frases = this.frases.filter(l => !idsParaApagar.includes(l.id));
        this.idFrasesSelecionadas = [];
        console.log(`${idsParaApagar.length} livra apagada com sucesso.`);
      })
      .catch(err => {
        console.error('Erro ao excluir um ou mais livros:', err);
      });
  }
}
