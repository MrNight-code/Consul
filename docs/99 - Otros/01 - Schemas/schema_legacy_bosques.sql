Table asiento {
  pk_asiento int [pk]
  fecha datetime
  documento varchar
  numerodocumento varchar
  fk_banco int
  cheque varchar
  tc decimal
  tipoasiento varchar
  fk_cuenta int
  fk_deuda int
  glosa varchar
  activo varchar
  fechabaja datetime
  fechaupdate datetime
  fk_usuariobaja int
  fk_usuarioupdate int
  fk_proceso int
  formulario varchar
  numero int
  fechacreacion datetime
  fk_usuario int
  motivobaja varchar
}

Table autorizacion {
  pk_autorizacion int [pk]
  descripcion varchar
  activo varchar
}

Table avisoconbranza {
  pk_avisoconbranza int [pk]
  pk_servicio1 int [pk]
  pk_servicio2 int
  pk_servicio3 int
  pk_servicio4 int
  pk_servicio5 int
  servicio1 varchar
  servicio2 varchar
  servicio3 varchar
  servicio4 varchar
  servicio5 varchar
  mes1 int
  mes2 int
  mes3 int
  mes4 int
  mes5 int
  anio1 int
  anio2 int
  anio3 int
  anio4 int
  anio5 int
}

Table banco {
  pk_banco int [pk]
  descripcion varchar
  fk_cuenta int
  numero varchar
  cuenta varchar
  activo varchar
  cuentabanco varchar
}

Table blog {
  pk_blog int [pk]
  fecha datetime
  descripcion varchar
  titulo varchar
  imagen varchar
  archivo varchar
  activo int
}

Table condominio {
  pk_condominio int [pk]
  codigo varchar
  nombre varchar
  administrador int
  m2 decimal
  tipo varchar
  vh varchar
  diacobro varchar
}

Table confaviso {
  pk_confaviso int [pk]
  fechaemision datetime
  header varchar
  fechavencimiento datetime
  condominio varchar
  footer varchar
}

Table confejecucion {
  pk_confejecucion int [pk]
  descripcion varchar
  tipo varchar
  activo varchar
}

Table confejecuciontitulo {
  pk_confejecuciontitulo int [pk]
  fk_confejecucion int
  descripcion varchar
  activo varchar
}

Table configuracionevento {
  id int
  PrecioChurrasquera decimal
  PrecioSalon decimal
  PrecioSalonChurrasquera decimal
  MontoGarantia decimal
  NumeroCuenta decimal
  Banco varchar
}

Table contrato {
  pk_contrato int [pk]
  fk_propiedad int
  fecha datetime
  fechaini datetime
  fechafin datetime
  fechaingreso datetime
  expensa decimal
  valido varchar
  fechabaja datetime
  motivo varchar
  fk_usuarioinsert int
  fk_usuariodelete int
  diaini int
  diafin int
  fk_condominio int
}

Table correo {
  pk_correo int [pk]
  tipo varchar
  email varchar
  fk_persona int
}

Table cuenta {
  pk_cuenta int [pk]
  numero int
  descripcion varchar
  nivel int
  fk_cuenta int
  nivel1 int
  nivel2 int
  nivel3 int
  nivel4 int
  nivel5 int
}

Table cuentasistema {
  pk_cuentasistema int [pk]
  descripcion varchar
  pk_cuenta int [pk]
  cuenta varchar
  opcion varchar
}

Table cuota {
  pk_cuota int [pk]
  fk_deuda int
  fk_persona int
  fecha datetime
  monto decimal
  formapago varchar
  banco varchar
  cuenta varchar
  tipocambio decimal
  fechabaja datetime
  motivo varchar
  fk_usuarioalta int
  fk_usuariobaja int
  activo varchar
  recibogeneral int
  recibopersonal int
  fk_banco int
  fk_formapago int
  fechadeposito datetime
  cuentadebe int
  cuentahaber int
  fk_moneda int
  fk_opcion_pago int
  ano int
  mes int
  fk_servicio int
  fk_contrato int
  deuda decimal
  deuda_estado varchar
  deuda_activo varchar
  nombre varchar
}

Table detalleasiento {
  pk_detalleasiento int [pk]
  fk_cuenta int
  fk_asiento int
  concepto varchar
  debe decimal
  haber decimal
  glosa varchar
  activo varchar
}

Table detalleconfejecucion {
  fk_confejecuciontitulo int
  fk_confejecucion int
  pk_cuenta int [pk]
  cuenta varchar
  descripcion varchar
  nivel1 int
  descripcionejecucion varchar
  tipo int
}

Table deuda {
  pk_deuda int [pk]
  fk_servicio int
  fk_contrato int
  fecha datetime
  fechadeuda datetime
  monto decimal
  pagado varchar
  estado varchar
  fechabaja datetime
  motivobaja varchar
  fk_usuarioalta int
  fk_usuariobaja int
  activo varchar
  nombre varchar
  ano int
  mes int
}

