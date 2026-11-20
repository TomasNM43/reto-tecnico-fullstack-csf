export interface Producto {
  idProducto: number;
  nombreProducto: string;
  nroLote: string;
  fecRegistro: string;
  costo: number;
  precioVenta: number;
}

// Se usa para registrar y para actualizar un producto.
export interface ProductoRequest {
  nombreProducto: string;
  nroLote: string;
  costo: number;
  precioVenta: number;
}
