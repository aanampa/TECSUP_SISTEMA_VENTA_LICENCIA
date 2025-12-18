-- Crear la base de datos
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'sistema_licencias_db')
BEGIN
    CREATE DATABASE sistema_licencias_db;
END
GO

USE sistema_licencias_db;
GO

-- Tabla Productos
CREATE TABLE Productos (
    IdProducto INT PRIMARY KEY IDENTITY(1,1),
    Nombre NVARCHAR(200) NOT NULL,
    Imagen NVARCHAR(500) NULL,
    Precio DECIMAL(10,2) NOT NULL,
    CantidadLicenciasDisponibles INT NOT NULL,
    FechaCreacion DATETIME DEFAULT GETDATE(),
    Activo BIT DEFAULT 1
);

-- Tabla Clientes
CREATE TABLE Clientes (
    IdCliente INT PRIMARY KEY IDENTITY(1,1),
    Documento NVARCHAR(20) NOT NULL,
    Nombre NVARCHAR(200) NOT NULL,
    Email NVARCHAR(200) NOT NULL,
    FechaRegistro DATETIME DEFAULT GETDATE()
);

-- Tabla Pedidos
CREATE TABLE Pedidos (
    IdPedido INT PRIMARY KEY IDENTITY(1,1),
    IdCliente INT NOT NULL,
    Fecha DATETIME DEFAULT GETDATE(),
    IGV DECIMAL(10,2) NOT NULL,
    Total DECIMAL(10,2) NOT NULL,
    EstadoPago NVARCHAR(50) NOT NULL, -- Pendiente, Pagado, Cancelado
    EstadoPedido NVARCHAR(50) NOT NULL, -- Procesando, Completado, Cancelado
    FOREIGN KEY (IdCliente) REFERENCES Clientes(IdCliente)
);

-- Tabla Detalle Pedidos
CREATE TABLE DetallePedidos (
    IdDetalle INT PRIMARY KEY IDENTITY(1,1),
    IdPedido INT NOT NULL,
    IdProducto INT NOT NULL,
    Cantidad INT NOT NULL,
    PrecioUnitario DECIMAL(10,2) NOT NULL,
    Subtotal DECIMAL(10,2) NOT NULL,
    FOREIGN KEY (IdPedido) REFERENCES Pedidos(IdPedido),
    FOREIGN KEY (IdProducto) REFERENCES Productos(IdProducto)
);

-- Tabla Licencias
CREATE TABLE Licencias (
    IdLicencia INT PRIMARY KEY IDENTITY(1,1),
    IdProducto INT NOT NULL,
    CodigoLicencia NVARCHAR(100) NOT NULL UNIQUE,
    IdPedido INT NULL,
    FechaAsignacion DATETIME NULL,
    Activo BIT DEFAULT 1,
    FOREIGN KEY (IdProducto) REFERENCES Productos(IdProducto),
    FOREIGN KEY (IdPedido) REFERENCES Pedidos(IdPedido)
);

-- Tabla Usuarios Administradores
CREATE TABLE Usuarios (
    IdUsuario INT PRIMARY KEY IDENTITY(1,1),
    Username NVARCHAR(100) NOT NULL UNIQUE,
    Password NVARCHAR(500) NOT NULL,
    Nombre NVARCHAR(200) NOT NULL,
    Email NVARCHAR(200) NOT NULL,
    Activo BIT DEFAULT 1
);

-- Índices
CREATE INDEX IX_Licencias_Producto ON Licencias(IdProducto);
CREATE INDEX IX_Licencias_Pedido ON Licencias(IdPedido);
CREATE INDEX IX_Pedidos_Cliente ON Pedidos(IdCliente);
CREATE INDEX IX_DetallePedidos_Pedido ON DetallePedidos(IdPedido);

-- Datos de Prueba

-- Insertar Productos
INSERT INTO Productos (Nombre, Imagen, Precio, CantidadLicenciasDisponibles) VALUES
('Microsoft Office 2021 Professional Plus', 'office2021.jpg', 299.99, 50),
('Windows 11 Pro', 'windows11.jpg', 199.99, 100),
('Norton Antivirus Plus', 'norton.jpg', 49.99, 75),
('Kaspersky Internet Security', 'kaspersky.jpg', 59.99, 60),
('Adobe Creative Cloud', 'adobe.jpg', 599.99, 30),
('Microsoft 365 Personal (1 año)', 'office365.jpg', 69.99, 150);

-- Insertar Licencias para cada producto
DECLARE @ProductoId INT;
DECLARE @Counter INT;

-- Licencias Office 2021
SET @ProductoId = 1;
SET @Counter = 1;
WHILE @Counter <= 50
BEGIN
    INSERT INTO Licencias (IdProducto, CodigoLicencia) 
    VALUES (@ProductoId, 'OFF21-' + RIGHT('00000' + CAST(@Counter AS VARCHAR(5)), 5) + '-' + CONVERT(VARCHAR(100),NEWID() ));
    SET @Counter = @Counter + 1;
END

-- Licencias Windows 11
SET @ProductoId = 2;
SET @Counter = 1;
WHILE @Counter <= 100
BEGIN
    INSERT INTO Licencias (IdProducto, CodigoLicencia) 
    VALUES (@ProductoId, 'WIN11-' + RIGHT('00000' + CAST(@Counter AS VARCHAR(5)), 5) + '-' + CONVERT(VARCHAR(100),NEWID() ));
    SET @Counter = @Counter + 1;
END

