namespace Project.Security;
public class PageRole : BaseEntity
{
    public int PageId { get; set; }
    public string? RoleId { get; set; }

    public bool CreateAction { get; set; }

    public bool ReadAction { get; set; }

    public bool UpdateAction { get; set; }

    public bool DeleteAction { get; set; }

    public ApplicationRole? Role { get; set; }

    public Page? Page { get; set; }

}
