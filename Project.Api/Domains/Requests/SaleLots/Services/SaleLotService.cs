namespace Project.Api;

public class SaleLotService : BaseService<SaleLot, SaleLotDto>, ISaleLotService
{
    private readonly ISaleLotRepository _saleLotRepository;
    private readonly IValidator<CreateSaleLotDto> _createLotValidator;
    private readonly IValidator<LotSettlementDto> _settlementValidator;
    private readonly IValidator<LotFinanceDecisionDto> _rejectValidator;

    public SaleLotService(
        IValidator<SaleLot> validator,
        ISaleLotRepository repository,
        IMapper mapper,
        IStringLocalizer<SharedService> localizer,
        IValidator<CreateSaleLotDto> createLotValidator,
        IValidator<LotSettlementDto> settlementValidator,
        IValidator<LotFinanceDecisionDto> rejectValidator)
        : base(repository, validator, mapper, localizer)
    {
        _saleLotRepository = repository;
        _createLotValidator = createLotValidator;
        _settlementValidator = settlementValidator;
        _rejectValidator = rejectValidator;
    }

    public async Task<OperationResult<List<AllocatableItemDto>>> GetAllocatableItems()
    {
        try
        {
            var items = await _saleLotRepository.GetAllocatableItems();
            return OperationResult<List<AllocatableItemDto>>.Succeeded("تم استرجاع المضبوطات المتاحة للتخصيص", items);
        }
        catch (Exception)
        {
            return OperationResult<List<AllocatableItemDto>>.InternalServerError(_localizer["UnexpectedError"]);
        }
    }

    public async Task<OperationResult<List<AllocatableItemDto>>> GetAllocatableItemsScoped(string? firmCode, bool isGlobal)
    {
        try
        {
            var items = await _saleLotRepository.GetAllocatableItemsScoped(firmCode, isGlobal);
            return OperationResult<List<AllocatableItemDto>>.Succeeded("تم استرجاع المضبوطات المتاحة للبيع المباشر", items);
        }
        catch (Exception)
        {
            return OperationResult<List<AllocatableItemDto>>.InternalServerError(_localizer["UnexpectedError"]);
        }
    }

    public async Task<OperationResult<List<ItemAggregatedSummaryDto>>> GetItemsAggregatedSummary(string? firmCode, bool isGlobal)
    {
        try
        {
            var summary = await _saleLotRepository.GetItemsAggregatedSummary(firmCode, isGlobal);
            return OperationResult<List<ItemAggregatedSummaryDto>>.Succeeded("تم استرجاع ملخص الأصناف", summary);
        }
        catch (Exception)
        {
            return OperationResult<List<ItemAggregatedSummaryDto>>.InternalServerError(_localizer["UnexpectedError"]);
        }
    }

    public async Task<OperationResult<List<AllocatableItemDto>>> GetServicesAgencyQueue(string? firmCode, bool isGlobal)
    {
        try
        {
            var items = await _saleLotRepository.GetServicesAgencyQueue(firmCode, isGlobal);
            return OperationResult<List<AllocatableItemDto>>.Succeeded("تم استرجاع قائمة انتظار جهاز الخدمات العامة", items);
        }
        catch (Exception)
        {
            return OperationResult<List<AllocatableItemDto>>.InternalServerError(_localizer["UnexpectedError"]);
        }
    }

    public async Task<OperationResult<string>> AssignToServicesAgency(List<AssignToServicesAgencyItemDto> items, string? userName)
    {
        try
        {
            await _saleLotRepository.AssignToServicesAgency(items, userName);
            return OperationResult<string>.Succeeded("تم إرسال الأصناف إلى جهاز الخدمات العامة بنجاح");
        }
        catch (DomainException ex)
        {
            return OperationResult<string>.Failed(ex.Message);
        }
        catch (Exception)
        {
            return OperationResult<string>.InternalServerError(_localizer["UnexpectedError"]);
        }
    }

    public async Task<OperationResult<string>> PatchUnitPrice(int detailId, decimal unitPrice, string? userName)
    {
        try
        {
            await _saleLotRepository.PatchUnitPrice(detailId, unitPrice, userName);
            return OperationResult<string>.Succeeded("تم تحديث سعر الوحدة بنجاح");
        }
        catch (DomainException ex)
        {
            return OperationResult<string>.Failed(ex.Message);
        }
        catch (Exception)
        {
            return OperationResult<string>.InternalServerError(_localizer["UnexpectedError"]);
        }
    }

