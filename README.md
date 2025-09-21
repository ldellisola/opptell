# Opptell API

A simple FastEndpoints API for recording timestamped values to PostgreSQL tables with dynamic table creation.

## Features

- **Simple Endpoint**: `POST /{table}/{value}` 
- **Dynamic Tables**: Creates tables automatically if they don't exist
- **API Key Authentication**: Simple X-API-Key header authentication
- **Modern C#**: Uses latest C# features (primary constructors, required properties)
- **Vertical Slice Architecture**: Single feature per folder

## Authentication

Uses ASP.NET Core authentication with a custom API key authentication handler. All requests require an API key in the `X-API-Key` header.

### Configuration

Set your API key in `appsettings.json`:

```json
{
  "Authentication": {
    "ApiKey": "your-secure-api-key-here"
  }
}
```

## Usage

### Examples

```bash
# Add a record to the 'users' table with value 123.45
curl -X POST "http://localhost:5000/users/123.45" \
  -H "X-API-Key: dev-api-key-12345"

# Add a record to the 'sensors' table with value 98.7
curl -X POST "http://localhost:5000/sensors/98.7" \
  -H "X-API-Key: dev-api-key-12345"
```

### Responses

- **201 Created**: Record successfully added
- **401 Unauthorized**: Missing or invalid API key
- **500 Internal Server Error**: Database error

### Database Schema

Each table automatically created with:
- `Timestamp` (TIMESTAMP WITH TIME ZONE, defaults to NOW())
- `Value` (DOUBLE PRECISION)

## Setup

1. **Configure PostgreSQL** connection in `appsettings.json`
2. **Set API Key** in `appsettings.json`
3. **Run the application**: `dotnet run`

## Development

- **Framework**: .NET 10 with FastEndpoints
- **Database**: PostgreSQL with Npgsql
- **Architecture**: Vertical slice with minimal abstractions