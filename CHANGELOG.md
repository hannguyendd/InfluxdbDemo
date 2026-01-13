# Changelog

## 2026-01-13 - Initial Implementation

### Summary of Actions Performed

#### 1. Initial Setup
| Command | Purpose |
|---------|---------|
| `dotnet add InfluxdbService package InfluxDB.Client` | Added InfluxDB client NuGet package |
| `dotnet add InfluxdbService package Scalar.AspNetCore` | Added Scalar for API documentation |

#### 2. Files Created
| File | Description |
|------|-------------|
| `Models/VitalSign.cs` | Data model with InfluxDB annotations |
| `Services/VitalSignService.cs` | Service for InfluxDB read/write operations |
| `Controllers/VitalSignsController.cs` | REST API controller |

#### 3. Files Modified
| File | Changes |
|------|---------|
| `Program.cs` | Added controllers, Scalar, removed weather forecast |
| `appsettings.json` | Added InfluxDB configuration |
| `InfluxdbService.http` | Updated test examples for vital signs API |

#### 4. InfluxDB v3 Setup
| Command | Purpose |
|---------|---------|
| `curl http://localhost:8181/ping` | Verified InfluxDB v3.8.0 is running |
| `curl .../api/v3/configure/database?format=json` | Listed existing databases |
| `curl -X POST .../api/v3/configure/database -d '{"db":"vitals"}'` | Created `vitals` database |

#### 5. API Testing
| Command | Result |
|---------|--------|
| `POST /vitalsigns` with patient data | 201 Created |
| `GET /vitalsigns/patient-001` | Returned 2 records |

#### 6. Git Commit
| Command | Purpose |
|---------|---------|
| `git add .gitignore CLAUDE.md InfluxdbDemo.sln InfluxdbService/` | Staged files |
| `git commit -m "Add vital signs REST API..."` | Created commit `df35ce6` |

#### Key Fix
Updated `VitalSignService` from **Flux queries** (InfluxDB v2) to **SQL queries** (InfluxDB v3) since the local instance is v3.8.0.
