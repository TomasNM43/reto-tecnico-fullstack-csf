export interface CompraItemRequest {
  idProducto: number;
  cantidad: number;
  precio: number;
}

export interface RegistrarCompraRequest {
  detalles: CompraItemRequest[];
}

export interface CompraDetalle {
  idCompraDet: number;
  idProducto: number;
  cantidad: number;
  precio: number;
  subTotal: number;
  igv: number;
  total: number;
}

export interface Compra {
  idCompraCab: number;
  fecRegistro: string;
  subTotal: number;
  igv: number;
  total: number;
  detalles: CompraDetalle[];
}
