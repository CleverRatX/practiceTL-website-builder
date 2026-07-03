namespace PracticeTL.Api.Models;

public class Direction
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Badges { get; set; } = "";
    public string Content { get; set; } = "";
    public int SortOrder { get; set; }
}
