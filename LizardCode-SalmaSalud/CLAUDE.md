# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

LizardCode SalmaSalud is a healthcare management system built with .NET 8 using a layered architecture pattern. The system manages patient appointments, medical records, billing, and administrative functions for healthcare facilities.

### Architecture Structure

The project follows Clean Architecture principles with these main layers:

- **LizardCode.SalmaSalud.Frontend** - MVC Web Application (main user interface)
- **LizardCode.SalmaSalud.API** - Web API for external integrations and mobile access
- **LizardCode.SalmaSalud.Application** - Business logic and application services
- **LizardCode.SalmaSalud.Domain** - Core entities, enums, and domain models
- **LizardCode.SalmaSalud.Infrastructure** - Data access layer using Dapper ORM

### External Dependencies

The project relies on an external **LizardCode-Framework** for shared functionality including:
- AFIP integration (Argentine tax authority)
- PDF generation, Excel handling, barcode generation
- Chat API integration, SendGrid email services
- Database utilities and extensions

## Development Commands

### Building the Solution
```bash
# Build entire solution
dotnet build LizardCode.SalmaSalud.sln

# Build specific projects
dotnet build LizardCode.SalmaSalud.Frontend/LizardCode.SalmaSalud.csproj
dotnet build LizardCode.SalmaSalud.API/LizardCode.SalmaSalud.API.csproj
```

### Running Applications
```bash
# Run main web application (Frontend)
dotnet run --project LizardCode.SalmaSalud.Frontend

# Run API service
dotnet run --project LizardCode.SalmaSalud.API
```

### Docker Support
Both Frontend and API projects include Dockerfiles for containerized deployment.

## Key Business Domains

### Core Entities
- **Pacientes** (Patients) - Patient management and medical records
- **Profesionales** (Professionals) - Healthcare provider management  
- **Turnos** (Appointments) - Appointment scheduling and management
- **Especialidades** (Specialties) - Medical specialties and services
- **Consultorios** (Offices) - Medical office and room management
- **Evoluciones** (Medical Records) - Patient medical history and treatments

### Financial Management
- Complete accounting system with chart of accounts
- Invoice generation and billing (ComprobanteVenta/Compra)
- Payment processing (Recibos, OrdenesPago)
- Bank reconciliation and cash flow management
- AFIP integration for tax compliance

### Integration Features
- **n8n workflow automation** (see n8n/A_SalmaTurnos.json) for appointment booking via WhatsApp
- **Chat API integration** for patient communication
- **AFIP web services** for tax and billing compliance
- **Email notifications** via SendGrid

## WhatsApp Automation (n8n Integration)

The system includes a sophisticated n8n workflow (`A_SalmaTurnos.json`) that handles appointment booking through WhatsApp using a conversational interface:

### Workflow Process
1. **Webhook Reception** - Receives WhatsApp messages via WAAPI integration
2. **Message Processing** - Extracts chat data and validates message format
3. **Session Management** - Maintains user conversation state in JSONBin storage
4. **Multi-Step Conversation** - Guides users through appointment booking:
   - DNI validation (7-8 digits, multiple formats accepted)
   - Specialty selection (Cardiología, Clínico, Traumatología, etc.)
   - Multiple day selection (Lunes-Sábado)
   - Multiple time slot selection (07-19 hs in 2-hour blocks)
   - Booking confirmation

### Technical Architecture
- **State Management**: Uses conversation states (INICIO, ESPERANDO_DNI, ESPERANDO_ESPECIALIDAD, etc.)
- **Data Persistence**: JSONBin for session storage across conversation turns
- **API Integration**: Connects to `https://api.salmasalud.com.ar/` endpoints:
  - `/login` - Authentication (admin/1234)
  - `/solicitar` - Submit appointment request
- **WhatsApp API**: WAAPI service for message sending/receiving
- **Multiple Selection Support**: Users can select multiple days and time slots using comma separation

### Key Features
- Robust DNI validation with format cleaning
- Multi-language conversation flow (Spanish)
- Error handling and user guidance
- Session restart capability for repeat users
- Real-time typing indicators
- Formatted response messages with emojis

## Database Architecture

The system uses **SQL Server** with **Dapper ORM** for data access. Key patterns:
- Repository pattern in Infrastructure layer
- Custom entities in EntitiesCustom/ for complex queries
- Stored procedures for reporting and complex operations
- Entity relationships managed through custom business classes

## Configuration

### Application Settings
- `appsettings.json` - Base configuration
- `appsettings.Development.json` - Development environment
- `appsettings.Production.json` - Production environment

### Key Configuration Areas
- Database connection strings
- AFIP integration credentials  
- Email service configuration
- Chat API endpoints
- External framework dependencies

## Important Development Notes

### Framework Dependencies
Always ensure the LizardCode-Framework projects are available and properly referenced. The solution expects framework projects at `../LizardCode-Framework/` relative path.

### Multi-Project Coordination
When making changes affecting multiple layers:
1. Domain changes may require Application layer updates
2. Application changes typically require Frontend controller updates
3. New entities need corresponding repository implementations
4. Business logic should remain in Application layer, not in controllers

### Deployment
The Frontend project includes automated build tasks for:
- Static file versioning
- Zip packaging for deployment
- Configuration file management between environments