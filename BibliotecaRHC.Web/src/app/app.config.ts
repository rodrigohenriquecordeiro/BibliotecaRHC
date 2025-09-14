import { ApplicationConfig, provideZoneChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { routes } from './app.routes';
import { provideClientHydration, withEventReplay } from '@angular/platform-browser';
import { AuthService } from './services/auth/auth.service';
import { AuthGuard } from './auth.guard';

export const appConfig: ApplicationConfig = {
  providers: [
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes),
    provideClientHydration(withEventReplay()),

    provideHttpClient(
      withInterceptors([
        (req, next) => {
           const token = localStorage.getItem('token');
           if (token) {
             const clonedRequest = req.clone({
               setHeaders: {
                 Authorization: `Bearer ${token}`
               }
             });
             
             return next(clonedRequest);
           }
           
           return next(req);
        }
      ])
    ),
    
    AuthService,
    AuthGuard
  ]
};
