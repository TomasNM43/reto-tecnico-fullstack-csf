import { CurrencyPipe, DatePipe } from '@angular/common';
import { Component, OnInit, computed, effect, inject, signal, untracked } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { FormArray, FormBuilder, FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { startWith } from 'rxjs';

import { Producto } from '../../core/models/producto.models';
import { Venta } from '../../core/models/venta.models';
import { KardexService } from '../../core/services/kardex.service';
import { ProductosService } from '../../core/services/productos.service';
import { VentasService } from '../../core/services/ventas.service';
import { mensajeDeError } from '../../core/utils/errores';
import { calcularLinea, calcularTotales } from '../../core/utils/importes';
import { validarCantidad } from './validar-stock';

type FilaForm = FormGroup<{
  idProducto: FormControl<number | null>;
  cantidad: FormControl<number>;
}>;

@Component({
  selector: 'app-ventas',
  imports: [ReactiveFormsModule, CurrencyPipe, DatePipe],
  templateUrl: './ventas.html',
})
export class Ventas implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly productosApi = inject(ProductosService);
  private readonly kardexApi = inject(KardexService);
  private readonly ventasApi = inject(VentasService);

  protected readonly productos = signal<Producto[]>([]);
  protected readonly ventas = signal<Venta[]>([]);
  protected readonly guardando = signal(false);
  protected readonly error = signal<string | null>(null);
  protected readonly erroresStock = signal<string[]>([]);
  protected readonly exito = signal<string | null>(null);

  // Stock por producto. Si todavía no se pidió no existe la clave; null significa que no se pudo obtener.
  protected readonly stocks = signal<Record<number, number | null>>({});
  private readonly stocksSolicitados = new Set<number>();

  // Cada fila del detalle es un producto con la cantidad a vender (el precio sale del producto).
  protected readonly detalles = new FormArray<FilaForm>([this.nuevaFila()]);

  // El formulario necesita un FormGroup para que (ngSubmit) funcione y no se haga un envío nativo del navegador.
  protected readonly formulario = this.fb.group({ detalles: this.detalles });

  private readonly valores = toSignal(this.detalles.valueChanges.pipe(startWith(this.detalles.value)), {
    initialValue: this.detalles.value,
  });

  // Lo que se muestra de cada fila: producto, precio, stock y el error de stock si lo hay.
  protected readonly filas = computed(() =>
    this.valores().map((v) => {
      const producto = this.productos().find((p) => p.idProducto === v.idProducto) ?? null;
      const stock = producto ? this.stocks()[producto.idProducto] : undefined;
      const cantidad = Number(v.cantidad) || 0;
      return {
        producto,
        precio: producto?.precioVenta ?? 0,
        stock,
        cantidad,
        errorStock: this.errorDeStock(cantidad, stock),
      };
    }),
  );

  // Importes por línea y de toda la venta; se recalculan al teclear la cantidad.
  protected readonly calculo = computed(() => {
    const lineas = this.filas().map((f) => calcularLinea(f.cantidad, f.precio));
    return { lineas, totales: calcularTotales(lineas) };
  });

  constructor() {
    // Cada vez que se elige un producto se consulta su stock (una sola vez por producto).
    effect(() => {
      for (const v of this.valores()) {
        if (v.idProducto !== null && v.idProducto !== undefined) {
          untracked(() => this.cargarStock(v.idProducto as number));
        }
      }
    });
  }

  ngOnInit(): void {
    this.cargarProductos();
    this.cargarVentas();
  }

  protected agregarFila(): void {
    this.detalles.push(this.nuevaFila());
  }

  protected quitarFila(indice: number): void {
    // La venta siempre debe tener al menos un renglón.
    if (this.detalles.length > 1) this.detalles.removeAt(indice);
  }

  protected guardar(): void {
    this.error.set(null);
    this.exito.set(null);
    this.erroresStock.set([]);

    if (this.detalles.invalid) {
      this.detalles.markAllAsTouched();
      return;
    }

    // Antes de enviar se revisa que ninguna cantidad supere el stock.
    const errores = this.filas()
      .filter((f) => f.errorStock !== null)
      .map((f) => `${f.producto?.nombreProducto}: ${f.errorStock}`);

    if (errores.length > 0) {
      this.erroresStock.set(errores);
      return;
    }

    this.guardando.set(true);

    const detalles = this.detalles.getRawValue().map((fila) => ({
      idProducto: fila.idProducto as number,
      cantidad: fila.cantidad,
    }));

    this.ventasApi.registrar({ detalles }).subscribe({
      next: (venta) => {
        this.exito.set(`Venta N° ${venta.idVentaCab} registrada correctamente.`);
        this.reiniciarFormulario();
        this.guardando.set(false);
        this.cargarVentas();
      },
      error: (e) => {
        this.error.set(mensajeDeError(e));
        this.guardando.set(false);
      },
    });
  }

  private errorDeStock(cantidad: number, stock: number | null | undefined): string | null {
    // Sin stock conocido o sin cantidad no hay nada que validar (el servidor igual lo valida al guardar).
    if (typeof stock !== 'number' || cantidad <= 0) return null;

    return validarCantidad(cantidad, stock);
  }

  private reiniciarFormulario(): void {
    // Tras vender el stock cambió, así que se vuelve a consultar.
    this.stocks.set({});
    this.stocksSolicitados.clear();

    // Se deja una sola fila y se vacían sus campos (sin recrearla, para no rehacer toda la tabla).
    while (this.detalles.length > 1) this.detalles.removeAt(this.detalles.length - 1);
    this.detalles.at(0).reset();
  }

  private nuevaFila(): FilaForm {
    return this.fb.group({
      idProducto: new FormControl<number | null>(null, Validators.required),
      cantidad: this.fb.nonNullable.control(1, [Validators.required, Validators.min(1)]),
    });
  }

  private cargarStock(idProducto: number): void {
    if (this.stocksSolicitados.has(idProducto)) return;
    this.stocksSolicitados.add(idProducto);

    this.kardexApi.stock(idProducto).subscribe({
      next: (stock) => this.stocks.update((s) => ({ ...s, [idProducto]: stock })),
      error: () => this.stocks.update((s) => ({ ...s, [idProducto]: null })),
    });
  }

  private cargarProductos(): void {
    this.productosApi.listar().subscribe({
      next: (lista) => this.productos.set(lista),
      error: (e) => this.error.set(mensajeDeError(e)),
    });
  }

  private cargarVentas(): void {
    this.ventasApi.listar().subscribe({
      next: (lista) => this.ventas.set(lista),
      error: (e) => this.error.set(mensajeDeError(e)),
    });
  }
}
