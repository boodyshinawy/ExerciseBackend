namespace Project.Security;

public class ApplicationRole
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public string Name { get; set; } = null!;

    public string? NameAr { get; set; }

    public bool IsDeleted { get; set; }

    public string Color { get; set; } = null!;

    public string? RoleMember { get; set; }

    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

    public ICollection<PageRole> PageRoles { get; set; } = new List<PageRole>();
}
