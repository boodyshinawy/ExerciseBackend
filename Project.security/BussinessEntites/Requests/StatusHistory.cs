namespace Project.Security;

public class StatusHistory : BaseAuditEntity
{
    public string EntityType { get; set; }
    public int EntityId { get; set; }
    public int? SeizureRequestId { get; set; }
    public int FromStatusId { get; set; }
    public int ToStatusId { get; set; }
    public string Action { get; set; }
    public string? Notes { get; set; }
    public string? UserName { get; set; }
}
