// Devuelve el mensaje de error si la cantidad no se puede vender con el stock disponible, o null si está bien.
export function validarCantidad(cantidad: number, stock: number): string | null {
  if (cantidad <= stock) return null;

  if (stock <= 0) return 'No hay stock disponible.';

  return stock === 1 ? 'Solo hay 1 unidad disponible.' : `Solo hay ${stock} unidades disponibles.`;
}
