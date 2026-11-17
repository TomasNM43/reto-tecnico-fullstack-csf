-- Script 03: datos semilla. Se puede ejecutar varias veces sin duplicar.

-- Usuario de prueba: admin / Admin123*  (hash BCrypt generado con pgcrypto)
INSERT INTO usuarios (username, passwordhash, rol)
SELECT 'admin', crypt('Admin123*', gen_salt('bf', 11)), 'Admin'
WHERE NOT EXISTS (SELECT 1 FROM usuarios WHERE username = 'admin');

-- Productos de ejemplo (solo se insertan si la tabla está vacía)
INSERT INTO productos (nombre_producto, nrolote, costo, precioventa)
SELECT v.nombre, v.lote, v.costo, v.precio
FROM (VALUES
    ('Paracetamol 500 mg x 100',  'L2026-001', 10.00, 13.50),
    ('Ibuprofeno 400 mg x 50',    'L2026-002', 12.00, 16.20),
    ('Alcohol medicinal 1 L',     'L2026-003',  8.00, 10.80),
    ('Jeringa descartable 5 ml',  'L2026-004',  0.50,  0.68),
    ('Guantes de látex (caja)',   'L2026-005', 25.00, 33.75)
) AS v(nombre, lote, costo, precio)
WHERE NOT EXISTS (SELECT 1 FROM productos);
