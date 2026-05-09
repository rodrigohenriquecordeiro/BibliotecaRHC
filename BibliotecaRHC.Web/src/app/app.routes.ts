import { Routes } from '@angular/router';
import { MinhaEstanteComponent } from './componentes/navbar/navbar-itens/minha-estante/minha-estante.component';
import { CadastrarComponent } from './componentes/crud/cadastrar/cadastrar.component';
import { EditarComponent } from './componentes/crud/editar/editar.component';
import { ProjetosLeituraComponent } from './componentes/projeto-leitura/projeto-leitura.component';
import { LoginComponent } from './componentes/login/login.component';
import { NavbarComponent } from './componentes/navbar/navbar-menu/navbar.component';
import { AuthGuard } from './auth.guard';
import { FrasesInesqueciveisDashboardComponent } from './componentes/frases-inesqueciveis/frases-inesqueciveis-dashboard/frases-inesqueciveis-dashboard.component';
import { FrasesInesqueciveisAdicionarComponent } from './componentes/frases-inesqueciveis/frases-inesqueciveis-adicionar/frases-inesqueciveis-adicionar.component';
import { FrasesInesqueciveisEditarComponent } from './componentes/frases-inesqueciveis/frases-inesqueciveis-editar/frases-inesqueciveis-editar.component';
import { ImportarPlanilhaComponent } from './componentes/navbar/navbar-itens/importar-planilha/importar-planilha.component';

export const routes: Routes = [
  { path: 'login', component: LoginComponent },
  {
    path: '',
    redirectTo: 'login',
    pathMatch: 'full'
  },
  {
    path: '',
    component: NavbarComponent,
    canActivate: [AuthGuard],
    children: [
      { path: 'minha-estante', component: MinhaEstanteComponent },
      { path: 'projeto-leitura', component: ProjetosLeituraComponent },
      { path: 'cadastrar', component: CadastrarComponent },
      { path: 'editar', component: EditarComponent },
      { path: 'frases-inesqueciveis-dashboard', component: FrasesInesqueciveisDashboardComponent },
      { path: 'frases-inesqueciveis-adicionar', component: FrasesInesqueciveisAdicionarComponent},
      { path: 'frases-inesqueciveis-editar', component: FrasesInesqueciveisEditarComponent},
      { path: 'importar-planilha', component: ImportarPlanilhaComponent}
    ]
  },
  { path: '**', redirectTo: 'login' }
];