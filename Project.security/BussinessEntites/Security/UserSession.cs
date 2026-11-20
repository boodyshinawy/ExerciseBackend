namespace Project.Security;

public class UserSession : BaseEntity
{
    public required string UserId { get; set; } 
    public required string SessionId { get; set; } 
    public bool IsActive { get; set; } 
}