-- --------------------------------------------------------
-- Host:                         netdreams.pe
-- Versión del servidor:         5.7.23-23 - Percona Server (GPL), Release 23, Revision 500fcf5
-- SO del servidor:              Linux
-- HeidiSQL Versión:             12.8.0.6908
-- --------------------------------------------------------

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8 */;
/*!50503 SET NAMES utf8mb4 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;


-- Volcando estructura de base de datos para netdrepe_tesis
CREATE DATABASE IF NOT EXISTS `netdrepe_tesis` /*!40100 DEFAULT CHARACTER SET utf8 COLLATE utf8_unicode_ci */;
USE `netdrepe_tesis`;

-- Volcando estructura para tabla netdrepe_tesis.ServicioHotel
CREATE TABLE IF NOT EXISTS `ServicioHotel` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `NegocioId` int(11) DEFAULT NULL,
  `CantidadPersonas` int(11) DEFAULT NULL,
  `WiFi` varchar(3) COLLATE utf8_unicode_ci DEFAULT NULL,
  `AguaCaliente` varchar(3) COLLATE utf8_unicode_ci DEFAULT NULL,
  `RoomService` varchar(3) COLLATE utf8_unicode_ci DEFAULT NULL,
  `Cochera` varchar(3) COLLATE utf8_unicode_ci DEFAULT NULL,
  `Cable` varchar(3) COLLATE utf8_unicode_ci DEFAULT NULL,
  `DesayunoIncluido` varchar(3) COLLATE utf8_unicode_ci DEFAULT NULL,
  `Precio` decimal(10,2) DEFAULT NULL,
  `Fotos` text COLLATE utf8_unicode_ci,
  `Estado` varchar(50) COLLATE utf8_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `ServicioHotel_ibfk_1` (`NegocioId`),
  CONSTRAINT `ServicioHotel_ibfk_1` FOREIGN KEY (`NegocioId`) REFERENCES `TbNegocio` (`TbNgcId`)
) ENGINE=InnoDB AUTO_INCREMENT=23 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

-- Volcando datos para la tabla netdrepe_tesis.ServicioHotel: ~6 rows (aproximadamente)
INSERT INTO `ServicioHotel` (`Id`, `NegocioId`, `CantidadPersonas`, `WiFi`, `AguaCaliente`, `RoomService`, `Cochera`, `Cable`, `DesayunoIncluido`, `Precio`, `Fotos`, `Estado`) VALUES
	(17, 1, 2, 'Si', 'Si', 'Si', 'No', 'Si', 'Si', 120.00, NULL, NULL),
	(18, 1, 3, 'Si', 'Si', 'No', 'No', 'Si', 'Si', 150.00, NULL, NULL),
	(19, 1, 2, 'Si', 'Si', 'Si', 'Si', 'Si', 'Si', 145.00, NULL, NULL),
	(20, 2, 2, 'Si', 'Si', 'Si', 'Si', 'Si', 'Si', 140.00, NULL, NULL),
	(21, 2, 2, 'Si', 'Si', 'Si', 'Si', 'No', 'No', 130.00, NULL, NULL),
	(22, 2, 4, 'Si', 'Si', 'Si', 'Si', 'Si', 'Si', 200.00, NULL, NULL);

-- Volcando estructura para tabla netdrepe_tesis.ServicioRestaurante
CREATE TABLE IF NOT EXISTS `ServicioRestaurante` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `NegocioId` int(11) DEFAULT NULL,
  `NombrePlato` varchar(100) COLLATE utf8_unicode_ci DEFAULT NULL,
  `TipoPlato` enum('bebida','postre','desayuno','entrada','almuerzo','cena') COLLATE utf8_unicode_ci DEFAULT 'almuerzo',
  `Precio` decimal(10,2) DEFAULT NULL,
  `Descripcion` text COLLATE utf8_unicode_ci,
  PRIMARY KEY (`Id`),
  KEY `ServicioRestaurante_ibfk_1` (`NegocioId`),
  CONSTRAINT `ServicioRestaurante_ibfk_1` FOREIGN KEY (`NegocioId`) REFERENCES `TbNegocio` (`TbNgcId`)
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

