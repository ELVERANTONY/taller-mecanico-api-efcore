-- ============================================================
-- Taller Mecánico "El Motor" - Base de datos (MySQL 8)
-- Laboratorio 12 - Web API REST + EF Core (Database First)
-- ============================================================
-- Motor: MySQL / MariaDB (MonsterASP.NET plan gratuito)
-- Diferencias respecto a SQL Server:
--   * AUTO_INCREMENT en lugar de IDENTITY
--   * TINYINT(1) en lugar de BIT
--   * DATETIME en lugar de DATETIME2
--   * Sin esquema "dbo" ni instrucciones GO
-- ============================================================


-- Fuerza UTF-8 para que los acentos se guarden correctamente.
SET NAMES utf8mb4;

DROP TABLE IF EXISTS Vehiculos;
DROP TABLE IF EXISTS Clientes;

-- ------------------------------------------------------------
-- Tabla padre: Clientes
-- ------------------------------------------------------------
CREATE TABLE Clientes (
    ClienteId     INT           NOT NULL AUTO_INCREMENT,
    Documento     VARCHAR(15)   NOT NULL,
    Nombre        VARCHAR(80)   NOT NULL,
    Apellido      VARCHAR(80)   NOT NULL,
    Telefono      VARCHAR(20)   NULL,
    Email         VARCHAR(120)  NULL,
    Direccion     VARCHAR(150)  NULL,
    Activo        TINYINT(1)    NOT NULL DEFAULT 1,
    FechaRegistro DATETIME      NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (ClienteId),
    UNIQUE KEY UQ_Clientes_Documento (Documento),
    UNIQUE KEY UQ_Clientes_Email (Email)
) ENGINE=InnoDB;

-- ------------------------------------------------------------
-- Tabla hija: Vehiculos (1:N con Clientes)
-- ------------------------------------------------------------
CREATE TABLE Vehiculos (
    VehiculoId  INT          NOT NULL AUTO_INCREMENT,
    ClienteId   INT          NOT NULL,
    Placa       VARCHAR(10)  NOT NULL,
    Marca       VARCHAR(50)  NOT NULL,
    Modelo      VARCHAR(50)  NOT NULL,
    Anio        SMALLINT     NOT NULL,
    Color       VARCHAR(30)  NULL,
    Kilometraje INT          NOT NULL DEFAULT 0,
    PRIMARY KEY (VehiculoId),
    UNIQUE KEY UQ_Vehiculos_Placa (Placa),
    KEY IX_Vehiculos_ClienteId (ClienteId),
    CONSTRAINT FK_Vehiculos_Clientes
        FOREIGN KEY (ClienteId) REFERENCES Clientes (ClienteId)
        ON DELETE RESTRICT
        ON UPDATE CASCADE
) ENGINE=InnoDB;

-- ------------------------------------------------------------
-- Datos de ejemplo
-- ------------------------------------------------------------
INSERT INTO Clientes (Documento, Nombre, Apellido, Telefono, Email, Direccion, Activo) VALUES
('12345678', 'Carlos',  'Ramírez',  '987654321', 'carlos.ramirez@correo.com', 'Av. Los Álamos 120, Lima',      1),
('45678912', 'María',   'Torres',   '912345678', 'maria.torres@correo.com',   'Jr. Puno 455, Arequipa',        1),
('78912345', 'Jorge',   'Salazar',  '955112233', 'jorge.salazar@correo.com',  'Calle Real 89, Trujillo',       1),
('23456789', 'Lucía',   'Fernández','944556677', 'lucia.fernandez@correo.com','Av. Brasil 2100, Lima',         1),
('56789123', 'Pedro',   'Quispe',   '933221100', NULL,                        'Urb. San Isidro Mz. B Lt. 4',   0);

INSERT INTO Vehiculos (ClienteId, Placa, Marca, Modelo, Anio, Color, Kilometraje) VALUES
(1, 'ABC-123', 'Toyota',     'Corolla',  2019, 'Blanco',  85000),
(1, 'XYZ-789', 'Nissan',     'Frontier', 2021, 'Gris',    42000),
(2, 'LMN-456', 'Hyundai',    'Tucson',   2020, 'Azul',    61000),
(3, 'PQR-321', 'Kia',        'Rio',      2018, 'Rojo',   112000),
(3, 'JKL-654', 'Chevrolet',  'Spark',    2017, 'Plata',   98000),
(4, 'DEF-987', 'Honda',      'Civic',    2022, 'Negro',   25000),
(4, 'GHI-147', 'Mazda',      'CX-5',     2023, 'Blanco',  18000),
(5, 'MNO-258', 'Volkswagen', 'Gol',      2016, 'Amarillo',140000);
