-- =====================================================================
--  SISTEMA DE PRÉSTAMO DE MATERIALES - Script completo (MariaDB / MySQL)
--  Coincide con el código actual del proyecto (modelos y DAO).
--  Se puede ejecutar varias veces sin error: no borra datos existentes.
--
--  CÓMO USARLO: abra este archivo en DBeaver / MySQL Workbench / phpMyAdmin
--  (pestaña SQL) conectado como root y ejecútelo COMPLETO.
--  Requiere MySQL 8.0.16+ o MariaDB 10.2+ (para que los CHECK se apliquen).
-- =====================================================================

-- ---------------------------------------------------------------------
-- 1. BASE DE DATOS
-- ---------------------------------------------------------------------
CREATE DATABASE IF NOT EXISTS SistemaPrestamos
  CHARACTER SET utf8mb4
  COLLATE utf8mb4_unicode_ci;

USE SistemaPrestamos;

-- ---------------------------------------------------------------------
-- 2. TABLAS
-- ---------------------------------------------------------------------

-- Usuario: alumnos y administradores de la base de datos (el Rol los distingue).
-- PasswordHash guarda PBKDF2-SHA256 con sal; NUNCA la contraseña en texto plano.
CREATE TABLE IF NOT EXISTS Usuario (
    IdUsuario        INT AUTO_INCREMENT PRIMARY KEY,
    Nombre           VARCHAR(120) NOT NULL,
    Correo           VARCHAR(150) NOT NULL UNIQUE,
    NumeroEstudiante VARCHAR(30)  NULL UNIQUE,
    Carrera          VARCHAR(100) NULL,
    PasswordHash     VARCHAR(255) NOT NULL,
    Rol              VARCHAR(20)  NOT NULL DEFAULT 'Alumno',
    Activo           TINYINT(1)   NOT NULL DEFAULT 1,
    FechaRegistro    DATETIME     NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT CK_Usuario_Rol    CHECK (Rol IN ('Alumno', 'Administrador')),
    CONSTRAINT CK_Usuario_Alumno CHECK (Rol <> 'Alumno' OR NumeroEstudiante IS NOT NULL)
) ENGINE=InnoDB;

-- Material: equipo del inventario. Estado en texto; RutaImagen = ruta relativa (carpeta ImagenesDatos).
CREATE TABLE IF NOT EXISTS Material (
    IdMaterial    INT AUTO_INCREMENT PRIMARY KEY,
    NumeroSerie   VARCHAR(80)  NULL UNIQUE,
    Nombre        VARCHAR(120) NOT NULL,
    Categoria     VARCHAR(60)  NOT NULL,
    Estado        VARCHAR(30)  NOT NULL DEFAULT 'Disponible',
    Descripcion   VARCHAR(500) NULL,
    Precio        DECIMAL(10,2) NOT NULL DEFAULT 0,
    Stock         INT           NOT NULL DEFAULT 1,
    RutaImagen    VARCHAR(255) NULL,
    FechaRegistro DATETIME     NOT NULL DEFAULT CURRENT_TIMESTAMP,
    Activo        TINYINT(1) AS (Estado <> 'Baja') VIRTUAL,
    CONSTRAINT CK_Material_Estado CHECK (Estado IN ('Disponible', 'Prestado', 'Mantenimiento', 'Dañado', 'Baja'))
) ENGINE=InnoDB;

-- Prestamo: Usuario 1:N Prestamo y Material 1:N Prestamo.
-- FechaLimite = plazo pactado; FechaDevolucionReal = entrega efectiva (NULL mientras siga abierto).
CREATE TABLE IF NOT EXISTS Prestamo (
    IdPrestamo          INT AUTO_INCREMENT PRIMARY KEY,
    IdUsuario           INT  NOT NULL,
    IdMaterial          INT  NOT NULL,
    FechaPrestamo       DATE NOT NULL,
    FechaLimite         DATE NOT NULL,
    FechaDevolucionReal DATE NULL,
    Estado              VARCHAR(20) NOT NULL DEFAULT 'Activo',
    FechaRegistro       DATETIME    NOT NULL DEFAULT CURRENT_TIMESTAMP,
    INDEX IX_Prestamo_Estado (Estado),
    CONSTRAINT FK_Prestamo_Usuario  FOREIGN KEY (IdUsuario)  REFERENCES Usuario(IdUsuario)
        ON UPDATE CASCADE ON DELETE RESTRICT,
    CONSTRAINT FK_Prestamo_Material FOREIGN KEY (IdMaterial) REFERENCES Material(IdMaterial)
        ON UPDATE CASCADE ON DELETE RESTRICT,
    CONSTRAINT CK_Prestamo_Estado CHECK (Estado IN ('Activo', 'Finalizado', 'Mora')),
    CONSTRAINT CK_Prestamo_Fechas CHECK (FechaLimite >= FechaPrestamo),
    CONSTRAINT CK_Prestamo_Real   CHECK (FechaDevolucionReal IS NULL OR FechaDevolucionReal >= FechaPrestamo),
    CONSTRAINT CK_Prestamo_Cierre CHECK ((Estado = 'Finalizado' AND FechaDevolucionReal IS NOT NULL)
                                      OR (Estado <> 'Finalizado' AND FechaDevolucionReal IS NULL))
) ENGINE=InnoDB;

