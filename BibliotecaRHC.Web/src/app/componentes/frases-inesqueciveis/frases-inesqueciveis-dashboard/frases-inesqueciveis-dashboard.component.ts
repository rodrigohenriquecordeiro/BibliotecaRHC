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
  constructor(
    private service: BibliotecaService,
    private router: Router
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
}
