-- A4.5 / A4.6 / A4.7 - Migración para una base YA creada. Ejecutar como root (DBeaver: Alt+X). Se puede repetir.
USE SistemaPrestamos;

ALTER TABLE Material ADD COLUMN IF NOT EXISTS Precio DECIMAL(10,2) NOT NULL DEFAULT 0 AFTER Descripcion;
ALTER TABLE Material ADD COLUMN IF NOT EXISTS Stock  INT NOT NULL DEFAULT 1 AFTER Precio;
ALTER TABLE Material ADD COLUMN IF NOT EXISTS Activo TINYINT(1) AS (Estado <> 'Baja') VIRTUAL;

GRANT DELETE ON SistemaPrestamos.Material TO 'app_prestamos'@'localhost';
FLUSH PRIVILEGES;

SELECT IdMaterial, Nombre, Estado, Activo, Precio, Stock FROM Material;
