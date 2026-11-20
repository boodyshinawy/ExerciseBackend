namespace Project.Api;

public interface ISaleLotRepository : IBaseRepository<SaleLot>
{
    Task<List<AllocatableItemDto>> GetAllocatableItems();
    Task<List<AllocatableItemDto>> GetAllocatableItemsScoped(string? firmCode, bool isGlobal);
    Task<List<ItemAggregatedSummaryDto>> GetItemsAggregatedSummary(string? firmCode, bool isGlobal);
    Task<List<AllocatableItemDto>> GetServicesAgencyQueue(string? firmCode, bool isGlobal);
    Task AssignToServicesAgency(List<AssignToServicesAgencyItemDto> items, string? userName);
    Task PatchUnitPrice(int detailId, decimal unitPrice, string? userName);
    Task<SaleLotReadDto> CreateLot(CreateSaleLotDto dto, string? userName, string? firmCode = null, bool isGlobal = true);
    Task<List<SaleLotReadDto>> GetLots(int? statusId, bool? isPublicService = null, int? saleEntityId = null, string? firmCode = null, bool isGlobal = true);
    Task<SaleLotReadDto?> GetLot(int id, string? firmCode = null, bool isGlobal = true);
    Task SubmitSettlement(int id, LotSettlementDto dto, string? userName, string? firmCode = null, bool isGlobal = true);
    Task ApproveSettlement(int id, string? userName);
    Task RejectSettlement(int id, string reason, string? userName);
    Task<SaleLotReceiptDto> GetReceipt(int id);
    Task<List<StatusHistoryReadDto>> GetHistory(string entityType, int entityId);
}
