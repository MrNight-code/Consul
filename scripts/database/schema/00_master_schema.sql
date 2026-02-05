CREATE DATABASE IF NOT EXISTS `db_consulcon_master` CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
USE `db_consulcon_master`;

-- 1. Usuarios Globales
CREATE TABLE IF NOT EXISTS `UsuariosMaster` (
    `Id` INT AUTO_INCREMENT PRIMARY KEY,
    `Username` VARCHAR(100) NOT NULL UNIQUE,
    `PasswordHash` VARCHAR(255) NOT NULL,
    `Email` VARCHAR(150),
    `FechaCreacion` DATETIME DEFAULT CURRENT_TIMESTAMP,
    `EsSuperAdmin` BOOLEAN DEFAULT FALSE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 2. Condominios Disponibles
CREATE TABLE IF NOT EXISTS `CondominiosMaster` (
    `Id` INT AUTO_INCREMENT PRIMARY KEY,
    `TenantId` VARCHAR(50) NOT NULL UNIQUE COMMENT 'Identificador usado en header X-Tenant-Id (ej: foret)',
    `Nombre` VARCHAR(150) NOT NULL,
    `ConnectionString` VARCHAR(500) NULL COMMENT 'Opcional si sigue patrón estándar',
    `FechaRegistro` DATETIME DEFAULT CURRENT_TIMESTAMP
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 3. Relación Usuario - Condominio
CREATE TABLE IF NOT EXISTS `UsuarioCondominio` (
    `Id` INT AUTO_INCREMENT PRIMARY KEY,
    `UsuarioId` INT NOT NULL,
    `CondominioId` INT NOT NULL,
    `RolInicial` VARCHAR(50) DEFAULT 'Usuario' COMMENT 'Rol sugerido al entrar al tenant',
    FOREIGN KEY (`UsuarioId`) REFERENCES `UsuariosMaster`(`Id`) ON DELETE CASCADE,
    FOREIGN KEY (`CondominioId`) REFERENCES `CondominiosMaster`(`Id`) ON DELETE CASCADE,
    UNIQUE KEY `UK_Usuario_Condominio` (`UsuarioId`, `CondominioId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