-- Licencias Norton
SET @ProductoId = 3;
SET @Counter = 1;
WHILE @Counter <= 75
BEGIN
    INSERT INTO Licencias (IdProducto, CodigoLicencia) 
    VALUES (@ProductoId, 'NRT-' + RIGHT('00000' + CAST(@Counter AS VARCHAR(5)), 5) + '-' + CONVERT(VARCHAR(100),NEWID() ));
    SET @Counter = @Counter + 1;
END

-- Licencias Kaspersky
SET @ProductoId = 4;
SET @Counter = 1;
WHILE @Counter <= 60
BEGIN
    INSERT INTO Licencias (IdProducto, CodigoLicencia) 
    VALUES (@ProductoId, 'KAS-' + RIGHT('00000' + CAST(@Counter AS VARCHAR(5)), 5) + '-' + CONVERT(VARCHAR(100),NEWID() ));
    SET @Counter = @Counter + 1;
END

-- Licencias Adobe
SET @ProductoId = 5;
SET @Counter = 1;
WHILE @Counter <= 30
BEGIN
    INSERT INTO Licencias (IdProducto, CodigoLicencia) 
    VALUES (@ProductoId, 'ADO-' + RIGHT('00000' + CAST(@Counter AS VARCHAR(5)), 5) + '-' + CONVERT(VARCHAR(100),NEWID() ) );
    SET @Counter = @Counter + 1;
END

-- Licencias Office 365
SET @ProductoId = 6;
SET @Counter = 1;
WHILE @Counter <= 150
BEGIN
    INSERT INTO Licencias (IdProducto, CodigoLicencia) 
    VALUES (@ProductoId, 'O365-' + RIGHT('00000' + CAST(@Counter AS VARCHAR(5)), 5) + '-' + CONVERT(VARCHAR(100),NEWID() ) );
    SET @Counter = @Counter + 1;
END

-- Insertar Usuario Administrador (Password: Admin123!)
-- Hash simple para demostración - en producción usar bcrypt o similar
INSERT INTO Usuarios (Username, Password, Nombre, Email) VALUES
('admin', 'AQAAAAEAACcQAAAAEGKqVH3tG4qvC4hU8t7Ej8ViE7DPcJXdZHZh5lMqR3C7xN8wP9qL3vK2mA==', 'Administrador', 'admin@licencias.com');

-- Insertar Clientes de Prueba
INSERT INTO Clientes (Documento, Nombre, Email) VALUES
('12345678', 'Juan Pérez', 'juan.perez@email.com'),
('87654321', 'María García', 'maria.garcia@email.com'),
('45678912', 'Carlos López', 'carlos.lopez@email.com');

-- Insertar Pedidos de Prueba
INSERT INTO Pedidos (IdCliente, Fecha, IGV, Total, EstadoPago, EstadoPedido) VALUES
(1, GETDATE(), 53.99, 353.99, 'Pagado', 'Completado'),
(2, GETDATE(), 8.99, 58.99, 'Pagado', 'Completado'),
(3, GETDATE(), 107.99, 707.99, 'Pendiente', 'Procesando');

-- Insertar Detalles de Pedidos
INSERT INTO DetallePedidos (IdPedido, IdProducto, Cantidad, PrecioUnitario, Subtotal) VALUES
(1, 1, 1, 299.99, 299.99),
(2, 3, 1, 49.99, 49.99),
(3, 2, 2, 199.99, 399.99),
(3, 6, 1, 69.99, 69.99);

-- Asignar licencias a pedidos completados
UPDATE TOP(1) Licencias SET IdPedido = 1, FechaAsignacion = GETDATE() WHERE IdProducto = 1 AND IdPedido IS NULL;
UPDATE TOP(1) Licencias SET IdPedido = 2, FechaAsignacion = GETDATE() WHERE IdProducto = 3 AND IdPedido IS NULL;

GO

-- Procedimiento para obtener licencias disponibles
CREATE PROCEDURE sp_ObtenerLicenciasDisponibles
    @IdProducto INT,
    @Cantidad INT
AS
BEGIN
    SELECT TOP (@Cantidad) IdLicencia, CodigoLicencia
    FROM Licencias
    WHERE IdProducto = @IdProducto AND IdPedido IS NULL AND Activo = 1
    ORDER BY IdLicencia;
END
GO

-- Procedimiento para asignar licencias a pedido
CREATE PROCEDURE sp_AsignarLicenciasPedido
    @IdPedido INT,
    @IdProducto INT,
    @Cantidad INT
AS
BEGIN
    UPDATE TOP (@Cantidad) Licencias
    SET IdPedido = @IdPedido, FechaAsignacion = GETDATE()
    WHERE IdProducto = @IdProducto AND IdPedido IS NULL AND Activo = 1;
    
    SELECT IdLicencia, CodigoLicencia
    FROM Licencias
    WHERE IdPedido = @IdPedido;
END
GO

-- Vista para reporte de pedidos
CREATE VIEW vw_PedidosDetalle AS
SELECT 
    p.IdPedido,
    p.IdCliente,
    c.Nombre AS NombreCliente,
    c.Documento AS DocumentoCliente,
    c.Email AS EmailCliente,
    p.Fecha,
    dp.IdProducto,
    pr.Nombre AS NombreProducto,
    dp.Cantidad,
    dp.PrecioUnitario AS Precio,
    p.IGV,
    p.Total,
    p.EstadoPago,
    p.EstadoPedido
FROM Pedidos p
INNER JOIN Clientes c ON p.IdCliente = c.IdCliente
INNER JOIN DetallePedidos dp ON p.IdPedido = dp.IdPedido
INNER JOIN Productos pr ON dp.IdProducto = pr.IdProducto;
GO
