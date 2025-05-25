import { Routes } from '@angular/router';
import { MinhaEstanteComponent } from './componentes/navbar/navbar-itens/minha-estante/minha-estante.component';
import { CadastrarComponent } from './componentes/crud/cadastrar/cadastrar.component';
import { EditarComponent } from './componentes/crud/editar/editar.component';
import { LidosComponent } from './componentes/navbar/navbar-itens/lidos/lidos.component';
import { ProjetosDeLeituraComponent } from './componentes/navbar/navbar-itens/projetos-de-leitura/projetos-de-leitura.component';
import { LoginComponent } from './componentes/login/login.component';
import { AuthGuard } from './auth.guard';
import { LayoutAutenticadoComponent } from './componentes/layout-autenticado/layout-autenticado.component';

export const routes: Routes = [
  { path: 'login', component: LoginComponent },
  {
    path: '',
    redirectTo: 'login', 
    pathMatch: 'full'
  },
  {
    path: '',
    component: LayoutAutenticadoComponent,
    canActivate: [AuthGuard],
    children: [
      { path: 'minha-estante', component: MinhaEstanteComponent },
      { path: 'cadastrar', component: CadastrarComponent },
      { path: 'editar', component: EditarComponent },
      { path: 'lidos', component: LidosComponent },
      { path: 'projetos-de-leitura', component: ProjetosDeLeituraComponent }
    ]
  },
  { path: '**', redirectTo: 'login' } 
]
