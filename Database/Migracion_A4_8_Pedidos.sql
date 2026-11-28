-- A4.8 - Migración para una base YA creada. Ejecutar como root (DBeaver: Alt+X). Se puede repetir.
USE SistemaPrestamos;

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

SELECT COUNT(*) AS Pedidos FROM Pedidos;
SELECT COUNT(*) AS Detalles FROM Detalles_Pedidos;
