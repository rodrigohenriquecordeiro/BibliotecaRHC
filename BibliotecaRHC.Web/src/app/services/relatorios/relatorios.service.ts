import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Relatorios } from '../../models/relatorios';
import { environment } from '../../../environments/environment.development'; 

@Injectable({
  providedIn: 'root'
})
export class RelatoriosService {
  private readonly http = inject(HttpClient);
  private readonly API = `${environment.apiUrl}/relatorios`; 

  getDadosDashboard(): Observable<Relatorios> {
    return this.http.get<Relatorios>(this.API);
  }
}