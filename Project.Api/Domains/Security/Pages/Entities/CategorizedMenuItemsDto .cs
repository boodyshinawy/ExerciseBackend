namespace Project.Api;

public class CategorizedMenuItemsDto
{
    public List<MenuItemDto> SearchItems { get; set; }
    public List<string> NavigableItems { get; set; }
    public List<MenuItemDto> SidebarItems { get; set; }
}
