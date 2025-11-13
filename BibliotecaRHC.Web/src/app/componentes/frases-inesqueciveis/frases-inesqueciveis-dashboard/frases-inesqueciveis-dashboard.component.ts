import { Component, ViewChild, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FrasesInesqueciveisAdicionarComponent } from '../frases-inesqueciveis-adicionar/frases-inesqueciveis-adicionar.component';
import { BibliotecaService } from '../../../services/biblioteca/biblioteca.service';
import { Frase } from '../../../models/frase'; 

@Component({
  selector: 'app-frases-inesqueciveis-dashboard',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './frases-inesqueciveis-dashboard.component.html',
  styleUrl: './frases-inesqueciveis-dashboard.component.css'
})
export class FrasesInesqueciveisDashboardComponent implements OnInit {

  frases: Frase[] = []; 
  constructor(
    private service: BibliotecaService,
  ) {}

  @ViewChild(FrasesInesqueciveisAdicionarComponent)
  adicionarModal!: FrasesInesqueciveisAdicionarComponent;

  ngOnInit(): void {
    this.carregarFrases();
  }

  carregarFrases(): void {
    this.service.listarFrases().subscribe({
      next: (listaFrases) => {
        this.frases = listaFrases; 
        console.log('Frases carregadas:', listaFrases);
      },
      error: (err) => {
        console.error('Falha ao carregar frases no dashboard:', err);
      }
    });
  }

  abrirModal() {
    this.adicionarModal.visible = true;
    console.log('Abrindo modal...');
  }
}
