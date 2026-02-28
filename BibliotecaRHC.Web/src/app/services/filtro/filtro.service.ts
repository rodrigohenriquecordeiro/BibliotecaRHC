import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

export interface FiltroBusca {
  tipo: string;
  termo: string;
}

@Injectable({
  providedIn: 'root'
})
export class FiltroService {
  private filtroSource = new BehaviorSubject<FiltroBusca>({ tipo: 'Livro', termo: '' });
  
  filtroAtual$ = this.filtroSource.asObservable();

  atualizarFiltro(tipo: string, termo: string) {
    this.filtroSource.next({ tipo, termo });
  }
}
