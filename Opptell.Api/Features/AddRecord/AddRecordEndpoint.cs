using FastEndpoints;
using Npgsql;

namespace Opptell.Api.Features.AddRecord;

public class AddRecordEndpoint(IConfiguration configuration) : EndpointWithoutRequest
{
    private readonly string _connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING") 
        ?? configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("CONNECTION_STRING environment variable or DefaultConnection not configured");

    public override void Configure()
    {
        Post("/{table}/{value}");
        AuthSchemes("ApiKey");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var table = Route<string>("table");
        var value = Route<double>("value");
        
        Logger.LogInformation("Recording {value} for table {table}", value, table);
        
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(ct);

        await EnsureTableExistsAsync(connection, table, ct);
        await InsertRecordAsync(connection, table, value, ct);

        await Send.NoContentAsync(ct);
    }

    private static async Task EnsureTableExistsAsync(NpgsqlConnection connection, string tableName, CancellationToken cancellationToken)
    {
        var createTableSql = $"""
            CREATE TABLE IF NOT EXISTS "{tableName}" (
                "Timestamp" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW(),
                "Value" DOUBLE PRECISION NOT NULL
            )
            """;

        await using var command = new NpgsqlCommand(createTableSql, connection);
        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    private static async Task InsertRecordAsync(NpgsqlConnection connection, string tableName, double value, CancellationToken cancellationToken)
    {
        var insertSql = $"""INSERT INTO "{tableName}" ("Timestamp", "Value") VALUES (NOW(), @value)""";

        await using var command = new NpgsqlCommand(insertSql, connection);
        command.Parameters.AddWithValue("@value", value);
        await command.ExecuteNonQueryAsync(cancellationToken);
    }
}