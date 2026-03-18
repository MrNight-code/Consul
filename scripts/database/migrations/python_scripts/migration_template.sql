-- 0. Script de Migración Automática
-- Generado por: Python Script
-- Origen: {{SOURCE_DB}}
-- Destino: {{TARGET_DB}}

USE {{TARGET_DB}};

-- 1. DESACTIVAR SEGURIDAD PARA LIMPIEZA
SET FOREIGN_KEY_CHECKS = 0;
SET SQL_SAFE_UPDATES = 0;

-- 2. LIMPIEZA PROFUNDA (RESET)
-- Borramos todo para asegurar que no haya conflictos de IDs
TRUNCATE TABLE comunicado_blog;
TRUNCATE TABLE reserva;
TRUNCATE TABLE recurso_comun;
TRUNCATE TABLE asiento_detalle;
TRUNCATE TABLE asiento_contable;
TRUNCATE TABLE egreso;
TRUNCATE TABLE transaccion_pago;
TRUNCATE TABLE deuda_detalle;
TRUNCATE TABLE deuda_cabecera;
TRUNCATE TABLE lectura_servicio;
TRUNCATE TABLE contrato_servicio_suscrito;
TRUNCATE TABLE contrato_participante;
TRUNCATE TABLE contrato;
TRUNCATE TABLE propiedad;
TRUNCATE TABLE manzano;
TRUNCATE TABLE config_aviso_cobranza;
TRUNCATE TABLE condominio;
TRUNCATE TABLE autorizacion_gasto;
TRUNCATE TABLE usuario;
TRUNCATE TABLE rol_permiso;
TRUNCATE TABLE permiso;
TRUNCATE TABLE medio_contacto;
TRUNCATE TABLE persona;
TRUNCATE TABLE proveedor;
TRUNCATE TABLE forma_pago;
TRUNCATE TABLE banco;
TRUNCATE TABLE plan_cuentas; 
TRUNCATE TABLE catalogo_servicio;
TRUNCATE TABLE rol;

-- ==============================================================================
-- 3. MIGRACIÓN DE DATOS
-- ==============================================================================

-- ------------------------------------------------------------------------------
-- A. CONFIGURACIÓN BÁSICA Y CATÁLOGOS
-- ------------------------------------------------------------------------------
INSERT INTO rol (id_rol, nombre) 
SELECT pk_tipousuario, descripcion FROM {{SOURCE_DB}}.tipousuario;

-- Bancos
INSERT INTO banco (id_banco, nombre_entidad, numero_cuenta, moneda, Tipo, activo)
SELECT pk_banco, descripcion, numero, 'BOB', 'Cuenta Corriente', IF(activo='SI', 1, 0) 
FROM {{SOURCE_DB}}.banco;

-- Formas de Pago
INSERT INTO forma_pago (id_forma_pago, descripcion)
SELECT pk_formpago, descripcion FROM {{SOURCE_DB}}.formpago;

-- Servicios (Agua, Expensa, etc.)
INSERT INTO catalogo_servicio (id_servicio, nombre, costo_base, activo)
SELECT pk_serviciopago, nombre, costo, IF(activo='S', 1, 0) 
FROM {{SOURCE_DB}}.serviciopago;

-- Proveedores
INSERT INTO proveedor (id_proveedor, razon_social, nit, contacto, direccion, activo)
SELECT pk_proveedor, nombre, nit, CONCAT(telefono, ' / ', celular), direccion, IF(activo='SI', 1, 0)
FROM {{SOURCE_DB}}.proveedor;

-- Autorizaciones
INSERT INTO autorizacion_gasto (id_autorizacion, descripcion, activo)
SELECT pk_autorizacion, descripcion, IF(activo='SI', 1, 0)
FROM {{SOURCE_DB}}.autorizacion;

-- ------------------------------------------------------------------------------
-- B. PERSONAS Y CONTACTOS
-- ------------------------------------------------------------------------------

-- 1. Personas base
INSERT INTO persona (id_persona, nombre_completo, ci, fecha_nacimiento, sexo, estado_civil, es_activo)
SELECT pk_persona, nombre, ci, fechanac, sexo, estadocivil, IF(activo='S', 1, 0)
FROM {{SOURCE_DB}}.persona;

