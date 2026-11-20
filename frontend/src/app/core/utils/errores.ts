import { HttpErrorResponse } from '@angular/common/http';

// Saca un mensaje legible de una respuesta de error del API (formato ProblemDetails).
export function mensajeDeError(error: unknown): string {
  if (!(error instanceof HttpErrorResponse)) {
    return 'Ocurrió un error inesperado.';
  }

  if (error.status === 0) {
    return 'No se pudo conectar con el servidor. Revisa que el API esté en línea.';
  }

  if (error.status === 429) {
    return 'Demasiados intentos. Espera un momento y vuelve a probar.';
  }

  const cuerpo = error.error;

  // Errores de validación: { errors: { Campo: ['mensaje'] } }
  if (cuerpo?.errors && typeof cuerpo.errors === 'object') {
    const mensajes = Object.values(cuerpo.errors).flat() as string[];
    if (mensajes.length > 0) {
      return mensajes.join(' ');
    }
  }

  return cuerpo?.detail || cuerpo?.title || 'Ocurrió un error al procesar la solicitud.';
}
