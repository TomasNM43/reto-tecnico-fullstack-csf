import { DOCUMENT } from '@angular/common';
import { Component, HostListener, OnDestroy, OnInit, computed, inject, input, output, signal } from '@angular/core';

import { KardexItem, MovimientoProducto } from '../../../core/models/kardex.models';
import { KardexService } from '../../../core/services/kardex.service';
import { mensajeDeError } from '../../../core/utils/errores';
import { formatearMovimiento } from '../formato-movimientos';

// Modal con los movimientos (entradas y salidas) de un producto del Kardex.
@Component({
  selector: 'app-movimientos-modal',
  templateUrl: './movimientos-modal.html',
})
export class MovimientosModal implements OnInit, OnDestroy {
  private readonly kardexApi = inject(KardexService);
  private readonly document = inject(DOCUMENT);

  readonly producto = input.required<KardexItem>();
  readonly cerrado = output<void>();

  protected readonly cargando = signal(true);
  protected readonly error = signal<string | null>(null);
  protected readonly movimientos = signal<MovimientoProducto[]>([]);

  protected readonly filas = computed(() => this.movimientos().map((m) => formatearMovimiento(m)));

  ngOnInit(): void {
    // Evita que la página de fondo se desplace mientras el modal está abierto.
    this.document.body.classList.add('modal-open');

    this.kardexApi.movimientos(this.producto().idProducto).subscribe({
      next: (lista) => {
        this.movimientos.set(lista);
        this.cargando.set(false);
      },
      error: (e) => {
        this.error.set(mensajeDeError(e));
        this.cargando.set(false);
      },
    });
  }

  ngOnDestroy(): void {
    this.document.body.classList.remove('modal-open');
  }

  @HostListener('document:keydown.escape')
  protected cerrar(): void {
    this.cerrado.emit();
  }
}
