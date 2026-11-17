-- Script 02: procedimientos almacenados (insertar, listar, actualizar, eliminar).
-- En PostgreSQL se implementan como funciones. Solo hacen acceso a datos:
-- los cálculos (totales, stock, Kardex) son lógica de negocio y viven en el código .NET.

-- ===================== PRODUCTOS =====================
CREATE OR REPLACE FUNCTION sp_producto_insertar(
    p_nombre VARCHAR, p_nrolote VARCHAR, p_costo NUMERIC, p_precioventa NUMERIC)
RETURNS INT LANGUAGE sql AS $$
    INSERT INTO productos (nombre_producto, nrolote, costo, precioventa)
    VALUES (p_nombre, p_nrolote, p_costo, p_precioventa)
    RETURNING id_producto;
$$;

CREATE OR REPLACE FUNCTION sp_producto_listar()
RETURNS SETOF productos LANGUAGE sql AS $$
    SELECT * FROM productos ORDER BY id_producto;
$$;

CREATE OR REPLACE FUNCTION sp_producto_actualizar(
    p_id INT, p_nombre VARCHAR, p_nrolote VARCHAR, p_costo NUMERIC, p_precioventa NUMERIC)
RETURNS INT LANGUAGE sql AS $$
    WITH u AS (
        UPDATE productos
           SET nombre_producto = p_nombre, nrolote = p_nrolote,
               costo = p_costo, precioventa = p_precioventa
         WHERE id_producto = p_id
        RETURNING 1)
    SELECT COUNT(*)::INT FROM u; -- filas afectadas
$$;

CREATE OR REPLACE FUNCTION sp_producto_eliminar(p_id INT)
RETURNS INT LANGUAGE sql AS $$
    WITH d AS (DELETE FROM productos WHERE id_producto = p_id RETURNING 1)
    SELECT COUNT(*)::INT FROM d;
$$;

-- ===================== COMPRAS =====================
CREATE OR REPLACE FUNCTION sp_compracab_insertar(
    p_subtotal NUMERIC, p_igv NUMERIC, p_total NUMERIC)
RETURNS INT LANGUAGE sql AS $$
    INSERT INTO compracab (subtotal, igv, total)
    VALUES (p_subtotal, p_igv, p_total)
    RETURNING id_compracab;
$$;

CREATE OR REPLACE FUNCTION sp_compradet_insertar(
    p_id_compracab INT, p_id_producto INT, p_cantidad INT, p_precio NUMERIC,
    p_sub_total NUMERIC, p_igv NUMERIC, p_total NUMERIC)
RETURNS INT LANGUAGE sql AS $$
    INSERT INTO compradet (id_compracab, id_producto, cantidad, precio, sub_total, igv, total)
    VALUES (p_id_compracab, p_id_producto, p_cantidad, p_precio, p_sub_total, p_igv, p_total)
    RETURNING id_compradet;
$$;

CREATE OR REPLACE FUNCTION sp_compracab_listar()
RETURNS SETOF compracab LANGUAGE sql AS $$
    SELECT * FROM compracab ORDER BY id_compracab DESC;
$$;

CREATE OR REPLACE FUNCTION sp_compradet_listar(p_id_compracab INT)
RETURNS SETOF compradet LANGUAGE sql AS $$
    SELECT * FROM compradet WHERE id_compracab = p_id_compracab ORDER BY id_compradet;
$$;

CREATE OR REPLACE FUNCTION sp_compracab_actualizar(
    p_id INT, p_subtotal NUMERIC, p_igv NUMERIC, p_total NUMERIC)
RETURNS INT LANGUAGE sql AS $$
    WITH u AS (
        UPDATE compracab SET subtotal = p_subtotal, igv = p_igv, total = p_total
         WHERE id_compracab = p_id RETURNING 1)
    SELECT COUNT(*)::INT FROM u;
$$;

CREATE OR REPLACE FUNCTION sp_compracab_eliminar(p_id INT) -- el detalle se borra en cascada
RETURNS INT LANGUAGE sql AS $$
    WITH d AS (DELETE FROM compracab WHERE id_compracab = p_id RETURNING 1)
    SELECT COUNT(*)::INT FROM d;
$$;

-- ===================== VENTAS =====================
CREATE OR REPLACE FUNCTION sp_ventacab_insertar(
    p_subtotal NUMERIC, p_igv NUMERIC, p_total NUMERIC)
