import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';
import { Compra, RegistrarCompraRequest } from '../models/compra.models';

@Injectable({ providedIn: 'root' })
export class ComprasService {
  private readonly http = inject(HttpClient);
  private readonly url = `${environment.apiUrl}/api/compras`;

  listar(): Observable<Compra[]> {
    return this.http.get<Compra[]>(this.url);
  }

  registrar(compra: RegistrarCompraRequest): Observable<Compra> {
    return this.http.post<Compra>(this.url, compra);
  }
}
