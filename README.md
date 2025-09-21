# Opptell API

A simple FastEndpoints API for recording timestamped values to PostgreSQL tables with dynamic table creation.

## Features

- **Simple Endpoint**: `POST /{table}/{value}` 
- **Dynamic Tables**: Creates tables automatically if they don't exist
- **API Key Authentication**: Simple X-API-Key header authentication
- **Environment Variables**: Clean configuration using `CONNECTION_STRING` and `ADMIN_TOKEN`
- **Docker Ready**: Containerized with PostgreSQL
- **Modern C#**: Uses latest C# features (primary constructors, required properties)
- **Vertical Slice Architecture**: Single feature per folder

## Configuration

The application supports **both** configuration methods with environment variables taking precedence:

### Option 1: Environment Variables (Docker/Production)

- **`CONNECTION_STRING`**: PostgreSQL connection string
- **`ADMIN_TOKEN`**: API key for authentication

### Option 2: appsettings.json (Development)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=opptell;Username=postgres;Password=password"
  },
  "Authentication": {
    "AdminToken": "dev-api-key-12345"
  }
}
```

**Priority**: Environment variables override appsettings.json values.

## Docker Deployment

### Quick Start with Docker Compose

```bash
# Clone and run
git clone <repository>
cd opptell
docker-compose up -d
```

### Example Docker Run

```bash
docker run -d \
  -p 8080:8080 \
  -e CONNECTION_STRING="Host=postgres;Database=opptell;Username=postgres;Password=postgres" \
  -e ADMIN_TOKEN="your-secure-token-123" \
  opptell-api
```

## Usage

### Examples

```bash
# Add a record to the 'users' table with value 123.45
curl -X POST "http://localhost:8080/users/123.45" \
  -H "X-API-Key: your-secure-token-123"

# Add a record to the 'sensors' table with value 98.7
curl -X POST "http://localhost:8080/sensors/98.7" \
  -H "X-API-Key: your-secure-token-123"
```

### Responses

- **201 Created**: Record successfully added
- **401 Unauthorized**: Missing or invalid API key
- **500 Internal Server Error**: Database error

### Database Schema

Each table automatically created with:
- `Timestamp` (TIMESTAMP WITH TIME ZONE, defaults to NOW())
- `Value` (DOUBLE PRECISION)

## Local Development

### Option 1: Using appsettings.json (Recommended for development)

```bash
# Configure appsettings.json with your values
dotnet run
```

### Option 2: Using environment variables

```bash
# Set environment variables (overrides appsettings.json)
export CONNECTION_STRING="Host=localhost;Database=opptell;Username=postgres;Password=password"
export ADMIN_TOKEN="dev-token-123"

dotnet run
```

## Architecture

- **Framework**: .NET 10 with FastEndpoints
- **Database**: PostgreSQL with Npgsql  
- **Authentication**: Custom ASP.NET Core authentication handler
- **Configuration**: Simple environment variables (no appsettings.json needed)
- **Deployment**: Docker container with non-root user