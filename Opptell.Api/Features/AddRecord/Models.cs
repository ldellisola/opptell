namespace Opptell.Api.Features.AddRecord;

public class AddRecordRequest
{
    public required string Table { get; init; }
    public required double Value { get; init; }
}