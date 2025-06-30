# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Prerequisites

Before working with this project, ensure you have the following installed:

### .NET SDK 8.0

Install .NET SDK 8.0 using Homebrew:

```bash
# Install .NET SDK 8.0
brew install dotnet@8

# Make it the default version (if needed)
brew link --force dotnet@8

# Verify installation
dotnet --version
```

### Additional Requirements

- **Git**: For version control
- **Visual Studio Code**: Recommended IDE with C# extension
- **SQL Server**: Database server (LocalDB for development)

## Project Overview

This is **GestionSalud** (Health Management System), a comprehensive healthcare management application built in C# using .NET 8. The project follows Clean Architecture principles and consists of two main components:

1. **LizardCode-Framework**: A reusable framework providing common functionality
2. **LizardCode-SalmaSalud**: The main health management application

## Architecture

The solution uses Clean Architecture with the following layers:

- **Domain**: Core business entities and enums
- **Application**: Business logic, interfaces, and application services
- **Infrastructure**: Data access, external services, and infrastructure concerns
- **Frontend**: ASP.NET Core MVC web application
- **API**: RESTful API endpoints for external integration

### Key Technologies

- **.NET 8**: Target framework
- **Dapper**: Micro ORM for data access
- **Mapster**: Object mapping
- **JWT Bearer**: Authentication
- **Swagger**: API documentation
- **NLog**: Logging framework
- **SendGrid**: Email services
- **AFIP Integration**: Argentine tax authority integration

## Development Commands

### VS Code Launch Configurations

The project includes comprehensive VS Code launch configurations in `.vscode/launch.json`:

#### Individual Services
- **🚀 API - Development**: Runs the API in development mode with Swagger
- **🚀 API - Production**: Runs the API in production mode
- **🌐 Frontend - Development**: Runs the web frontend in development mode
- **🌐 Frontend - Production**: Runs the web frontend in production mode
- **🔧 Appointments Service - Development**: Runs the appointments microservice
- **🔔 Notifications Service - Development**: Runs the notifications microservice

#### Compound Configurations
- **🚀 Full Microservices - Development**: Runs all services simultaneously
- **🌐 Frontend + API - Development**: Runs frontend and main API together

### Building the Solution

```bash
# Build the main solution
dotnet build LizardCode-SalmaSalud/LizardCode.SalmaSalud.sln

# Build the framework
dotnet build LizardCode-Framework/LizardCode-Framework.sln

# Clean and restore
dotnet clean LizardCode-SalmaSalud/LizardCode.SalmaSalud.sln
dotnet restore LizardCode-SalmaSalud/LizardCode.SalmaSalud.sln
```

### Running the Applications

```bash
# Run the web frontend (port 5000)
cd LizardCode-SalmaSalud/LizardCode.SalmaSalud.Frontend
dotnet run

# Run the API (ports 5034/7293)
cd LizardCode-SalmaSalud/LizardCode.SalmaSalud.API
dotnet run

# Watch mode for development (auto-restart on changes)
dotnet watch run --project LizardCode-SalmaSalud/LizardCode.SalmaSalud.API
dotnet watch run --project LizardCode-SalmaSalud/LizardCode.SalmaSalud.Frontend
```

### Development Profiles and Ports

- **Frontend**: `http://localhost:5000`
- **Main API**: `http://localhost:5034` (HTTP) or `https://localhost:7293` (HTTPS)
- **Appointments Service**: `http://localhost:5001` (HTTP) or `https://localhost:7001` (HTTPS)
- **Notifications Service**: `http://localhost:5002` (HTTP) or `https://localhost:7002` (HTTPS)
- **API Documentation**: Available at `/swagger` endpoint when running in development

### Health Check Endpoint

The API includes a single health check endpoint for monitoring:

- **`GET /api/health`**: Complete application health status including database connectivity and configuration validation

#### Health Check Features
- **Database**: SQL Server connectivity test with query execution
- **Configuration**: Critical configuration validation (JWT, API Keys, Connection String)
- **Response Codes**: 200 (Healthy), 207 (Degraded), 503 (Unhealthy)
- **JSON Response**: Structured response with timestamp, duration, and detailed check results

### Production Deployment

```bash
# Publish API for production
dotnet publish LizardCode-SalmaSalud/LizardCode.SalmaSalud.API -c Release -o ./publish/api

# Publish Frontend for production
dotnet publish LizardCode-SalmaSalud/LizardCode.SalmaSalud.Frontend -c Release -o ./publish/frontend
```

## Project Structure

### Main Application (LizardCode-SalmaSalud)

- **Domain/Entities**: Core business entities (Paciente, Turno, Profesional, etc.)
- **Domain/EntitiesCustom**: Extended entities with additional properties
- **Domain/Enums**: Business enumerations
- **Application/Business**: Business logic implementations
- **Application/Interfaces**: Business service interfaces
- **Infrastructure**: Data repositories and external service implementations
- **Frontend**: MVC web application with controllers and views
- **API**: REST API controllers and models

### Framework (LizardCode-Framework)

- **ApiBase**: Base classes for application and infrastructure layers
- **Helpers**: Utility libraries (AFIP, Barcode, Excel, PDF, SendGrid, etc.)
- **Dapper Extensions**: Custom Dapper extensions and DataTables integration
- **Caching**: Memory caching implementation
- **RestClient**: HTTP client utilities

## Key Business Domains

The system manages several healthcare domains:

