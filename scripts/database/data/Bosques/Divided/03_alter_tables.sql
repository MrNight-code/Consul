SET FOREIGN_KEY_CHECKS=0;

ALTER TABLE `asiento`
  ADD CONSTRAINT `fk_asiento_banco1` FOREIGN KEY (`fk_banco`) REFERENCES `banco` (`pk_banco`) ON DELETE NO ACTION ON UPDATE NO ACTION;

ALTER TABLE `condominio`
  ADD CONSTRAINT `fk_condominio_persona1` FOREIGN KEY (`administrador`) REFERENCES `persona` (`pk_persona`) ON DELETE NO ACTION ON UPDATE NO ACTION;

ALTER TABLE `contrato`
  ADD CONSTRAINT `fk_contrato_propiedad1` FOREIGN KEY (`fk_propiedad`) REFERENCES `propiedad` (`pk_propiedad`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  ADD CONSTRAINT `fk_contrato_usuario1` FOREIGN KEY (`fk_usuarioinsert`) REFERENCES `usuario` (`pk_usuario`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  ADD CONSTRAINT `fk_contrato_usuario2` FOREIGN KEY (`fk_usuariodelete`) REFERENCES `usuario` (`pk_usuario`) ON DELETE NO ACTION ON UPDATE NO ACTION;

ALTER TABLE `correo`
  ADD CONSTRAINT `fk_correo_persona1` FOREIGN KEY (`fk_persona`) REFERENCES `persona` (`pk_persona`) ON DELETE NO ACTION ON UPDATE NO ACTION;

ALTER TABLE `cuenta`
  ADD CONSTRAINT `fk_cuenta_cuenta1` FOREIGN KEY (`fk_cuenta`) REFERENCES `cuenta` (`pk_cuenta`) ON DELETE NO ACTION ON UPDATE NO ACTION;

ALTER TABLE `cuota`
  ADD CONSTRAINT `fk_cuota_banco1` FOREIGN KEY (`fk_banco`) REFERENCES `banco` (`pk_banco`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  ADD CONSTRAINT `fk_cuota_deuda1` FOREIGN KEY (`fk_deuda`) REFERENCES `deuda` (`pk_deuda`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  ADD CONSTRAINT `fk_cuota_formpago1` FOREIGN KEY (`fk_formapago`) REFERENCES `formpago` (`pk_formpago`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  ADD CONSTRAINT `fk_cuota_persona1` FOREIGN KEY (`fk_persona`) REFERENCES `persona` (`pk_persona`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  ADD CONSTRAINT `fk_cuota_usuario1` FOREIGN KEY (`fk_usuarioalta`) REFERENCES `usuario` (`pk_usuario`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  ADD CONSTRAINT `fk_cuota_usuario2` FOREIGN KEY (`fk_usuariobaja`) REFERENCES `usuario` (`pk_usuario`) ON DELETE NO ACTION ON UPDATE NO ACTION;

ALTER TABLE `detalleasiento`
  ADD CONSTRAINT `fk_detalleasiento_asiento` FOREIGN KEY (`fk_asiento`) REFERENCES `asiento` (`pk_asiento`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  ADD CONSTRAINT `fk_detalleasiento_cuenta1` FOREIGN KEY (`fk_cuenta`) REFERENCES `cuenta` (`pk_cuenta`) ON DELETE NO ACTION ON UPDATE NO ACTION;

ALTER TABLE `deuda`
  ADD CONSTRAINT `fk_deuda_contrato1` FOREIGN KEY (`fk_contrato`) REFERENCES `contrato` (`pk_contrato`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  ADD CONSTRAINT `fk_deuda_serviciopago1` FOREIGN KEY (`fk_servicio`) REFERENCES `serviciopago` (`pk_serviciopago`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  ADD CONSTRAINT `fk_deuda_usuario1` FOREIGN KEY (`fk_usuarioalta`) REFERENCES `usuario` (`pk_usuario`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  ADD CONSTRAINT `fk_deuda_usuario2` FOREIGN KEY (`fk_usuariobaja`) REFERENCES `usuario` (`pk_usuario`) ON DELETE NO ACTION ON UPDATE NO ACTION;

ALTER TABLE `egreso`
  ADD CONSTRAINT `fk_egreso_autorizacion1` FOREIGN KEY (`fk_autorizacion`) REFERENCES `autorizacion` (`pk_autorizacion`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  ADD CONSTRAINT `fk_egreso_banco1` FOREIGN KEY (`fk_banco`) REFERENCES `banco` (`pk_banco`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  ADD CONSTRAINT `fk_egreso_formpago1` FOREIGN KEY (`fk_formpago`) REFERENCES `formpago` (`pk_formpago`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  ADD CONSTRAINT `fk_egreso_persona1` FOREIGN KEY (`fk_persona`) REFERENCES `persona` (`pk_persona`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  ADD CONSTRAINT `fk_egreso_proveedor1` FOREIGN KEY (`fk_proveedor`) REFERENCES `proveedor` (`pk_proveedor`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  ADD CONSTRAINT `fk_egreso_usuario1` FOREIGN KEY (`fk_usuario`) REFERENCES `usuario` (`pk_usuario`) ON DELETE NO ACTION ON UPDATE NO ACTION;

ALTER TABLE `manzano`
  ADD CONSTRAINT `fk_manzano_condominio1` FOREIGN KEY (`fk_condominio`) REFERENCES `condominio` (`pk_condominio`) ON DELETE NO ACTION ON UPDATE NO ACTION;

ALTER TABLE `persona`
  ADD CONSTRAINT `fk_persona_persona1` FOREIGN KEY (`fk_persona`) REFERENCES `persona` (`pk_persona`) ON DELETE NO ACTION ON UPDATE NO ACTION;

ALTER TABLE `persona_contrato`
  ADD CONSTRAINT `fk_persona_contrato_contrato1` FOREIGN KEY (`fk_contrato`) REFERENCES `contrato` (`pk_contrato`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  ADD CONSTRAINT `fk_persona_contrato_persona1` FOREIGN KEY (`fk_persona`) REFERENCES `persona` (`pk_persona`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  ADD CONSTRAINT `fk_persona_contrato_usuario1` FOREIGN KEY (`fk_usuarioinsert`) REFERENCES `usuario` (`pk_usuario`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  ADD CONSTRAINT `fk_persona_contrato_usuario2` FOREIGN KEY (`fk_usuariodelete`) REFERENCES `usuario` (`pk_usuario`) ON DELETE NO ACTION ON UPDATE NO ACTION;

ALTER TABLE `propiedad`
  ADD CONSTRAINT `fk_propiedad_manzano1` FOREIGN KEY (`fk_manzano`) REFERENCES `manzano` (`pk_manzano`) ON DELETE NO ACTION ON UPDATE NO ACTION;

ALTER TABLE `recibogeneral`
  ADD CONSTRAINT `fk_recibogeneral_condominio1` FOREIGN KEY (`fk_condominio`) REFERENCES `condominio` (`pk_condominio`) ON DELETE NO ACTION ON UPDATE NO ACTION;

ALTER TABLE `recibopersonal`
  ADD CONSTRAINT `fk_recibopersonal_contrato1` FOREIGN KEY (`fk_contrato`) REFERENCES `contrato` (`pk_contrato`) ON DELETE NO ACTION ON UPDATE NO ACTION;

ALTER TABLE `serviciocuenta`
  ADD CONSTRAINT `fk_serviciocuenta_serviciopago1` FOREIGN KEY (`fk_servicio`) REFERENCES `serviciopago` (`pk_serviciopago`) ON DELETE NO ACTION ON UPDATE NO ACTION;

ALTER TABLE `servicio_contrato`
  ADD CONSTRAINT `fk_servicio_contrato_contrato1` FOREIGN KEY (`fk_contrato`) REFERENCES `contrato` (`pk_contrato`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  ADD CONSTRAINT `fk_servicio_contrato_serviciopago1` FOREIGN KEY (`fk_serviciopago`) REFERENCES `serviciopago` (`pk_serviciopago`) ON DELETE NO ACTION ON UPDATE NO ACTION;

ALTER TABLE `telefono`
  ADD CONSTRAINT `fk_telefono_persona1` FOREIGN KEY (`fk_persona`) REFERENCES `persona` (`pk_persona`) ON DELETE NO ACTION ON UPDATE NO ACTION;

ALTER TABLE `tipo_permiso`
  ADD CONSTRAINT `fk_tipo_permiso_permiso1` FOREIGN KEY (`fk_permiso`) REFERENCES `permiso` (`pk_permiso`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  ADD CONSTRAINT `fk_tipo_permiso_tipousuario1` FOREIGN KEY (`fk_tipousuario`) REFERENCES `tipousuario` (`pk_tipousuario`) ON DELETE NO ACTION ON UPDATE NO ACTION;

ALTER TABLE `usuario`
  ADD CONSTRAINT `fk_usuario_persona1` FOREIGN KEY (`fk_persona`) REFERENCES `persona` (`pk_persona`) ON DELETE NO ACTION ON UPDATE NO ACTION,
  ADD CONSTRAINT `fk_usuario_tipousuario1` FOREIGN KEY (`fk_tipousuario`) REFERENCES `tipousuario` (`pk_tipousuario`) ON DELETE NO ACTION ON UPDATE NO ACTION;

SET FOREIGN_KEY_CHECKS=1;
