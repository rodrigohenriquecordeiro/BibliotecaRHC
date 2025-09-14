import { ApplicationConfig } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideHttpClient, withInterceptors } from '@angular/common/http'; // Importe withInterceptors

import { routes } from './app.routes';
import { authInterceptor } from './interceptors/auth.interceptor'; // Importe o interceptor

export const appConfig: ApplicationConfig = {
  providers: [
    provideRouter(routes),
    // Registre o interceptor aqui
    provideHttpClient(withInterceptors([authInterceptor]))
  ]
};