Table egreso {
  pk_egreso int [pk]
  numerorecibo int
  concepto varchar
  glosa varchar
  monto decimal
  fechacreacion datetime
  fecha datetime
  fk_usuario int
  activo varchar
  fk_usuariobaja int
  motivobaja varchar
  fechabaja datetime
  cuentadebe int
  cuentahaber int
  fk_formpago int
  numerotrans varchar
  fk_proveedor int
  fk_persona int
  fk_autorizacion int
  fk_banco int
  cuenta varchar
  nota varchar
  fechacobro datetime
  fechaconfirmado datetime
  cobrado varchar
}

Table empresa {
  pk_empresa int [pk]
  empresa varchar
  telefono varchar
  direccion varchar
  facebook varchar
  twiter varchar
  habilitado varchar
}

Table evento {
  pk_evento int [pk]
  fecha datetime
  anio int
  mes int
  dia int
  horainicio time
  horafin time
  fk_contrato int
  fk_propietario int
  fk_persona int
  disponible varchar
  numero int
  colorChurrasquera varchar
  checkChurrasquera varchar
  checkSalon varchar
  MotivoEvento varchar
  NumeroInvitados varchar
  CelularContacto varchar
  Amenizado varchar
  PropietarioInquilino varchar
}

Table formpago {
  pk_formpago int [pk]
  descripcion varchar
  fk_cuenta int
  numero varchar
  cuenta varchar
  activo varchar
}

Table manzano {
  pk_manzano int [pk]
  codigo varchar
  nombre varchar
  fk_condominio int
}

Table moneda {
  pk_moneda int [pk]
  moneda varchar
  tipocambio decimal
  abreviado varchar
}

Table opcioncobro {
  pk_opcioncobro int [pk]
  descripcion varchar
}

Table permiso {
  pk_permiso int [pk]
  descripcion varchar
}

Table persona {
  pk_persona int [pk]
  nombre varchar
  ci varchar
  sexo varchar
  fechanac date
  estadocivil varchar
  direccion varchar
  relacion varchar
  activo varchar
  fk_persona int
  telefono varchar
  celular varchar
  email varchar
}

Table persona_contrato {
  fk_persona int
  fk_contrato int
  tipo varchar
  fecharegistro datetime
  fecharetiro datetime
  valido varchar
  activo varchar
  fk_usuarioinsert int
  fk_usuariodelete int
}

Table propiedad {
  pk_propiedad int [pk]
  codigo varchar
  nombre varchar
  m2 decimal
  expensa decimal
  tipo varchar
  activo boolean
  fk_manzano int
  fk_condominio int
}

Table proveedor {
  pk_proveedor int [pk]
  nombre varchar
  nit varchar
  telefono varchar
  celular varchar
  direccion varchar
  email varchar
  activo varchar
}

Table reciboegreso {
  pk_reciboegreso int [pk]
  fk_condominio int
}

Table recibogeneral {
  pk_recibogeneral int [pk]
  fk_condominio int
}

Table recibopersonal {
  pk_recibopersonal int [pk]
  fk_contrato int
}

Table serviciocuenta {
  pk_serviciocuenta int [pk]
  fk_servicio int
  descripcion varchar
  pk_cuenta int
  cuenta varchar
}

Table serviciopago {
  pk_serviciopago int [pk]
  nombre varchar
  costo decimal
  estado varchar
  activo varchar
}

Table servicio_contrato {
  pk_servicio_contrato int [pk]
  costo decimal
  activo varchar
  fk_contrato int
  fk_serviciopago int
}

Table tabla_servicio {
  pk_contrato int [pk]
  codigo varchar
  propietario varchar
  nservicio1 varchar
  nservicio2 varchar
  nservicio3 varchar
  nservicio4 varchar
  nservicio5 varchar
  servicio1 decimal
  servicio2 decimal
  servicio3 decimal
  servicio4 decimal
  servicio5 decimal
  total decimal
}

Table TbReporteFinal {
  id int [pk]
  propiedad varchar
  propietario varchar
  monto decimal
  notas varchar [note: 'Tabla temporal de reportes con columnas dinamicas']
}

Table tbreportefinal {
  id int [pk]
  propiedad varchar
  propietario varchar
  monto decimal
  notas varchar [note: 'Tabla temporal de reportes con columnas dinamicas']
}

Table telefono {
  pk_telefono int [pk]
  tipo varchar
  numero varchar
  estado varchar
  fk_persona int
}

Table TempContratoServicio {
  pk_contrato int [pk]
  fk_propiedad int
  nombre varchar
  fk_servicio int
  ano int
  mes int
}

