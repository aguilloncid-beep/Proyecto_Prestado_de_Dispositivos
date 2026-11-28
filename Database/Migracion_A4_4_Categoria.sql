-- A4.4 - Migración para una base YA creada: agrega el catálogo Categoria y la llave Material.IdCategoria.
-- Ejecutar como root sobre SistemaPrestamos (requiere MariaDB 10.5+). Se puede repetir sin error.
USE SistemaPrestamos;

CREATE TABLE IF NOT EXISTS Categoria (
    IdCategoria INT AUTO_INCREMENT PRIMARY KEY,
    Nombre      VARCHAR(60) NOT NULL UNIQUE,
    Activa      TINYINT(1)  NOT NULL DEFAULT 1
) ENGINE=InnoDB;

INSERT IGNORE INTO Categoria (Nombre) VALUES
  ('Equipo de cómputo'), ('Material didáctico'), ('Recreativo'), ('Otro');

ALTER TABLE Material ADD COLUMN IF NOT EXISTS IdCategoria INT NULL AFTER Categoria;
SET @existe = (SELECT COUNT(*) FROM information_schema.TABLE_CONSTRAINTS
               WHERE CONSTRAINT_SCHEMA = DATABASE() AND TABLE_NAME = 'Material'
                 AND CONSTRAINT_NAME = 'FK_Material_Categoria');
SET @sql = IF(@existe = 0,
  'ALTER TABLE Material ADD CONSTRAINT FK_Material_Categoria FOREIGN KEY (IdCategoria) REFERENCES Categoria(IdCategoria) ON UPDATE CASCADE ON DELETE RESTRICT',
  'SELECT 1');
PREPARE s FROM @sql; EXECUTE s; DEALLOCATE PREPARE s;

UPDATE Material m JOIN Categoria c ON c.Nombre = m.Categoria
   SET m.IdCategoria = c.IdCategoria
 WHERE m.IdCategoria IS NULL;

SELECT m.IdMaterial, m.Nombre, m.Categoria, m.IdCategoria FROM Material m;
