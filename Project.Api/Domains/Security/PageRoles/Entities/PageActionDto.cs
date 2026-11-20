namespace Project.Api;

public class PageActionDto
{
    public string PageName { get; set; }

    public List<string> ActionsName { get; set; } = new List<string>();     
}
