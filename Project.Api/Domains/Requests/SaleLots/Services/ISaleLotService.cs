namespace Project.Api;

public interface ISaleLotService : IBaseService<SaleLot, SaleLotDto>
{
    Task<OperationResult<List<AllocatableItemDto>>> GetAllocatableItems();
    Task<OperationResult<List<AllocatableItemDto>>> GetAllocatableItemsScoped(string? firmCode, bool isGlobal);
    Task<OperationResult<List<ItemAggregatedSummaryDto>>> GetItemsAggregatedSummary(string? firmCode, bool isGlobal);
    Task<OperationResult<List<AllocatableItemDto>>> GetServicesAgencyQueue(string? firmCode, bool isGlobal);
    Task<OperationResult<string>> AssignToServicesAgency(List<AssignToServicesAgencyItemDto> items, string? userName);
    Task<OperationResult<string>> PatchUnitPrice(int detailId, decimal unitPrice, string? userName);
    Task<OperationResult<SaleLotReadDto>> CreateLot(CreateSaleLotDto dto, string? userName, string? firmCode = null, bool isGlobal = true);
    Task<OperationResult<List<SaleLotReadDto>>> GetLots(int? statusId, bool? isPublicService = null, int? saleEntityId = null, string? firmCode = null, bool isGlobal = true);
    Task<OperationResult<SaleLotReadDto>> GetLot(int id, string? firmCode = null, bool isGlobal = true);
    Task<OperationResult<string>> SubmitSettlement(int id, LotSettlementDto dto, string? userName, string? firmCode = null, bool isGlobal = true);
    Task<OperationResult<string>> ApproveSettlement(int id, string? userName);
    Task<OperationResult<string>> RejectSettlement(int id, LotFinanceDecisionDto dto, string? userName);
    Task<OperationResult<SaleLotReceiptDto>> GetReceipt(int id);
    Task<OperationResult<List<StatusHistoryReadDto>>> GetHistory(string entityType, int entityId);
}
