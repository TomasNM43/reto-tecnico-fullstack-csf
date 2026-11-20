// El precio no se envía: el servidor usa el precio de venta del producto.
export interface VentaItemRequest {
  idProducto: number;
  cantidad: number;
}

export interface RegistrarVentaRequest {
  detalles: VentaItemRequest[];
}

export interface VentaDetalle {
  idVentaDet: number;
  idProducto: number;
  cantidad: number;
  precio: number;
  subTotal: number;
  igv: number;
  total: number;
}

export interface Venta {
  idVentaCab: number;
  fecRegistro: string;
  subTotal: number;
  igv: number;
  total: number;
  detalles: VentaDetalle[];
}
