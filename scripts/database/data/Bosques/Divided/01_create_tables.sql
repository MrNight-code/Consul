SET FOREIGN_KEY_CHECKS=0;
SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";

DROP TABLE IF EXISTS `asiento`;
CREATE TABLE `asiento` (
  `pk_asiento` int(11) NOT NULL AUTO_INCREMENT,
  `fecha` datetime DEFAULT NULL,
  `documento` varchar(50) COLLATE utf8_unicode_ci DEFAULT NULL,
  `numerodocumento` varchar(15) COLLATE utf8_unicode_ci DEFAULT NULL,
  `fk_banco` int(11) DEFAULT NULL,
  `cheque` varchar(15) COLLATE utf8_unicode_ci DEFAULT NULL,
  `tc` decimal(14,4) DEFAULT NULL,
  `tipoasiento` varchar(10) COLLATE utf8_unicode_ci DEFAULT NULL,
  `fk_cuenta` int(11) DEFAULT NULL,
  `fk_deuda` int(11) DEFAULT NULL,
  `glosa` varchar(500) COLLATE utf8_unicode_ci DEFAULT NULL,
  `activo` varchar(2) COLLATE utf8_unicode_ci DEFAULT NULL,
  `fechabaja` datetime DEFAULT NULL,
  `fechaupdate` datetime DEFAULT NULL,
  `fk_usuariobaja` int(11) DEFAULT NULL,
  `fk_usuarioupdate` int(11) DEFAULT NULL,
  `fk_proceso` int(11) DEFAULT NULL,
  `formulario` varchar(45) COLLATE utf8_unicode_ci DEFAULT NULL,
  `numero` int(11) DEFAULT NULL,
  `fechacreacion` datetime NOT NULL,
  `fk_usuario` int(11) NOT NULL,
  `motivobaja` varchar(50) COLLATE utf8_unicode_ci NOT NULL,
  PRIMARY KEY (`pk_asiento`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

DROP TABLE IF EXISTS `autorizacion`;
CREATE TABLE `autorizacion` (
  `pk_autorizacion` int(11) NOT NULL AUTO_INCREMENT,
  `descripcion` varchar(70) DEFAULT NULL,
  `activo` varchar(2) DEFAULT NULL,
  PRIMARY KEY (`pk_autorizacion`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `avisoconbranza`;
CREATE TABLE `avisoconbranza` (
  `pk_avisoconbranza` int(11) NOT NULL AUTO_INCREMENT,
  `pk_servicio1` int(11) NOT NULL,
  `pk_servicio2` int(11) DEFAULT NULL,
  `pk_servicio3` int(11) DEFAULT NULL,
  `pk_servicio4` int(11) DEFAULT NULL,
  `pk_servicio5` int(11) DEFAULT NULL,
  `servicio1` varchar(100) NOT NULL,
  `servicio2` varchar(100) DEFAULT NULL,
  `servicio3` varchar(100) DEFAULT NULL,
  `servicio4` varchar(100) DEFAULT NULL,
  `servicio5` varchar(100) DEFAULT NULL,
  `mes1` int(11) DEFAULT '0',
  `mes2` int(11) DEFAULT '0',
  `mes3` int(11) DEFAULT '0',
  `mes4` int(11) DEFAULT '0',
  `mes5` int(11) DEFAULT '0',
  `anio1` int(11) DEFAULT '0',
  `anio2` int(11) DEFAULT '0',
  `anio3` int(11) DEFAULT '0',
  `anio4` int(11) DEFAULT '0',
  `anio5` int(11) DEFAULT '0',
  PRIMARY KEY (`pk_avisoconbranza`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

DROP TABLE IF EXISTS `banco`;
CREATE TABLE `banco` (
  `pk_banco` int(11) NOT NULL AUTO_INCREMENT,
  `descripcion` varchar(100) DEFAULT NULL,
  `fk_cuenta` int(11) DEFAULT NULL,
  `numero` varchar(20) DEFAULT NULL,
  `cuenta` varchar(100) DEFAULT NULL,
  `activo` varchar(2) DEFAULT NULL,
  `cuentabanco` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`pk_banco`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

DROP TABLE IF EXISTS `blog`;
CREATE TABLE `blog` (
  `pk_blog` int(11) NOT NULL AUTO_INCREMENT,
  `fecha` datetime NOT NULL,
  `descripcion` varchar(300) DEFAULT NULL,
  `titulo` varchar(150) NOT NULL,
  `imagen` varchar(100) DEFAULT NULL,
  `archivo` varchar(100) DEFAULT NULL,
  `activo` int(11) DEFAULT NULL,
  PRIMARY KEY (`pk_blog`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `condominio`;
CREATE TABLE `condominio` (
  `pk_condominio` int(11) NOT NULL AUTO_INCREMENT,
  `codigo` varchar(15) NOT NULL,
  `nombre` varchar(50) NOT NULL,
  `administrador` int(11) DEFAULT NULL,
  `m2` decimal(10,2) DEFAULT NULL,
  `tipo` varchar(6) DEFAULT NULL,
  `vh` varchar(1) DEFAULT NULL,
  `diacobro` varchar(70) DEFAULT NULL,
  PRIMARY KEY (`pk_condominio`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `confaviso`;
CREATE TABLE `confaviso` (
  `pk_confaviso` int(11) NOT NULL AUTO_INCREMENT,
  `fechaemision` datetime DEFAULT NULL,
  `header` varchar(300) DEFAULT NULL,
  `fechavencimiento` datetime DEFAULT NULL,
  `condominio` varchar(100) DEFAULT NULL,
  `footer` varchar(300) DEFAULT NULL,
  PRIMARY KEY (`pk_confaviso`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

DROP TABLE IF EXISTS `confejecucion`;
CREATE TABLE `confejecucion` (
  `pk_confejecucion` int(11) NOT NULL AUTO_INCREMENT,
  `descripcion` varchar(100) DEFAULT NULL,
  `tipo` varchar(10) DEFAULT NULL,
  `activo` varchar(2) DEFAULT NULL,
  PRIMARY KEY (`pk_confejecucion`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

DROP TABLE IF EXISTS `confejecuciontitulo`;
CREATE TABLE `confejecuciontitulo` (
  `pk_confejecuciontitulo` int(11) NOT NULL AUTO_INCREMENT,
  `fk_confejecucion` int(11) DEFAULT NULL,
  `descripcion` varchar(100) NOT NULL,
  `activo` varchar(2) NOT NULL,
  PRIMARY KEY (`pk_confejecuciontitulo`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

DROP TABLE IF EXISTS `configuracionevento`;
CREATE TABLE `configuracionevento` (
  `id` int(11) NOT NULL DEFAULT '1',
  `PrecioChurrasquera` decimal(12,2) DEFAULT NULL COMMENT '100',
  `PrecioSalon` decimal(12,2) DEFAULT NULL COMMENT '200',
  `PrecioSalonChurrasquera` decimal(12,2) DEFAULT NULL COMMENT '300',
  `MontoGarantia` decimal(12,2) DEFAULT NULL,
  `NumeroCuenta` decimal(12,2) DEFAULT NULL,
  `Banco` varchar(200) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `contrato`;
CREATE TABLE `contrato` (
  `pk_contrato` int(11) NOT NULL AUTO_INCREMENT,
  `fk_propiedad` int(11) DEFAULT NULL,
  `fecha` datetime NOT NULL,
  `fechaini` datetime DEFAULT NULL,
  `fechafin` datetime DEFAULT NULL,
  `fechaingreso` datetime DEFAULT NULL,
  `expensa` decimal(10,2) NOT NULL,
  `valido` varchar(2) DEFAULT NULL,
  `fechabaja` datetime DEFAULT NULL,
  `motivo` varchar(30) DEFAULT NULL,
  `fk_usuarioinsert` int(11) DEFAULT NULL,
  `fk_usuariodelete` int(11) DEFAULT NULL,
  `diaini` int(11) DEFAULT NULL,
  `diafin` int(11) DEFAULT NULL,
  `fk_condominio` int(11) DEFAULT NULL,
  PRIMARY KEY (`pk_contrato`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `correo`;
CREATE TABLE `correo` (
  `pk_correo` int(11) NOT NULL AUTO_INCREMENT,
  `tipo` varchar(20) NOT NULL,
  `email` varchar(50) NOT NULL,
  `fk_persona` int(11) DEFAULT NULL,
  PRIMARY KEY (`pk_correo`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `cuenta`;
CREATE TABLE `cuenta` (
  `pk_cuenta` int(11) NOT NULL AUTO_INCREMENT,
  `numero` int(11) DEFAULT NULL,
  `descripcion` varchar(100) COLLATE utf8_unicode_ci DEFAULT NULL,
  `nivel` int(11) DEFAULT NULL,
  `fk_cuenta` int(11) DEFAULT NULL,
  `nivel1` int(11) DEFAULT NULL,
  `nivel2` int(11) DEFAULT NULL,
  `nivel3` int(11) DEFAULT NULL,
  `nivel4` int(11) DEFAULT NULL,
  `nivel5` int(11) DEFAULT NULL,
  PRIMARY KEY (`pk_cuenta`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

DROP TABLE IF EXISTS `cuentasistema`;
CREATE TABLE `cuentasistema` (
  `pk_cuentasistema` int(11) NOT NULL,
  `descripcion` varchar(70) DEFAULT NULL,
  `pk_cuenta` int(11) NOT NULL,
  `cuenta` varchar(70) NOT NULL,
  `opcion` varchar(70) DEFAULT NULL,
  PRIMARY KEY (`pk_cuentasistema`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `cuota`;
CREATE TABLE `cuota` (
  `pk_cuota` int(11) NOT NULL AUTO_INCREMENT,
  `fk_deuda` int(11) DEFAULT NULL,
  `fk_persona` int(11) DEFAULT NULL,
  `fecha` datetime DEFAULT NULL,
  `monto` decimal(10,2) DEFAULT NULL,
  `formapago` varchar(15) DEFAULT NULL,
  `banco` varchar(15) DEFAULT NULL,
  `cuenta` varchar(15) DEFAULT NULL,
  `tipocambio` decimal(4,2) DEFAULT NULL,
  `fechabaja` datetime DEFAULT NULL,
  `motivo` varchar(150) DEFAULT NULL,
  `fk_usuarioalta` int(11) DEFAULT NULL,
  `fk_usuariobaja` int(11) DEFAULT NULL,
  `activo` varchar(2) DEFAULT NULL,
  `recibogeneral` int(11) DEFAULT NULL,
  `recibopersonal` int(11) DEFAULT NULL,
  `fk_banco` int(11) DEFAULT NULL,
  `fk_formapago` int(11) DEFAULT NULL,
  `fechadeposito` datetime DEFAULT NULL,
  `cuentadebe` int(11) DEFAULT NULL,
  `cuentahaber` int(11) DEFAULT NULL,
  `fk_moneda` int(11) DEFAULT NULL,
  `fk_opcion_pago` int(11) DEFAULT NULL,
  `ano` int(11) DEFAULT NULL,
  `mes` int(11) DEFAULT NULL,
  `fk_servicio` int(11) DEFAULT NULL,
  `fk_contrato` int(11) DEFAULT NULL,
  `deuda` decimal(10,2) DEFAULT NULL,
  `deuda_estado` varchar(5) DEFAULT NULL,
  `deuda_activo` varchar(5) DEFAULT NULL,
  `nombre` varchar(300) DEFAULT NULL,
  PRIMARY KEY (`pk_cuota`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `detalleasiento`;
CREATE TABLE `detalleasiento` (
  `pk_detalleasiento` int(11) NOT NULL AUTO_INCREMENT,
  `fk_cuenta` int(11) DEFAULT NULL,
  `fk_asiento` int(11) DEFAULT NULL,
  `concepto` varchar(500) COLLATE utf8_unicode_ci DEFAULT NULL,
  `debe` decimal(14,4) DEFAULT NULL,
  `haber` decimal(14,4) DEFAULT NULL,
  `glosa` varchar(500) COLLATE utf8_unicode_ci DEFAULT NULL,
  `activo` varchar(2) COLLATE utf8_unicode_ci DEFAULT NULL,
  PRIMARY KEY (`pk_detalleasiento`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

DROP TABLE IF EXISTS `detalleconfejecucion`;
CREATE TABLE `detalleconfejecucion` (
  `fk_confejecuciontitulo` int(11) NOT NULL,
  `fk_confejecucion` int(11) NOT NULL,
  `pk_cuenta` int(11) NOT NULL,
  `cuenta` varchar(30) DEFAULT NULL,
  `descripcion` varchar(100) DEFAULT NULL,
  `nivel1` int(11) DEFAULT NULL,
  `descripcionejecucion` varchar(100) DEFAULT NULL,
  `tipo` int(11) DEFAULT NULL,
  PRIMARY KEY (`fk_confejecucion`,`pk_cuenta`,`fk_confejecuciontitulo`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

DROP TABLE IF EXISTS `deuda`;
CREATE TABLE `deuda` (
  `pk_deuda` int(11) NOT NULL AUTO_INCREMENT,
  `fk_servicio` int(11) DEFAULT NULL,
  `fk_contrato` int(11) DEFAULT NULL,
  `fecha` datetime DEFAULT NULL,
  `fechadeuda` datetime DEFAULT NULL,
  `monto` decimal(10,2) DEFAULT NULL,
  `pagado` varchar(2) DEFAULT NULL,
  `estado` varchar(1) DEFAULT NULL,
  `fechabaja` datetime DEFAULT NULL,
  `motivobaja` varchar(30) DEFAULT NULL,
  `fk_usuarioalta` int(11) DEFAULT NULL,
  `fk_usuariobaja` int(11) DEFAULT NULL,
  `activo` varchar(1) DEFAULT NULL,
  `nombre` varchar(300) DEFAULT NULL,
  `ano` int(11) DEFAULT NULL,
  `mes` int(11) DEFAULT NULL,
  PRIMARY KEY (`pk_deuda`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `egreso`;
CREATE TABLE `egreso` (
  `pk_egreso` int(11) NOT NULL AUTO_INCREMENT,
  `numerorecibo` int(11) NOT NULL,
  `concepto` varchar(45) DEFAULT NULL,
  `glosa` varchar(500) DEFAULT NULL,
  `monto` decimal(12,2) NOT NULL,
  `fechacreacion` datetime DEFAULT NULL,
  `fecha` datetime NOT NULL,
  `fk_usuario` int(11) DEFAULT NULL,
  `activo` varchar(2) DEFAULT NULL,
  `fk_usuariobaja` int(11) DEFAULT NULL,
  `motivobaja` varchar(50) DEFAULT NULL,
  `fechabaja` datetime DEFAULT NULL,
  `cuentadebe` int(11) DEFAULT NULL,
  `cuentahaber` int(11) DEFAULT NULL,
  `fk_formpago` int(11) DEFAULT NULL,
  `numerotrans` varchar(45) DEFAULT NULL,
  `fk_proveedor` int(11) DEFAULT NULL,
  `fk_persona` int(11) DEFAULT NULL,
  `fk_autorizacion` int(11) DEFAULT NULL,
  `fk_banco` int(11) DEFAULT NULL,
  `cuenta` varchar(45) DEFAULT NULL,
  `nota` varchar(100) DEFAULT NULL,
  `fechacobro` datetime DEFAULT NULL,
  `fechaconfirmado` datetime DEFAULT NULL,
  `cobrado` varchar(2) DEFAULT NULL,
  PRIMARY KEY (`pk_egreso`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `empresa`;
CREATE TABLE `empresa` (
  `pk_empresa` int(11) NOT NULL AUTO_INCREMENT,
  `empresa` varchar(40) NOT NULL,
  `telefono` varchar(40) DEFAULT NULL,
  `direccion` varchar(40) DEFAULT NULL,
  `facebook` varchar(45) DEFAULT NULL,
  `twiter` varchar(45) DEFAULT NULL,
  `habilitado` varchar(2) DEFAULT NULL,
  PRIMARY KEY (`pk_empresa`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `evento`;
CREATE TABLE `evento` (
  `pk_evento` int(11) NOT NULL AUTO_INCREMENT,
  `fecha` datetime DEFAULT NULL,
  `anio` int(11) DEFAULT NULL,
  `mes` int(11) DEFAULT NULL,
  `dia` int(11) DEFAULT NULL,
  `horainicio` time DEFAULT NULL,
  `horafin` time DEFAULT NULL,
  `fk_contrato` int(11) DEFAULT NULL,
  `fk_propietario` int(11) DEFAULT NULL,
  `fk_persona` int(11) DEFAULT NULL,
  `disponible` varchar(100) DEFAULT NULL,
  `numero` int(11) DEFAULT '1',
  `colorChurrasquera` varchar(60) DEFAULT NULL,
  `checkChurrasquera` varchar(2) DEFAULT NULL,
  `checkSalon` varchar(2) DEFAULT NULL,
  `MotivoEvento` varchar(300) DEFAULT NULL,
  `NumeroInvitados` varchar(45) DEFAULT NULL,
  `CelularContacto` varchar(60) DEFAULT NULL,
  `Amenizado` varchar(45) DEFAULT NULL,
  `PropietarioInquilino` varchar(300) DEFAULT 'PROPIETARIO',
  PRIMARY KEY (`pk_evento`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `formpago`;
CREATE TABLE `formpago` (
  `pk_formpago` int(11) NOT NULL AUTO_INCREMENT,
  `descripcion` varchar(100) DEFAULT NULL,
  `fk_cuenta` int(11) DEFAULT NULL,
  `numero` varchar(20) DEFAULT NULL,
  `cuenta` varchar(100) DEFAULT NULL,
  `activo` varchar(2) DEFAULT NULL,
  PRIMARY KEY (`pk_formpago`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

DROP TABLE IF EXISTS `manzano`;
CREATE TABLE `manzano` (
  `pk_manzano` int(11) NOT NULL AUTO_INCREMENT,
  `codigo` varchar(15) NOT NULL,
  `nombre` varchar(20) DEFAULT NULL,
  `fk_condominio` int(11) DEFAULT NULL,
  PRIMARY KEY (`pk_manzano`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `moneda`;
CREATE TABLE `moneda` (
  `pk_moneda` int(11) NOT NULL AUTO_INCREMENT,
  `moneda` varchar(15) NOT NULL,
  `tipocambio` decimal(4,2) NOT NULL,
  `abreviado` varchar(3) NOT NULL,
  PRIMARY KEY (`pk_moneda`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `opcioncobro`;
CREATE TABLE `opcioncobro` (
  `pk_opcioncobro` int(11) NOT NULL,
  `descripcion` varchar(50) NOT NULL,
  PRIMARY KEY (`pk_opcioncobro`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `permiso`;
CREATE TABLE `permiso` (
  `pk_permiso` int(11) NOT NULL AUTO_INCREMENT,
  `descripcion` varchar(40) DEFAULT NULL,
  PRIMARY KEY (`pk_permiso`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `persona`;
CREATE TABLE `persona` (
  `pk_persona` int(11) NOT NULL AUTO_INCREMENT,
  `nombre` varchar(50) DEFAULT NULL,
  `ci` varchar(10) DEFAULT NULL,
  `sexo` varchar(1) DEFAULT NULL,
  `fechanac` date DEFAULT NULL,
  `estadocivil` varchar(10) DEFAULT NULL,
  `direccion` varchar(50) DEFAULT NULL,
  `relacion` varchar(20) DEFAULT NULL,
  `activo` varchar(1) DEFAULT NULL,
  `fk_persona` int(11) DEFAULT NULL,
  `telefono` varchar(15) DEFAULT NULL,
  `celular` varchar(15) DEFAULT NULL,
  `email` varchar(50) DEFAULT NULL,
  PRIMARY KEY (`pk_persona`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `persona_contrato`;
CREATE TABLE `persona_contrato` (
  `fk_persona` int(11) NOT NULL,
  `fk_contrato` int(11) NOT NULL,
  `tipo` varchar(15) NOT NULL,
  `fecharegistro` datetime NOT NULL,
  `fecharetiro` datetime DEFAULT NULL,
  `valido` varchar(2) NOT NULL,
  `activo` varchar(1) NOT NULL,
  `fk_usuarioinsert` int(11) DEFAULT NULL,
  `fk_usuariodelete` int(11) DEFAULT NULL,
  PRIMARY KEY (`fk_persona`,`fk_contrato`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `propiedad`;
CREATE TABLE `propiedad` (
  `pk_propiedad` int(11) NOT NULL AUTO_INCREMENT,
  `codigo` varchar(15) NOT NULL,
  `nombre` varchar(30) DEFAULT NULL,
  `m2` decimal(10,2) DEFAULT NULL,
  `expensa` decimal(10,2) NOT NULL,
  `tipo` varchar(15) DEFAULT NULL,
  `activo` bit(1) DEFAULT NULL,
  `fk_manzano` int(11) DEFAULT NULL,
  `fk_condominio` int(11) DEFAULT NULL,
  PRIMARY KEY (`pk_propiedad`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `proveedor`;
CREATE TABLE `proveedor` (
  `pk_proveedor` int(11) NOT NULL AUTO_INCREMENT,
  `nombre` varchar(70) DEFAULT NULL,
  `nit` varchar(20) DEFAULT NULL,
  `telefono` varchar(15) DEFAULT NULL,
  `celular` varchar(15) DEFAULT NULL,
  `direccion` varchar(100) DEFAULT NULL,
  `email` varchar(50) DEFAULT NULL,
  `activo` varchar(2) DEFAULT NULL,
  PRIMARY KEY (`pk_proveedor`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `reciboegreso`;
CREATE TABLE `reciboegreso` (
  `pk_reciboegreso` int(11) NOT NULL,
  `fk_condominio` int(11) NOT NULL,
  PRIMARY KEY (`pk_reciboegreso`,`fk_condominio`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `recibogeneral`;
CREATE TABLE `recibogeneral` (
  `pk_recibogeneral` int(11) NOT NULL,
  `fk_condominio` int(11) NOT NULL,
  PRIMARY KEY (`pk_recibogeneral`,`fk_condominio`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `recibopersonal`;
CREATE TABLE `recibopersonal` (
  `pk_recibopersonal` int(11) NOT NULL,
  `fk_contrato` int(11) NOT NULL,
  PRIMARY KEY (`pk_recibopersonal`,`fk_contrato`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `serviciocuenta`;
CREATE TABLE `serviciocuenta` (
  `pk_serviciocuenta` int(11) NOT NULL,
  `fk_servicio` int(11) NOT NULL,
  `descripcion` varchar(50) DEFAULT NULL,
  `pk_cuenta` int(11) DEFAULT NULL,
  `cuenta` varchar(50) DEFAULT NULL,
  PRIMARY KEY (`pk_serviciocuenta`,`fk_servicio`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

DROP TABLE IF EXISTS `serviciopago`;
CREATE TABLE `serviciopago` (
  `pk_serviciopago` int(11) NOT NULL AUTO_INCREMENT,
  `nombre` varchar(45) NOT NULL,
  `costo` decimal(10,2) NOT NULL,
  `estado` varchar(2) DEFAULT NULL,
  `activo` varchar(1) DEFAULT NULL,
  PRIMARY KEY (`pk_serviciopago`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `servicio_contrato`;
CREATE TABLE `servicio_contrato` (
  `pk_servicio_contrato` int(11) NOT NULL AUTO_INCREMENT,
  `costo` decimal(10,2) DEFAULT NULL,
  `activo` varchar(1) DEFAULT NULL,
  `fk_contrato` int(11) NOT NULL,
  `fk_serviciopago` int(11) NOT NULL,
  PRIMARY KEY (`pk_servicio_contrato`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `tabla_servicio`;
CREATE TABLE `tabla_servicio` (
  `pk_contrato` int(11) NOT NULL,
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

DROP TABLE IF EXISTS `TbReporteFinal`;
CREATE TABLE `TbReporteFinal` (
  `Propiedad` varchar(15) CHARACTER SET latin1 DEFAULT NULL,
  `Propietario` varchar(100) CHARACTER SET utf8 DEFAULT NULL,
  `Fecha` varchar(15) CHARACTER SET utf8 DEFAULT NULL,
  `NumeroDeuda` int(11) DEFAULT NULL,
  `Expensa` decimal(10,2) DEFAULT NULL,
  `pk_contrato` int(11) NOT NULL DEFAULT '0',
  `EXPENSA DE ABRIL DEL2023` decimal(33,2) DEFAULT NULL,
  `EXPENSA DE ABRIL DEL2024` decimal(33,2) DEFAULT NULL,
  `EXPENSA DE ABRIL DEL2025` decimal(33,2) DEFAULT NULL,
  `EXPENSA DE AGOSTO DEL2023` decimal(33,2) DEFAULT NULL,
  `EXPENSA DE AGOSTO DEL2024` decimal(33,2) DEFAULT NULL,
  `EXPENSA DE AGOSTO DEL2025` decimal(33,2) DEFAULT NULL,
  `EXPENSA DE DICIEMBRE DEL2022` decimal(33,2) DEFAULT NULL,
  `EXPENSA DE DICIEMBRE DEL2023` decimal(33,2) DEFAULT NULL,
  `EXPENSA DE DICIEMBRE DEL2024` decimal(33,2) DEFAULT NULL,
  `EXPENSA DE ENERO DEL2023` decimal(33,2) DEFAULT NULL,
  `EXPENSA DE ENERO DEL2024` decimal(33,2) DEFAULT NULL,
  `EXPENSA DE ENERO DEL2025` decimal(33,2) DEFAULT NULL,
  `EXPENSA DE FEBRERO DEL2023` decimal(33,2) DEFAULT NULL,
  `EXPENSA DE FEBRERO DEL2024` decimal(33,2) DEFAULT NULL,
  `EXPENSA DE FEBRERO DEL2025` decimal(33,2) DEFAULT NULL,
  `EXPENSA DE JULIO DEL2023` decimal(33,2) DEFAULT NULL,
  `EXPENSA DE JULIO DEL2024` decimal(33,2) DEFAULT NULL,
  `EXPENSA DE JULIO DEL2025` decimal(33,2) DEFAULT NULL,
  `EXPENSA DE JUNIO DEL2023` decimal(33,2) DEFAULT NULL,
  `EXPENSA DE JUNIO DEL2024` decimal(33,2) DEFAULT NULL,
  `EXPENSA DE JUNIO DEL2025` decimal(33,2) DEFAULT NULL,
  `EXPENSA DE MARZO DEL2023` decimal(33,2) DEFAULT NULL,
  `EXPENSA DE MARZO DEL2024` decimal(33,2) DEFAULT NULL,
  `EXPENSA DE MARZO DEL2025` decimal(33,2) DEFAULT NULL,
  `EXPENSA DE MARZO DEL2026` decimal(33,2) DEFAULT NULL,
  `EXPENSA DE MAYO DEL2023` decimal(33,2) DEFAULT NULL,
  `EXPENSA DE MAYO DEL2024` decimal(33,2) DEFAULT NULL,
  `EXPENSA DE MAYO DEL2025` decimal(33,2) DEFAULT NULL,
  `EXPENSA DE NOVIEMBRE DEL2022` decimal(33,2) DEFAULT NULL,
  `EXPENSA DE NOVIEMBRE DEL2023` decimal(33,2) DEFAULT NULL,
  `EXPENSA DE NOVIEMBRE DEL2024` decimal(33,2) DEFAULT NULL,
  `EXPENSA DE NOVIEMBRE DEL2025` decimal(33,2) DEFAULT NULL,
  `EXPENSA DE OCTUBRE DEL2022` decimal(33,2) DEFAULT NULL,
  `EXPENSA DE OCTUBRE DEL2023` decimal(33,2) DEFAULT NULL,
  `EXPENSA DE OCTUBRE DEL2024` decimal(33,2) DEFAULT NULL,
  `EXPENSA DE OCTUBRE DEL2025` decimal(33,2) DEFAULT NULL,
  `EXPENSA DE SEPTIEMBRE DEL2022` decimal(33,2) DEFAULT NULL,
  `EXPENSA DE SEPTIEMBRE DEL2023` decimal(33,2) DEFAULT NULL,
  `EXPENSA DE SEPTIEMBRE DEL2024` decimal(33,2) DEFAULT NULL,
  `EXPENSA DE SEPTIEMBRE DEL2025` decimal(33,2) DEFAULT NULL,
  `MULTA INCUMPLIMIENTO DE NORMAS DE ABRIL DEL2025` decimal(33,2) DEFAULT NULL,
  `MULTA INCUMPLIMIENTO DE NORMAS DE JULIO DEL2025` decimal(33,2) DEFAULT NULL,
  `MULTA INCUMPLIMIENTO DE NORMAS DE JUNIO DEL2025` decimal(33,2) DEFAULT NULL,
  `MULTA MASCOTA DE JULIO DEL2025` decimal(33,2) DEFAULT NULL,
  `Deuda` decimal(54,2) DEFAULT NULL,
  `Pago` decimal(54,2) DEFAULT NULL,
  `Saldo` decimal(55,2) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

DROP TABLE IF EXISTS `tbreportefinal_dup`;
CREATE TABLE `tbreportefinal_dup` (
  `Propiedad` varchar(15) CHARACTER SET latin1 DEFAULT NULL,
  `Propietario` varchar(100) CHARACTER SET utf8 DEFAULT NULL,
  `Fecha` varchar(15) CHARACTER SET utf8 DEFAULT NULL,
  `NumeroDeuda` int(11) DEFAULT NULL,
  `Expensa` decimal(10,2) DEFAULT NULL,
  `pk_contrato` int(11) NOT NULL DEFAULT '0',
  `2da CUOTA EXTRAORDINARIA (GASTOS VARIOS) DE NOVIEMBRE DEL2023` decimal(33,2) DEFAULT NULL,
  `APORTE PARA FONDO DE EMERGENCIA DE ABRIL DEL2024` decimal(33,2) DEFAULT NULL,
  `APORTE PARA FONDO DE EMERGENCIA DE AGOSTO DEL2024` decimal(33,2) DEFAULT NULL,
  `APORTE PARA FONDO DE EMERGENCIA DE DICIEMBRE DEL2023` decimal(33,2) DEFAULT NULL,
  `APORTE PARA FONDO DE EMERGENCIA DE ENERO DEL2024` decimal(33,2) DEFAULT NULL,
  `APORTE PARA FONDO DE EMERGENCIA DE FEBRERO DEL2024` decimal(33,2) DEFAULT NULL,
  `APORTE PARA FONDO DE EMERGENCIA DE JULIO DEL2024` decimal(33,2) DEFAULT NULL,
  `APORTE PARA FONDO DE EMERGENCIA DE JUNIO DEL2024` decimal(33,2) DEFAULT NULL,
  `APORTE PARA FONDO DE EMERGENCIA DE MARZO DEL2024` decimal(33,2) DEFAULT NULL,
  `APORTE PARA FONDO DE EMERGENCIA DE MAYO DEL2024` decimal(33,2) DEFAULT NULL,
  `APORTE PARA FONDO DE EMERGENCIA DE NOVIEMBRE DEL2023` decimal(33,2) DEFAULT NULL,
  `APORTE PARA FONDO DE EMERGENCIA DE NOVIEMBRE DEL2024` decimal(33,2) DEFAULT NULL,
  `APORTE PARA FONDO DE EMERGENCIA DE OCTUBRE DEL2023` decimal(33,2) DEFAULT NULL,
  `APORTE PARA FONDO DE EMERGENCIA DE OCTUBRE DEL2024` decimal(33,2) DEFAULT NULL,
  `APORTE PARA FONDO DE EMERGENCIA DE SEPTIEMBRE DEL2023` decimal(33,2) DEFAULT NULL,
  `APORTE PARA FONDO DE EMERGENCIA DE SEPTIEMBRE DEL2024` decimal(33,2) DEFAULT NULL,
  `CUOTA EXTRAORDINARIA (GASTOS VARIOS) DE OCTUBRE DEL2023` decimal(33,2) DEFAULT NULL,
  `EXPENSA DE ABRIL DEL2024` decimal(33,2) DEFAULT NULL,
  `EXPENSA DE AGOSTO DEL2024` decimal(33,2) DEFAULT NULL,
  `EXPENSA DE JULIO DEL2024` decimal(33,2) DEFAULT NULL,
  `EXPENSA DE NOVIEMBRE DEL2024` decimal(33,2) DEFAULT NULL,
  `EXPENSA DE OCTUBRE DEL2024` decimal(33,2) DEFAULT NULL,
  `EXPENSA DE SEPTIEMBRE DEL2024` decimal(33,2) DEFAULT NULL,
  `PAGO DE AGUA DE ABRIL DEL2024` decimal(33,2) DEFAULT NULL,
  `PAGO DE AGUA DE AGOSTO DEL2024` decimal(33,2) DEFAULT NULL,
  `PAGO DE AGUA DE JULIO DEL2024` decimal(33,2) DEFAULT NULL,
  `PAGO DE AGUA DE JUNIO DEL2023` decimal(33,2) DEFAULT NULL,
  `PAGO DE AGUA DE JUNIO DEL2024` decimal(33,2) DEFAULT NULL,
  `PAGO DE AGUA DE MAYO DEL2023` decimal(33,2) DEFAULT NULL,
  `PAGO DE AGUA DE MAYO DEL2024` decimal(33,2) DEFAULT NULL,
  `PAGO DE AGUA DE NOVIEMBRE DEL2024` decimal(33,2) DEFAULT NULL,
  `PAGO DE AGUA DE OCTUBRE DEL2023` decimal(33,2) DEFAULT NULL,
  `PAGO DE AGUA DE OCTUBRE DEL2024` decimal(33,2) DEFAULT NULL,
  `PAGO DE AGUA DE SEPTIEMBRE DEL2024` decimal(33,2) DEFAULT NULL,
  `Deuda` decimal(54,2) DEFAULT NULL,
  `Pago` decimal(54,2) DEFAULT NULL,
  `Saldo` decimal(55,2) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

DROP TABLE IF EXISTS `telefono`;
CREATE TABLE `telefono` (
  `pk_telefono` int(11) NOT NULL AUTO_INCREMENT,
  `tipo` varchar(25) DEFAULT NULL,
  `numero` varchar(10) NOT NULL,
  `estado` varchar(15) DEFAULT NULL,
  `fk_persona` int(11) DEFAULT NULL,
  PRIMARY KEY (`pk_telefono`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `TempContratoServicio`;
CREATE TABLE `TempContratoServicio` (
  `pk_contrato` int(11) NOT NULL DEFAULT '0',
  `fk_propiedad` int(11) DEFAULT NULL,
  `nombre` varchar(300) CHARACTER SET utf8 DEFAULT NULL,
  `fk_servicio` int(11) DEFAULT NULL,
  `ano` int(11) DEFAULT NULL,
  `mes` int(11) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

DROP TABLE IF EXISTS `tempcontratoservicio_dup`;
CREATE TABLE `tempcontratoservicio_dup` (
  `pk_contrato` int(11) NOT NULL DEFAULT '0',
  `fk_propiedad` int(11) DEFAULT NULL,
  `nombre` varchar(300) CHARACTER SET utf8 DEFAULT NULL,
  `fk_servicio` int(11) DEFAULT NULL,
  `ano` int(11) DEFAULT NULL,
  `mes` int(11) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

DROP TABLE IF EXISTS `TempTablaMadre`;
CREATE TABLE `TempTablaMadre` (
  `fk_contrato` int(11) DEFAULT NULL,
  `ano` int(11) DEFAULT NULL,
  `mes` int(11) DEFAULT NULL,
  `fk_servicio` int(11) DEFAULT NULL,
  `deuda` decimal(32,2) DEFAULT NULL,
  `nombre` varchar(300) CHARACTER SET utf8 DEFAULT NULL,
  `pago` decimal(32,2) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

DROP TABLE IF EXISTS `temptablamadre_dup`;
CREATE TABLE `temptablamadre_dup` (
  `fk_contrato` int(11) DEFAULT NULL,
  `ano` int(11) DEFAULT NULL,
  `mes` int(11) DEFAULT NULL,
  `fk_servicio` int(11) DEFAULT NULL,
  `deuda` decimal(32,2) DEFAULT NULL,
  `nombre` varchar(300) CHARACTER SET utf8 DEFAULT NULL,
  `pago` decimal(32,2) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

DROP TABLE IF EXISTS `TempTotalTable`;
CREATE TABLE `TempTotalTable` (
  `pk_contrato` int(11) NOT NULL DEFAULT '0',
  `propiedad` varchar(15) CHARACTER SET latin1 DEFAULT NULL,
  `propietario` varchar(100) CHARACTER SET utf8 DEFAULT NULL,
  `Fecha` varchar(15) CHARACTER SET utf8 DEFAULT NULL,
  `expensa` decimal(10,2) DEFAULT NULL,
  `numerodeuda` int(11) DEFAULT NULL,
  `deuda` decimal(54,2) DEFAULT NULL,
  `pago` decimal(54,2) DEFAULT NULL,
  `saldo` decimal(55,2) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

DROP TABLE IF EXISTS `temptotaltable_dup`;
CREATE TABLE `temptotaltable_dup` (
  `pk_contrato` int(11) NOT NULL DEFAULT '0',
  `propiedad` varchar(15) CHARACTER SET latin1 DEFAULT NULL,
  `propietario` varchar(100) CHARACTER SET utf8 DEFAULT NULL,
  `Fecha` varchar(15) CHARACTER SET utf8 DEFAULT NULL,
  `expensa` decimal(10,2) DEFAULT NULL,
  `numerodeuda` int(11) DEFAULT NULL,
  `deuda` decimal(54,2) DEFAULT NULL,
  `pago` decimal(54,2) DEFAULT NULL,
  `saldo` decimal(55,2) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

DROP TABLE IF EXISTS `temResultado`;
CREATE TABLE `temResultado` (
  `pk_contrato` int(11) NOT NULL DEFAULT '0',
  `nombre` varchar(300) CHARACTER SET utf8 DEFAULT NULL,
  `saldo` decimal(33,2) DEFAULT NULL,
  `fk_servicio` int(11) DEFAULT NULL,
  `ano` int(11) DEFAULT NULL,
  `mes` int(11) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

DROP TABLE IF EXISTS `temresultado_dup`;
CREATE TABLE `temresultado_dup` (
  `pk_contrato` int(11) NOT NULL DEFAULT '0',
  `nombre` varchar(300) CHARACTER SET utf8 DEFAULT NULL,
  `saldo` decimal(33,2) DEFAULT NULL,
  `fk_servicio` int(11) DEFAULT NULL,
  `ano` int(11) DEFAULT NULL,
  `mes` int(11) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

DROP TABLE IF EXISTS `tipousuario`;
CREATE TABLE `tipousuario` (
  `pk_tipousuario` int(11) NOT NULL AUTO_INCREMENT,
  `descripcion` varchar(40) DEFAULT NULL,
  PRIMARY KEY (`pk_tipousuario`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `tipo_permiso`;
CREATE TABLE `tipo_permiso` (
  `fk_tipousuario` int(11) NOT NULL,
  `fk_permiso` int(11) NOT NULL,
  PRIMARY KEY (`fk_tipousuario`,`fk_permiso`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `ttcuota`;
CREATE TABLE `ttcuota` (
  `pk_cuota` int(11) NOT NULL,
  `cuota` decimal(12,2) DEFAULT NULL,
  `fk_deuda` int(11) DEFAULT NULL,
  PRIMARY KEY (`pk_cuota`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

DROP TABLE IF EXISTS `ttdeuda`;
CREATE TABLE `ttdeuda` (
  `pk_deuda` int(11) NOT NULL,
  `monto` decimal(12,2) DEFAULT NULL,
  PRIMARY KEY (`pk_deuda`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

DROP TABLE IF EXISTS `usuario`;
CREATE TABLE `usuario` (
  `pk_usuario` int(11) NOT NULL AUTO_INCREMENT,
  `fk_persona` int(11) NOT NULL,
  `fecha` datetime DEFAULT NULL,
  `usuario` varchar(10) DEFAULT NULL,
  `contrasena` varchar(10) DEFAULT NULL,
  `habilitado` varchar(2) DEFAULT NULL,
  `activo` varchar(1) DEFAULT NULL,
  `nombrepersona` varchar(100) DEFAULT NULL,
  `fk_tipousuario` int(11) DEFAULT NULL,
  PRIMARY KEY (`pk_usuario`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

SET FOREIGN_KEY_CHECKS=1;
COMMIT;
