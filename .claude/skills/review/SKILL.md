---
name: review
description: Revisa el código del proyecto buscando code smells, violaciones de arquitectura y oportunidades de mejora. Reporta hallazgos sin modificar archivos.
allowed-tools: Read, Glob, Grep
---

Revisá el código de QuickOrder buscando code smells y mejoras. $ARGUMENTS

## Qué revisar

### 🏗️ Violaciones de arquitectura
- Handlers que usen `AppDbContext` o `IAppDbContext` directamente (deben usar repositorios)
- Lógica de negocio en Controllers (deben ser thin)
- Lógica de negocio en Repositories (solo debe haber queries)
- Entidades de dominio expuestas en responses (deben usarse DTOs)
- Referencias incorrectas entre capas (ej: Domain referenciando Application)

### 🔁 CQRS / MediatR
- Commands que no tienen Validator asociado
- Queries que retornan entidades en lugar de DTOs
- Handlers con demasiadas responsabilidades (más de una operación de negocio)

### 🧱 Repositorios
- Queries complejas en handlers en lugar de repositorios
- Falta de `AsNoTracking()` en queries de solo lectura
- N+1 queries (falta de `Include` o `ThenInclude`)
- Paginación implementada en memoria en lugar de en base de datos

### 🧹 Code smells generales
- Métodos muy largos (más de 30 líneas)
- Clases con demasiadas dependencias inyectadas (más de 5)
- Código duplicado entre handlers similares
- Magic strings o números hardcodeados
- Nombres poco descriptivos o inconsistentes con el resto del proyecto
- `catch` vacíos o que silencian excepciones

### ✅ Validación
- Commands sin FluentValidator
- Validaciones duplicadas en Validator y en Handler
- Falta de validación en campos string (longitud máxima, not empty)

### ⚡ Performance
- `ToList()` antes de un `Where()` (filtrando en memoria)
- `Select` o proyecciones faltantes cuando se carga una entidad completa solo para leer un campo
- Operaciones sincrónicas donde debería haber async/await

## Cómo reportar

Para cada hallazgo indicá:
- 📁 Archivo y línea aproximada
- 🔴 Severidad: Alta / Media / Baja
- 🐛 Problema exacto
- ✅ Sugerencia de corrección

Al final mostrá un resumen con conteo por severidad y los 3 problemas más importantes a resolver primero.

Si $ARGUMENTS está vacío, revisá todo el proyecto. Si se especifica un archivo o carpeta, revisá solo eso.
