# Reto técnico Fullstack – Clínica San Felipe

Solución al reto técnico para el puesto de Especialista Desarrollo Core en la Clínica San Felipe. Es un sistema de compras, ventas e inventario (Kardex) hecho con una API en .NET 8 dividida en microservicios y una aplicación web en Angular.

## Cómo está armado

- **Backend:** microservicios en .NET 8 (Auth, Productos, Compras, Ventas y Movimientos) detrás de un API Gateway con YARP. La seguridad se hace con JWT.
- **Frontend:** Angular con Bootstrap 5, adaptable a celular y escritorio.
- **Base de datos:** PostgreSQL corriendo en Docker.

Cada microservicio sigue Clean Architecture, con sus capas Domain, Application, Infrastructure y Api.

## Decisiones que tomé

- El IGV se calcula como `Subtotal × 0.18`. La tasa queda configurable y no escrita a mano en el código.
- Los productos tienen su propio servicio (`Productos.Service`). El servicio de Movimientos se encarga de los movimientos, el stock y el Kardex.
- El enunciado habla de SQL Server, pero usé PostgreSQL. Por eso los procedimientos almacenados son funciones de PL/pgSQL.
- Para entrar al sistema hay un usuario de prueba: `admin` / `Admin123*`.

## Estructura del repositorio

```
backend/
  src/
    ApiGateway/            puerta de entrada (YARP)
    BuildingBlocks/        seguridad y utilidades compartidas
    Auth.Service/          login y emisión del JWT
    Productos.Service/     catálogo de productos
    Compras.Service/       compras
    Ventas.Service/        ventas
    Movimientos.Service/   movimientos, stock y Kardex
  database/                scripts SQL
  postman/                 colección de Postman
  ClinicaSanFelipe.sln
frontend/                  aplicación Angular
docs/                      documentación
```

## Base de datos

Los scripts están en `backend/database` y se ejecutan en este orden:

1. `01_tablas.sql`: tablas, llaves e índices.
2. `02_procedimientos.sql`: funciones para insertar, listar, actualizar y eliminar.
3. `03_datos_semilla.sql`: usuario de prueba y algunos productos.

Para probarlos rápido con Docker:

```powershell
docker run -d --name pg-test -e POSTGRES_PASSWORD=test -e POSTGRES_DB=clinica -p 54329:5432 postgres:16
cd backend\database
foreach($f in '01_tablas','02_procedimientos','03_datos_semilla'){ Get-Content "$f.sql" -Raw -Encoding UTF8 | docker exec -i pg-test psql -U postgres -d clinica -v ON_ERROR_STOP=1 -q }
```

## Seguridad

Toda la seguridad compartida vive en `BuildingBlocks` y cada servicio la activa con `builder.AddBuildingBlocks(...)` y `app.UseBuildingBlocks()`:

- **JWT** con expiración de 30 minutos y sin margen de tolerancia.
- **Rate limiting** por IP, con una política más estricta para el login.
- **CORS** limitado a los orígenes de `Cors:AllowedOrigins` (por defecto `http://localhost:4200`).
- **Cabeceras HTTP de seguridad** (`X-Content-Type-Options`, `X-Frame-Options`, `Content-Security-Policy`, `Referrer-Policy`, `Strict-Transport-Security`) y sin la cabecera `Server`.
- Manejo global de errores con respuestas `ProblemDetails`.
- Swagger con el botón Authorize, solo en desarrollo.

La clave del JWT en `appsettings.json` es solo para desarrollo; en un entorno real se cambia por una variable de entorno (`Jwt__Key`).

## Login

`Auth.Service` expone `POST /api/auth/login` con `{ "username": "...", "password": "..." }`. Si las credenciales son correctas devuelve el JWT (válido 30 minutos), su fecha de expiración y el rol. Si no, responde 401 con el mismo mensaje tanto para usuario inexistente como para clave incorrecta. Este endpoint tiene un límite de 5 intentos por minuto.

La cadena de conexión está en `ConnectionStrings:Default` del `appsettings.json` del servicio. Si ya tienes otro PostgreSQL en el puerto 5432, cámbiala con la variable de entorno `ConnectionStrings__Default`.

## Productos, movimientos y Kardex

`Productos.Service` (puerto 5102 en desarrollo) maneja el catálogo: `GET/POST /api/productos`, `GET/PUT /api/productos/{id}`.

`Movimientos.Service` (puerto 5103) guarda los movimientos de inventario y arma el Kardex:

- `POST /api/movimientos`: registra un movimiento (Entrada = 1, Salida = 2) con su detalle. Lo usan Compras y Ventas.
- `GET /api/kardex`: tabla del Kardex con el stock actual de cada producto.
- `GET /api/kardex/{idProducto}/movimientos`: movimientos de un producto.
- `GET /api/kardex/{idProducto}/stock`: stock actual de un producto.

Movimientos consulta a Productos por HTTP a través de una Facade (`IProductosFacade`) y reenvía el JWT del usuario. La URL está en `Services:Productos` del `appsettings.json`.

## Compras

`Compras.Service` (puerto 5104 en desarrollo): `GET /api/compras` y `POST /api/compras`.

Para registrar una compra llama a dos servicios a través de Facades: `IProductosFacade` (actualiza el costo y el precio de venta del producto) e `IMovimientosFacade` (registra la entrada). La Facade de movimientos va envuelta en un Decorator que deja en el log cada llamada.

La tasa del IGV (`0.18`) y el factor del precio de venta (`1.35`) están en la sección `Negocio` del `appsettings.json`.

## Compilar el backend

```powershell
cd backend
dotnet build
```
