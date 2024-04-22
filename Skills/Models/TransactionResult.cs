using Skills.Models.Enums;

namespace Skills.Models;

public class TransactionResult<T>
{
    public T? Value { get; set; } = default;
    public ImportState State { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? ErrorMessage { get; set; }
}