-- 2. Contactos desde la tabla 'persona' (Celular, Telefono, Email)
INSERT INTO medio_contacto (id_persona, tipo, valor, es_principal)
SELECT pk_persona, 'Celular', celular, 1 FROM {{SOURCE_DB}}.persona WHERE celular IS NOT NULL AND celular != ''
UNION
SELECT pk_persona, 'Telefono', telefono, 0 FROM {{SOURCE_DB}}.persona WHERE telefono IS NOT NULL AND telefono != ''
UNION
SELECT pk_persona, 'Email', email, 0 FROM {{SOURCE_DB}}.persona WHERE email IS NOT NULL AND email != '';

-- 3. Contactos desde la tabla externa 'telefono'
INSERT INTO medio_contacto (id_persona, tipo, valor, es_principal)
SELECT fk_persona, IFNULL(tipo, 'Telefono'), numero, 0
FROM {{SOURCE_DB}}.telefono 
WHERE fk_persona IS NOT NULL;

-- 4. Contactos desde la tabla externa 'correo'
INSERT INTO medio_contacto (id_persona, tipo, valor, es_principal)
SELECT fk_persona, IFNULL(tipo, 'Email'), email, 0
FROM {{SOURCE_DB}}.correo 
WHERE fk_persona IS NOT NULL;

-- ------------------------------------------------------------------------------
-- C. USUARIOS
-- ------------------------------------------------------------------------------
INSERT INTO usuario (pk_usuario, id_persona, username, password_hash, esta_habilitado, id_rol_principal)
SELECT pk_usuario, fk_persona, usuario, contrasena, IF(habilitado='SI', 1, 0), fk_tipousuario
FROM {{SOURCE_DB}}.usuario;

-- ------------------------------------------------------------------------------
-- D. INMOBILIARIO
-- ------------------------------------------------------------------------------
INSERT INTO condominio (id_condominio, nombre, codigo, id_admin_persona, superficie_total_m2, config_dia_cobro)
SELECT pk_condominio, nombre, codigo, IFNULL(administrador, 1), m2, diacobro
FROM {{SOURCE_DB}}.condominio;

INSERT INTO manzano (id_manzano, id_condominio, codigo, nombre)
SELECT pk_manzano, IFNULL(fk_condominio, 1), codigo, nombre
FROM {{SOURCE_DB}}.manzano;

INSERT INTO propiedad (id_propiedad, id_manzano, codigo_unidad, nombre_funcional, superficie_m2, expensa_base_defecto, tipo, activo, saldo_deudor, saldo_a_favor)
SELECT pk_propiedad, fk_manzano, codigo, IFNULL(nombre, CONCAT('Unidad ', codigo)), m2, expensa, tipo, 1, 0, 0
FROM {{SOURCE_DB}}.propiedad;

-- ------------------------------------------------------------------------------
-- E. CONTRATOS
-- ------------------------------------------------------------------------------
INSERT INTO contrato (id_contrato, id_propiedad, fecha_firma, fecha_inicio, fecha_fin, fecha_ingreso_real, monto_expensa_pactada, estado, motivo_baja, id_usuario_creador)
SELECT pk_contrato, fk_propiedad, fecha, fechaini, fechafin, fechaingreso, expensa, 
       CASE WHEN valido = 'SI' THEN 'Vigente' ELSE 'Finalizado' END, motivo, fk_usuarioinsert
FROM {{SOURCE_DB}}.contrato;

INSERT INTO contrato_participante (id_contrato, id_persona, rol_contrato, fecha_alta, fecha_baja, activo)
SELECT fk_contrato, fk_persona, tipo, fecharegistro, fecharetiro, IF(activo='S', 1, 0)
FROM {{SOURCE_DB}}.persona_contrato;

INSERT INTO contrato_servicio_suscrito (id_contrato, id_servicio, costo_personalizado, activo)
SELECT fk_contrato, fk_serviciopago, costo, IF(activo='S', 1, 0)
FROM {{SOURCE_DB}}.servicio_contrato;

-- ------------------------------------------------------------------------------
-- F. FINANZAS
-- ------------------------------------------------------------------------------

-- 1. Deudas (Cabecera)
INSERT INTO deuda_cabecera (id_deuda, id_contrato, anio_periodo, mes_periodo, fecha_emision, fecha_vencimiento, total_deuda, estado_pago, id_usuario_generador)
SELECT pk_deuda, fk_contrato, 
       IFNULL(ano, YEAR(fecha)), IFNULL(mes, MONTH(fecha)), 
       fecha, fechadeuda, monto, 
       CASE WHEN pagado = 'SI' THEN 'PAGADO' WHEN estado = 'A' THEN 'ANULADO' ELSE 'PENDIENTE' END, 
       fk_usuarioalta
FROM {{SOURCE_DB}}.deuda;

