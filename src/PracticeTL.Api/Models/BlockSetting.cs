namespace PracticeTL.Api.Models;

public class BlockSetting
{
    public int Id { get; set; }
    public string Key { get; set; } = "";
    public string Title { get; set; } = "";
    public bool Visible { get; set; } = true;
    public int SortOrder { get; set; }
}
