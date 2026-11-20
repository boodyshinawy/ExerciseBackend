namespace Project.Api;

public class UserInformation
{
    public string Name { get; set; }
    public string firmName { get; set; }
    public List<string>? RoleMembers { get; set; }
    public List<RoleWithFlag>? RolesWithFlags { get; set;}
}
