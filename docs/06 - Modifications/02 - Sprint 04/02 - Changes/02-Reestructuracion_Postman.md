# 2. Reestructuración de la Colección de Postman

**Sprint:** 04
**Tipo:** Change
**Fecha:** 20/02/2026
**Módulo:** Global

## Descripción

La colección de peticiones de **Postman**, que contaba con una carpeta aglomerada llamada "Entities", fue completamente reorganizada para reflejar fielmente la arquitectura por Dominios o Módulos que maneja el sistema backend en Consulcon.

## Estructura Actualizada

Se creó el siguiente agrupamiento a nivel raíz:

- **Seguridad:** Auth, Usuario, Persona
- **Inmuebles:** Condominio, Manzano, Propiedad, Ownership
- **Facturación:** Contrato, CatalogoServicio, Deuda, Cobranzas
- **Contabilidad:** AsientoContable, PlanCuenta, FiscalPeriods
- **Tesorería:** Tesoreria, AutorizacionGasto, Accounts, Proveedores
- **Financiero:** Financiero
- **Reservas:** Reservas, RecursoComun
- **Comunicación:** ComunicadoBlog
- **Dashboard:** Dashboard Metrics

## Automatización

El proceso de migración de todas las peticiones dentro del archivo `postman_collection.json`, así como la redirección de las rutas obsoletas (ej. `Entities > Manzano`) en todos los archivos Markdown de la carpeta `/docs`, fue realizado íntegramente mediante scripts de Python (`restructure_postman.py` y `update_md_refs.py`) para evitar la pérdida o desconfiguración de Headers (como los Tokens de Auth y los IDs de Condominio).

## Impacto

- Facilita de gran medida el descubrimiento de Endpoints para nuevos desarrolladores y para la documentación con la WebApp.
- La documentación del _Sprint 03_ que poseía referencias antiguas al folder "Entities" fue arreglada retroactivamente.
