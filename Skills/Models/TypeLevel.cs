namespace Skills.Models;

public class TypeLevel
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TypeId { get; set; }
    public SKillInfo? Type { get; set; }
    public int Level { get; set; }
    public string Value { get; set; } = string.Empty;
}