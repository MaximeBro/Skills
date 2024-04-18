namespace Skills.Models;

public class CommonTypeModel
{
    public Guid Id { get; set; }
    public Guid ParentId { get; set; }
    public int Level { get; set; }
    public string Value { get; set; } = string.Empty;
    public bool IsSoftLevel { get; init; }
}