- **Patient Management**: Pacientes, historiales médicos
- **Appointment Scheduling**: Turnos, disponibilidad, confirmaciones
- **Medical Records**: Evoluciones, recetas, órdenes médicas
- **Financial Management**: Facturación, obras sociales, liquidaciones
- **Administration**: Usuarios, permisos, configuración

## Configuration

### Environment-Specific Settings

The application supports multiple environments with dedicated configuration files:

- **Development**: `appsettings.json` (contains actual values for dev/testing)
- **Production**: `appsettings.Production.json` (uses environment variables for security)

### Production Environment Variables

For production deployment, copy `.env.production.example` to `.env.production` and configure:

```bash
# Key environment variables for production
DATABASE_CONNECTION_STRING=Server=prod-server;Database=SALMA_SALUD_PROD;...
JWT_SECRET=your_secure_jwt_secret_256_bits_minimum
SENDGRID_API_KEY=your_sendgrid_api_key
AFIP_CUIT=your_company_cuit
# ... see .env.production.example for complete list
```

### Database Connection

The application uses SQL Server with Dapper for data access. Connection strings are configured in `appsettings.json` files or environment variables for production.

### Authentication

- **JWT Bearer tokens** for API authentication
- **API Key authentication** for service-to-service communication
- **Session-based authentication** for web frontend

### Logging

NLog is configured for structured logging. Configuration files are located at:
- `LizardCode.SalmaSalud.Frontend/NLog.config`
- Framework template also includes NLog configuration

### External Service Integrations

- **AFIP**: Argentine tax authority integration for electronic invoicing
- **SendGrid**: Email delivery service
- **Google Maps**: Location services
- **WhatsApp API**: Message notifications
- **reCAPTCHA**: Bot protection

## Development Notes

### Dependency Injection

All business services are registered in `Application/Injection.cs` with scoped lifetime. The framework provides lazy resolution for dependencies.

### Mapping

Mapster is used for object mapping with configuration in the Application layer.

### Code Generation

The framework includes code generation tools for creating entities and business layers from database schemas.

### AFIP Integration

The system integrates with Argentina's tax authority (AFIP) for electronic invoicing and tax compliance.

## Testing

No test projects are currently configured in the solution. When adding tests:
- Create test projects following the naming convention `*.Tests`
- Use xUnit, NUnit, or MSTest as testing framework
- Include integration tests for business logic and API endpoints

## Troubleshooting

### .NET Version Management

This project targets .NET 8.0. If you have multiple .NET versions installed and encounter compatibility issues, use the official uninstall tool:

#### Installing dotnet-core-uninstall tool

```bash
# macOS/Linux
curl -sSL https://dot.net/v1/dotnet-core-uninstall.sh | bash

# Windows PowerShell
Invoke-WebRequest -Uri "https://dot.net/v1/dotnet-core-uninstall.ps1" -OutFile "dotnet-core-uninstall.ps1"
.\dotnet-core-uninstall.ps1
```

#### Common Usage Examples

```bash
# List all installed .NET versions
dotnet-core-uninstall list

# Remove all .NET 9.x versions while keeping .NET 8.x
dotnet-core-uninstall remove --sdk --version-regex "^9\."
dotnet-core-uninstall remove --runtime --version-regex "^9\."

# Remove specific version
dotnet-core-uninstall remove --sdk --version 9.0.0

# Remove all preview versions but keep latest stable
dotnet-core-uninstall remove --all-previews-but-latest

# Verify current version after cleanup
dotnet --version
dotnet --list-sdks
dotnet --list-runtimes
```

#### Version Compatibility Issues

If you encounter build errors related to .NET versions:

1. **Check your global.json** (if exists):
   ```json
   {
     "sdk": {
       "version": "8.0.404"
     }
   }
   ```

2. **Verify project targets**:
   ```xml
   <TargetFramework>net8.0</TargetFramework>
   ```

3. **Clear and restore**:
   ```bash
   dotnet clean
   dotnet restore
   dotnet build
   ```

### Common Build Issues

- **"SDK not found"**: Use `dotnet-core-uninstall` to clean up conflicting versions
- **Package version conflicts**: Run `dotnet restore --force` after version cleanup
- **Global tools issues**: Run `dotnet tool update --global dotnet-ef` (or other tools)

## Monitoring and Health Checks

### Health Check Usage

The application includes a single health check endpoint for production monitoring:

```bash
# Test health check locally
curl http://localhost:5034/api/health

# Health check responses
# 200: Healthy - All systems operational
# 207: Degraded - Some non-critical issues
# 503: Unhealthy - Critical failures detected
```

### Docker/Kubernetes Configuration

For container orchestration, use this health check configuration:

```yaml
# Kubernetes deployment example
livenessProbe:
  httpGet:
    path: /api/health
    port: 80
  initialDelaySeconds: 30
  periodSeconds: 10
  failureThreshold: 3

readinessProbe:
  httpGet:
    path: /api/health
    port: 80
  initialDelaySeconds: 5
  periodSeconds: 5
  failureThreshold: 2
```

```dockerfile
# Dockerfile health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=5s --retries=3 \
  CMD curl -f http://localhost:80/api/health || exit 1
```

### Monitoring Integration

The health check endpoint can be integrated with monitoring systems:

- **Load Balancers**: Configure `/api/health` for traffic routing decisions
- **Monitoring Tools**: Use for uptime monitoring and alerting
- **Application Insights**: Automatic health check logging and metrics
- **Alerting**: Set up alerts based on HTTP status codes (503 = critical, 207 = warning)