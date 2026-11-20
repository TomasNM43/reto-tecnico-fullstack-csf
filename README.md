# Clínica San Felipe – Compras, Ventas y Kardex

Sistema de compras, ventas e inventario (Kardex). El backend es una API en .NET 8 dividida en microservicios detrás de un API Gateway (YARP) con JWT, y el frontend es una aplicación Angular con Bootstrap 5. La base de datos es PostgreSQL.

## Estructura

```
backend/
  src/
    ApiGateway/            punto de entrada (YARP)
    BuildingBlocks/        seguridad y utilidades compartidas
    Auth.Service/          login y emisión del JWT
    Productos.Service/     catálogo de productos
    Compras.Service/       compras
    Ventas.Service/        ventas
    Movimientos.Service/   movimientos, stock y Kardex
  database/                scripts SQL
  postman/                 colección de Postman
  tests/                   pruebas unitarias
frontend/                  aplicación Angular
```

Cada microservicio sigue Clean Architecture (Domain, Application, Infrastructure y Api).

## Requisitos

- Docker con Compose
- Node.js 22 (para el frontend)
- .NET SDK 8 o superior (solo si quieres compilar o probar el backend sin Docker)

## Ejecución

Backend y base de datos, desde la raíz del repositorio:

```powershell
docker compose up -d --build
```

El gateway queda en `http://localhost:5100` y la base de datos en el puerto `54320`. Los scripts de `backend/database` se cargan solos la primera vez. Los puertos, la clave del JWT y la contraseña de la base se pueden cambiar con un archivo `.env` (ver `.env.example`).

Frontend:

```powershell
cd frontend
npm install
npm start
```

Se abre en `http://localhost:4200`.

Usuario de prueba: `admin` / `Admin123*`.

Para detener todo: `docker compose down` (con `-v` también borra los datos).

## Servicios expuestos por el gateway

| Ruta | Servicio | Requiere token |
|---|---|---|
| `/api/auth/login` | Auth | no |
| `/api/productos` | Productos | sí |
| `/api/kardex` | Movimientos | sí |
| `/api/compras` | Compras | sí |
| `/api/ventas` | Ventas | sí |

## Seguridad

JWT con expiración de 30 minutos, rate limiting, CORS limitado al origen del frontend y cabeceras HTTP de seguridad.

## Postman

En `backend/postman` están la colección y el environment. Ejecuta primero `0. Auth > Login`; el token se guarda solo.

## Pruebas

```powershell
cd backend
dotnet test tests\ClinicaSanFelipe.Tests

cd ..\frontend
npm test -- --watch=false
```

## Notas

- El IGV es `Subtotal × 0.18` y la tasa es configurable en la sección `Negocio` de `appsettings.json`.
- Se usó PostgreSQL en lugar de SQL Server, por eso los procedimientos almacenados son funciones PL/pgSQL.
