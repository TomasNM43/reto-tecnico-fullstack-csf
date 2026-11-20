import { CurrencyPipe, DatePipe } from '@angular/common';
import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { FormArray, FormBuilder, FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { startWith } from 'rxjs';

import { Compra } from '../../core/models/compra.models';
import { Producto } from '../../core/models/producto.models';
import { ComprasService } from '../../core/services/compras.service';
import { ProductosService } from '../../core/services/productos.service';
import { mensajeDeError } from '../../core/utils/errores';
import { calcularLinea, calcularTotales } from '../../core/utils/importes';
import { ProductoModal } from '../../shared/producto-modal/producto-modal';

type FilaForm = FormGroup<{
  idProducto: FormControl<number | null>;
  cantidad: FormControl<number>;
  precio: FormControl<number>;
}>;

@Component({
  selector: 'app-compras',
  imports: [ReactiveFormsModule, CurrencyPipe, DatePipe, ProductoModal],
  templateUrl: './compras.html',
})
export class Compras implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly productosApi = inject(ProductosService);
  private readonly comprasApi = inject(ComprasService);

  protected readonly productos = signal<Producto[]>([]);
  protected readonly compras = signal<Compra[]>([]);
  protected readonly guardando = signal(false);
  protected readonly error = signal<string | null>(null);
  protected readonly exito = signal<string | null>(null);
  protected readonly modalProductoAbierto = signal(false);

  // Cada fila del detalle es un producto con su cantidad y precio de compra.
  protected readonly detalles = new FormArray<FilaForm>([this.nuevaFila()]);

  // El formulario necesita un FormGroup para que (ngSubmit) funcione y no se haga un envío nativo del navegador.
  protected readonly formulario = this.fb.group({ detalles: this.detalles });

  private readonly valores = toSignal(this.detalles.valueChanges.pipe(startWith(this.detalles.value)), {
    initialValue: this.detalles.value,
  });

  // Importes por línea y de toda la compra; se recalculan al teclear.
  protected readonly calculo = computed(() => {
    const lineas = this.valores().map((v) => calcularLinea(Number(v.cantidad) || 0, Number(v.precio) || 0));
    return { lineas, totales: calcularTotales(lineas) };
  });

  ngOnInit(): void {
    this.cargarProductos();
    this.cargarCompras();
  }

  protected agregarFila(): void {
    this.detalles.push(this.nuevaFila());
  }

  protected quitarFila(indice: number): void {
    // La compra siempre debe tener al menos un renglón.
    if (this.detalles.length > 1) this.detalles.removeAt(indice);
  }

  protected abrirModalProducto(): void {
    this.modalProductoAbierto.set(true);
  }

  protected cerrarModalProducto(): void {
    this.modalProductoAbierto.set(false);
  }

  // Cuando se registra un producto nuevo se agrega a la lista y se deja elegido en una fila libre.
  protected alRegistrarProducto(producto: Producto): void {
    this.productos.update((lista) => [...lista, producto]);

    const filaLibre = this.detalles.controls.find((fila) => fila.controls.idProducto.value === null);
    if (filaLibre) {
      filaLibre.controls.idProducto.setValue(producto.idProducto);
    } else {
      const fila = this.nuevaFila();
      fila.controls.idProducto.setValue(producto.idProducto);
      this.detalles.push(fila);
    }

    this.modalProductoAbierto.set(false);
  }

  protected guardar(): void {
    this.error.set(null);
    this.exito.set(null);

    if (this.detalles.invalid) {
      this.detalles.markAllAsTouched();
      return;
    }

    this.guardando.set(true);

    const detalles = this.detalles.getRawValue().map((fila) => ({
      idProducto: fila.idProducto as number,
      cantidad: fila.cantidad,
      precio: fila.precio,
    }));

    this.comprasApi.registrar({ detalles }).subscribe({
      next: (compra) => {
        this.exito.set(`Compra N° ${compra.idCompraCab} registrada correctamente.`);
        this.reiniciarFormulario();
        this.guardando.set(false);
        this.cargarCompras();
      },
      error: (e) => {
        this.error.set(mensajeDeError(e));
        this.guardando.set(false);
      },
    });
  }

  private reiniciarFormulario(): void {
    // Se deja una sola fila y se vacían sus campos (sin recrearla, para no rehacer toda la tabla).
    while (this.detalles.length > 1) this.detalles.removeAt(this.detalles.length - 1);
    this.detalles.at(0).reset();
  }

  private nuevaFila(): FilaForm {
    return this.fb.group({
      idProducto: new FormControl<number | null>(null, Validators.required),
      cantidad: this.fb.nonNullable.control(1, [Validators.required, Validators.min(1)]),
      precio: this.fb.nonNullable.control(0, [Validators.required, Validators.min(0.01)]),
    });
  }

  private cargarProductos(): void {
    this.productosApi.listar().subscribe({
      next: (lista) => this.productos.set(lista),
      error: (e) => this.error.set(mensajeDeError(e)),
    });
  }

  private cargarCompras(): void {
    this.comprasApi.listar().subscribe({
      next: (lista) => this.compras.set(lista),
      error: (e) => this.error.set(mensajeDeError(e)),
    });
  }
}
