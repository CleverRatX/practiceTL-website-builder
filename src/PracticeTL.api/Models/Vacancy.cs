namespace PracticeTL.Api.Models;

public class Vacancy
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Address { get; set; } = "";
    public string Url { get; set; } = "";
    public int SortOrder { get; set; }
}
