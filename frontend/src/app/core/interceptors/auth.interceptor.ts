import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';

import { environment } from '../../../environments/environment';
import { AuthService } from '../services/auth.service';

// Agrega el JWT a las peticiones al API y, si el servidor responde 401, cierra la sesión y manda al login.
export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const auth = inject(AuthService);
  const router = inject(Router);

  const esDelApi = req.url.startsWith(environment.apiUrl);
  const esLogin = req.url.endsWith('/api/auth/login');
  const token = auth.token;

  // El token no se manda a otros dominios ni al propio login.
  const peticion =
    esDelApi && !esLogin && token
      ? req.clone({ setHeaders: { Authorization: `Bearer ${token}` } })
      : req;

  return next(peticion).pipe(
    catchError((error: HttpErrorResponse) => {
      if (error.status === 401 && esDelApi && !esLogin) {
        auth.cerrarSesion();
        if (!router.url.startsWith('/login')) {
          router.navigate(['/login'], { queryParams: { returnUrl: router.url } });
        }
      }
      return throwError(() => error);
    }),
  );
};
