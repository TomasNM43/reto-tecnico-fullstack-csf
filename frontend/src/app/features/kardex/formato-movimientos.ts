import { formatDate } from '@angular/common';

import { MovimientoProducto, TIPO_ENTRADA } from '../../core/models/kardex.models';

// Cómo se muestra un movimiento en el modal del Kardex.
export interface MovimientoFila {
  fecha: string;
  tipo: string;
  cantidad: string;
  esEntrada: boolean;
}

// La fecha se muestra como dd/MM/yyyy HH:mm, el tipo como Entrada o Salida,
// y la cantidad lleva "+" si entra mercadería y "-" si sale.
export function formatearMovimiento(movimiento: MovimientoProducto): MovimientoFila {
  const esEntrada = movimiento.idTipoMovimiento === TIPO_ENTRADA;

  return {
    fecha: formatDate(movimiento.fecRegistro, 'dd/MM/yyyy HH:mm', 'en-US'),
    tipo: esEntrada ? 'Entrada' : 'Salida',
    cantidad: `${esEntrada ? '+' : '-'}${movimiento.cantidad}`,
    esEntrada,
  };
}
