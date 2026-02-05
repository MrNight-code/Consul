Table rol {
  id_rol int [pk, increment]
  nombre varchar
}

Table banco {
  id_banco int [pk, increment]
  nombre_entidad varchar
  numero_cuenta varchar
  moneda varchar
  Tipo varchar
  activo boolean
}

Table forma_pago {
  id_forma_pago int [pk, increment]
  descripcion varchar
}

Table catalogo_servicio {
  id_servicio int [pk, increment]
  nombre varchar
  costo_base decimal
  activo boolean
}

Table proveedor {
  id_proveedor int [pk, increment]
  razon_social varchar
  nit varchar
  contacto varchar
  direccion varchar
  activo boolean
}

Table autorizacion_gasto {
  id_autorizacion int [pk, increment]
  descripcion varchar
  activo boolean
}

Table plan_cuentas {
  id_cuenta int [pk, increment]
  codigo varchar
  descripcion varchar
}

Table persona {
  id_persona int [pk, increment]
  nombre_completo varchar
  ci varchar
  fecha_nacimiento date
  sexo varchar
  estado_civil varchar
  es_activo boolean
}

Table medio_contacto {
  id_medio int [pk, increment]
  id_persona int [ref: > persona.id_persona]
  tipo varchar [note: 'Celular, Telefono, Email']
  valor varchar
  es_principal boolean
}

Table usuario {
  pk_usuario int [pk, increment]
  id_persona int [ref: > persona.id_persona]
  username varchar
  password_hash varchar
  esta_habilitado boolean
  id_rol_principal int [ref: > rol.id_rol]
}

Table permiso {
  id_permiso int [pk, increment]
  nombre varchar
  descripcion varchar
}

Table rol_permiso {
  id_rol int [ref: > rol.id_rol]
  id_permiso int [ref: > permiso.id_permiso]
  
  indexes {
    (id_rol, id_permiso) [pk]
  }
}

Table condominio {
  id_condominio int [pk, increment]
  nombre varchar
  codigo varchar
  id_admin_persona int [ref: > persona.id_persona]
  superficie_total_m2 decimal
  config_dia_cobro int
}

Table config_aviso_cobranza {
  id_config int [pk, increment]
  id_condominio int [ref: > condominio.id_condominio]
  dias_antes_vencimiento int
  mensaje varchar
}

Table manzano {
  id_manzano int [pk, increment]
  id_condominio int [ref: > condominio.id_condominio]
  codigo varchar
  nombre varchar
}

Table propiedad {
  id_propiedad int [pk, increment]
  id_manzano int [ref: > manzano.id_manzano]
  codigo_unidad varchar
  nombre_funcional varchar
  superficie_m2 decimal
  expensa_base_defecto decimal
  tipo varchar [note: 'CASA, DEPARTAMENTO']
  activo boolean
  saldo_deudor decimal
}

Table contrato {
  id_contrato int [pk, increment]
  id_propiedad int [ref: > propiedad.id_propiedad]
  fecha_firma date
  fecha_inicio date
  fecha_fin date
  fecha_ingreso_real date
  monto_expensa_pactada decimal
  estado varchar [note: 'Vigente, Finalizado']
  motivo_baja varchar
  id_usuario_creador int [ref: > usuario.pk_usuario]
}

Table contrato_participante {
  id int [pk, increment]
  id_contrato int [ref: > contrato.id_contrato]
  id_persona int [ref: > persona.id_persona]
  rol_contrato varchar [note: 'Propietario, Inquilino, etc']
  fecha_alta date
  fecha_baja date
  activo boolean
}

Table contrato_servicio_suscrito {
  id int [pk, increment]
  id_contrato int [ref: > contrato.id_contrato]
  id_servicio int [ref: > catalogo_servicio.id_servicio]
  costo_personalizado decimal
  activo boolean
}

Table lectura_servicio {
  id_lectura int [pk, increment]
  id_contrato int [ref: > contrato.id_contrato]
  id_servicio int [ref: > catalogo_servicio.id_servicio]
  fecha_lectura date
  valor_anterior decimal
  valor_actual decimal
  consumo decimal
}

Table deuda_cabecera {
  id_deuda int [pk, increment]
  id_contrato int [ref: > contrato.id_contrato]
  anio_periodo int
  mes_periodo int
  fecha_emision date
  fecha_vencimiento date
  total_deuda decimal
  estado_pago varchar [note: 'PENDIENTE, PAGADO, ANULADO']
  id_usuario_generador int [ref: > usuario.pk_usuario]
}

Table deuda_detalle {
  id int [pk, increment]
  id_deuda int [ref: > deuda_cabecera.id_deuda]
  id_servicio int [ref: > catalogo_servicio.id_servicio]
  concepto varchar
  monto_unitario decimal
  subtotal decimal
}

Table transaccion_pago {
  id_pago int [pk, increment]
  id_deuda int [ref: > deuda_cabecera.id_deuda]
  id_persona_pagador int [ref: > persona.id_persona]
  id_banco_destino int [ref: > banco.id_banco]
  id_forma_pago int [ref: > forma_pago.id_forma_pago]
  fecha_pago date
  monto_abonado decimal
  estado varchar [note: 'CONFIRMADO, ANULADO']
}

Table egreso {
  id_egreso int [pk, increment]
  id_condominio int [ref: > condominio.id_condominio]
  concepto varchar
  monto_total decimal
  fecha_egreso date
  id_usuario_registro int [ref: > usuario.pk_usuario]
  id_autorizacion int [ref: > autorizacion_gasto.id_autorizacion]
  id_banco_origen int [ref: > banco.id_banco]
  id_proveedor int [ref: > proveedor.id_proveedor]
  id_persona_beneficiario int [ref: > persona.id_persona]
  id_forma_pago int [ref: > forma_pago.id_forma_pago]
}

Table asiento_contable {
  id_asiento int [pk, increment]
  id_condominio int [ref: > condominio.id_condominio]
  fecha date
  numero_asiento int
  tipo varchar [note: 'INGRESO, EGRESO']
  glosa varchar
  activo boolean
}

Table asiento_detalle {
  id_detalle int [pk, increment]
  id_asiento int [ref: > asiento_contable.id_asiento]
  id_cuenta int [ref: > plan_cuentas.id_cuenta]
  debe decimal
  haber decimal
}

Table recurso_comun {
  id_recurso int [pk, increment]
  id_condominio int [ref: > condominio.id_condominio]
  nombre varchar
  costo_reserva decimal
  costo_garantia decimal
}

Table reserva {
  id_reserva int [pk, increment]
  id_recurso int [ref: > recurso_comun.id_recurso]
  id_contrato int [ref: > contrato.id_contrato]
  fecha_inicio datetime
  fecha_fin datetime
  cantidad_invitados int
  motivo varchar
  amenizado_por varchar
  estado varchar [note: 'PENDIENTE, CONFIRMADA, FINALIZADA']
}

Table comunicado_blog {
  id_blog int [pk, increment]
  id_condominio int [ref: > condominio.id_condominio]
  fecha_publicacion date
  titulo varchar
  contenido_html text
  url_imagen varchar
  activo boolean
}
