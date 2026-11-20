import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';
import { RegistrarVentaRequest, Venta } from '../models/venta.models';

@Injectable({ providedIn: 'root' })
export class VentasService {
  private readonly http = inject(HttpClient);
  private readonly url = `${environment.apiUrl}/api/ventas`;

  listar(): Observable<Venta[]> {
    return this.http.get<Venta[]>(this.url);
  }

  registrar(venta: RegistrarVentaRequest): Observable<Venta> {
    return this.http.post<Venta>(this.url, venta);
  }
}
