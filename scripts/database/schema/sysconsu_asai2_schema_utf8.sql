-- MySQL dump 10.13  Distrib 8.0.44, for Linux (x86_64)
--
-- Host: localhost    Database: sysconsu_asai2
-- ------------------------------------------------------
-- Server version	8.0.44

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8mb4 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `asiento`
--

DROP TABLE IF EXISTS `asiento`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `asiento` (
  `pk_asiento` int NOT NULL AUTO_INCREMENT,
  `fecha` datetime DEFAULT NULL,
  `documento` varchar(50) CHARACTER SET utf8mb3 COLLATE utf8mb3_unicode_ci DEFAULT NULL,
  `numerodocumento` varchar(15) CHARACTER SET utf8mb3 COLLATE utf8mb3_unicode_ci DEFAULT NULL,
  `fk_banco` int DEFAULT NULL,
  `cheque` varchar(15) CHARACTER SET utf8mb3 COLLATE utf8mb3_unicode_ci DEFAULT NULL,
  `tc` decimal(14,4) DEFAULT NULL,
  `tipoasiento` varchar(10) CHARACTER SET utf8mb3 COLLATE utf8mb3_unicode_ci DEFAULT NULL,
  `fk_cuenta` int DEFAULT NULL,
  `fk_deuda` int DEFAULT NULL,
  `glosa` varchar(500) CHARACTER SET utf8mb3 COLLATE utf8mb3_unicode_ci DEFAULT NULL,
  `activo` varchar(2) CHARACTER SET utf8mb3 COLLATE utf8mb3_unicode_ci DEFAULT NULL,
  `fechabaja` datetime DEFAULT NULL,
  `fechaupdate` datetime DEFAULT NULL,
  `fk_usuariobaja` int DEFAULT NULL,
  `fk_usuarioupdate` int DEFAULT NULL,
  `fk_proceso` int DEFAULT NULL,
  `formulario` varchar(45) CHARACTER SET utf8mb3 COLLATE utf8mb3_unicode_ci DEFAULT NULL,
  `numero` int DEFAULT NULL,
  `fechacreacion` datetime NOT NULL,
  `fk_usuario` int NOT NULL,
  `motivobaja` varchar(50) CHARACTER SET utf8mb3 COLLATE utf8mb3_unicode_ci NOT NULL,
  PRIMARY KEY (`pk_asiento`),
  KEY `fk_asiento_banco1_idx` (`fk_banco`),
  CONSTRAINT `fk_asiento_banco1` FOREIGN KEY (`fk_banco`) REFERENCES `banco` (`pk_banco`)
) ENGINE=InnoDB AUTO_INCREMENT=11369 DEFAULT CHARSET=utf8mb3 COLLATE=utf8mb3_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `autorizacion`
--

