namespace PracticeTL.Api.Models;

public class Benefit
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Text { get; set; } = "";
    public int SortOrder { get; set; }
}
