import { HttpClient, provideHttpClient, withInterceptors } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { Router, provideRouter } from '@angular/router';

import { environment } from '../../../environments/environment';
import { AuthService } from '../services/auth.service';
import { authInterceptor } from './auth.interceptor';

describe('authInterceptor', () => {
  // Deja una sesión vigente guardada y arma el módulo de pruebas (así AuthService la lee al crearse).
  function prepararConSesion() {
    sessionStorage.setItem(
      'clinica.sesion',
      JSON.stringify({
        token: 'jwt-de-prueba',
        expiraEnUtc: new Date(Date.now() + 30 * 60_000).toISOString(),
        username: 'admin',
        rol: 'Admin',
      }),
    );

    TestBed.configureTestingModule({
      providers: [
        provideRouter([]),
        provideHttpClient(withInterceptors([authInterceptor])),
        provideHttpClientTesting(),
      ],
    });

    return {
      http: TestBed.inject(HttpClient),
      controller: TestBed.inject(HttpTestingController),
      router: TestBed.inject(Router),
      auth: TestBed.inject(AuthService),
    };
  }

  beforeEach(() => sessionStorage.clear());

  it('agrega el JWT a las peticiones al API', () => {
    const { http, controller } = prepararConSesion();

    http.get(`${environment.apiUrl}/api/productos`).subscribe();

    const req = controller.expectOne(`${environment.apiUrl}/api/productos`);
    expect(req.request.headers.get('Authorization')).toBe('Bearer jwt-de-prueba');
    req.flush([]);
    controller.verify();
  });

  it('no manda el JWT a otros dominios', () => {
    const { http, controller } = prepararConSesion();

    http.get('https://otro-dominio.com/datos').subscribe();

    const req = controller.expectOne('https://otro-dominio.com/datos');
    expect(req.request.headers.has('Authorization')).toBe(false);
    req.flush({});
    controller.verify();
  });

  it('ante un 401 cierra la sesión y manda al login', () => {
    const { http, controller, router, auth } = prepararConSesion();
    const navegar = vi.spyOn(router, 'navigate').mockResolvedValue(true);

    http.get(`${environment.apiUrl}/api/productos`).subscribe({ error: () => undefined });
    controller
      .expectOne(`${environment.apiUrl}/api/productos`)
      .flush({}, { status: 401, statusText: 'Unauthorized' });

    expect(auth.estaAutenticado()).toBe(false);
    expect(navegar).toHaveBeenCalledWith(['/login'], expect.anything());
    controller.verify();
  });
});