-- Devolucion: Prestamo 1:0..1 Devolucion (IdPrestamo UNIQUE).
CREATE TABLE IF NOT EXISTS Devolucion (
    IdDevolucion  INT AUTO_INCREMENT PRIMARY KEY,
    IdPrestamo    INT  NOT NULL UNIQUE,
    FechaReal     DATE NOT NULL,
    Observaciones VARCHAR(500) NULL,
    FechaRegistro DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT FK_Devolucion_Prestamo FOREIGN KEY (IdPrestamo) REFERENCES Prestamo(IdPrestamo)
        ON UPDATE CASCADE ON DELETE RESTRICT
) ENGINE=InnoDB;

-- ---------------------------------------------------------------------
-- 2.1 A4.4 - CATÁLOGO DE CATEGORÍAS (dependencia de Material para el ComboBox)
--     Se puede ejecutar varias veces. Requiere MariaDB 10.5+ (ADD ... IF NOT EXISTS).
-- ---------------------------------------------------------------------
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

-- ---------------------------------------------------------------------
-- 3. USUARIO DE LA APLICACIÓN (permisos mínimos; la app NO usa root)
--    La contraseña coincide con la de Data/ConexionBD.cs. Si la cambia aquí,
--    cámbiela también allí (o use la variable SISTEMA_PRESTAMOS_CONN).
-- ---------------------------------------------------------------------
CREATE USER IF NOT EXISTS 'app_prestamos'@'localhost' IDENTIFIED BY 'Prestamos2026!';
GRANT SELECT, INSERT, UPDATE ON SistemaPrestamos.* TO 'app_prestamos'@'localhost';
-- A4.6: borrado físico solo sobre Material (la app lo restringe además al rol Administrador).
GRANT DELETE ON SistemaPrestamos.Material TO 'app_prestamos'@'localhost';
FLUSH PRIVILEGES;

-- ---------------------------------------------------------------------
-- 4. DATOS INICIALES
-- ---------------------------------------------------------------------

-- Administrador de la base de datos (para el botón "Acceso de alumnos / BD").
--   Correo: admin@sistema.local   Contraseña: Admin2026!   <- cámbiela al entrar (módulo Usuarios)
INSERT INTO Usuario (Nombre, Correo, NumeroEstudiante, Carrera, PasswordHash, Rol, Activo)
SELECT 'Administrador del sistema', 'admin@sistema.local', NULL, NULL,
       'PBKDF2-SHA256 con sal, NUNCA la contraseña.',
       'Administrador', 1
FROM DUAL
WHERE NOT EXISTS (SELECT 1 FROM Usuario WHERE Correo = 'admin@sistema.local');

-- OPCIONAL - datos de ejemplo para probar (puede borrar esta sección):
--   alumno de prueba   Correo: alumno@sistema.local   Contraseña: Alumno2026!
INSERT INTO Usuario (Nombre, Correo, NumeroEstudiante, Carrera, PasswordHash, Rol, Activo)
SELECT 'Alumno de Prueba', 'alumno@sistema.local', '200000001', 'Ing. Ciencias Computacionales',
       'PBKDF2-SHA256$600000$ZVsWN84nArS5q8LCvK0dgA==$/6e0TlVxAwtmAqMMLHkqm8shMaRIn5MBHQrBg1V8hJ4=',
       'Alumno', 1
FROM DUAL
WHERE NOT EXISTS (SELECT 1 FROM Usuario WHERE Correo = 'alumno@sistema.local');

