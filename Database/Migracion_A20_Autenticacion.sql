-- A20 - Migración para una base YA creada. Ejecutar como root (DBeaver: Alt+X). Se puede repetir.
USE SistemaPrestamos;

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

SELECT * FROM CuentaAcceso;
