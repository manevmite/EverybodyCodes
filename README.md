# EveryoneCodes - Backend Architecture

A .NET 9.0 solution for managing and searching camera data with both Web API and CLI interfaces.

## Overview

EveryoneCodes is a camera management system that provides functionality to search and retrieve camera information from embedded CSV data. The solution follows Clean Architecture principles with clear separation of concerns across multiple projects.

## Architecture

The solution follows Clean Architecture principles and is organized into the following projects:

### Core Projects

- **EveryoneCodes.Core** - Domain models, interfaces, and configuration
- **EveryoneCodes.Application** - Business logic and application services
- **EveryoneCodes.Infrastructure** - Data access, parsers, and external dependencies

### Application Projects

- **EveryoneCodes.Api** - Web API for camera management with global exception handling
- **EveryoneCodes.Cli** - Command-line interface for camera search

### Test Projects

- **EveryoneCodes.Api.Tests** - API integration tests
- **EveryoneCodes.Application.Tests** - Application service tests

## Features

### Camera Management
- Search cameras by name (case-insensitive)
- Retrieve all cameras
- Parse camera data from embedded CSV files
- Extract camera codes, names, and coordinates

### API Features
- **Global Exception Handling** - Centralized error handling with structured JSON responses
- **CORS Support** - Configured for Angular development (localhost:4200)
- **OpenAPI Documentation** - Swagger/Scalar API documentation
- **Configuration Management** - Configurable camera store settings with caching support

### Architecture Features
- **Dependency Injection** - Comprehensive DI container setup
- **Modular Design** - Separate parsers, resource readers, and data stores
- **Clean Architecture** - Clear separation of concerns across layers
- **Configuration System** - Flexible settings with environment-specific configurations

### Data Model
Each camera contains:
- **Number**: Numeric identifier extracted from camera code
- **Code**: Camera code (e.g., "UTR-CM-501")
- **Name**: Human-readable camera name
- **Latitude**: Geographic latitude coordinate
- **Longitude**: Geographic longitude coordinate

### Data Source
The application uses embedded CSV data (`cameras-defb.csv`) containing camera information for what appears to be a traffic monitoring system (likely Utrecht, Netherlands based on the "UTR" prefix).

### Error Handling
The API includes a comprehensive global exception handler that:
- Provides structured JSON error responses
- Maps exceptions to appropriate HTTP status codes
- Includes request tracing with TraceId
- Shows detailed error information in development mode
- Returns sanitized error messages in production mode

### Configuration
The application supports flexible configuration through `CameraStoreSettings`:
- **ResourcePath**: Path to embedded CSV resource (default: "Data.cameras-defb.csv")
- **EnableCaching**: Enable/disable caching (default: true)
- **CacheExpiration**: Cache expiration time (default: 30 minutes)

## Getting Started

### Prerequisites
- .NET 9.0 SDK
- Visual Studio 2022 or VS Code (recommended)

### Building the Solution

```bash
# Clone the repository
git clone <repository-url>
cd EveryoneCodes

# Restore dependencies
dotnet restore

# Build the solution
dotnet build
```

### Running the Web API

```bash
# Navigate to the API project
cd EveryoneCodes.Api

# Run the API
dotnet run
```

The API will be available at:
- HTTP: `https://localhost:7028` or `http://localhost:5036`
- Scalar UI: `https://localhost:7028/scalar/v1`
- Root redirect: `https://localhost:7028/` (redirects to Scalar UI in development)

### API Endpoints

#### Get All Cameras
```http
GET /api/cameras
```

#### Search Cameras by Name
```http
GET /api/cameras/search?name={searchTerm}
```

Example:
```http
GET /api/cameras/search?name=Neude
```

#### Error Response Format
When errors occur, the API returns structured JSON responses:
```json
{
  "error": {
    "message": "A required parameter is missing.",
    "details": "Detailed error information (development only)",
    "timestamp": "2024-01-15T10:30:00.000Z",
    "traceId": "0HMQ7Q5Q5Q5Q5Q5Q5Q5Q5Q5Q5Q5Q5"
  }
}
```

### Running the CLI Application

```bash
# Navigate to the CLI project
cd EveryoneCodes.Cli

# Run with search term
dotnet run -- --name "Neude"
# or
dotnet run -- -n "Neude"
```

The CLI will output cameras matching the search term in the format:
```
Number | Code Name | Latitude | Longitude
```

### Running Tests

```bash
# Run all tests
dotnet test

# Run tests for a specific project
dotnet test EveryoneCodes.Api.Tests
dotnet test EveryoneCodes.Application.Tests
```

## Project Structure

```
EveryoneCodes/
├── EveryoneCodes.Core/           # Domain models and interfaces
│   ├── Configuration/
│   │   └── CameraStoreSettings.cs # Configuration settings
│   ├── Models/
│   │   └── Camera.cs            # Camera domain model
│   └── Interfaces/
│       ├── ICameraService.cs    # Camera service interface
│       ├── ICameraStore.cs      # Data store interface
│       ├── ICsvParser.cs        # CSV parser interface
│       └── IResourceReader.cs   # Resource reader interface
├── EveryoneCodes.Application/    # Business logic
│   └── CameraService.cs         # Camera service implementation
├── EveryoneCodes.Infrastructure/ # Data access and external dependencies
│   ├── Data/
│   │   └── cameras-defb.csv     # Embedded camera data
│   ├── Extensions/
│   │   └── ServiceCollectionExtensions.cs # DI configuration
│   ├── Parsers/
│   │   └── CameraCsvParser.cs   # CSV parser implementation
│   ├── ResourceReaders/
│   │   └── EmbeddedResourceReader.cs # Embedded resource reader
│   └── EmbeddedCsvCameraStore.cs # CSV data store implementation
├── EveryoneCodes.Api/            # Web API
│   ├── Controllers/
│   │   └── CamerasController.cs # Camera API endpoints
│   ├── Middleware/
│   │   └── GlobalExceptionHandlerMiddleware.cs # Global error handling
│   └── Program.cs               # API startup configuration
├── EveryoneCodes.Cli/            # Command-line interface
│   ├── Program.cs               # CLI entry point
│   └── SearchRunner.cs          # Search command runner
└── [Test Projects]/             # Unit and integration tests
```

## Dependencies

### NuGet Packages
- **CsvHelper** (33.1.0) - CSV parsing functionality
- **Microsoft.AspNetCore.OpenApi** (9.0.9) - OpenAPI/Swagger support
- **Microsoft.Extensions.Logging** (9.0.9) - Logging framework
- **Scalar.AspNetCore** (2.8.5) - Alternative API documentation UI

## Development

### Adding New Features
1. Define interfaces in `EveryoneCodes.Core`
2. Implement business logic in `EveryoneCodes.Application`
3. Add infrastructure implementations in `EveryoneCodes.Infrastructure`
4. Register services in `ServiceCollectionExtensions`
5. Add API endpoints in `EveryoneCodes.Api`
6. Update CLI commands in `EveryoneCodes.Cli`
7. Add comprehensive tests

### Configuration
Configuration can be customized in `appsettings.json`:
```json
{
  "CameraStore": {
    "ResourcePath": "Data.cameras-defb.csv",
    "EnableCaching": true,
    "CacheExpiration": "00:30:00"
  }
}
```

### Code Style
- Uses nullable reference types
- Implicit usings enabled
- Clean Architecture principles
- Dependency injection throughout
- Comprehensive logging
- Global exception handling
- Structured error responses

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests for new functionality
5. Ensure all tests pass
6. Submit a pull request
