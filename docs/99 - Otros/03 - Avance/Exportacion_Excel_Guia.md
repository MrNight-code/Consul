# Documentación Técnica: Funcionalidad "Exportar a Excel"

## 1. 🎯 El Objetivo (¿Qué se hizo?)
Se implementó una funcionalidad global en el backend de **Consulcon** para generar y descargar archivos Excel (`.xlsx`) reales. 
Esta funcionalidad permite exportar grandes volúmenes de datos financieros en forma de tableros listos para análisis, cumpliendo con la necesidad de los Administradores de trabajar datos externamente.

## 2. ⚙️ La Solución (¿Cómo funciona?)
Se creó una arquitectura reutilizable basada en Clean Architecture:

*   **Librería Principal:** Se instaló **`ClosedXML`** en la capa de Infrastructure.
*   **Servicio Genérico (`ExcelService`)**: Se creó un servicio universal capaz de recibir *cualquier* lista de datos (`List<T>`), leer dinámicamente sus columnas y "dibujar" un archivo Excel.
    *   *Ventaja Visual*: Se programó auto-ajuste de celdas, encabezados en fondo gris y negritas automáticas.
    *   *Ventaja Técnica*: Detecta si el dato es Fecha o Moneda y le aplica el formato numérico real de Excel (no exporta texto plano).
*   **Modificación de Paginación**: Los endpoints de exportación anulan temporalmente las reglas de paginación (`pageSize = int.MaxValue`) para extraer **todos** los registros filtrados en un solo archivo.

## 3. 🚦 Alcance (¿En qué endpoints se aplicó?)
Se seleccionaron e implementaron los **4 endpoints financieros más críticos** del sistema:

1.  ✅ **Cobranzas**: `/api/cobranzas/{id}/export` (Historial de recibos).
2.  ✅ **Deudas**: `/api/deuda/pendiente/export` (Listado de deudores).
3.  ✅ **Proveedores**: `/api/providers/export` (Cartera de cuentas por pagar).
4.  ✅ **Libro de Caja**: `/api/cashbook/export` (Flujo de ingresos y egresos).

### ¿Por qué NO se aplicó a los demás (~36 endpoints)?
1.  **Naturaleza del Endpoint:** El 80% de los endpoints en Consulcon son operaciones CRUD individuales (Crear un registro, Editar un ID, Borrar). *Un Excel solo tiene sentido para endpoints que devuelven Listados (Tablas).*
2.  **Valor para el Negocio:** Muchos listados son meramente internos o configuraciones del sistema (Ej. Catálogos de servicio, Periodos Fiscales). Exportar esto a Excel no aporta valor diario al administrador.
3.  **Patrón Reutilizable:** Si a futuro el cliente requiere exportar otros módulos vitales (Ej. Inquilinos o Inmuebles), la arquitectura de `ExcelService` ya está lista. Solo toma 5 líneas de código invocarla en cualquier controlador.

---

## 4. 🧪 Guía de Pruebas (Cómo usar en Postman)

Dado que los endpoints generan un archivo **binario** (`.xlsx`), no pueden leerse como texto normal en Postman. Si presionas `Send` saltarán caracteres ilegibles (`PK!...`).

**Pasos correctos para probar:**

1.  Asegúrate de haber recompilado el Docker del backend (`docker-compose up --build`).
2.  En Postman, ve a la nueva carpeta **"Exportaciones"** (o Cobranzas).
3.  Selecciona el endpoint elegido y verifica que tengas el token de autorización configurado.
4.  **En lugar de hacer clic en el botón azul de `Send`**, haz clic en la pequeña **flecha hacia abajo `v`** pegada a la derecha de ese botón.
5.  Selecciona la opción **`Send and Download`**.
6.  Aparecerá una ventana para guardar el archivo `.xlsx` en tu PC.
7.  Ábrelo con Microsoft Excel o Google Sheets.
