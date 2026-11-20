import { validarCantidad } from './validar-stock';

describe('validarCantidad', () => {
  it('acepta una cantidad menor o igual al stock', () => {
    expect(validarCantidad(3, 7)).toBeNull();
    expect(validarCantidad(7, 7)).toBeNull();
  });

  it('avisa cuántas unidades hay si la cantidad supera el stock', () => {
    expect(validarCantidad(8, 7)).toBe('Solo hay 7 unidades disponibles.');
  });

  it('usa el singular cuando hay una sola unidad', () => {
    expect(validarCantidad(2, 1)).toBe('Solo hay 1 unidad disponible.');
  });

  it('avisa que no hay stock cuando está en cero', () => {
    expect(validarCantidad(1, 0)).toBe('No hay stock disponible.');
  });
});
