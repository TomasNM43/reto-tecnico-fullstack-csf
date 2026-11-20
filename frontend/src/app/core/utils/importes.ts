import { environment } from '../../../environments/environment';

// Importes de una línea o de un documento completo (compra o venta).
export interface Calculo {
  subTotal: number;
  igv: number;
  total: number;
}

// Se trabaja en céntimos (números enteros) para evitar los errores de coma flotante de JavaScript.
const aCentimos = (valor: number): number => Math.round(valor * 100);
const deCentimos = (centimos: number): number => centimos / 100;

// SubTotal = cantidad × precio, IGV = SubTotal × tasa (redondeado a 2 decimales) y Total = SubTotal + IGV.
// Es la misma regla que aplica el servidor, así lo que se ve en pantalla coincide con lo que se guarda.
export function calcularLinea(cantidad: number, precio: number): Calculo {
  const subTotal = aCentimos(cantidad * precio);

  // La tasa se pasa a puntos básicos (0.18 -> 1800) para que la multiplicación siga siendo entera.
  const tasa = Math.round(environment.igvRate * 10_000);
  const igv = Math.round((subTotal * tasa) / 10_000);

  return { subTotal: deCentimos(subTotal), igv: deCentimos(igv), total: deCentimos(subTotal + igv) };
}

// Los totales de la cabecera son la suma de las líneas.
export function calcularTotales(lineas: Calculo[]): Calculo {
  const sumar = (campo: keyof Calculo): number => lineas.reduce((acumulado, l) => acumulado + aCentimos(l[campo]), 0);

  return {
    subTotal: deCentimos(sumar('subTotal')),
    igv: deCentimos(sumar('igv')),
    total: deCentimos(sumar('total')),
  };
}
