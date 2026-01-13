# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This is a .NET 9 ASP.NET Core Web API for recording and retrieving patient vital signs using InfluxDB v3 as the time-series database. The solution contains a single project `InfluxdbService`.

## Build and Run Commands

```bash
# Build the solution
dotnet build

# Run the service (uses http profile by default)
dotnet run --project InfluxdbService

# Run with HTTPS
dotnet run --project InfluxdbService --launch-profile https

# Restore NuGet packages
dotnet restore
```

## Development URLs

- HTTP: http://localhost:5220
- HTTPS: https://localhost:7170
- API Documentation (Scalar): http://localhost:5220/scalar/v1

## API Endpoints

### Vital Signs API

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/vitalsigns` | Record a vital sign measurement |
| GET | `/vitalsigns/{patientId}` | Get vital signs for a patient |
| GET | `/vitalsigns/{patientId}?range=-24h` | Get vital signs with time range |

### Example Requests

```bash
# Record vital signs
curl -X POST http://localhost:5220/vitalsigns \
  -H "Content-Type: application/json" \
  -d '{"patientId":"patient-001","heartRate":72,"bloodPressureSystolic":120,"bloodPressureDiastolic":80,"temperature":36.6,"oxygenSaturation":98,"respiratoryRate":16}'

# Get vital signs (last hour)
curl http://localhost:5220/vitalsigns/patient-001

# Get vital signs (last 24 hours)
curl http://localhost:5220/vitalsigns/patient-001?range=-24h
```

## Architecture

```
InfluxdbService/
├── Controllers/
│   └── VitalSignsController.cs    # REST API endpoints
├── Models/
│   └── VitalSign.cs               # Data model with InfluxDB annotations
├── Services/
│   └── VitalSignService.cs        # InfluxDB v3 read/write operations
├── Program.cs                     # Entry point with controller mapping
├── appsettings.json               # Base configuration
└── appsettings.Development.json   # Development configuration (with token)
```

## InfluxDB Configuration

This project uses **InfluxDB v3** which requires SQL queries (not Flux). Configuration is in `appsettings.Development.json`:

```json
{
  "InfluxDB": {
    "Url": "http://localhost:8181",
    "Token": "your-api-token",
    "Org": "your-org",
    "Bucket": "vitals"
  }
}
```

### InfluxDB v3 Setup

Create the database if it doesn't exist:

```bash
curl -X POST -H "Authorization: Bearer <token>" \
  -H "Content-Type: application/json" \
  "http://localhost:8181/api/v3/configure/database?format=json" \
  -d '{"db":"vitals"}'
```

## Testing

Use the `InfluxdbService.http` file in VS Code/Rider with REST Client extension for manual API testing.
