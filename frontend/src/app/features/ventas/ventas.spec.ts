import { TestBed } from '@angular/core/testing';
import { FormArray, FormControl, FormGroup } from '@angular/forms';
import { of } from 'rxjs';

import { KardexService } from '../../core/services/kardex.service';
import { ProductosService } from '../../core/services/productos.service';
import { VentasService } from '../../core/services/ventas.service';
import { Ventas } from './ventas';

interface Interno {
  detalles: FormArray<FormGroup<{ idProducto: FormControl<number | null>; cantidad: FormControl<number> }>>;
  guardar(): void;
}

const productos = [
  { idProducto: 1, nombreProducto: 'Paracetamol', nroLote: 'L1', fecRegistro: '', costo: 10, precioVenta: 13.5 },
];

describe('Ventas', () => {
  function preparar(stock: number) {
    const registrar = vi.fn().mockReturnValue(
      of({ idVentaCab: 1, fecRegistro: '', subTotal: 0, igv: 0, total: 0, detalles: [] }),
    );

    TestBed.configureTestingModule({
      imports: [Ventas],
      providers: [
        { provide: ProductosService, useValue: { listar: () => of(productos) } },
        { provide: KardexService, useValue: { stock: () => of(stock) } },
        { provide: VentasService, useValue: { listar: () => of([]), registrar } },
      ],
    });

    const fixture = TestBed.createComponent(Ventas);
    fixture.detectChanges();

    return { fixture, registrar, componente: fixture.componentInstance as unknown as Interno };
  }

  async function elegir(fixture: ReturnType<typeof preparar>['fixture'], componente: Interno, cantidad: number) {
    componente.detalles.at(0).patchValue({ idProducto: 1, cantidad });
    await fixture.whenStable();
    fixture.detectChanges();
    await fixture.whenStable();
    fixture.detectChanges();
  }

  const textoDe = (fixture: ReturnType<typeof preparar>['fixture']) => (fixture.nativeElement as HTMLElement).textContent ?? '';

  it('muestra el precio y el stock al elegir un producto', async () => {
    const { fixture, componente } = preparar(7);

    await elegir(fixture, componente, 2);

    expect(textoDe(fixture)).toMatch(/S\/ 13\.50\s*7/);
  });

  it('calcula subtotal, IGV y total al digitar la cantidad', async () => {
    const { fixture, componente } = preparar(7);

    await elegir(fixture, componente, 2);

    const texto = textoDe(fixture);
    expect(texto).toContain('S/ 27.00');
    expect(texto).toContain('S/ 4.86');
    expect(texto).toContain('S/ 31.86');
  });

  it('muestra el mensaje bajo la cantidad si supera el stock', async () => {
    const { fixture, componente } = preparar(7);

    await elegir(fixture, componente, 10);

    expect(textoDe(fixture)).toContain('Solo hay 7 unidades disponibles.');
  });

  it('no envía la venta si la cantidad supera el stock y lista el problema', async () => {
    const { fixture, registrar, componente } = preparar(7);
    await elegir(fixture, componente, 10);

    componente.guardar();
    fixture.detectChanges();

    expect(registrar).not.toHaveBeenCalled();
    expect(textoDe(fixture)).toContain('No se puede registrar la venta');
    expect(textoDe(fixture)).toContain('Paracetamol: Solo hay 7 unidades disponibles.');
  });

  it('envía al servicio los productos y cantidades elegidos', async () => {
    const { fixture, registrar, componente } = preparar(7);
    await elegir(fixture, componente, 2);

    componente.guardar();

    expect(registrar).toHaveBeenCalledWith({ detalles: [{ idProducto: 1, cantidad: 2 }] });
  });
});
