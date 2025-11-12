import { Component, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common'; 
import { FrasesInesqueciveisAdicionarComponent } from '../frases-inesqueciveis-adicionar/frases-inesqueciveis-adicionar.component';

@Component({
  selector: 'app-frases-inesqueciveis-dashboard',
  standalone: true,
  imports: [CommonModule], 
  templateUrl: './frases-inesqueciveis-dashboard.component.html',
  styleUrl: './frases-inesqueciveis-dashboard.component.css'
})
export class FrasesInesqueciveisDashboardComponent {

  @ViewChild(FrasesInesqueciveisAdicionarComponent) 
  adicionarModal!: FrasesInesqueciveisAdicionarComponent;

  abrirModal() {
    this.adicionarModal.visible = true;
    console.log('Abrindo modal...');
  }
}