using Skills.Models.Enums;

namespace Skills.Models;

public class TransactionResult<T>
{
    public T? Value { get; init; }
    public ImportState State { get; init; }
    public string Message { get; init; } = string.Empty;
    public string? ErrorMessage { get; init; }
}