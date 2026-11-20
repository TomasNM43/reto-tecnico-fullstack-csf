import { calcularLinea, calcularTotales } from './importes';

describe('importes', () => {
  it('calcula subtotal, IGV (18 %) y total de una línea', () => {
    expect(calcularLinea(10, 20)).toEqual({ subTotal: 200, igv: 36, total: 236 });
  });

  it('redondea el IGV a 2 decimales hacia arriba en el punto medio', () => {
    // 0.25 x 18 % = 0.045, que se redondea a 0.05.
    expect(calcularLinea(1, 0.25)).toEqual({ subTotal: 0.25, igv: 0.05, total: 0.3 });
  });

  it('evita los errores de coma flotante', () => {
    // En JavaScript 3 x 0.1 da 0.30000000000000004.
    expect(calcularLinea(3, 0.1).subTotal).toBe(0.3);
  });

  it('los totales son la suma de las líneas', () => {
    const lineas = [calcularLinea(1, 0.25), calcularLinea(3, 1.1)];

    expect(calcularTotales(lineas)).toEqual({ subTotal: 3.55, igv: 0.64, total: 4.19 });
  });

  it('una compra sin cantidades da todo en cero', () => {
    expect(calcularLinea(0, 0)).toEqual({ subTotal: 0, igv: 0, total: 0 });
    expect(calcularTotales([])).toEqual({ subTotal: 0, igv: 0, total: 0 });
  });
});
