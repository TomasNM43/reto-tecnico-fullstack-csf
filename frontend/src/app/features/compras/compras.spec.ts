import { TestBed } from '@angular/core/testing';
import { FormArray, FormControl, FormGroup } from '@angular/forms';
import { of } from 'rxjs';

import { ComprasService } from '../../core/services/compras.service';
import { ProductosService } from '../../core/services/productos.service';
import { Compras } from './compras';

interface Interno {
  detalles: FormArray<
    FormGroup<{ idProducto: FormControl<number | null>; cantidad: FormControl<number>; precio: FormControl<number> }>
  >;
  guardar(): void;
}

const productos = [
  { idProducto: 1, nombreProducto: 'Paracetamol', nroLote: 'L1', fecRegistro: '', costo: 10, precioVenta: 13.5 },
];

describe('Compras', () => {
  function preparar() {
    const registrar = vi.fn().mockReturnValue(
      of({ idCompraCab: 4, fecRegistro: '', subTotal: 200, igv: 36, total: 236, detalles: [] }),
    );

    TestBed.configureTestingModule({
      imports: [Compras],
      providers: [
        { provide: ProductosService, useValue: { listar: () => of(productos) } },
        { provide: ComprasService, useValue: { listar: () => of([]), registrar } },
      ],
    });

    const fixture = TestBed.createComponent(Compras);
    fixture.detectChanges();

    return { fixture, registrar, componente: fixture.componentInstance as unknown as Interno };
  }

  async function llenar(fixture: ReturnType<typeof preparar>['fixture'], componente: Interno) {
    componente.detalles.at(0).patchValue({ idProducto: 1, cantidad: 10, precio: 20 });
    await fixture.whenStable();
    fixture.detectChanges();
  }

  const textoDe = (fixture: ReturnType<typeof preparar>['fixture']) => (fixture.nativeElement as HTMLElement).textContent ?? '';

  it('calcula subtotal, IGV y total al digitar cantidad y precio', async () => {
    const { fixture, componente } = preparar();

    await llenar(fixture, componente);

    const texto = textoDe(fixture);
    expect(texto).toContain('S/ 200.00');
    expect(texto).toContain('S/ 36.00');
    expect(texto).toContain('S/ 236.00');
  });

  it('envía al servicio el detalle de la compra', async () => {
    const { fixture, registrar, componente } = preparar();
    await llenar(fixture, componente);

    componente.guardar();

    expect(registrar).toHaveBeenCalledWith({ detalles: [{ idProducto: 1, cantidad: 10, precio: 20 }] });
  });

  it('no envía la compra si falta el producto', async () => {
    const { fixture, registrar, componente } = preparar();
    componente.detalles.at(0).patchValue({ cantidad: 2, precio: 5 });

    componente.guardar();
    fixture.detectChanges();

    expect(registrar).not.toHaveBeenCalled();
  });

  it('al registrar bien muestra el mensaje y deja la misma fila vacía', async () => {
    const { fixture, componente } = preparar();
    await llenar(fixture, componente);
    const filaAntes = (fixture.nativeElement as HTMLElement).querySelector('tbody tr');

    componente.guardar();
    fixture.detectChanges();
    await fixture.whenStable();
    fixture.detectChanges();

    expect(textoDe(fixture)).toContain('Compra N° 4 registrada correctamente.');
    expect(componente.detalles.at(0).getRawValue()).toEqual({ idProducto: null, cantidad: 1, precio: 0 });
    expect((fixture.nativeElement as HTMLElement).querySelector('tbody tr')).toBe(filaAntes);
  });
});