-- Volcando datos para la tabla netdrepe_tesis.ServicioRestaurante: ~3 rows (aproximadamente)
INSERT INTO `ServicioRestaurante` (`Id`, `NegocioId`, `NombrePlato`, `TipoPlato`, `Precio`, `Descripcion`) VALUES
	(3, 3, 'Torta de chocolate', 'postre', 15.00, NULL),
	(4, 3, 'Tiramisu', 'postre', 18.00, NULL),
	(5, 3, 'Lomo Saltado', 'almuerzo', 32.00, NULL);

-- Volcando estructura para tabla netdrepe_tesis.ServicioTuristico
CREATE TABLE IF NOT EXISTS `ServicioTuristico` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `NegocioId` int(11) DEFAULT NULL,
  `Provincia` varchar(50) COLLATE utf8_unicode_ci DEFAULT NULL,
  `NombreLugar` varchar(100) COLLATE utf8_unicode_ci DEFAULT NULL,
  `Precio` decimal(10,2) DEFAULT NULL,
  `Descripcion` text COLLATE utf8_unicode_ci,
  PRIMARY KEY (`Id`),
  KEY `ServicioTuristico_ibfk_1` (`NegocioId`),
  CONSTRAINT `ServicioTuristico_ibfk_1` FOREIGN KEY (`NegocioId`) REFERENCES `TbNegocio` (`TbNgcId`)
) ENGINE=InnoDB AUTO_INCREMENT=8 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

-- Volcando datos para la tabla netdrepe_tesis.ServicioTuristico: ~2 rows (aproximadamente)
INSERT INTO `ServicioTuristico` (`Id`, `NegocioId`, `Provincia`, `NombreLugar`, `Precio`, `Descripcion`) VALUES
	(6, 4, 'Tacna', 'Museo Ferroviario', 15.00, NULL),
	(7, 4, 'Tacna', 'Zoologico', 20.00, NULL);

