namespace Project.Api;

public class PageDto : BaseDto
{
    public string? Label { get; set; }
    public string? To { get; set; }
    public string? Icon { get; set; }
    public int? ParentId { get; set; }
    public int SortNumber { get; set; }
    public bool IsSearchable { get; set; }
    public bool IsNavigable { get; set; }
    public bool IsSidebarItem { get; set; }


}
