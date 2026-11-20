using Project.Security;

namespace Project.Api;

public class RoleDto
{
    public string? Id { get; set; }
    public string Name { get; set; }
    public string? NameAr { get; set; }
    public string Color { get; set; }
    public string? RoleMember { get; set; }
}
