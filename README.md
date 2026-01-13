# Vital Signs API

A .NET 9 ASP.NET Core Web API for recording and retrieving patient vital signs using InfluxDB v3 as the time-series database.

## Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [InfluxDB v3](https://www.influxdata.com/)

## Getting Started

### 1. Configure InfluxDB

Update `InfluxdbService/appsettings.Development.json` with your InfluxDB settings:

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

Create the database if it doesn't exist:

```bash
curl -X POST -H "Authorization: Bearer <token>" \
  -H "Content-Type: application/json" \
  "http://localhost:8181/api/v3/configure/database?format=json" \
  -d '{"db":"vitals"}'
```

### 2. Build and Run

```bash
# Restore packages and build
dotnet build

# Run the service
dotnet run --project InfluxdbService
```

The API will be available at:
- HTTP: http://localhost:5220
- HTTPS: https://localhost:7170
- API Documentation (Scalar): http://localhost:5220/scalar/v1

## API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/vitalsigns` | Record a vital sign measurement |
| POST | `/vitalsigns/bulk` | Record multiple vital signs in bulk |
| GET | `/vitalsigns/{patientId}` | Get vital signs for a patient |
| GET | `/vitalsigns/{patientId}?range=-24h` | Get vital signs with time range |

## Usage Examples

### Record a single vital sign

```bash
curl -X POST http://localhost:5220/vitalsigns \
  -H "Content-Type: application/json" \
  -d '{
    "patientId": "patient-001",
    "heartRate": 72,
    "bloodPressureSystolic": 120,
    "bloodPressureDiastolic": 80,
    "temperature": 36.6,
    "oxygenSaturation": 98,
    "respiratoryRate": 16
  }'
```

### Record multiple vital signs in bulk

```bash
curl -X POST http://localhost:5220/vitalsigns/bulk \
  -H "Content-Type: application/json" \
  -d '[
    {
      "patientId": "patient-001",
      "heartRate": 72,
      "bloodPressureSystolic": 120,
      "bloodPressureDiastolic": 80,
      "temperature": 36.6,
      "oxygenSaturation": 98,
      "respiratoryRate": 16
    },
    {
      "patientId": "patient-002",
      "heartRate": 80,
      "bloodPressureSystolic": 130,
      "bloodPressureDiastolic": 85,
      "temperature": 37.0,
      "oxygenSaturation": 97,
      "respiratoryRate": 18
    }
  ]'
```

### Get vital signs for a patient

```bash
# Last hour (default)
curl http://localhost:5220/vitalsigns/patient-001

# Last 24 hours
curl http://localhost:5220/vitalsigns/patient-001?range=-24h
```

## Project Structure

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
└── appsettings.Development.json   # Development configuration
```

## Testing

Use the `InfluxdbService/InfluxdbService.http` file in VS Code or Rider with the REST Client extension for manual API testing.

## License

MIT
