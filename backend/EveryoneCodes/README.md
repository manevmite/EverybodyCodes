# EveryoneCodes

A .NET 9.0 solution for managing and searching camera data with both Web API and CLI interfaces.

## Overview

EveryoneCodes is a camera management system that provides functionality to search and retrieve camera information from embedded CSV data. The solution follows Clean Architecture principles with clear separation of concerns across multiple projects.

## Architecture

The solution is organized into the following projects:

### Core Projects

- **EveryoneCodes.Core** - Contains domain models and interfaces
- **EveryoneCodes.Application** - Business logic and application services
- **EveryoneCodes.Shared** - Shared functionality including CSV data storage

### Application Projects

- **EveryoneCodes.Api** - Web API for camera management
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

### Data Model
Each camera contains:
- **Number**: Numeric identifier extracted from camera code
- **Code**: Camera code (e.g., "UTR-CM-501")
- **Name**: Human-readable camera name
- **Latitude**: Geographic latitude coordinate
- **Longitude**: Geographic longitude coordinate

### Data Source
The application uses embedded CSV data (`cameras-defb.csv`) containing camera information for what appears to be a traffic monitoring system (likely Utrecht, Netherlands based on the "UTR" prefix).

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
│   ├── Models/
│   │   └── Camera.cs            # Camera domain model
│   └── Interfaces/
│       └── ICameraService.cs    # Camera service interface
├── EveryoneCodes.Application/    # Business logic
│   └── CameraService.cs         # Camera service implementation
├── EveryoneCodes.Shared/         # Shared functionality
│   ├── Data/
│   │   └── cameras-defb.csv     # Embedded camera data
│   ├── Interfaces/
│   │   └── ICameraStore.cs      # Data store interface
│   └── EmbeddedCsvCameraStore.cs # CSV data store implementation
├── EveryoneCodes.Api/            # Web API
│   ├── Controllers/
│   │   └── CamerasController.cs # Camera API endpoints
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
3. Add API endpoints in `EveryoneCodes.Api`
4. Update CLI commands in `EveryoneCodes.Cli`
5. Add comprehensive tests

### Code Style
- Uses nullable reference types
- Implicit usings enabled
- Clean Architecture principles
- Dependency injection throughout
- Comprehensive logging

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests for new functionality
5. Ensure all tests pass
6. Submit a pull request
