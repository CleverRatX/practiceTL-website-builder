namespace PracticeTL.Api.Models;

public class Hero
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Subtitle { get; set; } = "";
    public List<HeroStat> Stats { get; set; } = new();
}
