namespace Project.Api;

public class SeizureRequestDetailReadDto : BaseDto
{
    public int SeizureRequestId { get; set; }
    public int ItemId { get; set; }
    public decimal Quantity { get; set; }
    public decimal RemainingQuantity { get; set; }
    public decimal AllocatedQuantity { get; set; }
    public bool IsLocked { get; set; }
    public string ItemName { get; set; }
    public string? SubCategory { get; set; }
    public string? SubCategoryName { get; set; }
    public decimal? UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
    public int? UnitOfMeasureId { get; set; }
    public string? UnitOfMeasureName { get; set; }
    public string CategoryName { get; set; }
    public string? FirmName { get; set; }
    public int CategoryId { get; set; }
    public string CategoryColor { get; set; }
    public int StatusId { get; set; }
    public string? StatusName { get; set; }
    public int? Photos { get; set; }
    public List<string>? RoleMembers { get; set; }
    public bool HasFinanceRecieved { get; set; }
    public bool HasFinanceDecision { get; set; }
    public DateTime? RequestDate { get; set; }
    public DateTime? CreatedDate { get; set; }
    public string? MahdrNumber { get; set; }
    public string? CaseNumber { get; set; }
    public string? AmanaAcceptNumber { get; set; }
    public DateTime? AmanaAcceptDate { get; set; }
    public string? SubCommitteeNumber { get; set; }
    public DateTime? SubCommitteeDate { get; set; }
    public string? PermanentCommitteeNumber { get; set; }
    public DateTime? PermanentCommitteeDate { get; set; }
    public string? SubCommitteeOpinion { get; set; }
    public string? TechnicalReport { get; set; }
    public string? ReturnReasonFromPermanentCommite { get; set; }
    public string? ReturnReasonFromSubCommite { get; set; }
    public List<LotItemAllocationReadDto>? LotItemAllocations { get; set; }
}


public class LookupsForRequest
{
    public List<ItemCategoryDto> ItemCategories { get; set; }
    public List<RankDto> Ranks { get; set; }
    public List<ItemDto> Items { get; set; }
    public List<UnitOfMeasureDto> UnitOfMeasures { get; set; } = new();

}

public class SeizureDetailDto
{
    public List<SeizureRequestDetailReadDto>? SeizureRequestDetailReadDtos { get; set; }
    public List<string>? RoleMembers { get; set; }
}
