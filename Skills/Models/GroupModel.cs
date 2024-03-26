namespace Skills.Models;

public class GroupModel
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; }
    public string? Icon { get; set; }
}