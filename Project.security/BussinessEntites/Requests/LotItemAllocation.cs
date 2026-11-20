namespace Project.Security;

public class LotItemAllocation : BaseAuditEntity
{
    public int SaleLotId { get; set; }
    public int SeizureRequestDetailId { get; set; }
    public decimal AllocatedQuantity { get; set; }

    public SaleLot? SaleLot { get; set; }
    public SeizureRequestDetail? SeizureRequestDetail { get; set; }
}
