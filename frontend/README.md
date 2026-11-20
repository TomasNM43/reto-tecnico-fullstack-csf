# Frontend

Aplicación Angular 21 con Bootstrap 5. Se conecta al API Gateway (`http://localhost:5100` por defecto; se cambia en `src/environments/environment.ts`).

## Requisitos

Node 20.19 o superior (o 22.12+) y npm.

## Comandos

```powershell
npm install
npm start          # http://localhost:4200
npm test           # pruebas unitarias (una sola vez: npm test -- --watch=false)
npm run build      # genera dist/clinica-frontend
```

El gateway solo acepta peticiones desde `http://localhost:4200`, así que conviene levantar el front en ese puerto.

## Estructura

- `src/app/core`: modelos tipados del API, servicios HTTP, interceptor del JWT, guards y utilidades.
- `src/app/layout`: barra de navegación y contenedor de las vistas.
- `src/app/shared`: componentes que se reutilizan entre pantallas (por ahora, el modal de registrar producto).
- `src/app/features`: pantallas (login, compras, ventas y kardex).
