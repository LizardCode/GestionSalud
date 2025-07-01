# GestionSalud Development Container

Este directorio contiene la configuración para el entorno de desarrollo containerizado de GestionSalud, diseñado para ser utilizado por múltiples desarrolladores de forma consistente.

## 🚀 Inicio Rápido

### Prerrequisitos

- **Docker Desktop** instalado y ejecutándose
- **Visual Studio Code** con la extensión **Dev Containers** instalada
- **Git** configurado localmente

### Abrir en Dev Container

1. Clona el repositorio:
   ```bash
   git clone <repository-url>
   cd GestionSalud
   ```

2. Abre VS Code en el directorio del proyecto:
   ```bash
   code .
   ```

3. VS Code detectará la configuración del devcontainer y te ofrecerá abrir en container:
   - Haz clic en **"Reopen in Container"**
   - O usa `Ctrl+Shift+P` → `Dev Containers: Reopen in Container`

4. Espera a que se construya el container (primera vez puede tomar varios minutos)

## 🏗️ Arquitectura del Entorno

### Configuración Minimalista

- **devcontainer**: Entorno único con .NET 8 SDK
- **Base**: Microsoft .NET 8 SDK sobre Ubuntu Jammy
- **Usuario**: `vscode` (no-root)
- **Certificados**: HTTPS development certificate incluido

### Puertos Configurados

| Puerto | Servicio | Descripción |
|--------|----------|-------------|
| 5000 | Frontend MVC | Aplicación web principal |
| 5034 | Main API (HTTP) | API REST principal |
| 7293 | Main API (HTTPS) | API REST principal (HTTPS) |

## 🛠️ Herramientas Incluidas

### Extensiones de VS Code

- **C# Extension**: Desarrollo básico en C#
- **C# Dev Kit**: Herramientas avanzadas de desarrollo

### Herramientas de Línea de Comandos

- **.NET 8 SDK**: Framework principal
- **Entity Framework Core Tools**: Migraciones (`dotnet-ef`)
- **Git**: Control de versiones
- **curl**: Cliente HTTP básico
- **ca-certificates**: Certificados SSL

## 🏃‍♂️ Ejecutar la Aplicación

### Restaurar Paquetes

```bash
# Restaurar paquetes manualmente
dotnet restore LizardCode-SalmaSalud/LizardCode.SalmaSalud.sln
dotnet restore LizardCode-Framework/LizardCode-Framework.sln
```

### Ejecutar Aplicaciones

```bash
# Frontend MVC (puerto 5000)
cd LizardCode-SalmaSalud/LizardCode.SalmaSalud.Frontend
dotnet run

# API Principal (puertos 5034/7293)
cd LizardCode-SalmaSalud/LizardCode.SalmaSalud.API
dotnet run

# Con watch (auto-restart)
dotnet watch run
```

### Usando VS Code

Utiliza las configuraciones de lanzamiento predefinidas en `.vscode/launch.json` (F5)

## 📁 Estructura de Archivos

```
.devcontainer/
├── devcontainer.json     # Configuración principal del devcontainer
├── Dockerfile            # Imagen minimalista de desarrollo
└── README.md            # Esta documentación
```

## 🔄 Configuración Multi-Desarrollador

### Características

- **Portabilidad**: Configuración completamente contenida en el repositorio
- **Consistencia**: Mismo entorno para todos los desarrolladores
- **Simplicidad**: Solo .NET SDK sin servicios adicionales
- **Automatización**: Extensiones y configuraciones sincronizadas

### Configuración Inicial

```bash
# Configurar Git (primera vez)
git config --global user.name "Tu Nombre"
git config --global user.email "tu.email@ejemplo.com"
```

## 🔧 Configuración de Desarrollo

### Variables de Entorno

El container incluye:
```bash
ASPNETCORE_ENVIRONMENT=Development
DOTNET_USE_POLLING_FILE_WATCHER=true
DOTNET_RUNNING_IN_CONTAINER=true
DOTNET_CLI_TELEMETRY_OPTOUT=true
DOTNET_SKIP_FIRST_TIME_EXPERIENCE=true
DOTNET_NOLOGO=true
```

### Certificados HTTPS

El certificado de desarrollo se genera automáticamente al construir el container:
```bash
dotnet dev-certs https --trust
```

## 🐛 Troubleshooting

### Problemas Comunes

**Container no inicia:**
```bash
# Limpiar Docker
docker system prune -a
```

**Certificado HTTPS inválido:**
```bash
# Regenerar certificado dentro del container
dotnet dev-certs https --clean
dotnet dev-certs https --trust
```

**Problemas con paquetes NuGet:**
```bash
# Limpiar cache
dotnet nuget locals all --clear
dotnet restore
```

### Reiniciar Entorno

1. **Rebuild Container**: `Dev Containers: Rebuild Container`
2. **Reload Window**: `Dev Containers: Reload Window`
3. **Reset**: Eliminar containers y rebuildar

## ⚡ Características Simplificadas

### Lo que NO incluye

- ❌ SQL Server containerizado
- ❌ Redis
- ❌ Mailhog
- ❌ Node.js
- ❌ PowerShell
- ❌ Docker CLI
- ❌ GitHub CLI
- ❌ Herramientas adicionales

### Lo que SÍ incluye

- ✅ .NET 8 SDK completo
- ✅ Entity Framework Tools
- ✅ Certificados HTTPS de desarrollo
- ✅ Extensiones C# para VS Code
- ✅ Git y herramientas básicas
- ✅ Usuario no-root configurado

## 🎯 Filosofía de Diseño

Este devcontainer está diseñado con el principio de **minimalismo**:

- **Solo lo esencial**: .NET SDK + herramientas básicas
- **Sin dependencias externas**: No requiere servicios adicionales
- **Rápido inicio**: Build time mínimo
- **Fácil mantenimiento**: Configuración simple
- **Base sólida**: Extensible según necesidades específicas

---

**¡Happy Coding! 🏥💻**