    public async Task<OperationResult<SaleLotReadDto>> CreateLot(CreateSaleLotDto dto, string? userName, string? firmCode = null, bool isGlobal = true)
    {
        try
        {
            var validation = await _createLotValidator.ValidateAsync(dto);
            if (!validation.IsValid)
                return OperationResult<SaleLotReadDto>.BadRequest(validation.Errors.First().ErrorMessage);

            var lot = await _saleLotRepository.CreateLot(dto, userName, firmCode, isGlobal);
            return OperationResult<SaleLotReadDto>.Succeeded("تم تكوين اللوط بنجاح", lot);
        }
        catch (DomainException ex)
        {
            return OperationResult<SaleLotReadDto>.Failed(ex.Message);
        }
        catch (Exception)
        {
            return OperationResult<SaleLotReadDto>.InternalServerError(_localizer["UnexpectedError"]);
        }
    }

    public async Task<OperationResult<List<SaleLotReadDto>>> GetLots(int? statusId, bool? isPublicService = null, int? saleEntityId = null, string? firmCode = null, bool isGlobal = true)
    {
        try
        {
            var lots = await _saleLotRepository.GetLots(statusId, isPublicService, saleEntityId, firmCode, isGlobal);
            return OperationResult<List<SaleLotReadDto>>.Succeeded("تم استرجاع اللوطات", lots);
        }
        catch (Exception)
        {
            return OperationResult<List<SaleLotReadDto>>.InternalServerError(_localizer["UnexpectedError"]);
        }
    }

    public async Task<OperationResult<SaleLotReadDto>> GetLot(int id, string? firmCode = null, bool isGlobal = true)
    {
        try
        {
            var lot = await _saleLotRepository.GetLot(id, firmCode, isGlobal);
            if (lot == null)
                return OperationResult<SaleLotReadDto>.NotFound("اللوط غير موجود");
            return OperationResult<SaleLotReadDto>.Succeeded("تم استرجاع اللوط", lot);
        }
        catch (Exception)
        {
            return OperationResult<SaleLotReadDto>.InternalServerError(_localizer["UnexpectedError"]);
        }
    }

    public async Task<OperationResult<string>> SubmitSettlement(int id, LotSettlementDto dto, string? userName, string? firmCode = null, bool isGlobal = true)
    {
        try
        {
            var validation = await _settlementValidator.ValidateAsync(dto);
            if (!validation.IsValid)
                return OperationResult<string>.BadRequest(validation.Errors.First().ErrorMessage);

            await _saleLotRepository.SubmitSettlement(id, dto, userName, firmCode, isGlobal);
            return OperationResult<string>.Succeeded("تم إرسال التسوية للهيئة المالية");
        }
        catch (DomainException ex)
        {
            return OperationResult<string>.Failed(ex.Message);
        }
        catch (Exception)
        {
            return OperationResult<string>.InternalServerError(_localizer["UnexpectedError"]);
        }
    }

    public async Task<OperationResult<string>> ApproveSettlement(int id, string? userName)
    {
        try
        {
            await _saleLotRepository.ApproveSettlement(id, userName);
            return OperationResult<string>.Succeeded("تم اعتماد التسوية المالية");
        }
        catch (DomainException ex)
        {
            return OperationResult<string>.Failed(ex.Message);
        }
        catch (Exception)
        {
            return OperationResult<string>.InternalServerError(_localizer["UnexpectedError"]);
        }
    }

    public async Task<OperationResult<string>> RejectSettlement(int id, LotFinanceDecisionDto dto, string? userName)
    {
        try
        {
            var validation = await _rejectValidator.ValidateAsync(dto);
            if (!validation.IsValid)
                return OperationResult<string>.BadRequest(validation.Errors.First().ErrorMessage);

            await _saleLotRepository.RejectSettlement(id, dto.Reason!, userName);
            return OperationResult<string>.Succeeded("تم رفض التسوية وإرجاع اللوط لجهة البيع");
        }
        catch (DomainException ex)
        {
            return OperationResult<string>.Failed(ex.Message);
        }
        catch (Exception)
        {
            return OperationResult<string>.InternalServerError(_localizer["UnexpectedError"]);
        }
    }

    public async Task<OperationResult<SaleLotReceiptDto>> GetReceipt(int id)
    {
        try
        {
            var receipt = await _saleLotRepository.GetReceipt(id);
            return OperationResult<SaleLotReceiptDto>.Succeeded("تم استرجاع الإيصال", receipt);
        }
        catch (DomainException ex)
        {
            return OperationResult<SaleLotReceiptDto>.Failed(ex.Message);
        }
        catch (Exception)
        {
            return OperationResult<SaleLotReceiptDto>.InternalServerError(_localizer["UnexpectedError"]);
        }
    }

    public async Task<OperationResult<List<StatusHistoryReadDto>>> GetHistory(string entityType, int entityId)
    {
        try
        {
            var history = await _saleLotRepository.GetHistory(entityType, entityId);
            return OperationResult<List<StatusHistoryReadDto>>.Succeeded("تم استرجاع السجل", history);
        }
        catch (Exception)
        {
            return OperationResult<List<StatusHistoryReadDto>>.InternalServerError(_localizer["UnexpectedError"]);
        }
    }
}
