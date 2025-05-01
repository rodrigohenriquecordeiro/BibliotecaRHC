import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { AppComponent } from './app.component';
import { ReactiveFormsModule } from '@angular/forms';
import { NavbarComponent } from './componentes/navbar/navbar-menu/navbar.component';
import { LoginComponent } from './componentes/login/login.component';
import { RouterModule } from '@angular/router';

@NgModule({
  imports: [
    BrowserModule,
    ReactiveFormsModule, 
    NavbarComponent,
    AppComponent,
    LoginComponent,
    RouterModule.forRoot([
      { path: '', redirectTo: 'login', pathMatch: 'full' },
      { path: 'login', component: LoginComponent }
    ])
  ]
})

export class AppModule {}
