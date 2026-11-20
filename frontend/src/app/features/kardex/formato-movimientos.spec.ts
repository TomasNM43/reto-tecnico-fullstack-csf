import { formatearMovimiento } from './formato-movimientos';

describe('formatearMovimiento', () => {
  it('muestra una entrada con "+" y la fecha como dd/MM/yyyy HH:mm', () => {
    const fila = formatearMovimiento({
      fecRegistro: '2026-11-20T10:05:00',
      idTipoMovimiento: 1,
      tipoMovimiento: 'Entrada',
      cantidad: 10,
    });

    expect(fila).toEqual({ fecha: '20/11/2026 10:05', tipo: 'Entrada', cantidad: '+10', esEntrada: true });
  });

  it('muestra una salida con "-"', () => {
    const fila = formatearMovimiento({
      fecRegistro: '2026-11-20T11:30:00',
      idTipoMovimiento: 2,
      tipoMovimiento: 'Salida',
      cantidad: 3,
    });

    expect(fila).toEqual({ fecha: '20/11/2026 11:30', tipo: 'Salida', cantidad: '-3', esEntrada: false });
  });
});
