using FastEndpoints;
using Npgsql;

namespace Opptell.Api.Features.AddRecord;

public class AddRecordEndpoint(IConfiguration configuration) : Endpoint<AddRecordRequest>
{
    private readonly string _connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING") 
        ?? configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("CONNECTION_STRING environment variable or DefaultConnection not configured");

    public override void Configure()
    {
        Post("/{table}/{value}");
        AuthSchemes("ApiKey");
    }

    public override async Task HandleAsync(AddRecordRequest req, CancellationToken ct)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(ct);

        await EnsureTableExistsAsync(connection, req.Table, ct);
        await InsertRecordAsync(connection, req.Table, req.Value, ct);
            
        await Send.ResponseAsync("", 201, ct);
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