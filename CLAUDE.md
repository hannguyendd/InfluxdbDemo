# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This is a .NET 9 ASP.NET Core Web API demo project for demonstrating InfluxDB connectivity. The solution contains a single project `InfluxdbService`.

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

## Testing the API

Use the `InfluxdbService.http` file in VS Code/Rider with REST Client extension, or:

```bash
curl http://localhost:5220/weatherforecast
```

## Architecture

- **InfluxdbService/Program.cs**: Main entry point using minimal API pattern with OpenAPI support enabled in development
- **Configuration**: Standard ASP.NET Core configuration via `appsettings.json` and `appsettings.Development.json`

## Adding InfluxDB Integration

To integrate with InfluxDB, add the InfluxDB.Client NuGet package:

```bash
dotnet add InfluxdbService package InfluxDB.Client
```

Configure connection settings in `appsettings.json`:
```json
{
  "InfluxDB": {
    "Url": "http://localhost:8086",
    "Token": "your-token",
    "Org": "your-org",
    "Bucket": "your-bucket"
  }
}
```
