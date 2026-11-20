import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';
import { KardexItem, MovimientoProducto } from '../models/kardex.models';

@Injectable({ providedIn: 'root' })
export class KardexService {
  private readonly http = inject(HttpClient);
  private readonly url = `${environment.apiUrl}/api/kardex`;

  listar(): Observable<KardexItem[]> {
    return this.http.get<KardexItem[]>(this.url);
  }

  movimientos(idProducto: number): Observable<MovimientoProducto[]> {
    return this.http.get<MovimientoProducto[]>(`${this.url}/${idProducto}/movimientos`);
  }

  stock(idProducto: number): Observable<number> {
    return this.http.get<number>(`${this.url}/${idProducto}/stock`);
  }
}