DROP TABLE IF EXISTS `autorizacion`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `autorizacion` (
  `pk_autorizacion` int NOT NULL AUTO_INCREMENT,
  `descripcion` varchar(70) DEFAULT NULL,
  `activo` varchar(2) DEFAULT NULL,
  PRIMARY KEY (`pk_autorizacion`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `avisoconbranza`
--

DROP TABLE IF EXISTS `avisoconbranza`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `avisoconbranza` (
  `pk_avisoconbranza` int NOT NULL AUTO_INCREMENT,
  `pk_servicio1` int NOT NULL,
  `pk_servicio2` int DEFAULT NULL,
  `pk_servicio3` int DEFAULT NULL,
  `pk_servicio4` int DEFAULT NULL,
  `pk_servicio5` int DEFAULT NULL,
  `servicio1` varchar(100) NOT NULL,
  `servicio2` varchar(100) DEFAULT NULL,
  `servicio3` varchar(100) DEFAULT NULL,
  `servicio4` varchar(100) DEFAULT NULL,
  `servicio5` varchar(100) DEFAULT NULL,
  `mes1` int DEFAULT '0',
  `mes2` int DEFAULT '0',
  `mes3` int DEFAULT '0',
  `mes4` int DEFAULT '0',
  `mes5` int DEFAULT '0',
  `anio1` int DEFAULT '0',
  `anio2` int DEFAULT '0',
  `anio3` int DEFAULT '0',
  `anio4` int DEFAULT '0',
  `anio5` int DEFAULT '0',
  PRIMARY KEY (`pk_avisoconbranza`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `banco`
--

DROP TABLE IF EXISTS `banco`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `banco` (
  `pk_banco` int NOT NULL AUTO_INCREMENT,
  `descripcion` varchar(100) DEFAULT NULL,
  `fk_cuenta` int DEFAULT NULL,
  `numero` varchar(20) DEFAULT NULL,
  `cuenta` varchar(100) DEFAULT NULL,
  `activo` varchar(2) DEFAULT NULL,
  `cuentabanco` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`pk_banco`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `condominio`
--

DROP TABLE IF EXISTS `condominio`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `condominio` (
  `pk_condominio` int NOT NULL AUTO_INCREMENT,
  `codigo` varchar(15) NOT NULL,
  `nombre` varchar(50) NOT NULL,
  `administrador` int DEFAULT NULL,
  `m2` decimal(10,2) DEFAULT NULL,
  `tipo` varchar(6) DEFAULT NULL,
  `vh` varchar(1) DEFAULT NULL,
  `diacobro` varchar(70) DEFAULT NULL,
  PRIMARY KEY (`pk_condominio`),
  KEY `fk_condominio_persona1_idx` (`administrador`),
  CONSTRAINT `fk_condominio_persona1` FOREIGN KEY (`administrador`) REFERENCES `persona` (`pk_persona`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `confaviso`
--

DROP TABLE IF EXISTS `confaviso`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `confaviso` (
  `pk_confaviso` int NOT NULL AUTO_INCREMENT,
  `fechaemision` datetime DEFAULT NULL,
  `header` varchar(300) DEFAULT NULL,
  `fechavencimiento` datetime DEFAULT NULL,
  `condominio` varchar(100) DEFAULT NULL,
  `footer` varchar(300) DEFAULT NULL,
  PRIMARY KEY (`pk_confaviso`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `confejecucion`
--

DROP TABLE IF EXISTS `confejecucion`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `confejecucion` (
  `pk_confejecucion` int NOT NULL AUTO_INCREMENT,
  `descripcion` varchar(100) DEFAULT NULL,
  `tipo` varchar(10) DEFAULT NULL,
  `activo` varchar(2) DEFAULT NULL,
  PRIMARY KEY (`pk_confejecucion`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `confejecuciontitulo`
--

DROP TABLE IF EXISTS `confejecuciontitulo`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `confejecuciontitulo` (
  `pk_confejecuciontitulo` int NOT NULL AUTO_INCREMENT,
  `fk_confejecucion` int DEFAULT NULL,
  `descripcion` varchar(100) NOT NULL,
  `activo` varchar(2) NOT NULL,
  PRIMARY KEY (`pk_confejecuciontitulo`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `configuracionevento`
--

DROP TABLE IF EXISTS `configuracionevento`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `configuracionevento` (
  `id` int NOT NULL DEFAULT '1',
  `PrecioChurrasquera` decimal(12,2) DEFAULT NULL COMMENT '100',
  `PrecioSalon` decimal(12,2) DEFAULT NULL COMMENT '200',
  `PrecioSalonChurrasquera` decimal(12,2) DEFAULT NULL COMMENT '300',
  `MontoGarantia` decimal(12,2) DEFAULT NULL,
  `NumeroCuenta` decimal(12,2) DEFAULT NULL,
  `Banco` varchar(200) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `contrato`
--

DROP TABLE IF EXISTS `contrato`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `contrato` (
  `pk_contrato` int NOT NULL AUTO_INCREMENT,
  `fk_propiedad` int DEFAULT NULL,
  `fecha` datetime NOT NULL,
  `fechaini` datetime DEFAULT NULL,
  `fechafin` datetime DEFAULT NULL,
  `fechaingreso` datetime DEFAULT NULL,
  `expensa` decimal(10,2) NOT NULL,
  `valido` varchar(2) DEFAULT NULL,
  `fechabaja` datetime DEFAULT NULL,
  `motivo` varchar(30) DEFAULT NULL,
  `fk_usuarioinsert` int DEFAULT NULL,
  `fk_usuariodelete` int DEFAULT NULL,
  `diaini` int DEFAULT NULL,
  `diafin` int DEFAULT NULL,
  `fk_condominio` int DEFAULT NULL,
  PRIMARY KEY (`pk_contrato`),
  KEY `fk_contrato_propiedad1_idx` (`fk_propiedad`),
  KEY `fk_contrato_usuario1_idx` (`fk_usuarioinsert`),
  KEY `fk_contrato_usuario2_idx` (`fk_usuariodelete`),
  CONSTRAINT `fk_contrato_propiedad1` FOREIGN KEY (`fk_propiedad`) REFERENCES `propiedad` (`pk_propiedad`),
  CONSTRAINT `fk_contrato_usuario1` FOREIGN KEY (`fk_usuarioinsert`) REFERENCES `usuario` (`pk_usuario`),
  CONSTRAINT `fk_contrato_usuario2` FOREIGN KEY (`fk_usuariodelete`) REFERENCES `usuario` (`pk_usuario`)
) ENGINE=InnoDB AUTO_INCREMENT=164 DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `correo`
--

DROP TABLE IF EXISTS `correo`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `correo` (
  `pk_correo` int NOT NULL AUTO_INCREMENT,
  `tipo` varchar(20) NOT NULL,
  `email` varchar(50) NOT NULL,
  `fk_persona` int DEFAULT NULL,
  PRIMARY KEY (`pk_correo`),
  KEY `fk_correo_persona1_idx` (`fk_persona`),
  CONSTRAINT `fk_correo_persona1` FOREIGN KEY (`fk_persona`) REFERENCES `persona` (`pk_persona`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `cuenta`
--

DROP TABLE IF EXISTS `cuenta`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `cuenta` (
  `pk_cuenta` int NOT NULL AUTO_INCREMENT,
  `numero` int DEFAULT NULL,
  `descripcion` varchar(100) CHARACTER SET utf8mb3 COLLATE utf8mb3_unicode_ci DEFAULT NULL,
  `nivel` int DEFAULT NULL,
  `fk_cuenta` int DEFAULT NULL,
  `nivel1` int DEFAULT NULL,
  `nivel2` int DEFAULT NULL,
  `nivel3` int DEFAULT NULL,
  `nivel4` int DEFAULT NULL,
  `nivel5` int DEFAULT NULL,
  PRIMARY KEY (`pk_cuenta`),
  KEY `fk_cuenta_cuenta1_idx` (`fk_cuenta`),
  CONSTRAINT `fk_cuenta_cuenta1` FOREIGN KEY (`fk_cuenta`) REFERENCES `cuenta` (`pk_cuenta`)
) ENGINE=InnoDB AUTO_INCREMENT=870 DEFAULT CHARSET=utf8mb3 COLLATE=utf8mb3_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `cuentasistema`
--

DROP TABLE IF EXISTS `cuentasistema`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `cuentasistema` (
  `pk_cuentasistema` int NOT NULL,
  `descripcion` varchar(70) DEFAULT NULL,
  `pk_cuenta` int NOT NULL,
  `cuenta` varchar(70) NOT NULL,
  `opcion` varchar(70) DEFAULT NULL,
  PRIMARY KEY (`pk_cuentasistema`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `cuota`
--

DROP TABLE IF EXISTS `cuota`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `cuota` (
  `pk_cuota` int NOT NULL AUTO_INCREMENT,
  `fk_deuda` int DEFAULT NULL,
  `fk_persona` int DEFAULT NULL,
  `fecha` datetime DEFAULT NULL,
  `monto` decimal(10,2) DEFAULT NULL,
  `formapago` varchar(15) DEFAULT NULL,
  `banco` varchar(15) DEFAULT NULL,
  `cuenta` varchar(15) DEFAULT NULL,
  `tipocambio` decimal(4,2) DEFAULT NULL,
  `fechabaja` datetime DEFAULT NULL,
  `motivo` varchar(150) DEFAULT NULL,
  `fk_usuarioalta` int DEFAULT NULL,
  `fk_usuariobaja` int DEFAULT NULL,
  `activo` varchar(2) DEFAULT NULL,
  `recibogeneral` int DEFAULT NULL,
  `recibopersonal` int DEFAULT NULL,
  `fk_banco` int DEFAULT NULL,
  `fk_formapago` int DEFAULT NULL,
  `fechadeposito` datetime DEFAULT NULL,
  `cuentadebe` int DEFAULT NULL,
  `cuentahaber` int DEFAULT NULL,
  `fk_moneda` int DEFAULT NULL,
  `fk_opcion_pago` int DEFAULT NULL,
  PRIMARY KEY (`pk_cuota`),
  KEY `fk_cuota_deuda1_idx` (`fk_deuda`),
  KEY `fk_cuota_persona1_idx` (`fk_persona`),
  KEY `fk_cuota_usuario1_idx` (`fk_usuarioalta`),
  KEY `fk_cuota_usuario2_idx` (`fk_usuariobaja`),
  KEY `fk_cuota_formpago1_idx` (`fk_formapago`),
  KEY `fk_cuota_banco1_idx` (`fk_banco`),
  CONSTRAINT `fk_cuota_banco1` FOREIGN KEY (`fk_banco`) REFERENCES `banco` (`pk_banco`),
  CONSTRAINT `fk_cuota_deuda1` FOREIGN KEY (`fk_deuda`) REFERENCES `deuda` (`pk_deuda`),
  CONSTRAINT `fk_cuota_formpago1` FOREIGN KEY (`fk_formapago`) REFERENCES `formpago` (`pk_formpago`),
  CONSTRAINT `fk_cuota_persona1` FOREIGN KEY (`fk_persona`) REFERENCES `persona` (`pk_persona`),
  CONSTRAINT `fk_cuota_usuario1` FOREIGN KEY (`fk_usuarioalta`) REFERENCES `usuario` (`pk_usuario`),
  CONSTRAINT `fk_cuota_usuario2` FOREIGN KEY (`fk_usuariobaja`) REFERENCES `usuario` (`pk_usuario`)
) ENGINE=InnoDB AUTO_INCREMENT=10602 DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `detalleasiento`
--

DROP TABLE IF EXISTS `detalleasiento`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `detalleasiento` (
  `pk_detalleasiento` int NOT NULL AUTO_INCREMENT,
  `fk_cuenta` int DEFAULT NULL,
  `fk_asiento` int DEFAULT NULL,
  `concepto` varchar(500) CHARACTER SET utf8mb3 COLLATE utf8mb3_unicode_ci DEFAULT NULL,
  `debe` decimal(14,4) DEFAULT NULL,
  `haber` decimal(14,4) DEFAULT NULL,
  `glosa` varchar(500) CHARACTER SET utf8mb3 COLLATE utf8mb3_unicode_ci DEFAULT NULL,
  `activo` varchar(2) CHARACTER SET utf8mb3 COLLATE utf8mb3_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`pk_detalleasiento`),
  KEY `fk_detalleasiento_asiento_idx` (`fk_asiento`),
  KEY `fk_detalleasiento_cuenta1_idx` (`fk_cuenta`),
  CONSTRAINT `fk_detalleasiento_asiento` FOREIGN KEY (`fk_asiento`) REFERENCES `asiento` (`pk_asiento`),
  CONSTRAINT `fk_detalleasiento_cuenta1` FOREIGN KEY (`fk_cuenta`) REFERENCES `cuenta` (`pk_cuenta`)
) ENGINE=InnoDB AUTO_INCREMENT=26464 DEFAULT CHARSET=utf8mb3 COLLATE=utf8mb3_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `detalleconfejecucion`
--

DROP TABLE IF EXISTS `detalleconfejecucion`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `detalleconfejecucion` (
  `fk_confejecuciontitulo` int NOT NULL,
  `fk_confejecucion` int NOT NULL,
  `pk_cuenta` int NOT NULL,
  `cuenta` varchar(30) DEFAULT NULL,
  `descripcion` varchar(100) DEFAULT NULL,
  `nivel1` int DEFAULT NULL,
  `descripcionejecucion` varchar(100) DEFAULT NULL,
  `tipo` int DEFAULT NULL,
  PRIMARY KEY (`fk_confejecucion`,`pk_cuenta`,`fk_confejecuciontitulo`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `deuda`
--

DROP TABLE IF EXISTS `deuda`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `deuda` (
  `pk_deuda` int NOT NULL AUTO_INCREMENT,
  `fk_servicio` int DEFAULT NULL,
  `fk_contrato` int DEFAULT NULL,
  `fecha` datetime DEFAULT NULL,
  `fechadeuda` datetime DEFAULT NULL,
  `monto` decimal(10,2) DEFAULT NULL,
  `pagado` varchar(2) DEFAULT NULL,
  `estado` varchar(1) DEFAULT NULL,
  `fechabaja` datetime DEFAULT NULL,
  `motivobaja` varchar(30) DEFAULT NULL,
  `fk_usuarioalta` int DEFAULT NULL,
  `fk_usuariobaja` int DEFAULT NULL,
  `activo` varchar(1) DEFAULT NULL,
  PRIMARY KEY (`pk_deuda`),
  KEY `fk_deuda_contrato1_idx` (`fk_contrato`),
  KEY `fk_deuda_serviciopago1_idx` (`fk_servicio`),
  KEY `fk_deuda_usuario1_idx` (`fk_usuarioalta`),
  KEY `fk_deuda_usuario2_idx` (`fk_usuariobaja`),
  CONSTRAINT `fk_deuda_contrato1` FOREIGN KEY (`fk_contrato`) REFERENCES `contrato` (`pk_contrato`),
  CONSTRAINT `fk_deuda_serviciopago1` FOREIGN KEY (`fk_servicio`) REFERENCES `serviciopago` (`pk_serviciopago`),
  CONSTRAINT `fk_deuda_usuario1` FOREIGN KEY (`fk_usuarioalta`) REFERENCES `usuario` (`pk_usuario`),
  CONSTRAINT `fk_deuda_usuario2` FOREIGN KEY (`fk_usuariobaja`) REFERENCES `usuario` (`pk_usuario`)
) ENGINE=InnoDB AUTO_INCREMENT=12023 DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `egreso`
--

DROP TABLE IF EXISTS `egreso`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `egreso` (
  `pk_egreso` int NOT NULL AUTO_INCREMENT,
  `numerorecibo` int NOT NULL,
  `concepto` varchar(45) DEFAULT NULL,
  `glosa` varchar(500) DEFAULT NULL,
  `monto` decimal(12,2) NOT NULL,
  `fechacreacion` datetime DEFAULT NULL,
  `fecha` datetime NOT NULL,
  `fk_usuario` int DEFAULT NULL,
  `activo` varchar(2) DEFAULT NULL,
  `fk_usuariobaja` int DEFAULT NULL,
  `motivobaja` varchar(50) DEFAULT NULL,
  `fechabaja` datetime DEFAULT NULL,
  `cuentadebe` int DEFAULT NULL,
  `cuentahaber` int DEFAULT NULL,
  `fk_formpago` int DEFAULT NULL,
  `numerotrans` varchar(45) DEFAULT NULL,
  `fk_proveedor` int DEFAULT NULL,
  `fk_persona` int DEFAULT NULL,
  `fk_autorizacion` int DEFAULT NULL,
  `fk_banco` int DEFAULT NULL,
  `cuenta` varchar(45) DEFAULT NULL,
  `nota` varchar(100) DEFAULT NULL,
  `fechacobro` datetime DEFAULT NULL,
  `fechaconfirmado` datetime DEFAULT NULL,
  `cobrado` varchar(2) DEFAULT NULL,
  PRIMARY KEY (`pk_egreso`),
  KEY `fk_egreso_banco1_idx` (`fk_banco`),
  KEY `fk_egreso_proveedor1_idx` (`fk_proveedor`),
  KEY `fk_egreso_persona1_idx` (`fk_persona`),
  KEY `fk_egreso_autorizacion1_idx` (`fk_autorizacion`),
  KEY `fk_egreso_usuario1_idx` (`fk_usuario`),
  KEY `fk_egreso_formpago1_idx` (`fk_formpago`),
  CONSTRAINT `fk_egreso_autorizacion1` FOREIGN KEY (`fk_autorizacion`) REFERENCES `autorizacion` (`pk_autorizacion`),
  CONSTRAINT `fk_egreso_banco1` FOREIGN KEY (`fk_banco`) REFERENCES `banco` (`pk_banco`),
  CONSTRAINT `fk_egreso_formpago1` FOREIGN KEY (`fk_formpago`) REFERENCES `formpago` (`pk_formpago`),
  CONSTRAINT `fk_egreso_persona1` FOREIGN KEY (`fk_persona`) REFERENCES `persona` (`pk_persona`),
  CONSTRAINT `fk_egreso_proveedor1` FOREIGN KEY (`fk_proveedor`) REFERENCES `proveedor` (`pk_proveedor`),
  CONSTRAINT `fk_egreso_usuario1` FOREIGN KEY (`fk_usuario`) REFERENCES `usuario` (`pk_usuario`)
) ENGINE=InnoDB AUTO_INCREMENT=719 DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `empresa`
--

DROP TABLE IF EXISTS `empresa`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `empresa` (
  `pk_empresa` int NOT NULL AUTO_INCREMENT,
  `empresa` varchar(30) NOT NULL,
  `telefono` varchar(40) DEFAULT NULL,
  `direccion` varchar(40) DEFAULT NULL,
  `facebook` varchar(45) DEFAULT NULL,
  `twiter` varchar(45) DEFAULT NULL,
  `habilitado` varchar(2) DEFAULT NULL,
  PRIMARY KEY (`pk_empresa`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `evento`
--

DROP TABLE IF EXISTS `evento`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `evento` (
  `pk_evento` int NOT NULL AUTO_INCREMENT,
  `fecha` datetime DEFAULT NULL,
  `anio` int DEFAULT NULL,
  `mes` int DEFAULT NULL,
  `dia` int DEFAULT NULL,
  `horainicio` time DEFAULT NULL,
  `horafin` time DEFAULT NULL,
  `fk_contrato` int DEFAULT NULL,
  `fk_propietario` int DEFAULT NULL,
  `fk_persona` int DEFAULT NULL,
  `disponible` varchar(100) DEFAULT NULL,
  `numero` int DEFAULT '1',
  `colorChurrasquera` varchar(60) DEFAULT NULL,
  `checkChurrasquera` varchar(2) DEFAULT NULL,
  `checkSalon` varchar(2) DEFAULT NULL,
  `MotivoEvento` varchar(300) DEFAULT NULL,
  `NumeroInvitados` varchar(45) DEFAULT NULL,
  `CelularContacto` varchar(60) DEFAULT NULL,
  `Amenizado` varchar(45) DEFAULT NULL,
  `PropietarioInquilino` varchar(300) DEFAULT 'PROPIETARIO',
  PRIMARY KEY (`pk_evento`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `formpago`
--

DROP TABLE IF EXISTS `formpago`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `formpago` (
  `pk_formpago` int NOT NULL AUTO_INCREMENT,
  `descripcion` varchar(100) DEFAULT NULL,
  `fk_cuenta` int DEFAULT NULL,
  `numero` varchar(20) DEFAULT NULL,
  `cuenta` varchar(100) DEFAULT NULL,
  `activo` varchar(2) DEFAULT NULL,
  PRIMARY KEY (`pk_formpago`)
) ENGINE=InnoDB AUTO_INCREMENT=31 DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `manzano`
--

DROP TABLE IF EXISTS `manzano`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `manzano` (
  `pk_manzano` int NOT NULL AUTO_INCREMENT,
  `codigo` varchar(15) NOT NULL,
  `nombre` varchar(20) DEFAULT NULL,
  `fk_condominio` int DEFAULT NULL,
  PRIMARY KEY (`pk_manzano`),
  KEY `fk_manzano_condominio1_idx` (`fk_condominio`),
  CONSTRAINT `fk_manzano_condominio1` FOREIGN KEY (`fk_condominio`) REFERENCES `condominio` (`pk_condominio`)
) ENGINE=InnoDB AUTO_INCREMENT=8 DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `moneda`
--

DROP TABLE IF EXISTS `moneda`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `moneda` (
  `pk_moneda` int NOT NULL AUTO_INCREMENT,
  `moneda` varchar(15) NOT NULL,
  `tipocambio` decimal(4,2) NOT NULL,
  `abreviado` varchar(3) NOT NULL,
  PRIMARY KEY (`pk_moneda`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `opcioncobro`
--

DROP TABLE IF EXISTS `opcioncobro`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `opcioncobro` (
  `pk_opcioncobro` int NOT NULL,
  `descripcion` varchar(50) NOT NULL,
  PRIMARY KEY (`pk_opcioncobro`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `permiso`
--

DROP TABLE IF EXISTS `permiso`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `permiso` (
  `pk_permiso` int NOT NULL AUTO_INCREMENT,
  `descripcion` varchar(40) DEFAULT NULL,
  PRIMARY KEY (`pk_permiso`)
) ENGINE=InnoDB AUTO_INCREMENT=62 DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `persona`
--

DROP TABLE IF EXISTS `persona`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `persona` (
  `pk_persona` int NOT NULL AUTO_INCREMENT,
  `nombre` varchar(50) DEFAULT NULL,
  `ci` varchar(10) DEFAULT NULL,
  `sexo` varchar(1) DEFAULT NULL,
  `fechanac` date DEFAULT NULL,
  `estadocivil` varchar(10) DEFAULT NULL,
  `direccion` varchar(50) DEFAULT NULL,
  `relacion` varchar(20) DEFAULT NULL,
  `activo` varchar(1) DEFAULT NULL,
  `fk_persona` int DEFAULT NULL,
  `telefono` varchar(15) DEFAULT NULL,
  `celular` varchar(15) DEFAULT NULL,
  `email` varchar(50) DEFAULT NULL,
  PRIMARY KEY (`pk_persona`),
  KEY `fk_persona_persona1_idx` (`fk_persona`),
  CONSTRAINT `fk_persona_persona1` FOREIGN KEY (`fk_persona`) REFERENCES `persona` (`pk_persona`)
) ENGINE=InnoDB AUTO_INCREMENT=549 DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `persona_contrato`
--

DROP TABLE IF EXISTS `persona_contrato`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `persona_contrato` (
  `fk_persona` int NOT NULL,
  `fk_contrato` int NOT NULL,
  `tipo` varchar(15) NOT NULL,
  `fecharegistro` datetime NOT NULL,
  `fecharetiro` datetime DEFAULT NULL,
  `valido` varchar(2) NOT NULL,
  `activo` varchar(1) NOT NULL,
  `fk_usuarioinsert` int DEFAULT NULL,
  `fk_usuariodelete` int DEFAULT NULL,
  PRIMARY KEY (`fk_persona`,`fk_contrato`),
  KEY `fk_persona_contrato_persona1_idx` (`fk_persona`),
  KEY `fk_persona_contrato_contrato1_idx` (`fk_contrato`),
  KEY `fk_persona_contrato_usuario1_idx` (`fk_usuarioinsert`),
  KEY `fk_persona_contrato_usuario2_idx` (`fk_usuariodelete`),
  CONSTRAINT `fk_persona_contrato_contrato1` FOREIGN KEY (`fk_contrato`) REFERENCES `contrato` (`pk_contrato`),
  CONSTRAINT `fk_persona_contrato_persona1` FOREIGN KEY (`fk_persona`) REFERENCES `persona` (`pk_persona`),
  CONSTRAINT `fk_persona_contrato_usuario1` FOREIGN KEY (`fk_usuarioinsert`) REFERENCES `usuario` (`pk_usuario`),
  CONSTRAINT `fk_persona_contrato_usuario2` FOREIGN KEY (`fk_usuariodelete`) REFERENCES `usuario` (`pk_usuario`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `propiedad`
--

DROP TABLE IF EXISTS `propiedad`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `propiedad` (
  `pk_propiedad` int NOT NULL AUTO_INCREMENT,
  `codigo` varchar(15) NOT NULL,
  `nombre` varchar(30) DEFAULT NULL,
  `m2` decimal(10,2) DEFAULT NULL,
  `expensa` decimal(10,2) NOT NULL,
  `tipo` varchar(15) DEFAULT NULL,
  `activo` bit(1) DEFAULT NULL,
  `fk_manzano` int DEFAULT NULL,
  `fk_condominio` int DEFAULT NULL,
  PRIMARY KEY (`pk_propiedad`),
  KEY `fk_propiedad_manzano1_idx` (`fk_manzano`),
  CONSTRAINT `fk_propiedad_manzano1` FOREIGN KEY (`fk_manzano`) REFERENCES `manzano` (`pk_manzano`)
) ENGINE=InnoDB AUTO_INCREMENT=164 DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `proveedor`
--

DROP TABLE IF EXISTS `proveedor`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `proveedor` (
  `pk_proveedor` int NOT NULL AUTO_INCREMENT,
  `nombre` varchar(70) DEFAULT NULL,
  `nit` varchar(20) DEFAULT NULL,
  `telefono` varchar(15) DEFAULT NULL,
  `celular` varchar(15) DEFAULT NULL,
  `direccion` varchar(100) DEFAULT NULL,
  `email` varchar(50) DEFAULT NULL,
  `activo` varchar(2) DEFAULT NULL,
  PRIMARY KEY (`pk_proveedor`)
) ENGINE=InnoDB AUTO_INCREMENT=130 DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `reciboegreso`
--

DROP TABLE IF EXISTS `reciboegreso`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `reciboegreso` (
  `pk_reciboegreso` int NOT NULL,
  `fk_condominio` int NOT NULL,
  PRIMARY KEY (`pk_reciboegreso`,`fk_condominio`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `recibogeneral`
--

DROP TABLE IF EXISTS `recibogeneral`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `recibogeneral` (
  `pk_recibogeneral` int NOT NULL,
  `fk_condominio` int NOT NULL,
  PRIMARY KEY (`pk_recibogeneral`,`fk_condominio`),
  KEY `fk_recibogeneral_condominio1_idx` (`fk_condominio`),
  CONSTRAINT `fk_recibogeneral_condominio1` FOREIGN KEY (`fk_condominio`) REFERENCES `condominio` (`pk_condominio`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `recibopersonal`
--

DROP TABLE IF EXISTS `recibopersonal`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `recibopersonal` (
  `pk_recibopersonal` int NOT NULL,
  `fk_contrato` int NOT NULL,
  PRIMARY KEY (`pk_recibopersonal`,`fk_contrato`),
  KEY `fk_recibopersonal_contrato1_idx` (`fk_contrato`),
  CONSTRAINT `fk_recibopersonal_contrato1` FOREIGN KEY (`fk_contrato`) REFERENCES `contrato` (`pk_contrato`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `servicio_contrato`
--

DROP TABLE IF EXISTS `servicio_contrato`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `servicio_contrato` (
  `pk_servicio_contrato` int NOT NULL AUTO_INCREMENT,
  `costo` decimal(10,2) DEFAULT NULL,
  `activo` varchar(1) DEFAULT NULL,
  `fk_contrato` int NOT NULL,
  `fk_serviciopago` int NOT NULL,
  PRIMARY KEY (`pk_servicio_contrato`),
  KEY `fk_servicio_contrato_contrato1_idx` (`fk_contrato`),
  KEY `fk_servicio_contrato_serviciopago1_idx` (`fk_serviciopago`),
  CONSTRAINT `fk_servicio_contrato_contrato1` FOREIGN KEY (`fk_contrato`) REFERENCES `contrato` (`pk_contrato`),
  CONSTRAINT `fk_servicio_contrato_serviciopago1` FOREIGN KEY (`fk_serviciopago`) REFERENCES `serviciopago` (`pk_serviciopago`)
) ENGINE=InnoDB AUTO_INCREMENT=167 DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `serviciocuenta`
--

DROP TABLE IF EXISTS `serviciocuenta`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `serviciocuenta` (
  `pk_serviciocuenta` int NOT NULL,
  `fk_servicio` int NOT NULL,
  `descripcion` varchar(50) DEFAULT NULL,
  `pk_cuenta` int DEFAULT NULL,
  `cuenta` varchar(50) DEFAULT NULL,
  PRIMARY KEY (`pk_serviciocuenta`,`fk_servicio`),
  KEY `fk_serviciocuenta_serviciopago1_idx` (`fk_servicio`),
  CONSTRAINT `fk_serviciocuenta_serviciopago1` FOREIGN KEY (`fk_servicio`) REFERENCES `serviciopago` (`pk_serviciopago`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `serviciopago`
--

DROP TABLE IF EXISTS `serviciopago`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `serviciopago` (
  `pk_serviciopago` int NOT NULL AUTO_INCREMENT,
  `nombre` varchar(45) NOT NULL,
  `costo` decimal(10,2) NOT NULL,
  `estado` varchar(2) DEFAULT NULL,
  `activo` varchar(1) DEFAULT NULL,
  PRIMARY KEY (`pk_serviciopago`)
) ENGINE=InnoDB AUTO_INCREMENT=34 DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tabla_servicio`
--

DROP TABLE IF EXISTS `tabla_servicio`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `tabla_servicio` (
  `pk_contrato` int NOT NULL,
  `codigo` varchar(45) DEFAULT NULL,
  `propietario` varchar(100) DEFAULT NULL,
  `nservicio1` varchar(100) DEFAULT NULL,
  `nservicio2` varchar(100) DEFAULT NULL,
  `nservicio3` varchar(100) DEFAULT NULL,
  `nservicio4` varchar(100) DEFAULT NULL,
  `nservicio5` varchar(100) DEFAULT NULL,
  `servicio1` decimal(12,2) DEFAULT '0.00',
  `servicio2` decimal(12,2) DEFAULT '0.00',
  `servicio3` decimal(12,2) DEFAULT '0.00',
  `servicio4` decimal(12,2) DEFAULT '0.00',
  `servicio5` decimal(12,2) DEFAULT '0.00',
  `total` decimal(12,2) DEFAULT '0.00',
  PRIMARY KEY (`pk_contrato`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `telefono`
--

DROP TABLE IF EXISTS `telefono`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `telefono` (
  `pk_telefono` int NOT NULL AUTO_INCREMENT,
  `tipo` varchar(25) DEFAULT NULL,
  `numero` varchar(10) NOT NULL,
  `estado` varchar(15) DEFAULT NULL,
  `fk_persona` int DEFAULT NULL,
  PRIMARY KEY (`pk_telefono`),
  KEY `fk_telefono_persona1_idx` (`fk_persona`),
  CONSTRAINT `fk_telefono_persona1` FOREIGN KEY (`fk_persona`) REFERENCES `persona` (`pk_persona`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tipo_permiso`
--

DROP TABLE IF EXISTS `tipo_permiso`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `tipo_permiso` (
  `fk_tipousuario` int NOT NULL,
  `fk_permiso` int NOT NULL,
  PRIMARY KEY (`fk_tipousuario`,`fk_permiso`),
  KEY `fk_tipo_permiso_permiso1_idx` (`fk_permiso`),
  CONSTRAINT `fk_tipo_permiso_permiso1` FOREIGN KEY (`fk_permiso`) REFERENCES `permiso` (`pk_permiso`),
  CONSTRAINT `fk_tipo_permiso_tipousuario1` FOREIGN KEY (`fk_tipousuario`) REFERENCES `tipousuario` (`pk_tipousuario`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `tipousuario`
--

DROP TABLE IF EXISTS `tipousuario`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `tipousuario` (
  `pk_tipousuario` int NOT NULL AUTO_INCREMENT,
  `descripcion` varchar(40) DEFAULT NULL,
  PRIMARY KEY (`pk_tipousuario`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `ttcuota`
--

DROP TABLE IF EXISTS `ttcuota`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ttcuota` (
  `pk_cuota` int NOT NULL,
  `cuota` decimal(12,2) DEFAULT NULL,
  `fk_deuda` int DEFAULT NULL,
  PRIMARY KEY (`pk_cuota`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `ttdeuda`
--

DROP TABLE IF EXISTS `ttdeuda`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ttdeuda` (
  `pk_deuda` int NOT NULL,
  `monto` decimal(12,2) DEFAULT NULL,
  PRIMARY KEY (`pk_deuda`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `usuario`
--

DROP TABLE IF EXISTS `usuario`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `usuario` (
  `pk_usuario` int NOT NULL AUTO_INCREMENT,
  `fk_persona` int NOT NULL,
  `fecha` datetime DEFAULT NULL,
  `usuario` varchar(10) DEFAULT NULL,
  `contrasena` varchar(10) DEFAULT NULL,
  `habilitado` varchar(2) DEFAULT NULL,
  `activo` varchar(1) DEFAULT NULL,
  `nombrepersona` varchar(100) DEFAULT NULL,
  `fk_tipousuario` int DEFAULT NULL,
  PRIMARY KEY (`pk_usuario`),
  UNIQUE KEY `usuario_UNIQUE` (`usuario`),
  KEY `fk_usuario_persona1` (`fk_persona`),
  KEY `fk_usuario_tipousuario1_idx` (`fk_tipousuario`),
  CONSTRAINT `fk_usuario_persona1` FOREIGN KEY (`fk_persona`) REFERENCES `persona` (`pk_persona`),
  CONSTRAINT `fk_usuario_tipousuario1` FOREIGN KEY (`fk_tipousuario`) REFERENCES `tipousuario` (`pk_tipousuario`)
) ENGINE=InnoDB AUTO_INCREMENT=22 DEFAULT CHARSET=utf8mb3;
/*!40101 SET character_set_client = @saved_cs_client */;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2025-12-29 22:05:41
