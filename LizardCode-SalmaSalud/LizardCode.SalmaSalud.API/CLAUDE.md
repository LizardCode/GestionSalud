# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

LizardCode SalmaSalud is a comprehensive healthcare management system built with .NET 8 using Clean Architecture principles. The system manages patient appointments, medical records, billing, and administrative functions for healthcare facilities, with advanced WhatsApp integration for appointment booking automation.

### Architecture Structure

The project follows Clean Architecture with these main layers:

- **LizardCode.SalmaSalud.Frontend** - ASP.NET Core MVC Web Application (main user interface)
- **LizardCode.SalmaSalud.API** - ASP.NET Core Web API for external integrations and mobile access
- **LizardCode.SalmaSalud.Application** - Business logic and application services layer
- **LizardCode.SalmaSalud.Domain** - Core entities, enums, and domain models
- **LizardCode.SalmaSalud.Infrastructure** - Data access layer using Dapper ORM

### Microservices (Separate Solutions)
- **LizadCode.SalmaSalud.Appointments.Service** - Dedicated appointment management service
- **LizadCode.SalmaSalud.Notifications.Service** - Dedicated notification service

### External Framework Dependencies

The system heavily relies on the external **LizardCode-Framework** (located at `../LizardCode-Framework/`) providing:
- **AFIP integration** - Argentine tax authority web services
- **PDF generation** - iTextSharp integration for document generation
- **Excel handling** - NPOI integration for spreadsheet operations
- **Barcode generation** - QR codes and barcodes for documents
- **Chat API integration** - WhatsApp messaging services
- **SendGrid email services** - Transactional email delivery
- **Database utilities** - Dapper ORM extensions and helpers

## Development Commands

### Building the Solution
```bash
# Build entire solution from root directory
dotnet build LizardCode.SalmaSalud.sln

# Build specific projects
dotnet build LizardCode.SalmaSalud.Frontend/LizardCode.SalmaSalud.csproj
dotnet build LizardCode.SalmaSalud.API/LizardCode.SalmaSalud.API.csproj
dotnet build LizardCode.SalmaSalud.Application/LizardCode.SalmaSalud.Application.csproj
```

### Running Applications
```bash
# Run main web application (Frontend) - localhost:5000
dotnet run --project LizardCode.SalmaSalud.Frontend

# Run API service - check launchSettings.json for port configuration
dotnet run --project LizardCode.SalmaSalud.API
```

### Publishing Applications
```bash
# Publish Frontend (includes automated versioning and zip packaging)
dotnet publish LizardCode.SalmaSalud.Frontend/LizardCode.SalmaSalud.csproj -c Release

# Publish API
dotnet publish LizardCode.SalmaSalud.API/LizardCode.SalmaSalud.API.csproj -c Release
```

### Docker Support
```bash
# Build Docker images (Dockerfiles available for both Frontend and API)
docker build -t salmasalud-frontend -f LizardCode.SalmaSalud.Frontend/Dockerfile .
docker build -t salmasalud-api -f LizardCode.SalmaSalud.API/Dockerfile .
```

## Key Business Domains

### Core Healthcare Entities
- **Pacientes** (Patients) - Patient management, medical records, and portal access
- **Profesionales** (Professionals) - Healthcare provider management and scheduling
- **Turnos** (Appointments) - Appointment scheduling and management system
- **Especialidades** (Specialties) - Medical specialties and service definitions
- **Consultorios** (Offices) - Medical office and room management
- **Evoluciones** (Medical Records) - Patient medical history, treatments, and prescriptions

### Financial Management System
- **Complete accounting system** with chart of accounts (Plan de Cuentas)
- **Invoice generation** (ComprobanteVenta/Compra) with AFIP integration
- **Payment processing** (Recibos, OrdenesPago) and bank reconciliation
- **Professional fee management** (LiquidacionesProfesionales)
- **Expense management** (PlanillaGastos) and budget control
- **Tax compliance** with Argentine AFIP web services integration

