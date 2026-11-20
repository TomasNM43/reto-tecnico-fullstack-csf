import { CurrencyPipe } from '@angular/common';
import { Component, OnInit, computed, inject, signal } from '@angular/core';

import { KardexItem } from '../../core/models/kardex.models';
import { KardexService } from '../../core/services/kardex.service';
import { mensajeDeError } from '../../core/utils/errores';
import { MovimientosModal } from './movimientos-modal/movimientos-modal';

@Component({
  selector: 'app-kardex',
  imports: [CurrencyPipe, MovimientosModal],
  templateUrl: './kardex.html',
})
export class Kardex implements OnInit {
  private readonly kardexApi = inject(KardexService);

  protected readonly items = signal<KardexItem[]>([]);
  protected readonly cargando = signal(true);
  protected readonly error = signal<string | null>(null);
  protected readonly filtro = signal('');

  // Producto cuyo modal de movimientos está abierto (null si no hay ninguno).
  protected readonly productoSeleccionado = signal<KardexItem | null>(null);

  protected readonly itemsFiltrados = computed(() => {
    const texto = this.filtro().trim().toLowerCase();
    if (!texto) return this.items();

    return this.items().filter(
      (i) => i.nombreProducto.toLowerCase().includes(texto) || String(i.idProducto) === texto,
    );
  });

  ngOnInit(): void {
    this.cargar();
  }

  protected cargar(): void {
    this.cargando.set(true);
    this.error.set(null);

    this.kardexApi.listar().subscribe({
      next: (lista) => {
        this.items.set(lista);
        this.cargando.set(false);
      },
      error: (e) => {
        this.error.set(mensajeDeError(e));
        this.cargando.set(false);
      },
    });
  }

  protected verMovimientos(item: KardexItem): void {
    this.productoSeleccionado.set(item);
  }

  protected cerrarMovimientos(): void {
    this.productoSeleccionado.set(null);
  }
}
