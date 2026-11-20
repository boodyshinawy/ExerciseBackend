namespace Project.Api;

public class MenuItemDto
{
    public string Label { get; set; }
    public string Icon { get; set; }
    public string To { get; set; }
    public List<MenuItemDto>? Items { get; set; }


}