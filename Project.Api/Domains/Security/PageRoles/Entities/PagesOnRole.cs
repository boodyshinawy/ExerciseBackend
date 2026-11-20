namespace Project.Api;

public class PagesOnRole
{
    public string PageId { get; set; }
    public string? RoleId { get; set; }
    public string? PageName { get; set; }
    public bool CreateAction { get; set; }
    public bool ReadAction { get; set; }
    public bool UpdateAction { get; set; }
    public bool DeleteAction { get; set; }

}