-- Volcando estructura para tabla netdrepe_tesis.TbAuditoria
CREATE TABLE IF NOT EXISTS `TbAuditoria` (
  `AudId` int(11) NOT NULL AUTO_INCREMENT,
  `AudUsuarioId` int(11) NOT NULL,
  `AudAccion` varchar(50) COLLATE utf8_unicode_ci NOT NULL,
  `AudFecha` datetime DEFAULT CURRENT_TIMESTAMP,
  `AudEntidad` varchar(50) COLLATE utf8_unicode_ci NOT NULL,
  `AudDetalle` text COLLATE utf8_unicode_ci,
  PRIMARY KEY (`AudId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

-- Volcando datos para la tabla netdrepe_tesis.TbAuditoria: ~0 rows (aproximadamente)

-- Volcando estructura para tabla netdrepe_tesis.TbCategoria
CREATE TABLE IF NOT EXISTS `TbCategoria` (
  `TbCtgId` int(11) NOT NULL,
  `TbCtgDescripcion` varchar(50) COLLATE utf8_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`TbCtgId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

-- Volcando datos para la tabla netdrepe_tesis.TbCategoria: ~3 rows (aproximadamente)
INSERT INTO `TbCategoria` (`TbCtgId`, `TbCtgDescripcion`) VALUES
	(1, 'Hotel'),
	(2, 'Restaurante '),
	(3, 'Lugar turistico');

-- Volcando estructura para tabla netdrepe_tesis.TbCotizacion
CREATE TABLE IF NOT EXISTS `TbCotizacion` (
  `CotId` int(11) NOT NULL AUTO_INCREMENT,
  `CotUsrId` int(11) NOT NULL,
  `CotPresupuesto` int(11) NOT NULL,
  `CotTiempoEstadia` int(11) NOT NULL,
  `CotMotivoViaje` varchar(50) COLLATE utf8_unicode_ci NOT NULL,
  `CotCantidadPersonas` int(11) NOT NULL,
  `CotResultado` text COLLATE utf8_unicode_ci NOT NULL,
  `CotFechaGeneracion` datetime DEFAULT CURRENT_TIMESTAMP,
  `CotProvincia` int(11) DEFAULT NULL,
  PRIMARY KEY (`CotId`),
  KEY `FK_CotizacionUsuario` (`CotUsrId`),
  KEY `FK_TbCotizacion_TbProvincia` (`CotProvincia`),
  CONSTRAINT `FK_CotizacionUsuario` FOREIGN KEY (`CotUsrId`) REFERENCES `TbUsuario` (`UsrId`),
  CONSTRAINT `FK_TbCotizacion_TbProvincia` FOREIGN KEY (`CotProvincia`) REFERENCES `TbProvincia` (`TbPvncId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

-- Volcando datos para la tabla netdrepe_tesis.TbCotizacion: ~0 rows (aproximadamente)

-- Volcando estructura para tabla netdrepe_tesis.TbDetalleNegocio
CREATE TABLE IF NOT EXISTS `TbDetalleNegocio` (
  `TbDtlNegocioId` int(11) NOT NULL,
  `TbDtlDescripcion` varchar(50) COLLATE utf8_unicode_ci DEFAULT NULL,
  `TbDtlServicio` varchar(100) COLLATE utf8_unicode_ci DEFAULT NULL,
  `TbDtlPrecio` decimal(19,4) DEFAULT NULL,
  `TbDtlImagen` varchar(100) COLLATE utf8_unicode_ci DEFAULT NULL,
  `TbDtlNegocio` int(11) DEFAULT NULL,
  PRIMARY KEY (`TbDtlNegocioId`),
  KEY `FK_TbDetalleNegocio_TbNegocio` (`TbDtlNegocio`),
  CONSTRAINT `FK_TbDetalleNegocio_TbNegocio` FOREIGN KEY (`TbDtlNegocio`) REFERENCES `TbNegocio` (`TbNgcId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

-- Volcando datos para la tabla netdrepe_tesis.TbDetalleNegocio: ~0 rows (aproximadamente)

-- Volcando estructura para tabla netdrepe_tesis.TbFeedback
CREATE TABLE IF NOT EXISTS `TbFeedback` (
  `FbId` int(11) NOT NULL AUTO_INCREMENT,
  `FbNegocioId` int(11) NOT NULL,
  `FbUsuarioId` int(11) NOT NULL,
  `FbComentario` text COLLATE utf8_unicode_ci,
  `FbCalificacion` int(11) NOT NULL,
  `FbFecha` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`FbId`),
  KEY `FbNegocioId` (`FbNegocioId`),
  KEY `FbUsuarioId` (`FbUsuarioId`),
  CONSTRAINT `TbFeedback_ibfk_1` FOREIGN KEY (`FbNegocioId`) REFERENCES `TbNegocio` (`TbNgcId`),
  CONSTRAINT `TbFeedback_ibfk_2` FOREIGN KEY (`FbUsuarioId`) REFERENCES `TbUsuario` (`UsrId`)
) ENGINE=InnoDB AUTO_INCREMENT=9 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

-- Volcando datos para la tabla netdrepe_tesis.TbFeedback: ~0 rows (aproximadamente)
INSERT INTO `TbFeedback` (`FbId`, `FbNegocioId`, `FbUsuarioId`, `FbComentario`, `FbCalificacion`, `FbFecha`) VALUES
	(8, 2, 2, 'Buenos cuartos', 5, '2024-12-02 17:14:35');

-- Volcando estructura para tabla netdrepe_tesis.TbHistorial
CREATE TABLE IF NOT EXISTS `TbHistorial` (
  `HstId` int(11) NOT NULL AUTO_INCREMENT,
  `HstUsuario` int(11) NOT NULL,
  `HstCotizacion` int(11) NOT NULL,
  `HstFechaVisualisacion` datetime DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`HstId`),
  KEY `FK_HistorialUsuario` (`HstUsuario`),
  KEY `FK_TbHistorial_TbCotizacion` (`HstCotizacion`),
  CONSTRAINT `FK_HistorialUsuario` FOREIGN KEY (`HstUsuario`) REFERENCES `TbUsuario` (`UsrId`),
  CONSTRAINT `FK_TbHistorial_TbCotizacion` FOREIGN KEY (`HstCotizacion`) REFERENCES `TbCotizacion` (`CotId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

-- Volcando datos para la tabla netdrepe_tesis.TbHistorial: ~0 rows (aproximadamente)

-- Volcando estructura para tabla netdrepe_tesis.TbImagenNegocio
CREATE TABLE IF NOT EXISTS `TbImagenNegocio` (
  `TbImgId` int(11) NOT NULL AUTO_INCREMENT,
  `TbImgNegocioId` int(11) DEFAULT NULL,
  `TbImgRuta` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`TbImgId`),
  KEY `TbImgNegocioId` (`TbImgNegocioId`),
  CONSTRAINT `TbImagenNegocio_ibfk_1` FOREIGN KEY (`TbImgNegocioId`) REFERENCES `TbNegocio` (`TbNgcId`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=20 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

-- Volcando datos para la tabla netdrepe_tesis.TbImagenNegocio: ~11 rows (aproximadamente)
INSERT INTO `TbImagenNegocio` (`TbImgId`, `TbImgNegocioId`, `TbImgRuta`) VALUES
	(13, 1, 'http://159.223.123.38/imagenes/Takora1.jpg'),
	(14, 1, 'http://159.223.123.38/imagenes/Takora2.jpg'),
	(15, 1, 'http://159.223.123.38/imagenes/Takora3.jpg'),
	(16, 1, 'http://159.223.123.38/imagenes/Takora4.jpg'),
	(17, 2, 'http://159.223.123.38/imagenes/PacificSuites1.jpg'),
	(18, 3, 'http://159.223.123.38/imagenes/Siena1.jpg'),
	(19, 3, 'http://159.223.123.38/imagenes/Siena1.jpg');

-- Volcando estructura para tabla netdrepe_tesis.TbNegocio
CREATE TABLE IF NOT EXISTS `TbNegocio` (
  `TbNgcId` int(11) NOT NULL AUTO_INCREMENT,
  `TbNgcUsuario` int(11) DEFAULT NULL,
  `TbNgcNombre` varchar(70) COLLATE utf8_unicode_ci DEFAULT NULL,
  `TbNgcTipoNegocio` int(11) DEFAULT NULL,
  `TbNgcProvincia` int(11) DEFAULT NULL,
  `TbNgcDireccion` varchar(100) COLLATE utf8_unicode_ci NOT NULL,
  `TbNgcTelefono` varchar(9) COLLATE utf8_unicode_ci NOT NULL,
  PRIMARY KEY (`TbNgcId`),
  KEY `FK_TbNegocio_TbProvincia` (`TbNgcProvincia`),
  KEY `FK_TbNegocio_TbTipoNegocio` (`TbNgcTipoNegocio`),
  KEY `FK_TbNegocio_TbUsuario` (`TbNgcUsuario`),
  CONSTRAINT `FK_TbNegocio_TbProvincia` FOREIGN KEY (`TbNgcProvincia`) REFERENCES `TbProvincia` (`TbPvncId`),
  CONSTRAINT `FK_TbNegocio_TbTipoNegocio` FOREIGN KEY (`TbNgcTipoNegocio`) REFERENCES `TbTipoNegocio` (`TpNgcId`),
  CONSTRAINT `FK_TbNegocio_TbUsuario` FOREIGN KEY (`TbNgcUsuario`) REFERENCES `TbUsuario` (`UsrId`)
) ENGINE=InnoDB AUTO_INCREMENT=17 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

-- Volcando datos para la tabla netdrepe_tesis.TbNegocio: ~4 rows (aproximadamente)
INSERT INTO `TbNegocio` (`TbNgcId`, `TbNgcUsuario`, `TbNgcNombre`, `TbNgcTipoNegocio`, `TbNgcProvincia`, `TbNgcDireccion`, `TbNgcTelefono`) VALUES
	(1, 1, 'Takora Inn', 1, 1, 'Av. Coronel Justo Arias Araguez 662', '992678696'),
	(2, 1, 'Pacific Suites', 1, 1, 'Av. Vicente Dagnino 262', '952522248'),
	(3, 1, 'Siena Café', 2, 1, 'Pasaje Vigil s/n, a un costado del Club Unión', '980648240'),
	(4, 1, 'Turismo Andes', 3, 1, 'Av. San Martin 685', '952001815');

-- Volcando estructura para tabla netdrepe_tesis.TbProvincia
CREATE TABLE IF NOT EXISTS `TbProvincia` (
  `TbPvncId` int(11) NOT NULL,
  `TbPvncDescripcion` varchar(50) COLLATE utf8_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`TbPvncId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

-- Volcando datos para la tabla netdrepe_tesis.TbProvincia: ~4 rows (aproximadamente)
INSERT INTO `TbProvincia` (`TbPvncId`, `TbPvncDescripcion`) VALUES
	(1, 'Tacna'),
	(2, 'Tarata'),
	(3, 'Jorge Basadre'),
	(4, 'Candarave');

-- Volcando estructura para tabla netdrepe_tesis.TbTipoNegocio
CREATE TABLE IF NOT EXISTS `TbTipoNegocio` (
  `TpNgcId` int(11) NOT NULL AUTO_INCREMENT,
  `TpNgcDescripcion` varchar(50) COLLATE utf8_unicode_ci NOT NULL,
  `TpNgcCategoria` int(11) DEFAULT NULL,
  PRIMARY KEY (`TpNgcId`),
  KEY `FK_TbTipoNegocio_TbCategoria` (`TpNgcCategoria`),
  CONSTRAINT `FK_TbTipoNegocio_TbCategoria` FOREIGN KEY (`TpNgcCategoria`) REFERENCES `TbCategoria` (`TbCtgId`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

-- Volcando datos para la tabla netdrepe_tesis.TbTipoNegocio: ~0 rows (aproximadamente)
INSERT INTO `TbTipoNegocio` (`TpNgcId`, `TpNgcDescripcion`, `TpNgcCategoria`) VALUES
	(1, 'Hotel', 1),
	(2, 'Resturante', 2),
	(3, 'Lugar turistico', 3);

-- Volcando estructura para tabla netdrepe_tesis.TbTipoUsuario
CREATE TABLE IF NOT EXISTS `TbTipoUsuario` (
  `TpUsrId` int(11) NOT NULL AUTO_INCREMENT,
  `TbUsrRol` varchar(15) COLLATE utf8_unicode_ci NOT NULL,
  PRIMARY KEY (`TpUsrId`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

-- Volcando datos para la tabla netdrepe_tesis.TbTipoUsuario: ~2 rows (aproximadamente)
INSERT INTO `TbTipoUsuario` (`TpUsrId`, `TbUsrRol`) VALUES
	(1, 'Turista'),
	(2, 'Socio');

-- Volcando estructura para tabla netdrepe_tesis.TbUsuario
CREATE TABLE IF NOT EXISTS `TbUsuario` (
  `UsrId` int(11) NOT NULL AUTO_INCREMENT,
  `UsrDniRut` varchar(8) COLLATE utf8_unicode_ci DEFAULT NULL,
  `UsrRuc` varchar(11) COLLATE utf8_unicode_ci DEFAULT NULL,
  `UsrNombresCompleto` varchar(50) CHARACTER SET utf8 NOT NULL,
  `UsrCorreo` varchar(200) COLLATE utf8_unicode_ci NOT NULL,
  `UsrTipoUsuario` int(11) DEFAULT NULL,
  `UsrEstado` int(11) NOT NULL,
  `UsrContraseña` varchar(50) COLLATE utf8_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`UsrId`),
  KEY `FK_UsrTipoUsuario` (`UsrTipoUsuario`),
  CONSTRAINT `FK_UsrTipoUsuario` FOREIGN KEY (`UsrTipoUsuario`) REFERENCES `TbTipoUsuario` (`TpUsrId`)
) ENGINE=InnoDB AUTO_INCREMENT=38 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

-- Volcando datos para la tabla netdrepe_tesis.TbUsuario: ~37 rows (aproximadamente)
INSERT INTO `TbUsuario` (`UsrId`, `UsrDniRut`, `UsrRuc`, `UsrNombresCompleto`, `UsrCorreo`, `UsrTipoUsuario`, `UsrEstado`, `UsrContraseña`) VALUES
	(1, '60170160', '60170160', 'Anthony', 'sucsoanthony@gmail.com', 2, 1, '123456789'),
	(2, '00488686', NULL, 'Anthony', 'ac2020067573@virtual.upt.pe', 1, 1, '123456789'),
	(3, NULL, '11111', 'Anthony', 'sucsoanthony@gmial.com', 2, 1, NULL),
	(4, '2165456', NULL, 'Anthony', 'dasdasdsa@gmail.com', 1, 1, NULL),
	(5, NULL, 'asda', 'dasdas', 'saa@gmail.com', 2, 1, NULL),
	(6, NULL, '231312', 'Anthony', 'saa@gmail.com', 2, 1, NULL),
	(7, NULL, '3123', 'Anthony', 'dasdasdsa@gmail.com', 2, 1, NULL),
	(8, NULL, '3123123', 'Anthony', 'saa@gmail.com', 2, 1, NULL),
	(9, NULL, 'dasdas', 'Anthony', 'adsd@gmail.com', 2, 1, NULL),
	(10, NULL, '2133123', 'Anthony', 'dasdasdsa@gmail.com', 2, 1, NULL),
	(11, NULL, '8884848', 'Anthony', 'sucsoanthony@gmial.com', 2, 1, NULL),
	(12, '21321', NULL, 'Anthony', 'dasdasdsa@gmail.com', 1, 1, NULL),
	(13, '5455554', NULL, 'Anthony', 'saa@gmail.com', 1, 1, NULL),
	(14, NULL, '212', 'Anthony', 'sucsoanthony@gmial.com', 2, 1, NULL),
	(15, NULL, '13231231', 'Anthony', 'dasdasdsa@gmail.com', 2, 1, NULL),
	(16, NULL, '546654', 'anthony', 'asdasd@gmai.com', 2, 1, NULL),
	(17, NULL, '31231', 'anfas', 'das@gmail.com', 2, 1, NULL),
	(18, NULL, '60177777', 'Anthony', 'wwwwas@gmail.com', 2, 1, NULL),
	(19, NULL, '1751545', 'Anthony', 'ac202006757773@virtual.upt.pe', 2, 1, NULL),
	(20, '60170160', NULL, 'Anthony Cano Sucso', 'Sucsoanthony@gmail.com', 1, 1, '123456789'),
	(21, '8799879', NULL, 'Anthony Cano Sucso', 'Sucsoanthony@gmail.com', 1, 1, '789789'),
	(22, '789789', NULL, 'Anthony Cano Sucso', 'Sucsoanthony@gmail.com', 1, 1, '5464'),
	(23, '123456', NULL, 'Anthony Cano Sucso', 'Sucsoanthony@gmail.com', 1, 1, '123456789'),
	(24, NULL, '123456', 'Anthony Cano Sucso', 'Sucsoanthony@gmail.com', 2, 1, '123546789'),
	(25, '159632', NULL, 'Jean Valverde', 'jeanvalverdezaamora@gmail.com', 1, 1, '123456789'),
	(26, '84848448', NULL, 'Anthony Cano Sucso', 'saas@gmail.com', 1, 1, '5465456'),
	(27, '4548228', NULL, 'dasda', 'Sucsoanthony@gmail.com', 1, 1, '65546'),
	(28, '88482256', NULL, 'Anthony Cano Sucso', 'Sucsoanthony@gmail.com', 1, 1, '123454'),
	(29, '879565', NULL, 'Anthony Cano Sucso', 'Sucsoanthony@gmail.com', 1, 1, 'sasa'),
	(30, NULL, '456456', 'Anthony Cano Sucso', 'ant@gmail.com', 2, 1, '45654'),
	(31, NULL, '847777', 'Anthony Cano Sucso', 'adas@gmail.com', 2, 1, '4546548'),
	(32, '87555154', NULL, 'Anthony Cano Sucso', 'sasa@gmail.com', 1, 1, '1231858'),
	(33, NULL, '888855', 'Anthony Cano Sucso', 'jeanvalverdezamora@gmail.com', 2, 1, '51651'),
	(34, NULL, '84848888', 'dasdas', 'safa@gmail.com', 2, 1, '5464'),
	(35, NULL, '00488686', 'Anthony Cano Sucso', 'sucsoanthony@gmail.com', 2, 1, '123456789'),
	(36, '60416916', NULL, 'Jean Valverde', 'jeanvalverdezamora@gmail.com', 1, 1, 'Contraseña123'),
	(37, NULL, '10456789012', 'anthony cano', 'antcano@gmail.com', 2, 1, 'contraseñanegocio456');

/*!40103 SET TIME_ZONE=IFNULL(@OLD_TIME_ZONE, 'system') */;
/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IFNULL(@OLD_FOREIGN_KEY_CHECKS, 1) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40111 SET SQL_NOTES=IFNULL(@OLD_SQL_NOTES, 1) */;
