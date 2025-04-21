import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { AppComponent } from './app.component';
import { ReactiveFormsModule } from '@angular/forms';
import { NavbarComponent } from './componentes/navbar/navbar-menu/navbar.component';

@NgModule({
  imports: [
    BrowserModule,
    ReactiveFormsModule, 
    NavbarComponent,
    AppComponent
  ]
})

export class AppModule {}
