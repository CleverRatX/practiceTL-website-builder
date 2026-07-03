namespace PracticeTL.Api.Models;

public class GalleryItem
{
    public int Id { get; set; }
    public string Type { get; set; } = "img";
    public string ImageUrl { get; set; } = "";
    public string Caption { get; set; } = "";
    public int SortOrder { get; set; }
}
