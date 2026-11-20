namespace Project.Api;

public class SearchReadDto
{
    public List<StatusDto> StatusDtos { get; set; }
    public List<ItemCategoryDto> Categories { get; set; }
    public List<ItemDto> Items { get; set; }
    public List<ItemDto> ItemCategories { get; set; }
    public List<FirmDto> Firms { get; set; }
    public List<SeizureRequestDetailReadDto> Requests { get; set; }
    public List<string> RoleMembers { get; set; }
}