-- 2. Deudas (Detalle)
INSERT INTO deuda_detalle (id_deuda, id_servicio, concepto, monto_unitario, subtotal)
SELECT pk_deuda, IFNULL(fk_servicio, 1), 
       CONCAT('Cobro del periodo ', IFNULL(mes, MONTH(fecha)), '/', IFNULL(ano, YEAR(fecha))), 
       monto, monto
FROM {{SOURCE_DB}}.deuda;

-- 3. Pagos
INSERT INTO transaccion_pago (id_pago, id_deuda, id_persona_pagador, id_banco_destino, id_forma_pago, fecha_pago, monto_abonado, estado)
SELECT pk_cuota, fk_deuda, 
       IFNULL(fk_persona, (SELECT id_persona FROM contrato_participante WHERE id_contrato = (SELECT id_contrato FROM deuda_cabecera WHERE id_deuda = fk_deuda) LIMIT 1)),
       IFNULL(fk_banco, 1), IFNULL(fk_formapago, 1), 
       fecha, monto, IF(activo='SI', 'CONFIRMADO', 'ANULADO')
FROM {{SOURCE_DB}}.cuota;

-- 4. Egresos
INSERT INTO egreso (id_egreso, id_condominio, concepto, monto_total, fecha_egreso, id_usuario_registro, id_autorizacion, id_banco_origen, id_proveedor, id_persona_beneficiario, id_forma_pago)
SELECT pk_egreso, 1, concepto, monto, fecha, IFNULL(fk_usuario, 1), IFNULL(fk_autorizacion, 1), IFNULL(fk_banco, 1), fk_proveedor, fk_persona, IFNULL(fk_formpago, 1)
FROM {{SOURCE_DB}}.egreso;

-- ------------------------------------------------------------------------------
-- G. EXTRAS Y EVENTOS
-- ------------------------------------------------------------------------------

-- 1. Crear Recursos Comunes
INSERT INTO recurso_comun (id_recurso, id_condominio, nombre, costo_reserva, costo_garantia)
VALUES (1, 1, 'Churrasquera General', 100.00, 50.00),
       (2, 1, 'Salón de Eventos', 200.00, 100.00);

-- 2. Migrar Eventos a Reservas
INSERT INTO reserva (id_reserva, id_recurso, id_contrato, fecha_inicio, fecha_fin, cantidad_invitados, motivo, amenizado_por, estado)
SELECT pk_evento, 
       CASE WHEN checkChurrasquera = 'SI' THEN 1 ELSE 2 END,
       IFNULL(fk_contrato, 1), 
       ADDTIME(fecha, horainicio), 
       ADDTIME(fecha, horafin),    
       CAST(NumeroInvitados AS UNSIGNED), 
       MotivoEvento, Amenizado, 
       'FINALIZADA'
FROM {{SOURCE_DB}}.evento
WHERE fk_contrato IS NOT NULL;

-- 3. Blog
INSERT INTO comunicado_blog (id_blog, id_condominio, fecha_publicacion, titulo, contenido_html, url_imagen, activo)
SELECT pk_blog, 1, fecha, titulo, descripcion, imagen, activo
FROM {{SOURCE_DB}}.blog;

-- ------------------------------------------------------------------------------
-- H. AJUSTES FINALES
-- ------------------------------------------------------------------------------
SET @max_persona = (SELECT IFNULL(MAX(id_persona), 0) + 1 FROM persona);
SET @sql = CONCAT('ALTER TABLE persona AUTO_INCREMENT = ', @max_persona);
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

SET @max_deuda = (SELECT IFNULL(MAX(id_deuda), 0) + 1 FROM deuda_cabecera);
SET @sql = CONCAT('ALTER TABLE deuda_cabecera AUTO_INCREMENT = ', @max_deuda);
PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

-- Crear Admin Default
INSERT INTO persona (id_persona, nombre_completo, ci, estado_civil, es_activo)
VALUES (9999, 'Super Admin', '0000', 'Soltero', 1)
ON DUPLICATE KEY UPDATE nombre_completo = 'Super Admin';

INSERT INTO usuario (pk_usuario, id_persona, username, password_hash, esta_habilitado, id_rol_principal)
VALUES (9999, 9999, 'admin', 'admin123', 1, 1)
ON DUPLICATE KEY UPDATE password_hash='admin123';

SET FOREIGN_KEY_CHECKS = 1;
SET SQL_SAFE_UPDATES = 1;

SELECT 'MIGRACIÓN COMPLETA DESDE {{SOURCE_DB}} A {{TARGET_DB}}' AS Resultado;
