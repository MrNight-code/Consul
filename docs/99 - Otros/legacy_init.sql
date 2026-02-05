CREATE DATABASE  IF NOT EXISTS `db_condominio_foret` /*!40100 DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci */ /*!80016 DEFAULT ENCRYPTION='N' */;
USE `db_condominio_foret`;
-- MySQL dump 10.13  Distrib 8.0.44, for Win64 (x86_64)
--
-- Host: 127.0.0.1    Database: db_condominio_foret
-- ------------------------------------------------------
-- Server version	8.0.44

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `asiento_contable`
--

DROP TABLE IF EXISTS `asiento_contable`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `asiento_contable` (
  `id_asiento` int NOT NULL AUTO_INCREMENT,
  `id_condominio` int NOT NULL,
  `fecha_contable` datetime NOT NULL,
  `glosa_general` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `tipo_asiento` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT 'Diario, Ajuste, Cierre',
  `nro_documento_respaldo` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `id_transaccion_origen_pago` int DEFAULT NULL COMMENT 'Link a Tesoreria',
  `id_transaccion_origen_egreso` int DEFAULT NULL COMMENT 'Link a Tesoreria',
  PRIMARY KEY (`id_asiento`),
  KEY `fk_asiento_condominio` (`id_condominio`),
  KEY `fk_asiento_pago` (`id_transaccion_origen_pago`),
  KEY `fk_asiento_egreso` (`id_transaccion_origen_egreso`),
  CONSTRAINT `fk_asiento_condominio` FOREIGN KEY (`id_condominio`) REFERENCES `condominio` (`id_condominio`),
  CONSTRAINT `fk_asiento_egreso` FOREIGN KEY (`id_transaccion_origen_egreso`) REFERENCES `egreso` (`id_egreso`),
  CONSTRAINT `fk_asiento_pago` FOREIGN KEY (`id_transaccion_origen_pago`) REFERENCES `transaccion_pago` (`id_pago`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `asiento_contable`
--

LOCK TABLES `asiento_contable` WRITE;
/*!40000 ALTER TABLE `asiento_contable` DISABLE KEYS */;
/*!40000 ALTER TABLE `asiento_contable` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `asiento_detalle`
--

DROP TABLE IF EXISTS `asiento_detalle`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `asiento_detalle` (
  `id_asiento_det` int NOT NULL AUTO_INCREMENT,
  `id_asiento` int NOT NULL,
  `id_cuenta` int NOT NULL,
  `glosa_linea` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `debe` decimal(12,2) DEFAULT '0.00',
  `haber` decimal(12,2) DEFAULT '0.00',
  PRIMARY KEY (`id_asiento_det`),
  KEY `fk_ad_asiento` (`id_asiento`),
  KEY `fk_ad_cuenta` (`id_cuenta`),
  CONSTRAINT `fk_ad_asiento` FOREIGN KEY (`id_asiento`) REFERENCES `asiento_contable` (`id_asiento`) ON DELETE CASCADE,
  CONSTRAINT `fk_ad_cuenta` FOREIGN KEY (`id_cuenta`) REFERENCES `plan_cuentas` (`id_cuenta`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `asiento_detalle`
--

LOCK TABLES `asiento_detalle` WRITE;
/*!40000 ALTER TABLE `asiento_detalle` DISABLE KEYS */;
/*!40000 ALTER TABLE `asiento_detalle` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `autorizacion_gasto`
--

DROP TABLE IF EXISTS `autorizacion_gasto`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `autorizacion_gasto` (
  `id_autorizacion` int NOT NULL AUTO_INCREMENT,
  `descripcion` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL COMMENT 'Niveles de firma para gastos',
  `activo` tinyint(1) DEFAULT '1',
  PRIMARY KEY (`id_autorizacion`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `autorizacion_gasto`
--

LOCK TABLES `autorizacion_gasto` WRITE;
/*!40000 ALTER TABLE `autorizacion_gasto` DISABLE KEYS */;
/*!40000 ALTER TABLE `autorizacion_gasto` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `banco`
--

DROP TABLE IF EXISTS `banco`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `banco` (
  `id_banco` int NOT NULL AUTO_INCREMENT,
  `nombre_entidad` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `numero_cuenta` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `moneda` varchar(10) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT 'BOB',
  `activo` tinyint(1) DEFAULT '1',
  `id_cuenta_contable_asociada` int DEFAULT NULL,
  PRIMARY KEY (`id_banco`),
  KEY `fk_banco_cuenta` (`id_cuenta_contable_asociada`),
  CONSTRAINT `fk_banco_cuenta` FOREIGN KEY (`id_cuenta_contable_asociada`) REFERENCES `plan_cuentas` (`id_cuenta`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `banco`
--

LOCK TABLES `banco` WRITE;
/*!40000 ALTER TABLE `banco` DISABLE KEYS */;
/*!40000 ALTER TABLE `banco` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `catalogo_servicio`
--

DROP TABLE IF EXISTS `catalogo_servicio`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `catalogo_servicio` (
  `id_servicio` int NOT NULL AUTO_INCREMENT COMMENT 'Antes: serviciopago',
  `nombre` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL COMMENT 'Agua, Luz, Multa, Expensa',
  `costo_base` decimal(10,2) DEFAULT '0.00',
  `es_recurrente` tinyint(1) DEFAULT '1',
  `activo` tinyint(1) DEFAULT '1',
  PRIMARY KEY (`id_servicio`)
) ENGINE=InnoDB AUTO_INCREMENT=47 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `catalogo_servicio`
--

LOCK TABLES `catalogo_servicio` WRITE;
/*!40000 ALTER TABLE `catalogo_servicio` DISABLE KEYS */;
/*!40000 ALTER TABLE `catalogo_servicio` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `comunicado_blog`
--

DROP TABLE IF EXISTS `comunicado_blog`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `comunicado_blog` (
  `id_blog` int NOT NULL AUTO_INCREMENT,
  `id_condominio` int NOT NULL,
  `fecha_publicacion` datetime DEFAULT CURRENT_TIMESTAMP,
  `titulo` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `contenido_html` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci,
  `url_imagen` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `url_archivo_adjunto` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `activo` tinyint(1) DEFAULT '1',
  PRIMARY KEY (`id_blog`),
  KEY `fk_blog_condominio` (`id_condominio`),
  CONSTRAINT `fk_blog_condominio` FOREIGN KEY (`id_condominio`) REFERENCES `condominio` (`id_condominio`)
) ENGINE=InnoDB AUTO_INCREMENT=8 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `comunicado_blog`
--

LOCK TABLES `comunicado_blog` WRITE;
/*!40000 ALTER TABLE `comunicado_blog` DISABLE KEYS */;
/*!40000 ALTER TABLE `comunicado_blog` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `condominio`
--

DROP TABLE IF EXISTS `condominio`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `condominio` (
  `id_condominio` int NOT NULL AUTO_INCREMENT,
  `codigo` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `nombre` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `direccion` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `superficie_total_m2` decimal(12,2) DEFAULT NULL,
  `id_admin_persona` int NOT NULL,
  `config_dia_cobro` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `logo` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`id_condominio`),
  KEY `fk_condominio_admin` (`id_admin_persona`),
  CONSTRAINT `fk_condominio_admin` FOREIGN KEY (`id_admin_persona`) REFERENCES `persona` (`id_persona`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `condominio`
--

LOCK TABLES `condominio` WRITE;
/*!40000 ALTER TABLE `condominio` DISABLE KEYS */;
/*!40000 ALTER TABLE `condominio` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `config_aviso_cobranza`
--

DROP TABLE IF EXISTS `config_aviso_cobranza`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `config_aviso_cobranza` (
  `id_config` int NOT NULL AUTO_INCREMENT COMMENT 'Antes: confaviso',
  `id_condominio` int NOT NULL,
  `texto_header` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci,
  `texto_footer` text CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci,
  `dias_vencimiento_defecto` int DEFAULT '10',
  PRIMARY KEY (`id_config`),
  KEY `fk_aviso_condominio` (`id_condominio`),
  CONSTRAINT `fk_aviso_condominio` FOREIGN KEY (`id_condominio`) REFERENCES `condominio` (`id_condominio`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `config_aviso_cobranza`
--

LOCK TABLES `config_aviso_cobranza` WRITE;
/*!40000 ALTER TABLE `config_aviso_cobranza` DISABLE KEYS */;
/*!40000 ALTER TABLE `config_aviso_cobranza` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `config_contable_servicio`
--

DROP TABLE IF EXISTS `config_contable_servicio`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `config_contable_servicio` (
  `id_servicio` int NOT NULL,
  `id_cuenta_ingreso` int NOT NULL,
  PRIMARY KEY (`id_servicio`,`id_cuenta_ingreso`),
  KEY `fk_ccs_cuenta` (`id_cuenta_ingreso`),
  CONSTRAINT `fk_ccs_cuenta` FOREIGN KEY (`id_cuenta_ingreso`) REFERENCES `plan_cuentas` (`id_cuenta`),
  CONSTRAINT `fk_ccs_servicio` FOREIGN KEY (`id_servicio`) REFERENCES `catalogo_servicio` (`id_servicio`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `config_contable_servicio`
--

LOCK TABLES `config_contable_servicio` WRITE;
/*!40000 ALTER TABLE `config_contable_servicio` DISABLE KEYS */;
/*!40000 ALTER TABLE `config_contable_servicio` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `contrato`
--

DROP TABLE IF EXISTS `contrato`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `contrato` (
  `id_contrato` int NOT NULL AUTO_INCREMENT,
  `id_propiedad` int NOT NULL,
  `fecha_firma` date DEFAULT NULL,
  `fecha_inicio` date NOT NULL,
  `fecha_fin` date DEFAULT NULL,
  `fecha_ingreso_real` date DEFAULT NULL,
  `monto_expensa_pactada` decimal(10,2) NOT NULL,
  `estado` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT 'Vigente' COMMENT 'Vigente, Finalizado, Rescindido',
  `motivo_baja` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `id_usuario_creador` int DEFAULT NULL,
  PRIMARY KEY (`id_contrato`),
  KEY `fk_contrato_propiedad` (`id_propiedad`),
  KEY `fk_contrato_creador` (`id_usuario_creador`),
  CONSTRAINT `fk_contrato_creador` FOREIGN KEY (`id_usuario_creador`) REFERENCES `usuario` (`id_usuario`),
  CONSTRAINT `fk_contrato_propiedad` FOREIGN KEY (`id_propiedad`) REFERENCES `propiedad` (`id_propiedad`)
) ENGINE=InnoDB AUTO_INCREMENT=492 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `contrato`
--

LOCK TABLES `contrato` WRITE;
/*!40000 ALTER TABLE `contrato` DISABLE KEYS */;
/*!40000 ALTER TABLE `contrato` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `contrato_participante`
--

DROP TABLE IF EXISTS `contrato_participante`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `contrato_participante` (
  `id_contrato` int NOT NULL,
  `id_persona` int NOT NULL,
  `rol_contrato` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL COMMENT 'Titular, Inquilino, Garante',
  `fecha_alta` date DEFAULT NULL,
  `fecha_baja` date DEFAULT NULL,
  `activo` tinyint(1) DEFAULT '1',
  PRIMARY KEY (`id_contrato`,`id_persona`),
  KEY `fk_cp_persona` (`id_persona`),
  CONSTRAINT `fk_cp_contrato` FOREIGN KEY (`id_contrato`) REFERENCES `contrato` (`id_contrato`),
  CONSTRAINT `fk_cp_persona` FOREIGN KEY (`id_persona`) REFERENCES `persona` (`id_persona`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `contrato_participante`
--

LOCK TABLES `contrato_participante` WRITE;
/*!40000 ALTER TABLE `contrato_participante` DISABLE KEYS */;
/*!40000 ALTER TABLE `contrato_participante` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `contrato_servicio_suscrito`
--

DROP TABLE IF EXISTS `contrato_servicio_suscrito`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `contrato_servicio_suscrito` (
  `id_suscripcion` int NOT NULL AUTO_INCREMENT COMMENT 'Antes: servicio_contrato',
  `id_contrato` int NOT NULL,
  `id_servicio` int NOT NULL,
  `costo_personalizado` decimal(10,2) DEFAULT NULL COMMENT 'Si difiere del base',
  `activo` tinyint(1) DEFAULT '1',
  PRIMARY KEY (`id_suscripcion`),
  KEY `fk_css_contrato` (`id_contrato`),
  KEY `fk_css_servicio` (`id_servicio`),
  CONSTRAINT `fk_css_contrato` FOREIGN KEY (`id_contrato`) REFERENCES `contrato` (`id_contrato`),
  CONSTRAINT `fk_css_servicio` FOREIGN KEY (`id_servicio`) REFERENCES `catalogo_servicio` (`id_servicio`)
) ENGINE=InnoDB AUTO_INCREMENT=487 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `contrato_servicio_suscrito`
--

LOCK TABLES `contrato_servicio_suscrito` WRITE;
/*!40000 ALTER TABLE `contrato_servicio_suscrito` DISABLE KEYS */;
/*!40000 ALTER TABLE `contrato_servicio_suscrito` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `deuda_cabecera`
--

DROP TABLE IF EXISTS `deuda_cabecera`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `deuda_cabecera` (
  `id_deuda` int NOT NULL AUTO_INCREMENT,
  `id_contrato` int NOT NULL,
  `anio_periodo` int NOT NULL,
  `mes_periodo` int NOT NULL,
  `fecha_emision` date DEFAULT NULL,
  `fecha_vencimiento` date DEFAULT NULL,
  `total_deuda` decimal(12,2) DEFAULT '0.00',
  `total_pagado` decimal(12,2) DEFAULT '0.00',
  `estado_pago` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT 'PENDIENTE' COMMENT 'PENDIENTE, PARCIAL, PAGADO, ANULADO',
  `id_usuario_generador` int DEFAULT NULL,
  PRIMARY KEY (`id_deuda`),
  KEY `fk_deuda_contrato` (`id_contrato`),
  KEY `fk_deuda_usuario` (`id_usuario_generador`),
  CONSTRAINT `fk_deuda_contrato` FOREIGN KEY (`id_contrato`) REFERENCES `contrato` (`id_contrato`),
  CONSTRAINT `fk_deuda_usuario` FOREIGN KEY (`id_usuario_generador`) REFERENCES `usuario` (`id_usuario`)
) ENGINE=InnoDB AUTO_INCREMENT=6697 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `deuda_cabecera`
--

LOCK TABLES `deuda_cabecera` WRITE;
/*!40000 ALTER TABLE `deuda_cabecera` DISABLE KEYS */;
/*!40000 ALTER TABLE `deuda_cabecera` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `deuda_detalle`
--

DROP TABLE IF EXISTS `deuda_detalle`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `deuda_detalle` (
  `id_deuda_det` int NOT NULL AUTO_INCREMENT,
  `id_deuda` int NOT NULL,
  `id_servicio` int NOT NULL COMMENT 'Origen del cobro',
  `concepto` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL COMMENT 'Ej: Expensa Mayo 2025',
  `monto_unitario` decimal(10,2) NOT NULL,
  `cantidad` decimal(10,2) DEFAULT '1.00',
  `subtotal` decimal(12,2) NOT NULL,
  PRIMARY KEY (`id_deuda_det`),
  KEY `fk_dd_cabecera` (`id_deuda`),
  KEY `fk_dd_servicio` (`id_servicio`),
  CONSTRAINT `fk_dd_cabecera` FOREIGN KEY (`id_deuda`) REFERENCES `deuda_cabecera` (`id_deuda`) ON DELETE CASCADE,
  CONSTRAINT `fk_dd_servicio` FOREIGN KEY (`id_servicio`) REFERENCES `catalogo_servicio` (`id_servicio`)
) ENGINE=InnoDB AUTO_INCREMENT=6668 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `deuda_detalle`
--

LOCK TABLES `deuda_detalle` WRITE;
/*!40000 ALTER TABLE `deuda_detalle` DISABLE KEYS */;
/*!40000 ALTER TABLE `deuda_detalle` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `egreso`
--

DROP TABLE IF EXISTS `egreso`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `egreso` (
  `id_egreso` int NOT NULL AUTO_INCREMENT,
  `id_condominio` int NOT NULL,
  `id_proveedor` int DEFAULT NULL COMMENT 'Opcional',
  `id_persona_beneficiario` int DEFAULT NULL COMMENT 'Opcional',
  `id_autorizacion` int NOT NULL,
  `id_banco_origen` int NOT NULL,
  `id_forma_pago` int NOT NULL,
  `concepto` varchar(300) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `monto_total` decimal(12,2) NOT NULL,
  `fecha_egreso` datetime DEFAULT CURRENT_TIMESTAMP,
  `nro_factura_proveedor` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `id_usuario_registro` int NOT NULL,
  PRIMARY KEY (`id_egreso`),
  KEY `fk_egreso_condominio` (`id_condominio`),
  KEY `fk_egreso_proveedor` (`id_proveedor`),
  KEY `fk_egreso_persona` (`id_persona_beneficiario`),
  KEY `fk_egreso_aut` (`id_autorizacion`),
  KEY `fk_egreso_banco` (`id_banco_origen`),
  KEY `fk_egreso_fp` (`id_forma_pago`),
  KEY `fk_egreso_usuario` (`id_usuario_registro`),
  CONSTRAINT `fk_egreso_aut` FOREIGN KEY (`id_autorizacion`) REFERENCES `autorizacion_gasto` (`id_autorizacion`),
  CONSTRAINT `fk_egreso_banco` FOREIGN KEY (`id_banco_origen`) REFERENCES `banco` (`id_banco`),
  CONSTRAINT `fk_egreso_condominio` FOREIGN KEY (`id_condominio`) REFERENCES `condominio` (`id_condominio`),
  CONSTRAINT `fk_egreso_fp` FOREIGN KEY (`id_forma_pago`) REFERENCES `forma_pago` (`id_forma_pago`),
  CONSTRAINT `fk_egreso_persona` FOREIGN KEY (`id_persona_beneficiario`) REFERENCES `persona` (`id_persona`),
  CONSTRAINT `fk_egreso_proveedor` FOREIGN KEY (`id_proveedor`) REFERENCES `proveedor` (`id_proveedor`),
  CONSTRAINT `fk_egreso_usuario` FOREIGN KEY (`id_usuario_registro`) REFERENCES `usuario` (`id_usuario`)
) ENGINE=InnoDB AUTO_INCREMENT=649 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `egreso`
--

LOCK TABLES `egreso` WRITE;
/*!40000 ALTER TABLE `egreso` DISABLE KEYS */;
/*!40000 ALTER TABLE `egreso` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `forma_pago`
--

DROP TABLE IF EXISTS `forma_pago`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `forma_pago` (
  `id_forma_pago` int NOT NULL AUTO_INCREMENT,
  `descripcion` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL COMMENT 'Efectivo, Cheque, Transferencia',
  `id_cuenta_contable_asociada` int DEFAULT NULL,
  PRIMARY KEY (`id_forma_pago`),
  KEY `fk_fp_cuenta` (`id_cuenta_contable_asociada`),
  CONSTRAINT `fk_fp_cuenta` FOREIGN KEY (`id_cuenta_contable_asociada`) REFERENCES `plan_cuentas` (`id_cuenta`)
) ENGINE=InnoDB AUTO_INCREMENT=51 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `forma_pago`
--

LOCK TABLES `forma_pago` WRITE;
/*!40000 ALTER TABLE `forma_pago` DISABLE KEYS */;
/*!40000 ALTER TABLE `forma_pago` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `lectura_servicio`
--

DROP TABLE IF EXISTS `lectura_servicio`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `lectura_servicio` (
  `id_lectura` int NOT NULL AUTO_INCREMENT,
  `id_suscripcion` int NOT NULL,
  `anio` int NOT NULL,
  `mes` int NOT NULL,
  `valor_leido` decimal(12,2) DEFAULT NULL COMMENT 'Para agua/luz variable',
  `monto_calculado` decimal(10,2) NOT NULL,
  `fecha_lectura` date DEFAULT NULL,
  PRIMARY KEY (`id_lectura`),
  KEY `fk_lectura_suscripcion` (`id_suscripcion`),
  CONSTRAINT `fk_lectura_suscripcion` FOREIGN KEY (`id_suscripcion`) REFERENCES `contrato_servicio_suscrito` (`id_suscripcion`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `lectura_servicio`
--

LOCK TABLES `lectura_servicio` WRITE;
/*!40000 ALTER TABLE `lectura_servicio` DISABLE KEYS */;
/*!40000 ALTER TABLE `lectura_servicio` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `manzano`
--

DROP TABLE IF EXISTS `manzano`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `manzano` (
  `id_manzano` int NOT NULL AUTO_INCREMENT,
  `id_condominio` int NOT NULL,
  `codigo` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `nombre` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`id_manzano`),
  KEY `fk_manzano_condominio` (`id_condominio`),
  CONSTRAINT `fk_manzano_condominio` FOREIGN KEY (`id_condominio`) REFERENCES `condominio` (`id_condominio`)
) ENGINE=InnoDB AUTO_INCREMENT=30 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `manzano`
--

LOCK TABLES `manzano` WRITE;
/*!40000 ALTER TABLE `manzano` DISABLE KEYS */;
/*!40000 ALTER TABLE `manzano` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `medio_contacto`
--

DROP TABLE IF EXISTS `medio_contacto`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `medio_contacto` (
  `id_contacto` int NOT NULL AUTO_INCREMENT,
  `id_persona` int NOT NULL,
  `tipo` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL COMMENT 'Telefono, Celular, Email, Facebook',
  `valor` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL COMMENT 'El numero o correo',
  `es_principal` tinyint(1) DEFAULT '0',
  PRIMARY KEY (`id_contacto`),
  KEY `fk_contacto_persona` (`id_persona`),
  CONSTRAINT `fk_contacto_persona` FOREIGN KEY (`id_persona`) REFERENCES `persona` (`id_persona`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `medio_contacto`
--

LOCK TABLES `medio_contacto` WRITE;
/*!40000 ALTER TABLE `medio_contacto` DISABLE KEYS */;
/*!40000 ALTER TABLE `medio_contacto` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `permiso`
--

DROP TABLE IF EXISTS `permiso`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `permiso` (
  `id_permiso` int NOT NULL AUTO_INCREMENT,
  `descripcion` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL COMMENT 'Antes: permiso',
  PRIMARY KEY (`id_permiso`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `permiso`
--

LOCK TABLES `permiso` WRITE;
/*!40000 ALTER TABLE `permiso` DISABLE KEYS */;
/*!40000 ALTER TABLE `permiso` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `persona`
--

DROP TABLE IF EXISTS `persona`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `persona` (
  `id_persona` int NOT NULL AUTO_INCREMENT,
  `nombre_completo` varchar(150) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL COMMENT 'Antes: nombre',
  `ci` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `fecha_nacimiento` date DEFAULT NULL,
  `sexo` char(1) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `estado_civil` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `es_activo` tinyint(1) DEFAULT '1',
  `id_familiar_responsable` int DEFAULT NULL COMMENT 'Recursiva: Para hijos/dependientes',
  PRIMARY KEY (`id_persona`),
  KEY `fk_persona_familiar` (`id_familiar_responsable`),
  CONSTRAINT `fk_persona_familiar` FOREIGN KEY (`id_familiar_responsable`) REFERENCES `persona` (`id_persona`)
) ENGINE=InnoDB AUTO_INCREMENT=10000 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `persona`
--

LOCK TABLES `persona` WRITE;
/*!40000 ALTER TABLE `persona` DISABLE KEYS */;
/*!40000 ALTER TABLE `persona` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `plan_cuentas`
--

DROP TABLE IF EXISTS `plan_cuentas`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `plan_cuentas` (
  `id_cuenta` int NOT NULL AUTO_INCREMENT,
  `codigo_cuenta` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL COMMENT 'Ej: 1.1.01',
  `nombre` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `id_cuenta_padre` int DEFAULT NULL COMMENT 'Recursiva',
  `nivel_jerarquia` int DEFAULT '1',
  `es_imputable` tinyint(1) DEFAULT '1' COMMENT 'Si/No',
  PRIMARY KEY (`id_cuenta`),
  KEY `fk_pc_padre` (`id_cuenta_padre`),
  CONSTRAINT `fk_pc_padre` FOREIGN KEY (`id_cuenta_padre`) REFERENCES `plan_cuentas` (`id_cuenta`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `plan_cuentas`
--

LOCK TABLES `plan_cuentas` WRITE;
/*!40000 ALTER TABLE `plan_cuentas` DISABLE KEYS */;
/*!40000 ALTER TABLE `plan_cuentas` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `propiedad`
--

DROP TABLE IF EXISTS `propiedad`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `propiedad` (
  `id_propiedad` int NOT NULL AUTO_INCREMENT,
  `id_manzano` int NOT NULL,
  `codigo_unidad` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `nombre_funcional` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `superficie_m2` decimal(10,2) DEFAULT NULL,
  `porcentaje_participacion` decimal(5,4) DEFAULT NULL COMMENT 'Para prorrateo',
  `expensa_base_defecto` decimal(10,2) DEFAULT NULL,
  `tipo` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT 'Casa, Depto, Lote',
  `activo` tinyint(1) DEFAULT '1',
  PRIMARY KEY (`id_propiedad`),
  KEY `fk_propiedad_manzano` (`id_manzano`),
  CONSTRAINT `fk_propiedad_manzano` FOREIGN KEY (`id_manzano`) REFERENCES `manzano` (`id_manzano`)
) ENGINE=InnoDB AUTO_INCREMENT=501 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `propiedad`
--

LOCK TABLES `propiedad` WRITE;
/*!40000 ALTER TABLE `propiedad` DISABLE KEYS */;
/*!40000 ALTER TABLE `propiedad` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `proveedor`
--

DROP TABLE IF EXISTS `proveedor`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `proveedor` (
  `id_proveedor` int NOT NULL AUTO_INCREMENT,
  `razon_social` varchar(150) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `nit` varchar(30) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `contacto` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `direccion` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `activo` tinyint(1) DEFAULT '1',
  PRIMARY KEY (`id_proveedor`)
) ENGINE=InnoDB AUTO_INCREMENT=87 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `proveedor`
--

LOCK TABLES `proveedor` WRITE;
/*!40000 ALTER TABLE `proveedor` DISABLE KEYS */;
/*!40000 ALTER TABLE `proveedor` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `recurso_comun`
--

DROP TABLE IF EXISTS `recurso_comun`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `recurso_comun` (
  `id_recurso` int NOT NULL AUTO_INCREMENT,
  `id_condominio` int NOT NULL,
  `nombre` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL COMMENT 'Churrasquera, Salon',
  `costo_reserva` decimal(10,2) DEFAULT '0.00',
  `costo_garantia` decimal(10,2) DEFAULT '0.00',
  `color_calendario` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL COMMENT 'Antes en tabla evento',
  PRIMARY KEY (`id_recurso`),
  KEY `fk_recurso_condominio` (`id_condominio`),
  CONSTRAINT `fk_recurso_condominio` FOREIGN KEY (`id_condominio`) REFERENCES `condominio` (`id_condominio`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `recurso_comun`
--

LOCK TABLES `recurso_comun` WRITE;
/*!40000 ALTER TABLE `recurso_comun` DISABLE KEYS */;
/*!40000 ALTER TABLE `recurso_comun` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `reserva`
--

DROP TABLE IF EXISTS `reserva`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `reserva` (
  `id_reserva` int NOT NULL AUTO_INCREMENT COMMENT 'Antes: evento',
  `id_recurso` int NOT NULL,
  `id_contrato` int NOT NULL COMMENT 'Quien reserva',
  `fecha_inicio` datetime NOT NULL,
  `fecha_fin` datetime NOT NULL,
  `cantidad_invitados` int DEFAULT NULL,
  `motivo` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `amenizado_por` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `monto_total_cobrado` decimal(10,2) DEFAULT NULL,
  `estado` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT 'PENDIENTE' COMMENT 'PENDIENTE, CONFIRMADA, FINALIZADA',
  PRIMARY KEY (`id_reserva`),
  KEY `fk_reserva_recurso` (`id_recurso`),
  KEY `fk_reserva_contrato` (`id_contrato`),
  CONSTRAINT `fk_reserva_contrato` FOREIGN KEY (`id_contrato`) REFERENCES `contrato` (`id_contrato`),
  CONSTRAINT `fk_reserva_recurso` FOREIGN KEY (`id_recurso`) REFERENCES `recurso_comun` (`id_recurso`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `reserva`
--

LOCK TABLES `reserva` WRITE;
/*!40000 ALTER TABLE `reserva` DISABLE KEYS */;
/*!40000 ALTER TABLE `reserva` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `rol`
--

DROP TABLE IF EXISTS `rol`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `rol` (
  `id_rol` int NOT NULL AUTO_INCREMENT COMMENT 'Antes: tipousuario',
  `nombre` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL COMMENT 'Admin, Guardia, Vecino',
  PRIMARY KEY (`id_rol`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `rol`
--

LOCK TABLES `rol` WRITE;
/*!40000 ALTER TABLE `rol` DISABLE KEYS */;
/*!40000 ALTER TABLE `rol` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `rol_permiso`
--

DROP TABLE IF EXISTS `rol_permiso`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `rol_permiso` (
  `id_rol` int NOT NULL,
  `id_permiso` int NOT NULL,
  PRIMARY KEY (`id_rol`,`id_permiso`),
  KEY `fk_rp_permiso` (`id_permiso`),
  CONSTRAINT `fk_rp_permiso` FOREIGN KEY (`id_permiso`) REFERENCES `permiso` (`id_permiso`) ON DELETE CASCADE,
  CONSTRAINT `fk_rp_rol` FOREIGN KEY (`id_rol`) REFERENCES `rol` (`id_rol`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `rol_permiso`
--

LOCK TABLES `rol_permiso` WRITE;
/*!40000 ALTER TABLE `rol_permiso` DISABLE KEYS */;
/*!40000 ALTER TABLE `rol_permiso` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `transaccion_pago`
--

DROP TABLE IF EXISTS `transaccion_pago`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `transaccion_pago` (
  `id_pago` int NOT NULL AUTO_INCREMENT COMMENT 'Antes: cuota',
  `id_deuda` int NOT NULL COMMENT 'Pago especifico de una deuda',
  `id_persona_pagador` int NOT NULL,
  `id_banco_destino` int NOT NULL,
  `id_forma_pago` int NOT NULL,
  `fecha_pago` datetime DEFAULT CURRENT_TIMESTAMP,
  `monto_abonado` decimal(12,2) NOT NULL,
  `tipo_cambio` decimal(10,4) DEFAULT '1.0000',
  `nro_comprobante_banco` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT NULL,
  `estado` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci DEFAULT 'CONFIRMADO' COMMENT 'CONFIRMADO, RECHAZADO',
  `id_recibo_generado` int DEFAULT NULL,
  PRIMARY KEY (`id_pago`),
  KEY `fk_tp_deuda` (`id_deuda`),
  KEY `fk_tp_persona` (`id_persona_pagador`),
  KEY `fk_tp_banco` (`id_banco_destino`),
  KEY `fk_tp_forma` (`id_forma_pago`),
  CONSTRAINT `fk_tp_banco` FOREIGN KEY (`id_banco_destino`) REFERENCES `banco` (`id_banco`),
  CONSTRAINT `fk_tp_deuda` FOREIGN KEY (`id_deuda`) REFERENCES `deuda_cabecera` (`id_deuda`),
  CONSTRAINT `fk_tp_forma` FOREIGN KEY (`id_forma_pago`) REFERENCES `forma_pago` (`id_forma_pago`),
  CONSTRAINT `fk_tp_persona` FOREIGN KEY (`id_persona_pagador`) REFERENCES `persona` (`id_persona`)
) ENGINE=InnoDB AUTO_INCREMENT=5701 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `transaccion_pago`
--

LOCK TABLES `transaccion_pago` WRITE;
/*!40000 ALTER TABLE `transaccion_pago` DISABLE KEYS */;
/*!40000 ALTER TABLE `transaccion_pago` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `usuario`
--

DROP TABLE IF EXISTS `usuario`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `usuario` (
  `id_usuario` int NOT NULL AUTO_INCREMENT,
  `id_persona` int NOT NULL,
  `username` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL,
  `password_hash` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci NOT NULL COMMENT 'Antes: contrasena',
  `fecha_creacion` datetime DEFAULT CURRENT_TIMESTAMP,
  `esta_habilitado` tinyint(1) DEFAULT '1',
  `id_rol_principal` int DEFAULT NULL,
  PRIMARY KEY (`id_usuario`),
  UNIQUE KEY `username` (`username`),
  KEY `fk_usuario_persona` (`id_persona`),
  KEY `fk_usuario_rol` (`id_rol_principal`),
  CONSTRAINT `fk_usuario_persona` FOREIGN KEY (`id_persona`) REFERENCES `persona` (`id_persona`),
  CONSTRAINT `fk_usuario_rol` FOREIGN KEY (`id_rol_principal`) REFERENCES `rol` (`id_rol`)
) ENGINE=InnoDB AUTO_INCREMENT=10000 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `usuario`
--

LOCK TABLES `usuario` WRITE;
/*!40000 ALTER TABLE `usuario` DISABLE KEYS */;
/*!40000 ALTER TABLE `usuario` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2026-01-05  8:50:11
