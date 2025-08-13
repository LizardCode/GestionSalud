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

The system includes a sophisticated n8n workflow (`n8n/A_SalmaTurnos.json`) that provides a complete automated appointment booking system through WhatsApp using advanced conversational AI.

### Workflow Architecture Overview

**File Location**: `n8n/A_SalmaTurnos.json`
**Primary Function**: Automated medical appointment scheduling via WhatsApp conversational interface

### Technical Implementation Details

#### Core Workflow Nodes
1. **Webhook** - Entry point for WhatsApp messages via WAAPI integration
2. **Message Processing** (`menssageIn`) - Extracts and validates chat data
3. **Session Management** (`Get User Session`) - Retrieves/creates user conversation state
4. **Business Logic Processing** (`Process Step Logic`) - Handles conversation flow and validations
5. **DNI Validation System** - Real-time patient verification against database
6. **API Integration** - Appointment creation and confirmation
7. **Response Formatting** - Message cleanup and WhatsApp-compatible output

#### Conversation State Machine
- **INICIO** - Initial welcome and flow initiation
- **ESPERANDO_DNI** - DNI collection and validation (7-8 digits, auto-cleaning)
- **ESPERANDO_ESPECIALIDAD** - Medical specialty selection
- **ESPERANDO_DIA** - Multiple day selection (Monday-Saturday)
- **ESPERANDO_HORARIO** - Multiple time slot selection (07-19hs, 2-hour blocks)
- **CONFIRMACION** - Final review and confirmation
- **COMPLETADO** - Process completion with restart capability

#### Advanced Features

**Multi-Selection Capability**:
- Users can select multiple days using comma separation (e.g., "1,3,5")
- Multiple time slots supported (e.g., "1,2,4")
- Dynamic combination generation for flexible scheduling

**Robust DNI Validation**:
- Automatic cleaning of dots, spaces, hyphens, and special characters
- Support for multiple input formats (25430441, 25.430.441, 25 430 441)
- Real-time API validation against patient database
- Comprehensive error messaging with examples

**Session Persistence**:
- JSONBin cloud storage for conversation state management
- Session data includes user progress, selections, and metadata
- Automatic cleanup and restart for returning users
- Cross-conversation data persistence

**API Integration Architecture**:
- **Authentication**: Bearer token system with `https://api.salmasalud.com.ar/login`
- **Patient Validation**: GET `/paciente-by-documento/{dni}` for user verification
- **Appointment Creation**: POST `/solicitar` with comprehensive booking data
- **Error Handling**: Graceful fallback for API failures

#### Medical Specialties Configuration
```javascript
"especialidades": [
  {"idEspecialidad": 1, "descripcion": "Cardiología", "turnosIntervalo": 30},
  {"idEspecialidad": 2, "descripcion": "Clínico", "turnosIntervalo": 30},
  {"idEspecialidad": 3, "descripcion": "Traumatología", "turnosIntervalo": 30},
  {"idEspecialidad": 4, "descripcion": "Gastro", "turnosIntervalo": 30},
  {"idEspecialidad": 5, "descripcion": "Otorrinolaringología", "turnosIntervalo": 30}
]
```

#### Schedule Configuration
- **Days**: Monday through Saturday (1-6)
- **Time Slots**: 6 two-hour blocks from 07:00 to 19:00
- **Flexible Selection**: Users can choose multiple combinations

#### WhatsApp Integration (WAAPI)
- **Instance ID**: 76556
- **Message Sending**: `/api/v1/instances/76556/client/action/send-message`
- **Typing Indicators**: `/api/v1/instances/76556/client/action/send-typing`
- **Authentication**: Bearer token system
- **Message Formatting**: Automatic cleanup for WhatsApp compatibility

#### Error Handling and User Experience
- **Input Validation**: Comprehensive validation at each step
- **Error Messages**: Clear, actionable feedback with examples
- **Flow Recovery**: Automatic restart and error correction
- **Progress Persistence**: Users can continue interrupted conversations
- **Multi-language Support**: Spanish conversation flow with emoji support

#### Security and Data Management
- **Token-based Authentication**: Secure API communications
- **Data Validation**: Input sanitization and format verification
- **Session Security**: Isolated user sessions with timeout management
- **API Rate Limiting**: Built-in request throttling and error handling

### Integration Points

**Backend API Endpoints**:
- `POST /login` - System authentication
- `GET /paciente-by-documento/{dni}` - Patient lookup and validation
- `POST /solicitar` - Appointment request submission

**External Services**:
- **WAAPI**: WhatsApp Business API integration
- **JSONBin**: Cloud-based session storage and persistence
- **SalmaSalud API**: Core healthcare management system

### Workflow Execution Flow
1. WhatsApp message triggers webhook
2. Message extraction and chat ID processing
3. Session retrieval/creation from JSONBin
4. Business logic processing based on current state
5. DNI validation against patient database (if applicable)
6. User input validation and response generation
7. Session state update and persistence
8. API calls for appointment creation (final step)
9. Response formatting and WhatsApp message delivery

This implementation represents a production-ready conversational AI system for healthcare appointment management, featuring enterprise-grade error handling, multi-step validation, and seamless integration with existing healthcare infrastructure.

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