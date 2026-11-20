import { HttpClient } from '@angular/common/http';
import { Injectable, computed, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { Observable, tap } from 'rxjs';

import { environment } from '../../../environments/environment';
import { LoginRequest, LoginResponse, Sesion } from '../models/auth.models';

const CLAVE_SESION = 'clinica.sesion';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly router = inject(Router);

  private readonly _sesion = signal<Sesion | null>(this.leerSesionGuardada());

  readonly usuario = computed(() => this._sesion()?.username ?? null);
  readonly rol = computed(() => this._sesion()?.rol ?? null);

  // Token solo si la sesión sigue vigente.
  get token(): string | null {
    return this.sesionVigente()?.token ?? null;
  }

  estaAutenticado(): boolean {
    return this.sesionVigente() !== null;
  }

  login(credenciales: LoginRequest): Observable<LoginResponse> {
    return this.http
      .post<LoginResponse>(`${environment.apiUrl}/api/auth/login`, credenciales)
      .pipe(tap((respuesta) => this.guardarSesion(respuesta)));
  }

  cerrarSesion(): void {
    sessionStorage.removeItem(CLAVE_SESION);
    this._sesion.set(null);
  }

  cerrarSesionYVolverAlLogin(): void {
    this.cerrarSesion();
    this.router.navigate(['/login']);
  }

  private guardarSesion(respuesta: LoginResponse): void {
    const sesion: Sesion = {
      token: respuesta.token,
      expiraEnUtc: respuesta.expiraEnUtc,
      username: respuesta.username,
      rol: respuesta.rol,
    };
    // sessionStorage: la sesión se borra al cerrar la pestaña.
    sessionStorage.setItem(CLAVE_SESION, JSON.stringify(sesion));
    this._sesion.set(sesion);
  }

  private sesionVigente(): Sesion | null {
    const sesion = this._sesion();
    if (!sesion) return null;

    if (this.haExpirado(sesion.expiraEnUtc)) {
      this.cerrarSesion();
      return null;
    }
    return sesion;
  }

  private haExpirado(expiraEnUtc: string): boolean {
    // El servidor manda 7 decimales en los segundos; se recortan a milisegundos para que JS lo lea bien.
    const fecha = Date.parse(expiraEnUtc.replace(/(\.\d{3})\d+/, '$1'));
    return Number.isNaN(fecha) || fecha <= Date.now();
  }

  private leerSesionGuardada(): Sesion | null {
    try {
      const texto = sessionStorage.getItem(CLAVE_SESION);
      return texto ? (JSON.parse(texto) as Sesion) : null;
    } catch {
      return null;
    }
  }
}
