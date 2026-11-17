-- Script 01: creación de tablas, llaves, restricciones e índices (PostgreSQL).
-- Los nombres coinciden con los del enunciado (PostgreSQL los guarda en minúsculas).

CREATE EXTENSION IF NOT EXISTS pgcrypto; -- se usa para generar el hash BCrypt del usuario semilla

-- Productos (dueño: Productos.Service)
CREATE TABLE IF NOT EXISTS productos (
    id_producto     INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    nombre_producto VARCHAR(150)  NOT NULL,
    nrolote         VARCHAR(50)   NOT NULL,
    fec_registro    TIMESTAMP     NOT NULL DEFAULT NOW(),
    costo           NUMERIC(18,2) NOT NULL DEFAULT 0 CHECK (costo >= 0),
    precioventa     NUMERIC(18,2) NOT NULL DEFAULT 0 CHECK (precioventa >= 0)
);
CREATE INDEX IF NOT EXISTS ix_productos_nombre ON productos (nombre_producto);

-- Compras (dueño: Compras.Service)
CREATE TABLE IF NOT EXISTS compracab (
    id_compracab INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    fecregistro  TIMESTAMP     NOT NULL DEFAULT NOW(),
    subtotal     NUMERIC(18,2) NOT NULL DEFAULT 0,
    igv          NUMERIC(18,2) NOT NULL DEFAULT 0,
    total        NUMERIC(18,2) NOT NULL DEFAULT 0
);

CREATE TABLE IF NOT EXISTS compradet (
    id_compradet INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    id_compracab INT           NOT NULL REFERENCES compracab (id_compracab) ON DELETE CASCADE,
    id_producto  INT           NOT NULL REFERENCES productos (id_producto),
    cantidad     INT           NOT NULL CHECK (cantidad > 0),
    precio       NUMERIC(18,2) NOT NULL CHECK (precio >= 0),
    sub_total    NUMERIC(18,2) NOT NULL DEFAULT 0,
    igv          NUMERIC(18,2) NOT NULL DEFAULT 0,
    total        NUMERIC(18,2) NOT NULL DEFAULT 0
);
CREATE INDEX IF NOT EXISTS ix_compradet_cab ON compradet (id_compracab);
CREATE INDEX IF NOT EXISTS ix_compradet_producto ON compradet (id_producto);

-- Ventas (dueño: Ventas.Service)
CREATE TABLE IF NOT EXISTS ventacab (
    id_ventacab INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    fecregistro TIMESTAMP     NOT NULL DEFAULT NOW(),
    subtotal    NUMERIC(18,2) NOT NULL DEFAULT 0,
    igv         NUMERIC(18,2) NOT NULL DEFAULT 0,
    total       NUMERIC(18,2) NOT NULL DEFAULT 0
);

CREATE TABLE IF NOT EXISTS ventadet (
    id_ventadet INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    id_ventacab INT           NOT NULL REFERENCES ventacab (id_ventacab) ON DELETE CASCADE,
    id_producto INT           NOT NULL REFERENCES productos (id_producto),
    cantidad    INT           NOT NULL CHECK (cantidad > 0),
    precio      NUMERIC(18,2) NOT NULL CHECK (precio >= 0),
    sub_total   NUMERIC(18,2) NOT NULL DEFAULT 0,
    igv         NUMERIC(18,2) NOT NULL DEFAULT 0,
    total       NUMERIC(18,2) NOT NULL DEFAULT 0
);
CREATE INDEX IF NOT EXISTS ix_ventadet_cab ON ventadet (id_ventacab);
CREATE INDEX IF NOT EXISTS ix_ventadet_producto ON ventadet (id_producto);

-- Movimientos (dueño: Movimientos.Service)
-- id_tipomovimiento: 1 = Entrada, 2 = Salida
-- id_documentoorigen: id de la compra (entrada) o de la venta (salida).
-- No lleva FK porque apunta a una tabla u otra según el tipo.
CREATE TABLE IF NOT EXISTS movimientocab (
    id_movimientocab   INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    fec_registro       TIMESTAMP NOT NULL DEFAULT NOW(),
    id_tipomovimiento  SMALLINT  NOT NULL CHECK (id_tipomovimiento IN (1, 2)),
    id_documentoorigen INT       NOT NULL
);
CREATE INDEX IF NOT EXISTS ix_movimientocab_origen ON movimientocab (id_tipomovimiento, id_documentoorigen);

CREATE TABLE IF NOT EXISTS movimientodet (
    id_movimientodet INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    id_movimientocab INT NOT NULL REFERENCES movimientocab (id_movimientocab) ON DELETE CASCADE,
    id_producto      INT NOT NULL REFERENCES productos (id_producto),
    cantidad         INT NOT NULL CHECK (cantidad > 0)
);
CREATE INDEX IF NOT EXISTS ix_movimientodet_cab ON movimientodet (id_movimientocab);
CREATE INDEX IF NOT EXISTS ix_movimientodet_producto ON movimientodet (id_producto);

-- Usuarios (solo para autenticar; dueño: Auth.Service)
CREATE TABLE IF NOT EXISTS usuarios (
    id           INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    username     VARCHAR(50)  NOT NULL UNIQUE,
    passwordhash VARCHAR(100) NOT NULL,
    rol          VARCHAR(30)  NOT NULL DEFAULT 'Usuario'
);
