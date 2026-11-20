namespace Project.Api;

public class SeizureRequestDetailDto : BaseDto
{
    public int SeizureRequestId { get; set; }
    public int ItemId { get; set; }
    public decimal Quantity { get; set; }
    public decimal RemainingQuantity { get; set; }
    public int? StatusId { get; set; }
    public int? UnitOfMeasureId { get; set; }
    public string? UnitOfMeasureName { get; set; }

    // Item metadata
    public string ItemName { get; set; } = string.Empty;
    public decimal? UnitPrice { get; set; }
}

