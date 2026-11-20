import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';
import { Producto, ProductoRequest } from '../models/producto.models';

@Injectable({ providedIn: 'root' })
export class ProductosService {
  private readonly http = inject(HttpClient);
  private readonly url = `${environment.apiUrl}/api/productos`;

  listar(): Observable<Producto[]> {
    return this.http.get<Producto[]>(this.url);
  }

  obtener(idProducto: number): Observable<Producto> {
    return this.http.get<Producto>(`${this.url}/${idProducto}`);
  }

  registrar(producto: ProductoRequest): Observable<Producto> {
    return this.http.post<Producto>(this.url, producto);
  }

  actualizar(idProducto: number, producto: ProductoRequest): Observable<Producto> {
    return this.http.put<Producto>(`${this.url}/${idProducto}`, producto);
  }
}
