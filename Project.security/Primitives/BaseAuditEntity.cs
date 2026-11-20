namespace Project.Security;

public class BaseAuditEntity: BaseEntity, IAuditableEntity
{
    public DateTime? CreatedDate { get; set; }
    public DateTime? LastModifiedDate { get; set; }
    public string? CreatedBy { get; set; }
    public string? LastModifiedBy { get; set; }
    public StateType OldState { get; set; }
    public StateType NewState { get; set; }
}
