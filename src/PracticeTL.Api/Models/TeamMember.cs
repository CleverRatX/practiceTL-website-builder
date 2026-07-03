namespace PracticeTL.Api.Models;

public class TeamMember
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Position { get; set; } = "";
    public string Photo { get; set; } = "";
    public int SortOrder { get; set; }
}
