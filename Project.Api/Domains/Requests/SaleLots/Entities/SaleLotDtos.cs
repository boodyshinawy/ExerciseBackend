using System.Globalization;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Project.Api;

public class InvariantDecimalModelBinder : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
        if (valueProviderResult == ValueProviderResult.None)
            return Task.CompletedTask;

        var rawValue = valueProviderResult.FirstValue;
        if (string.IsNullOrWhiteSpace(rawValue))
            return Task.CompletedTask;

        var normalized = rawValue.Trim().Replace('٫', '.').Replace(',', '.');
        if (decimal.TryParse(normalized, NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
        {
            bindingContext.Result = ModelBindingResult.Success(result);
        }
        else
        {
            bindingContext.ModelState.TryAddModelError(bindingContext.ModelName, "قيمة المبلغ غير صحيحة");
        }

        return Task.CompletedTask;
    }
}

public class LotAllocationItemDto
{
    public int SeizureRequestDetailId { get; set; }
    public decimal AllocatedQuantity { get; set; }
}

public class AssignToServicesAgencyItemDto
{
    public int SeizureRequestDetailId { get; set; }
    public decimal AllocatedQuantity { get; set; }
}

// Aggregated summary for committee Tab 2
public class ItemAggregatedSummaryDto
{
    public string MainCategory { get; set; } = string.Empty;
    public string SubCategory { get; set; } = string.Empty;
    public decimal TotalQuantity { get; set; }
    public decimal TotalAggregatedPrice { get; set; }
}

// Border Guard bulk submit to sub-committee
public class BulkSubmitToSubCommitteeDto
{
    public List<int> SeizureRequestIds { get; set; } = new();
}

// Sub-Committee price patch
public class PatchUnitPriceDto
{
    public decimal UnitPrice { get; set; }
}

public class CreateSaleLotDto
{
    public int SaleEntityId { get; set; }
    public string? Notes { get; set; }
    public List<LotAllocationItemDto> Items { get; set; } = new();
}

public class LotSettlementDto
{
    [ModelBinder(BinderType = typeof(InvariantDecimalModelBinder))]
    public decimal TotalAmountCollected { get; set; }
    public IFormFile? Receipt { get; set; }
}

public class LotFinanceDecisionDto
{
    public string? Reason { get; set; }
}

public class AllocatableItemDto
{
    public int Id { get; set; }
    public int SeizureRequestId { get; set; }
    public string FirmName { get; set; }
    public DateTime RequestDate { get; set; }
    public int ItemId { get; set; }
    public string ItemName { get; set; }
    public string MainCategory { get; set; } = string.Empty;
    public string SubCategory { get; set; } = string.Empty;
    public decimal? UnitPrice { get; set; }
    public int CategoryId { get; set; }
    public string CategoryName { get; set; }
    public string CategoryColor { get; set; }
    public decimal Quantity { get; set; }
    public decimal RemainingQuantity { get; set; }
    public string? UnitOfMeasureName { get; set; }
    public int StatusId { get; set; }
    public string? StatusName { get; set; }
}

public class LotItemAllocationReadDto
{
    public int Id { get; set; }
    public int SaleLotId { get; set; }
    public string? SaleEntityName { get; set; }
    public bool IsPublicService { get; set; }
    public int SeizureRequestDetailId { get; set; }
    public int SeizureRequestId { get; set; }
    public string FirmName { get; set; } = string.Empty;
    public string ItemName { get; set; } = string.Empty;
    public string MainCategory { get; set; } = string.Empty;
    public string SubCategory { get; set; } = string.Empty;
    public string? SubCategoryName { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public decimal AllocatedQuantity { get; set; }
    public decimal ItemQuantity { get; set; }
    public string? UnitOfMeasureName { get; set; }
    public decimal? UnitPrice { get; set; }
    public decimal TotalEstimatedPrice => AllocatedQuantity * (UnitPrice ?? 0m);
    public int StatusId { get; set; }
    public string? StatusName { get; set; }
}

public class SaleLotReadDto : BaseDto
{
    public int SaleEntityId { get; set; }
    public string SaleEntityName { get; set; }
    public bool IsPublicService { get; set; }
    public int StatusId { get; set; }
    public string StatusName { get; set; }
    public decimal? TotalAmountCollected { get; set; }
    public bool HasReceipt { get; set; }
    public bool DepositReceiptIsPdf { get; set; }
    public string? RejectionReason { get; set; }
    public string? Notes { get; set; }
    public DateTime? CreatedDate { get; set; }
    public DateTime? FinAccepetedDate { get; set; }
    public List<LotItemAllocationReadDto> Items { get; set; } = new();
}

public class SaleLotReceiptDto
{
    public int Id { get; set; }
    public bool IsPdf { get; set; }
    public string FileData { get; set; }
    public string FileName { get; set; }
}

public class StatusHistoryReadDto
{
    public int Id { get; set; }
    public string EntityType { get; set; }
    public int EntityId { get; set; }
    public int? SeizureRequestId { get; set; }
    public int FromStatusId { get; set; }
    public int ToStatusId { get; set; }
    public string Action { get; set; }
    public string? Notes { get; set; }
    public string? UserName { get; set; }
    public DateTime? CreatedDate { get; set; }
}

public class WorkflowSeizureDto
{
    public int Id { get; set; }
    public string FirmId { get; set; }
    public string FirmName { get; set; }
    public DateTime RequestDate { get; set; }
    public string? CaseNumber { get; set; }
    public string? MahdrNumber { get; set; }
    public int StatusId { get; set; }
    public string? StatusName { get; set; }
    public string? TechnicalReport { get; set; }
    public string? SubCommitteeOpinion { get; set; }
    public bool IsLocked { get; set; }
    public int ItemCount { get; set; }
    public int DistinctSubcategoriesCount { get; set; }
    public string? ReturnReasonFromSubCommite { get; set; }
    public string? ReturnReasonFromPermanentCommite { get; set; }
    public string? AmanaAcceptNumber { get; set; }
    public DateTime? AmanaAcceptDate { get; set; }
    public string? SubCommitteeNumber { get; set; }
    public DateTime? SubCommitteeDate { get; set; }
    public string? PermanentCommitteeNumber { get; set; }
    public DateTime? PermanentCommitteeDate { get; set; }
    public List<SeizureRequestDetailReadDto> Items { get; set; } = new();
    public List<LotItemAllocationReadDto> Allocations { get; set; } = new();
}

public class ReturnSeizureDto
{
    public int SeizureRequestId { get; set; }
    public int TargetStatusId { get; set; }
    public string Reason { get; set; }
}

public class ForwardSeizureDto
{
    public int SeizureRequestId { get; set; }
    public string? TechnicalReport { get; set; }
    public string? SubCommitteeOpinion { get; set; }
    public string? SubCommitteeNumber { get; set; }
    public DateTime? SubCommitteeDate { get; set; }
}

public class BulkApproveDto
{
    public List<int> SeizureRequestIds { get; set; } = new();
    public string? AmanaAcceptNumber { get; set; }
    public DateTime? AmanaAcceptDate { get; set; }
    public string? PermanentCommitteeNumber { get; set; }
    public DateTime? PermanentCommitteeDate { get; set; }
}

public class SaleLotDto : BaseDto
{
    public int SaleEntityId { get; set; }
    public int StatusId { get; set; }
    public string? Notes { get; set; }
}
