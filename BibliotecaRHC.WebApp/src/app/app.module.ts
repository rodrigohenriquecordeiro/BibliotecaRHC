import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { AppComponent } from './app.component';
import { ReactiveFormsModule } from '@angular/forms';
import { NavbarComponent } from './componentes/navbar/navbar-menu/navbar.component';
import { FooterComponent } from './componentes/footer/footer.component';
import { LayoutAutenticadoComponent } from './componentes/layout-autenticado/layout-autenticado.component';
import { LoginComponent } from './componentes/login/login.component';
import { RouterModule } from '@angular/router';
import { AuthInterceptor } from './auth.interceptor';

@NgModule({
  imports: [
    BrowserModule,
    ReactiveFormsModule,
    AppComponent,
    NavbarComponent,
    FooterComponent,
    LoginComponent,
    LayoutAutenticadoComponent,
    RouterModule.forRoot([
      { path: '', redirectTo: 'login', pathMatch: 'full' },
      { path: 'login', component: LoginComponent }
    ])
  ],
  providers: [
    {
      provide: HTTP_INTERCEPTORS,
      useClass: AuthInterceptor,
      multi: true,
    },
  ],
})
export class AppModule {}
