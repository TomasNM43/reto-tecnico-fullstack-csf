import { TestBed } from '@angular/core/testing';
import { of } from 'rxjs';

import { KardexService } from '../../core/services/kardex.service';
import { Kardex } from './kardex';

const items = [
  { idProducto: 1, nombreProducto: 'Paracetamol', stockActual: 7, costo: 10, precioVenta: 13.5 },
  { idProducto: 2, nombreProducto: 'Ibuprofeno', stockActual: 0, costo: 12, precioVenta: 16.2 },
];

const movimientos = [
  { fecRegistro: '2026-11-20T10:00:00', idTipoMovimiento: 1, tipoMovimiento: 'Entrada', cantidad: 10 },
  { fecRegistro: '2026-11-20T11:00:00', idTipoMovimiento: 2, tipoMovimiento: 'Salida', cantidad: 3 },
];

describe('Kardex', () => {
  function preparar() {
    TestBed.configureTestingModule({
      imports: [Kardex],
      providers: [{ provide: KardexService, useValue: { listar: () => of(items), movimientos: () => of(movimientos) } }],
    });

    const fixture = TestBed.createComponent(Kardex);
    fixture.detectChanges();
    return { fixture, el: fixture.nativeElement as HTMLElement };
  }

  it('muestra una fila por producto con su stock', async () => {
    const { fixture, el } = preparar();
    await fixture.whenStable();
    fixture.detectChanges();

    const filas = el.querySelectorAll('tbody tr');
    expect(filas.length).toBe(2);
    expect(filas[0].textContent).toContain('Paracetamol');
    expect(filas[0].textContent).toContain('7');
    expect(filas[1].textContent).toContain('Ibuprofeno');
  });

  it('abre el modal con los movimientos del producto elegido', async () => {
    const { fixture, el } = preparar();
    await fixture.whenStable();
    fixture.detectChanges();

    (el.querySelector('tbody button') as HTMLButtonElement).click();
    fixture.detectChanges();
    await fixture.whenStable();
    fixture.detectChanges();

    const modal = el.querySelector('app-movimientos-modal');
    expect(modal).not.toBeNull();
    expect(modal?.textContent).toContain('Paracetamol');
    expect(modal?.textContent).toContain('Entrada');
    expect(modal?.textContent).toContain('Salida');
  });
});