### Advanced Integration Features

#### WhatsApp Automation (n8n Integration)
The system includes a sophisticated n8n workflow (`n8n/A_SalmaTurnos.json`) for appointment booking:

**Workflow Features:**
- **Conversational interface** for WhatsApp appointment booking
- **Multi-step process:** DNI validation → Specialty selection → Multiple day/time selection → Confirmation
- **State management** using JSONBin for persistent conversation sessions
- **Robust DNI validation** with format cleaning (accepts multiple formats)
- **Multiple selection support** for days and time slots (comma-separated)
- **API integration** with `https://api.salmasalud.com.ar/` endpoints

**Technical Implementation:**
- **Webhook reception** via WAAPI integration
- **Session state management** (INICIO, ESPERANDO_DNI, ESPERANDO_ESPECIALIDAD, etc.)
- **Data validation** and error handling with user guidance
- **Real-time message processing** with typing indicators

#### Other Integrations
- **Chat API integration** for patient communication
- **SendGrid integration** for email notifications and templates
- **AFIP web services** for tax compliance and electronic invoicing

## Development Architecture Patterns

### Domain Layer Structure
- **100+ core entities** covering all business aspects
- **Custom entities** in `EntitiesCustom/` for complex queries and reporting
- **Comprehensive enums** for business rules and validations
- **Separate business entity definitions** for domain logic

### Application Layer Patterns
- **50+ business classes** following single responsibility principle
- **Business logic separation** from controllers and UI concerns
- **Service injection** configured in `Injection.cs`
- **Helper classes** for external API integrations (WAppApiHelper)

### Infrastructure Considerations
- **Dapper ORM** for high-performance data access
- **SQL Server** database with complex relational structure
- **Repository pattern** implementation
- **Connection string management** via configuration

### Frontend Architecture
- **80+ MVC controllers** covering all business domains
- **NLog integration** for comprehensive logging
- **Razor view components** for reusable UI elements
- **Static file versioning** with automated build tasks

## Configuration Management

### Application Settings Structure
- `appsettings.json` - Base configuration and external service settings
- `appsettings.Development.json` - Development environment overrides
- `appsettings.Production.json` - Production environment configuration

### Critical Configuration Areas
- **Database connections** - SQL Server connection strings
- **AFIP integration** - Tax authority service credentials
- **Email services** - SendGrid API configuration
- **Chat API** - WhatsApp integration endpoints
- **JWT authentication** - Security token configuration
- **External APIs** - WappApi and other service integrations

## Deployment and Build System

### Automated Build Features (Frontend)
- **Version auto-incrementing** on Release builds
- **Static file versioning** for cache management
- **Automated zip packaging** for deployment
- **Configuration file management** between environments
- **Cross-platform build support** (Windows/macOS/Linux)

### Pre-compiled Packages
The `publish/` directory contains ready-to-deploy packages with all dependencies included.

## Important Development Notes

### Framework Dependencies
Always ensure the LizardCode-Framework projects are available at the expected relative path (`../LizardCode-Framework/`). The solution has multiple framework project references that are critical for compilation.

### Multi-Project Development Workflow
When making changes affecting multiple layers:
1. **Domain changes** typically require Application layer updates
2. **Application changes** usually need Frontend controller modifications
3. **New entities** require corresponding repository implementations in Infrastructure
4. **Business logic** should remain in Application layer, not controllers
5. **Database changes** may require updates to both Domain entities and custom entities

### Security Considerations
- **JWT authentication** configured for API access
- **API key authentication** available as alternative
- **CORS policy** configured for cross-origin requests
- **HTTPS redirection** enforced in production
- **Sensitive configuration** managed through appsettings

### Healthcare Domain Expertise
This system requires understanding of:
- **Argentine healthcare regulations** and AFIP tax requirements
- **Medical appointment scheduling** patterns and constraints
- **Healthcare billing** and insurance (financiador) integration
- **Patient privacy** and medical record management requirements