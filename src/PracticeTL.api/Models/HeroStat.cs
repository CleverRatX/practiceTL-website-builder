namespace PracticeTL.Api.Models;

public class HeroStat
{
    public int Id { get; set; }
    public string Type { get; set; } = "text";
    public string Value { get; set; } = "";
    public string Label { get; set; } = "";
    public int SortOrder { get; set; }
    public int HeroId { get; set; }
}
