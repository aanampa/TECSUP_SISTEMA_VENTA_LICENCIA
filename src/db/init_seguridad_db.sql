-- ================================================
-- Script de Base de Datos: seguridad_db
-- Sistema de Autenticación y Permisos
-- ================================================

-- Crear la base de datos
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'seguridad_db')
BEGIN
    CREATE DATABASE seguridad_db;
END
GO

USE seguridad_db;
GO

-- ================================================
-- Tabla: Usuarios
-- Descripción: Almacena la información de los usuarios del sistema
-- ================================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Usuarios')
BEGIN
    CREATE TABLE Usuarios (
        IdUsuario INT PRIMARY KEY IDENTITY(1,1),
        Username NVARCHAR(100) NOT NULL UNIQUE,
        Password NVARCHAR(500) NOT NULL,
        Nombre NVARCHAR(200) NOT NULL,
        Email NVARCHAR(200) NOT NULL,
        Activo BIT DEFAULT 1
    );
END
GO

-- ================================================
-- Tabla: Opcion
-- Descripción: Almacena las opciones/módulos del sistema
-- ================================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Opcion')
BEGIN
    CREATE TABLE Opcion (
        IdOpcion INT PRIMARY KEY,
        NombreOpcion NVARCHAR(200) NOT NULL,
        UrlOpcion NVARCHAR(200) NOT NULL
    );
END
GO

-- ================================================
-- Tabla: Perfil
-- Descripción: Relaciona usuarios con opciones (permisos)
-- ================================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Perfil')
BEGIN
    CREATE TABLE Perfil (
        IdUsuario INT NOT NULL,
        IdOpcion INT NOT NULL,
        CONSTRAINT FK_Perfil_Usuario FOREIGN KEY (IdUsuario) REFERENCES Usuarios(IdUsuario),
        CONSTRAINT FK_Perfil_Opcion FOREIGN KEY (IdOpcion) REFERENCES Opcion(IdOpcion),
        CONSTRAINT PK_Perfil PRIMARY KEY (IdUsuario, IdOpcion)
    );
END
GO

-- ================================================
-- DATOS DE PRUEBA
-- ================================================

-- Insertar opciones del sistema
IF NOT EXISTS (SELECT * FROM Opcion)
BEGIN
    INSERT INTO Opcion (IdOpcion, NombreOpcion, UrlOpcion) VALUES
    (1, 'Productos', 'https://localhost:7201/Productos'),
    (2, 'Pedidos', 'https://localhost:7201/Pedidos'),
    (3, 'Clientes', 'https://localhost:7201/Clientes'),
    (4, 'Licencias', 'https://localhost:7201/Licencias');
    
    PRINT 'Opciones insertadas correctamente';
END
GO

-- Insertar usuario administrador
IF NOT EXISTS (SELECT * FROM Usuarios WHERE Username = 'admin')
BEGIN
    INSERT INTO Usuarios (Username, Password, Nombre, Email) VALUES
    ('admin', '12345678', 'Administrador', 'antonio.anampa@gmail.com');
    
    PRINT 'Usuario administrador creado correctamente';
END
GO

-- Asignar todas las opciones al administrador
IF NOT EXISTS (SELECT * FROM Perfil WHERE IdUsuario = 1)
BEGIN
    INSERT INTO Perfil (IdUsuario, IdOpcion) VALUES
    (1, 1),
    (1, 2),
    (1, 3),
    (1, 4);
    
    PRINT 'Permisos asignados al administrador';
END
GO

-- Insertar usuarios adicionales de prueba
IF NOT EXISTS (SELECT * FROM Usuarios WHERE Username = 'vendedor')
BEGIN
    INSERT INTO Usuarios (Username, Password, Nombre, Email) VALUES
    ('vendedor', 'vendedor123', 'Usuario Vendedor', 'vendedor@example.com');
    
    -- Asignar solo opciones de Productos y Clientes
    DECLARE @IdVendedor INT = SCOPE_IDENTITY();
    INSERT INTO Perfil (IdUsuario, IdOpcion) VALUES
    (@IdVendedor, 1),
    (@IdVendedor, 3);
    
    PRINT 'Usuario vendedor creado correctamente';
END
GO
