public class UserDto
{
    public string Id { get; set; }
    public string? Name { get; set; }
    public string? UserName { get; set; }

    public string? Status { get; set; }
    public List<RoleDto> Roles { get;  set;}

    public string? FirmId { get; set; }
    public string? FirmName { get; set; }

    //public string? FirmName { get; set; }

    //public int SystemInfoId { get; set; }

}
