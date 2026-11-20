import { DOCUMENT } from '@angular/common';
import { Component, HostListener, OnDestroy, OnInit, inject, output, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';

import { Producto } from '../../core/models/producto.models';
import { ProductosService } from '../../core/services/productos.service';
import { mensajeDeError } from '../../core/utils/errores';

// Modal para registrar un producto nuevo. Se muestra mientras exista en la pantalla (con @if).
@Component({
  selector: 'app-producto-modal',
  imports: [ReactiveFormsModule],
  templateUrl: './producto-modal.html',
})
export class ProductoModal implements OnInit, OnDestroy {
  private readonly fb = inject(FormBuilder);
  private readonly productosApi = inject(ProductosService);
  private readonly document = inject(DOCUMENT);

  readonly cerrado = output<void>();
  readonly registrado = output<Producto>();

  protected readonly guardando = signal(false);
  protected readonly error = signal<string | null>(null);

  protected readonly form = this.fb.nonNullable.group({
    nombreProducto: ['', [Validators.required, Validators.maxLength(150)]],
    nroLote: ['', [Validators.required, Validators.maxLength(50)]],
    costo: [0, [Validators.required, Validators.min(0)]],
    precioVenta: [0, [Validators.required, Validators.min(0)]],
  });

  ngOnInit(): void {
    // Evita que la página de fondo se desplace mientras el modal está abierto.
    this.document.body.classList.add('modal-open');
  }

  ngOnDestroy(): void {
    this.document.body.classList.remove('modal-open');
  }

  @HostListener('document:keydown.escape')
  protected cerrar(): void {
    if (!this.guardando()) this.cerrado.emit();
  }

  protected guardar(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.guardando.set(true);
    this.error.set(null);

    this.productosApi.registrar(this.form.getRawValue()).subscribe({
      next: (producto) => this.registrado.emit(producto),
      error: (e) => {
        this.error.set(mensajeDeError(e));
        this.guardando.set(false);
      },
    });
  }
}
