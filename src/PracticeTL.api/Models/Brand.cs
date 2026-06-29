namespace PracticeTL.Api.Models;

public class Brand
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Logo { get; set; } = "";
    public int SortOrder { get; set; }
}
