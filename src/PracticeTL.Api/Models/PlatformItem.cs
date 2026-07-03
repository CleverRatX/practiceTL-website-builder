namespace PracticeTL.Api.Models;

public class PlatformItem
{
    public int Id { get; set; }
    public string Year { get; set; } = "";
    public string Name { get; set; } = "";
    public int Variant { get; set; } = 1;
    public string Icon { get; set; } = "";
    public string Description { get; set; } = "";
    public int SortOrder { get; set; }
}
