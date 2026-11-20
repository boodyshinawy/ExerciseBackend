
namespace Project.Api;
public class PageRoleDto : BaseDto
{
    public string RoleId { get; set; }

    public string PageId { get; set; }

    public bool? CreateAction { get; set; }

    public bool? ReadAction { get; set; }

    public bool? UpdateAction { get; set; }

    public bool? DeleteAction { get; set; }
}
