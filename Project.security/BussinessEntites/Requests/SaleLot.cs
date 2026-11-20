namespace Project.Security;

public class SaleLot : BaseAuditEntity
{
    public int SaleEntityId { get; set; }
    public int StatusId { get; set; }
    public decimal? TotalAmountCollected { get; set; }
    public string? DepositReceiptPath { get; set; }
    public bool DepositReceiptIsPdf { get; set; }
    public string? RejectionReason { get; set; }
    public string? Notes { get; set; }
    public DateTime? FinAccepetedDate { get; set; }

    public SaleEntity? SaleEntity { get; set; }
    public ICollection<LotItemAllocation>? LotItemAllocations { get; set; }
}