RETURNS INT LANGUAGE sql AS $$
    INSERT INTO ventacab (subtotal, igv, total)
    VALUES (p_subtotal, p_igv, p_total)
    RETURNING id_ventacab;
$$;

CREATE OR REPLACE FUNCTION sp_ventadet_insertar(
    p_id_ventacab INT, p_id_producto INT, p_cantidad INT, p_precio NUMERIC,
    p_sub_total NUMERIC, p_igv NUMERIC, p_total NUMERIC)
RETURNS INT LANGUAGE sql AS $$
    INSERT INTO ventadet (id_ventacab, id_producto, cantidad, precio, sub_total, igv, total)
    VALUES (p_id_ventacab, p_id_producto, p_cantidad, p_precio, p_sub_total, p_igv, p_total)
    RETURNING id_ventadet;
$$;

CREATE OR REPLACE FUNCTION sp_ventacab_listar()
RETURNS SETOF ventacab LANGUAGE sql AS $$
    SELECT * FROM ventacab ORDER BY id_ventacab DESC;
$$;

CREATE OR REPLACE FUNCTION sp_ventadet_listar(p_id_ventacab INT)
RETURNS SETOF ventadet LANGUAGE sql AS $$
    SELECT * FROM ventadet WHERE id_ventacab = p_id_ventacab ORDER BY id_ventadet;
$$;

CREATE OR REPLACE FUNCTION sp_ventacab_actualizar(
    p_id INT, p_subtotal NUMERIC, p_igv NUMERIC, p_total NUMERIC)
RETURNS INT LANGUAGE sql AS $$
    WITH u AS (
        UPDATE ventacab SET subtotal = p_subtotal, igv = p_igv, total = p_total
         WHERE id_ventacab = p_id RETURNING 1)
    SELECT COUNT(*)::INT FROM u;
$$;

CREATE OR REPLACE FUNCTION sp_ventacab_eliminar(p_id INT) -- el detalle se borra en cascada
RETURNS INT LANGUAGE sql AS $$
    WITH d AS (DELETE FROM ventacab WHERE id_ventacab = p_id RETURNING 1)
    SELECT COUNT(*)::INT FROM d;
$$;

-- ===================== MOVIMIENTOS =====================
CREATE OR REPLACE FUNCTION sp_movimientocab_insertar(
    p_id_tipomovimiento SMALLINT, p_id_documentoorigen INT)
RETURNS INT LANGUAGE sql AS $$
    INSERT INTO movimientocab (id_tipomovimiento, id_documentoorigen)
    VALUES (p_id_tipomovimiento, p_id_documentoorigen)
    RETURNING id_movimientocab;
$$;

CREATE OR REPLACE FUNCTION sp_movimientodet_insertar(
    p_id_movimientocab INT, p_id_producto INT, p_cantidad INT)
RETURNS INT LANGUAGE sql AS $$
    INSERT INTO movimientodet (id_movimientocab, id_producto, cantidad)
    VALUES (p_id_movimientocab, p_id_producto, p_cantidad)
    RETURNING id_movimientodet;
$$;

-- Movimientos de un producto, sin calcular nada (el stock lo calcula el usuario en .NET).
CREATE OR REPLACE FUNCTION sp_movimiento_listar_por_producto(p_id_producto INT)
RETURNS TABLE (fec_registro TIMESTAMP, id_tipomovimiento SMALLINT, cantidad INT)
LANGUAGE sql AS $$
    SELECT c.fec_registro, c.id_tipomovimiento, d.cantidad
      FROM movimientodet d
      JOIN movimientocab c ON c.id_movimientocab = d.id_movimientocab
     WHERE d.id_producto = p_id_producto
     ORDER BY c.fec_registro, d.id_movimientodet;
$$;

CREATE OR REPLACE FUNCTION sp_movimientocab_eliminar(p_id INT) -- el detalle se borra en cascada
RETURNS INT LANGUAGE sql AS $$
    WITH d AS (DELETE FROM movimientocab WHERE id_movimientocab = p_id RETURNING 1)
    SELECT COUNT(*)::INT FROM d;
$$;

-- ===================== USUARIOS =====================
CREATE OR REPLACE FUNCTION sp_usuario_obtener_por_username(p_username VARCHAR)
RETURNS SETOF usuarios LANGUAGE sql AS $$
    SELECT * FROM usuarios WHERE username = p_username;
$$;