Table tempcontratoservicio {
  pk_contrato int [pk]
  fk_propiedad int
  nombre varchar
  fk_servicio int
  ano int
  mes int
}

Table TempTablaMadre {
  fk_contrato int
  ano int
  mes int
  fk_servicio int
  deuda decimal
  nombre varchar
  pago decimal
}

Table temptablamadre {
  fk_contrato int
  ano int
  mes int
  fk_servicio int
  deuda decimal
  nombre varchar
  pago decimal
}

Table TempTotalTable {
  pk_contrato int [pk]
  propiedad varchar
  propietario varchar
  Fecha varchar
  expensa decimal
  numerodeuda int
  deuda decimal
  pago decimal
  saldo decimal
}

Table temptotaltable {
  pk_contrato int [pk]
  propiedad varchar
  propietario varchar
  Fecha varchar
  expensa decimal
  numerodeuda int
  deuda decimal
  pago decimal
  saldo decimal
}

Table temResultado {
  pk_contrato int [pk]
  nombre varchar
  saldo decimal
  fk_servicio int
  ano int
  mes int
}

Table temresultado {
  pk_contrato int [pk]
  nombre varchar
  saldo decimal
  fk_servicio int
  ano int
  mes int
}

Table tipousuario {
  pk_tipousuario int [pk]
  descripcion varchar
}

Table tipo_permiso {
  fk_tipousuario int
  fk_permiso int
}

Table ttcuota {
  pk_cuota int [pk]
  cuota decimal
  fk_deuda int
}

Table ttdeuda {
  pk_deuda int [pk]
  monto decimal
}

Table usuario {
  pk_usuario int [pk]
  fk_persona int
  fecha datetime
  usuario varchar
  contrasena varchar
  habilitado varchar
  activo varchar
  nombrepersona varchar
  fk_tipousuario int
}

Ref: asiento.fk_banco > banco.pk_banco
Ref: asiento.fk_cuenta > cuenta.pk_cuenta
Ref: asiento.fk_deuda > deuda.pk_deuda
Ref: asiento.fk_usuario > usuario.pk_usuario
Ref: banco.fk_cuenta > cuenta.pk_cuenta
Ref: confejecuciontitulo.fk_confejecucion > confejecucion.pk_confejecucion
Ref: contrato.fk_propiedad > propiedad.pk_propiedad
Ref: contrato.fk_usuarioinsert > usuario.pk_usuario
Ref: contrato.fk_condominio > condominio.pk_condominio
Ref: correo.fk_persona > persona.pk_persona
Ref: cuenta.fk_cuenta > cuenta.pk_cuenta
Ref: cuota.fk_deuda > deuda.pk_deuda
Ref: cuota.fk_persona > persona.pk_persona
Ref: cuota.fk_banco > banco.pk_banco
Ref: cuota.fk_formapago > formpago.pk_formpago
Ref: cuota.fk_moneda > moneda.pk_moneda
Ref: cuota.fk_servicio > serviciopago.pk_serviciopago
Ref: cuota.fk_contrato > contrato.pk_contrato
Ref: detalleasiento.fk_cuenta > cuenta.pk_cuenta
Ref: detalleasiento.fk_asiento > asiento.pk_asiento
Ref: deuda.fk_servicio > serviciopago.pk_serviciopago
Ref: deuda.fk_contrato > contrato.pk_contrato
Ref: deuda.fk_usuarioalta > usuario.pk_usuario
Ref: egreso.fk_usuario > usuario.pk_usuario
Ref: egreso.fk_proveedor > proveedor.pk_proveedor
Ref: egreso.fk_persona > persona.pk_persona
Ref: egreso.fk_autorizacion > autorizacion.pk_autorizacion
Ref: egreso.fk_banco > banco.pk_banco
Ref: evento.fk_contrato > contrato.pk_contrato
Ref: evento.fk_persona > persona.pk_persona
Ref: formpago.fk_cuenta > cuenta.pk_cuenta
Ref: manzano.fk_condominio > condominio.pk_condominio
Ref: persona.fk_persona > persona.pk_persona
Ref: persona_contrato.fk_persona > persona.pk_persona
Ref: persona_contrato.fk_contrato > contrato.pk_contrato
Ref: propiedad.fk_manzano > manzano.pk_manzano
Ref: propiedad.fk_condominio > condominio.pk_condominio
Ref: servicio_contrato.fk_contrato > contrato.pk_contrato
Ref: servicio_contrato.fk_serviciopago > serviciopago.pk_serviciopago
Ref: telefono.fk_persona > persona.pk_persona
Ref: tipo_permiso.fk_tipousuario > tipousuario.pk_tipousuario
Ref: tipo_permiso.fk_permiso > permiso.pk_permiso
Ref: usuario.fk_persona > persona.pk_persona
Ref: usuario.fk_tipousuario > tipousuario.pk_tipousuario