-- Ejecutar SOLO si la tabla Material ya existía antes de la actividad de imágenes
-- (si ejecuta el script completo en una base nueva, NO lo necesita).
USE SistemaPrestamos;
ALTER TABLE Material ADD COLUMN RutaImagen VARCHAR(255) NULL AFTER Descripcion;
