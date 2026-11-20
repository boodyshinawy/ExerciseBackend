namespace Project.Security;

public class SeizureRequestDetail : BaseAuditEntity
{

    public int SeizureRequestId { get; set; }
    public int ItemId { get; set; }
    public decimal Quantity { get; set; }
    public decimal RemainingQuantity { get; set; }

    // Item metadata (populated by Border Guard at registration)
    public string ItemName { get; set; } = string.Empty;

    // Pricing (populated by Sub-Committee)
    public decimal? UnitPrice { get; set; }
    public decimal TotalPrice => Quantity * (UnitPrice ?? 0m);

    public int StatusId { get; set; }

    public int? UnitOfMeasureId { get; set; }

    public SeizureRequest? SeizureRequest { get; set; }
    public Item? Item { get; set; }
    public UnitOfMeasure? UnitOfMeasure { get; set; }
    public Status? Status { get; set; }
    public ICollection<RequestDetailAttachment>? RequestDetailAttachments { get; set; }
    public ICollection<LotItemAllocation>? LotItemAllocations { get; set; }


}

