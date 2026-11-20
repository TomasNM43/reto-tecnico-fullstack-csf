// Una fila de la tabla del Kardex.
export interface KardexItem {
  idProducto: number;
  nombreProducto: string;
  stockActual: number;
  costo: number;
  precioVenta: number;
}

// Una fila del modal de movimientos de un producto.
export interface MovimientoProducto {
  fecRegistro: string;
  idTipoMovimiento: number;
  tipoMovimiento: string;
  cantidad: number;
}

export const TIPO_ENTRADA = 1;
export const TIPO_SALIDA = 2;
