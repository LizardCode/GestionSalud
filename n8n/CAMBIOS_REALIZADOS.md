# Modificaciones Realizadas al Flow de n8n - SalmaTurnos

## Resumen de Cambios

Se ha modificado el flow de n8n para obtener los datos maestros (especialidades, días, rangos horarios) de la API en lugar de usar JSON estático.

## Cambios Principales

### 1. Reemplazo del Nodo "Maestros" Estático

**ANTES:**
- Nodo `Maestros` con JSON estático conteniendo especialidades, días y rangos horarios

**DESPUÉS:**
- `Get Maestros Token`: Obtiene token de autenticación para la API
- `Get Especialidades`: Llama a `GET https://api.salmasalud.com.ar/especialidades`
- `Format Maestros`: Formatea la respuesta para mantener compatibilidad

### 2. Nuevos Nodos para Datos Dinámicos

Se agregaron los siguientes nodos para obtener datos dinámicamente:

#### Nodos de Decisión:
- `Especialidad Selection Decision`: Detecta cuando se selecciona una especialidad para activar el flujo dinámico

#### Nodos de API:
- `Get Dias Horarios Token`: Obtiene token para APIs de días y horarios
- `Get Dias`: Llama a `GET https://api.salmasalud.com.ar/dias?idEspecialidad={id}`
- `Get Rangos Horarios`: Llama a `GET https://api.salmasalud.com.ar/rangos-horarios?idEspecialidad={id}`

#### Nodos de Procesamiento:
- `Process Dynamic Maestros`: Procesa las respuestas de las APIs y las formatea
- `Update Session - Dynamic Maestros`: Actualiza la sesión con los datos dinámicos
- `Write JSONBin - Dynamic Maestros`: Guarda la sesión actualizada
- `Format Dynamic Maestros Response`: Formatea la respuesta final para el usuario

### 3. Modificaciones al Process Step Logic

Se actualizó la lógica para usar datos dinámicos almacenados en la sesión:

#### Caso ESPERANDO_ESPECIALIDAD:
- Ahora activa el flujo dinámico en lugar de mostrar días inmediatamente
- Establece `triggerDynamicFlow: true` para activar la obtención de datos dinámicos

#### Casos ESPERANDO_DIA y ESPERANDO_HORARIO:
- Buscan primero datos dinámicos en `userSession.maestrosDinamicos`
- Fallback a datos estáticos si no hay dinámicos disponibles
- Mantienen la misma lógica de validación y selección múltiple

## Flujo de Ejecución

### 1. Inicio del Flow:
```
Webhook → menssageIn → Read JSONBin → [Get User Session + Get Maestros Token]
                                      ↓
Get Maestros Token → Get Especialidades → Format Maestros
```

### 2. Selección de Especialidad:
```
Process Step Logic → [DNI Validation Decision + Especialidad Selection Decision]
                     ↓
Especialidad Selection Decision → Get Dias Horarios Token → Get Dias → Get Rangos Horarios
                                 ↓
Process Dynamic Maestros → Update Session - Dynamic Maestros → [Write JSONBin + Format Response]
```

### 3. Uso de Datos Dinámicos:
- Los datos se almacenan en `userSession.maestrosDinamicos`
- Se acceden desde Process Step Logic en estados ESPERANDO_DIA y ESPERANDO_HORARIO
- Fallback automático a datos estáticos si los dinámicos no están disponibles

## Configuración de Headers

Todos los nodos HTTP usan los headers requeridos:
```
Authorization: Bearer {token}
accept: */*
```

## Endpoints de la API

### Especialidades (al inicio del flow):
- **URL**: `https://api.salmasalud.com.ar/especialidades`
- **Método**: GET
- **Headers**: Authorization Bearer, accept: */*

### Días (después de seleccionar especialidad):
- **URL**: `https://api.salmasalud.com.ar/dias?idEspecialidad={id}`
- **Método**: GET
- **Headers**: Authorization Bearer, accept: */*

### Rangos Horarios (después de seleccionar especialidad):
- **URL**: `https://api.salmasalud.com.ar/rangos-horarios?idEspecialidad={id}`
- **Método**: GET
- **Headers**: Authorization Bearer, accept: */*

## Compatibilidad

El flow modificado mantiene:
- ✅ Compatibilidad con la lógica existente de validación DNI
- ✅ Selección múltiple de días y horarios
- ✅ Misma estructura de mensajes al usuario
- ✅ Fallback a datos estáticos si las APIs fallan
- ✅ Misma funcionalidad de confirmación y envío final a la API

## Token de Ejemplo Configurado

Se mantiene el token de ejemplo proporcionado en todos los nodos de autenticación:
```
eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxIiwiZXhwIjoxNzU1MTUwOTQyLCJpYXQiOjE3NTUxNDM3NDIsIm5iZiI6MTc1NTE0Mzc0Mn0.UlmUCmM1C_YmppQiLTQpMqavB601duGUSVcxhir3PwA
```

## Validación y Testing

Para probar el flow modificado:

1. **Test de Especialidades**: Verificar que se carguen las especialidades desde la API al inicio
2. **Test de Selección de Especialidad**: Confirmar que al seleccionar una especialidad se activa el flujo dinámico
3. **Test de Días y Horarios**: Validar que se obtengan los días y horarios específicos para la especialidad seleccionada
4. **Test de Fallback**: Verificar que funcione el fallback a datos estáticos si las APIs no responden
5. **Test de Flujo Completo**: Probar todo el flujo de principio a fin con datos dinámicos

El flow ahora obtiene datos maestros dinámicamente de la API manteniendo toda la funcionalidad existente.