INSERT IGNORE INTO Material (NumeroSerie, Nombre, Categoria, Estado, Descripcion) VALUES
  ('LAP-001', 'Laptop Dell Latitude',      'Equipo de cómputo',  'Disponible', 'Laptop 14 pulgadas, 8 GB de RAM'),
  ('LAP-002', 'Laptop Lenovo ThinkPad',    'Equipo de cómputo',  'Disponible', 'Laptop 15 pulgadas, 16 GB de RAM'),
  ('PRO-001', 'Proyector Epson',           'Material didáctico', 'Disponible', 'Proyector HDMI con control remoto'),
  ('KIT-001', 'Kit de Arduino',            'Material didáctico', 'Disponible', 'Placa, cables y sensores básicos'),
  ('JGO-001', 'Juego de mesa Ajedrez',     'Recreativo',         'Disponible', 'Tablero y piezas');

-- A4.6 / A4.7: precio, existencias y borrado lógico (Activo = 0 cuando Estado = 'Baja').
ALTER TABLE Material ADD COLUMN IF NOT EXISTS Precio DECIMAL(10,2) NOT NULL DEFAULT 0 AFTER Descripcion;
ALTER TABLE Material ADD COLUMN IF NOT EXISTS Stock  INT NOT NULL DEFAULT 1 AFTER Precio;
ALTER TABLE Material ADD COLUMN IF NOT EXISTS Activo TINYINT(1) AS (Estado <> 'Baja') VIRTUAL;

-- A4.8: pedidos (relación 1:N). Pedidos = encabezado; Detalles_Pedidos = renglones, ligados por llave foránea.
CREATE TABLE IF NOT EXISTS Pedidos (
    IdPedido INT AUTO_INCREMENT PRIMARY KEY,
    Fecha    DATETIME      NOT NULL DEFAULT CURRENT_TIMESTAMP,
    Cliente  VARCHAR(120)  NOT NULL,
    Total    DECIMAL(12,2) NOT NULL,
    CONSTRAINT CK_Pedidos_Total CHECK (Total >= 0)
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS Detalles_Pedidos (
    IdDetalle  INT AUTO_INCREMENT PRIMARY KEY,
    IdPedido   INT           NOT NULL,
    IdMaterial INT           NOT NULL,
    Cantidad   INT           NOT NULL,
    Precio     DECIMAL(10,2) NOT NULL,
    CONSTRAINT FK_Detalles_Pedido   FOREIGN KEY (IdPedido)   REFERENCES Pedidos(IdPedido)     ON UPDATE CASCADE ON DELETE RESTRICT,
    CONSTRAINT FK_Detalles_Material FOREIGN KEY (IdMaterial) REFERENCES Material(IdMaterial) ON UPDATE CASCADE ON DELETE RESTRICT,
    CONSTRAINT CK_Detalles_Cantidad CHECK (Cantidad > 0)
) ENGINE=InnoDB;

-- A20: cuentas de acceso con Estado (Activo/Inactivo) y rol. La contraseña se guarda con hash PBKDF2.
-- Las cuentas iniciales (admin, operador, invitado) las crea la aplicación la primera vez que alguien inicia sesión.
CREATE TABLE IF NOT EXISTS CuentaAcceso (
    IdCuenta       INT AUTO_INCREMENT PRIMARY KEY,
    NombreUsuario  VARCHAR(50)  NOT NULL UNIQUE,
    Contrasena     VARCHAR(255) NOT NULL,
    NombreCompleto VARCHAR(120) NOT NULL,
    Rol            VARCHAR(20)  NOT NULL,
    Estado         VARCHAR(10)  NOT NULL DEFAULT 'Activo',
    CONSTRAINT CK_CuentaAcceso_Rol    CHECK (Rol IN ('Administrador', 'Operador', 'Invitado')),
    CONSTRAINT CK_CuentaAcceso_Estado CHECK (Estado IN ('Activo', 'Inactivo'))
) ENGINE=InnoDB;

-- A4.4: enlaza los materiales existentes con su categoría del catálogo (solo los que aún no tienen IdCategoria).
UPDATE Material m JOIN Categoria c ON c.Nombre = m.Categoria
   SET m.IdCategoria = c.IdCategoria
 WHERE m.IdCategoria IS NULL;

-- ---------------------------------------------------------------------
-- 5. VERIFICACIÓN (debe mostrar las 4 tablas y al menos 2 usuarios y 5 materiales)
-- ---------------------------------------------------------------------
SHOW TABLES;
SELECT (SELECT COUNT(*) FROM Usuario)  AS Usuarios,
       (SELECT COUNT(*) FROM Material) AS Materiales,
       (SELECT COUNT(*) FROM Prestamo) AS Prestamos;
