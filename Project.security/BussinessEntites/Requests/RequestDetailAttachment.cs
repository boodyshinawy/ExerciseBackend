namespace Project.Security;

public class RequestDetailAttachment : BaseAuditEntity
{
    public int SeizureRequestDetailId { get; set; }
    public string? FilePath { get; set; }
    public bool IsPdf { get; set; }
    public SeizureRequestDetail? SeizureRequestDetail { get; set; }

}