namespace Project.Api;

public class SaleLotRepository : BaseRepository<SaleLot>, ISaleLotRepository
{
    private readonly IConfiguration _configuration;

    public SaleLotRepository(ApplicationDbContext context, IConfiguration configuration) : base(context)
    {
        _configuration = configuration;
    }

    public async Task<List<AllocatableItemDto>> GetAllocatableItems()
    {
        try
        {
            return await BuildAllocatableQuery(null, true).ToListAsync();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error occurred while GetAllocatableItems");
            throw new RepositoryException("An error occurred while GetAllocatableItems.", ex);
        }
    }

    public async Task<List<AllocatableItemDto>> GetAllocatableItemsScoped(string? firmCode, bool isGlobal)
    {
        try
        {
            return await BuildAllocatableQuery(firmCode, isGlobal).ToListAsync();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error occurred while GetAllocatableItemsScoped");
            throw new RepositoryException("An error occurred while GetAllocatableItemsScoped.", ex);
        }
    }

    private IQueryable<AllocatableItemDto> BuildAllocatableQuery(string? firmCode, bool isGlobal)
    {
        var query = _context.Set<SeizureRequestDetail>()
            .Include(e => e.Item).ThenInclude(e => e!.ItemCategory)
            .Include(e => e.UnitOfMeasure)
            .Include(e => e.Status)
            .Include(e => e.SeizureRequest)!.ThenInclude(r => r!.Firm)
            .Where(e => e.RemainingQuantity > 0 &&
                        (e.StatusId == (int)SeizureStatus.SalesEntity ||
                         e.StatusId == (int)SeizureStatus.PartiallyAllocated))
            .AsQueryable();

        if (!isGlobal)
        {
            if (string.IsNullOrWhiteSpace(firmCode))
                return Enumerable.Empty<AllocatableItemDto>().AsQueryable();

            var trimmedFirm = firmCode.Trim();
            query = query.Where(e => e.SeizureRequest!.FirmId == trimmedFirm);
        }

        return query
            .OrderByDescending(e => e.SeizureRequest!.RequestDate)
            .Select(e => new AllocatableItemDto
            {
                Id = e.Id,
                SeizureRequestId = e.SeizureRequestId,
                FirmName = e.SeizureRequest != null && e.SeizureRequest.Firm != null ? e.SeizureRequest.Firm.Name : string.Empty,
                RequestDate = e.SeizureRequest.RequestDate,
                ItemId = e.ItemId,
                ItemName = e.ItemName.Length > 0 ? e.ItemName : (e.Item!.Name ?? string.Empty),
                MainCategory = e.Item!.ItemCategory!.Name,
                SubCategory = e.Item.Name,
                UnitPrice = e.UnitPrice,
                CategoryId = e.Item!.ItemCategory!.Id,
                CategoryName = e.Item.ItemCategory.Name,
                CategoryColor = e.Item.ItemCategory.Color,
                Quantity = e.Quantity,
                RemainingQuantity = e.RemainingQuantity,
                UnitOfMeasureName = e.UnitOfMeasure != null ? e.UnitOfMeasure.Name : string.Empty,
                StatusId = e.StatusId,
                StatusName = e.Status!.Name
            });
    }

    public async Task<List<ItemAggregatedSummaryDto>> GetItemsAggregatedSummary(string? firmCode, bool isGlobal)
    {
        try
        {
            var query = _context.Set<SeizureRequestDetail>()
                .Include(e => e.SeizureRequest)
                .Include(e => e.Item).ThenInclude(i => i!.ItemCategory)
                .AsQueryable();

            if (!isGlobal && !string.IsNullOrWhiteSpace(firmCode))
                query = query.Where(e => e.SeizureRequest!.FirmId == firmCode);

            var data = await query.ToListAsync();

            return data
                .GroupBy(e => new
                {
                    MainCategory = e.Item?.ItemCategory?.Name ?? "عام",
                    SubCategory = !string.IsNullOrWhiteSpace(e.ItemName) ? e.ItemName : (e.Item?.Name ?? "صنف")
                })
                .Select(g => new ItemAggregatedSummaryDto
                {
                    MainCategory = g.Key.MainCategory,
                    SubCategory = g.Key.SubCategory,
                    TotalQuantity = g.Sum(e => e.Quantity),
                    TotalAggregatedPrice = g.Sum(e => e.TotalPrice)
                })
                .OrderBy(x => x.MainCategory).ThenBy(x => x.SubCategory)
                .ToList();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error occurred while GetItemsAggregatedSummary");
            throw new RepositoryException("An error occurred while GetItemsAggregatedSummary.", ex);
        }
    }

    public async Task<List<AllocatableItemDto>> GetServicesAgencyQueue(string? firmCode, bool isGlobal)
    {
        try
        {
            var query = _context.Set<SeizureRequestDetail>()
                .Include(e => e.Item).ThenInclude(e => e!.ItemCategory)
                .Include(e => e.Status)
                .Include(e => e.SeizureRequest)!.ThenInclude(r => r!.Firm)
                .Where(e => e.StatusId == (int)SeizureStatus.ServicesAgency)
                .AsQueryable();

            if (!isGlobal && !string.IsNullOrWhiteSpace(firmCode))
                query = query.Where(e => e.SeizureRequest!.FirmId == firmCode);

            return await query
                .OrderByDescending(e => e.SeizureRequest!.RequestDate)
                .Select(e => new AllocatableItemDto
                {
                    Id = e.Id,
                    SeizureRequestId = e.SeizureRequestId,
                    FirmName = e.SeizureRequest != null && e.SeizureRequest.Firm != null ? e.SeizureRequest.Firm.Name : string.Empty,
                    RequestDate = e.SeizureRequest.RequestDate,
                    ItemId = e.ItemId,
                    ItemName = e.ItemName.Length > 0 ? e.ItemName : (e.Item!.Name ?? string.Empty),
                    MainCategory = e.Item!.ItemCategory!.Name,
                    SubCategory = e.Item.Name,
                    UnitPrice = e.UnitPrice,
                    CategoryId = e.Item!.ItemCategory!.Id,
                    CategoryName = e.Item.ItemCategory.Name,
                    CategoryColor = e.Item.ItemCategory.Color,
                    Quantity = e.Quantity,
                    RemainingQuantity = e.RemainingQuantity,
                    StatusId = e.StatusId,
                    StatusName = e.Status!.Name
                })
                .ToListAsync();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error occurred while GetServicesAgencyQueue");
            throw new RepositoryException("An error occurred while GetServicesAgencyQueue.", ex);
        }
    }

    public async Task AssignToServicesAgency(List<AssignToServicesAgencyItemDto> items, string? userName)
    {
        try
        {
            var detailIds = items.Select(i => i.SeizureRequestDetailId).ToList();
            var details = await _context.Set<SeizureRequestDetail>()
                .Where(e => detailIds.Contains(e.Id))
                .AsTracking()
                .ToListAsync();

            foreach (var item in items)
            {
                var detail = details.FirstOrDefault(d => d.Id == item.SeizureRequestDetailId);
                if (detail == null) continue;

                if (!SeizureWorkflow.CanAssignToServicesAgency(detail.StatusId))
                    throw new DomainException($"لا يمكن تخصيص الصنف قبل اعتماد اللجنة الدائمة ({detail.Id})");

                var qty = item.AllocatedQuantity <= 0 ? detail.RemainingQuantity : item.AllocatedQuantity;
                if (qty > detail.RemainingQuantity)
                    throw new DomainException($"الكمية المطلوبة ({qty}) تتجاوز الكمية المتبقية ({detail.RemainingQuantity}) للصنف ({detail.Id})");

                detail.RemainingQuantity -= qty;

                var fromStatus = detail.StatusId;
                detail.StatusId = detail.RemainingQuantity <= 0
                    ? (int)SeizureStatus.ServicesAgency
                    : (int)SeizureStatus.PartiallyAllocated;

                _context.Set<StatusHistory>().Add(SeizureWorkflow.History(
                    SeizureWorkflow.EntitySeizureDetail,
                    detail.Id,
                    fromStatus,
                    detail.StatusId,
                    "AssignServicesAgency",
                    $"إرسال إلى جهاز الخدمات العامة (الكمية: {qty})",
                    userName,
                    detail.SeizureRequestId));
            }

            await SaveChangesAsync();
        }
        catch (DomainException)
        {
            throw;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error occurred while AssignToServicesAgency");
            throw new RepositoryException("An error occurred while AssignToServicesAgency.", ex);
        }
    }

    public async Task PatchUnitPrice(int detailId, decimal unitPrice, string? userName)
    {
        try
        {
            if (unitPrice <= 0)
                throw new DomainException("يجب أن يكون سعر الوحدة أكبر من صفر");

            var detail = await _context.Set<SeizureRequestDetail>()
                .AsTracking()
                .FirstOrDefaultAsync(e => e.Id == detailId)
                ?? throw new DomainException("الصنف غير موجود");

            detail.UnitPrice = unitPrice;
            await SaveChangesAsync();
        }
        catch (DomainException)
        {
            throw;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error occurred while PatchUnitPrice");
            throw new RepositoryException("An error occurred while PatchUnitPrice.", ex);
        }
    }


    public async Task<SaleLotReadDto> CreateLot(CreateSaleLotDto dto, string? userName, string? firmCode = null, bool isGlobal = true)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var saleEntity = await _context.Set<SaleEntity>().FirstOrDefaultAsync(e => e.Id == dto.SaleEntityId)
                ?? throw new DomainException("جهة البيع غير موجودة");

            var grouped = dto.Items
                .GroupBy(i => i.SeizureRequestDetailId)
                .Select(g => new { DetailId = g.Key, Quantity = g.Sum(x => x.AllocatedQuantity) })
                .ToList();

            var detailIds = grouped.Select(g => g.DetailId).ToList();
            var details = await _context.Set<SeizureRequestDetail>()
                .Include(e => e.Status)
                .Include(e => e.SeizureRequest)
                .Where(e => detailIds.Contains(e.Id))
                .AsTracking()
                .ToListAsync();

            if (details.Count != detailIds.Count)
                throw new DomainException("أحد المضبوطات المحددة غير موجود");

            if (!isGlobal)
            {
                if (string.IsNullOrWhiteSpace(firmCode))
                    throw new DomainException("غير مصرح بتكوين هذا اللوط");

                var trimmedFirm = firmCode.Trim();
                if (details.Any(d => d.SeizureRequest?.FirmId != trimmedFirm))
                    throw new DomainException("لا يمكنك تخصيص مضبوطات لا تتبع وحدتك");
            }

            foreach (var item in grouped)
            {
                var detail = details.First(d => d.Id == item.DetailId);
                if (!SeizureWorkflow.CanAllocate(detail.StatusId))
                    throw new DomainException($"لا يمكن تخصيص الصنف قبل اعتماد اللجنة الدائمة ({detail.Id})");
                if (item.Quantity <= 0)
                    throw new DomainException("الكمية المخصصة يجب أن تكون أكبر من صفر");
                if (item.Quantity > detail.RemainingQuantity)
                    throw new DomainException($"الكمية المطلوبة تتجاوز المتبقي للصنف ({detail.RemainingQuantity})");
            }

            var lot = new SaleLot
            {
                SaleEntityId = dto.SaleEntityId,
                StatusId = (int)SaleLotStatus.DispatchedToSaleEntity,
                Notes = dto.Notes,
                LotItemAllocations = grouped.Select(g => new LotItemAllocation
                {
                    SeizureRequestDetailId = g.DetailId,
                    AllocatedQuantity = g.Quantity
                }).ToList()
            };

            await _dbSet.AddAsync(lot);

            foreach (var item in grouped)
            {
                var detail = details.First(d => d.Id == item.DetailId);
                var fromStatus = detail.StatusId;
                detail.RemainingQuantity -= item.Quantity;
                if (saleEntity.IsPublicService)
                {
                    detail.StatusId = detail.RemainingQuantity <= 0
                        ? (int)SeizureStatus.ServicesAgency
                        : (int)SeizureStatus.PartiallyAllocated;
                }
                else
                {
                    detail.StatusId = SeizureWorkflow.AllocationStatus(detail.Quantity, detail.RemainingQuantity);
                }

                _context.Set<StatusHistory>().Add(SeizureWorkflow.History(
                    SeizureWorkflow.EntitySeizureDetail,
                    detail.Id,
                    fromStatus,
                    detail.StatusId,
                    "Allocate",
                    $"تخصيص {item.Quantity} إلى لوط جهة {saleEntity.Name}",
                    userName,
                    detail.SeizureRequestId));
            }

            await SaveChangesAsync();

            _context.Set<StatusHistory>().Add(SeizureWorkflow.History(
                SeizureWorkflow.EntitySaleLot,
                lot.Id,
                0,
                lot.StatusId,
                "CreateLot",
                dto.Notes,
                userName));
            await SaveChangesAsync();
            await transaction.CommitAsync();

            return (await GetLot(lot.Id))!;
        }
        catch (DomainException)
        {
            await transaction.RollbackAsync();
            throw;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            Log.Error(ex, "Error occurred while CreateLot");
            throw new RepositoryException("An error occurred while CreateLot.", ex);
        }
    }

    public async Task<List<SaleLotReadDto>> GetLots(int? statusId, bool? isPublicService = null, int? saleEntityId = null, string? firmCode = null, bool isGlobal = true)
    {
        try
        {
            var query = _dbSet
                .Include(e => e.SaleEntity)
                .Include(e => e.LotItemAllocations!).ThenInclude(a => a.SeizureRequestDetail!).ThenInclude(d => d.Item!).ThenInclude(i => i.ItemCategory)
                .Include(e => e.LotItemAllocations!).ThenInclude(a => a.SeizureRequestDetail!).ThenInclude(d => d.UnitOfMeasure)
                .Include(e => e.LotItemAllocations!).ThenInclude(a => a.SeizureRequestDetail!).ThenInclude(d => d.SeizureRequest!).ThenInclude(r => r!.Firm)
                .AsQueryable();

            if (statusId.HasValue)
                query = query.Where(e => e.StatusId == statusId.Value);

            if (isPublicService.HasValue)
                query = query.Where(e => e.SaleEntity != null && e.SaleEntity.IsPublicService == isPublicService.Value);

            if (saleEntityId.HasValue)
                query = query.Where(e => e.SaleEntityId == saleEntityId.Value);

            if (!isGlobal)
            {
                if (string.IsNullOrWhiteSpace(firmCode))
                    return new List<SaleLotReadDto>();

                var trimmedFirm = firmCode.Trim();
                query = query.Where(e => e.LotItemAllocations!.Any(a => a.SeizureRequestDetail!.SeizureRequest!.FirmId == trimmedFirm));
            }

            var lots = await query.OrderByDescending(e => e.CreatedDate).ToListAsync();

            if (!isGlobal && !string.IsNullOrWhiteSpace(firmCode))
            {
                var trimmedFirm = firmCode.Trim();
                foreach (var lot in lots)
                {
                    if (lot.LotItemAllocations != null)
                    {
                        lot.LotItemAllocations = lot.LotItemAllocations
                            .Where(a => a.SeizureRequestDetail?.SeizureRequest?.FirmId == trimmedFirm)
                            .ToList();
                    }
                }
            }

            return lots.Select(MapLot).ToList();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error occurred while GetLots");
            throw new RepositoryException("An error occurred while GetLots.", ex);
        }
    }

    public async Task<SaleLotReadDto?> GetLot(int id, string? firmCode = null, bool isGlobal = true)
    {
        var lot = await _dbSet
            .Include(e => e.SaleEntity)
            .Include(e => e.LotItemAllocations!).ThenInclude(a => a.SeizureRequestDetail!).ThenInclude(d => d.Item)
            .Include(e => e.LotItemAllocations!).ThenInclude(a => a.SeizureRequestDetail!).ThenInclude(d => d.Item!).ThenInclude(i => i.ItemCategory)
            .Include(e => e.LotItemAllocations!).ThenInclude(a => a.SeizureRequestDetail!).ThenInclude(d => d.UnitOfMeasure)
            .Include(e => e.LotItemAllocations!).ThenInclude(a => a.SeizureRequestDetail!).ThenInclude(d => d.SeizureRequest!).ThenInclude(r => r!.Firm)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (lot == null)
            return null;

        if (!isGlobal)
        {
            if (string.IsNullOrWhiteSpace(firmCode))
                return null;

            var trimmedFirm = firmCode.Trim();
            if (lot.LotItemAllocations == null || !lot.LotItemAllocations.Any(a => a.SeizureRequestDetail?.SeizureRequest?.FirmId == trimmedFirm))
                return null;

            lot.LotItemAllocations = lot.LotItemAllocations
                .Where(a => a.SeizureRequestDetail?.SeizureRequest?.FirmId == trimmedFirm)
                .ToList();
        }

        return MapLot(lot);
    }

    public async Task SubmitSettlement(int id, LotSettlementDto dto, string? userName, string? firmCode = null, bool isGlobal = true)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var lot = await _dbSet
                .Include(e => e.LotItemAllocations!).ThenInclude(a => a.SeizureRequestDetail!).ThenInclude(d => d.SeizureRequest)
                .AsTracking()
                .FirstOrDefaultAsync(e => e.Id == id)
                ?? throw new DomainException("اللوط غير موجود");

            if (!isGlobal && !string.IsNullOrWhiteSpace(firmCode))
            {
                var trimmedFirm = firmCode.Trim();
                if (lot.LotItemAllocations == null || !lot.LotItemAllocations.Any(a => a.SeizureRequestDetail?.SeizureRequest?.FirmId == trimmedFirm))
                    throw new DomainException("غير مصرح بتسوية هذا اللوط");
            }

            if (lot.StatusId is not ((int)SaleLotStatus.DispatchedToSaleEntity or (int)SaleLotStatus.Rejected))
                throw new DomainException("لا يمكن تسجيل البيع إلا للوط لدى جهة البيع أو المرفوض للتصحيح");

            if (dto.Receipt == null || dto.Receipt.Length == 0)
                throw new DomainException("إيصال الإيداع البنكي إلزامي");

            var rawFileName = dto.Receipt.FileName?.Trim('"', ' ') ?? "receipt.pdf";
            var extension = Path.GetExtension(rawFileName).ToLowerInvariant();
            if (string.IsNullOrEmpty(extension) && !string.IsNullOrEmpty(dto.Receipt.ContentType))
            {
                extension = dto.Receipt.ContentType.ToLowerInvariant() switch
                {
                    "application/pdf" => ".pdf",
                    "image/jpeg" => ".jpg",
                    "image/jpg" => ".jpg",
                    "image/png" => ".png",
                    "image/webp" => ".webp",
                    _ => ""
                };
            }

            var allowed = new[] { ".pdf", ".jpg", ".jpeg", ".png", ".webp", ".jfif", ".bmp" };
            if (!allowed.Contains(extension))
                throw new DomainException("صيغة الإيصال يجب أن تكون PDF أو صورة");

            var uploadRoot = _configuration["PhysicalFile:FilePath"];
            var remainingPath = Path.Combine(uploadRoot ?? "Uploads", "SaleLots", id.ToString());
            Directory.CreateDirectory(remainingPath);
            var fileName = $"receipt_{DateTime.UtcNow:yyyyMMddHHmmss}{extension}";
            var filePath = Path.Combine(remainingPath, fileName);
            await using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await dto.Receipt.CopyToAsync(stream);
            }

            var fromStatus = lot.StatusId;
            lot.TotalAmountCollected = dto.TotalAmountCollected;
            lot.DepositReceiptPath = filePath;
            lot.DepositReceiptIsPdf = extension == ".pdf";
            lot.RejectionReason = null;
            lot.StatusId = (int)SaleLotStatus.SubmittedToFinance;

            _context.Set<StatusHistory>().Add(SeizureWorkflow.History(
                SeizureWorkflow.EntitySaleLot,
                lot.Id,
                fromStatus,
                lot.StatusId,
                "SubmitSettlement",
                $"مبلغ {dto.TotalAmountCollected}",
                userName));

            await SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch (DomainException)
        {
            await transaction.RollbackAsync();
            throw;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            Log.Error(ex, "Error occurred while SubmitSettlement");
            throw new RepositoryException("An error occurred while SubmitSettlement.", ex);
        }
    }

    public async Task ApproveSettlement(int id, string? userName)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var lot = await _dbSet
                .Include(e => e.LotItemAllocations!)
                .ThenInclude(a => a.SeizureRequestDetail)
                .AsTracking()
                .FirstOrDefaultAsync(e => e.Id == id)
                ?? throw new DomainException("اللوط غير موجود");

            if (lot.StatusId != (int)SaleLotStatus.SubmittedToFinance)
                throw new DomainException("لا يمكن اعتماد لوط غير مقدم للهيئة المالية");

            var fromStatus = lot.StatusId;
            lot.StatusId = (int)SaleLotStatus.Settled;
            lot.FinAccepetedDate = DateTime.UtcNow;
            _context.Set<StatusHistory>().Add(SeizureWorkflow.History(
                SeizureWorkflow.EntitySaleLot,
                lot.Id,
                fromStatus,
                lot.StatusId,
                "FinanceApprove",
                null,
                userName));

            var requestIds = lot.LotItemAllocations!
                .Select(a => a.SeizureRequestDetail!.SeizureRequestId)
                .Distinct()
                .ToList();

            foreach (var requestId in requestIds)
                await TryCloseSeizure(requestId, userName);

            await SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch (DomainException)
        {
            await transaction.RollbackAsync();
            throw;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            Log.Error(ex, "Error occurred while ApproveSettlement");
            throw new RepositoryException("An error occurred while ApproveSettlement.", ex);
        }
    }

    public async Task RejectSettlement(int id, string reason, string? userName)
    {
        try
        {
            var lot = await _dbSet.AsTracking().FirstOrDefaultAsync(e => e.Id == id)
                ?? throw new DomainException("اللوط غير موجود");

            if (lot.StatusId != (int)SaleLotStatus.SubmittedToFinance)
                throw new DomainException("لا يمكن رفض لوط غير مقدم للهيئة المالية");

            if (string.IsNullOrWhiteSpace(reason))
                throw new DomainException("سبب الرفض إلزامي");

            var fromStatus = lot.StatusId;
            lot.StatusId = (int)SaleLotStatus.Rejected;
            lot.RejectionReason = reason.Trim();
            _context.Set<StatusHistory>().Add(SeizureWorkflow.History(
                SeizureWorkflow.EntitySaleLot,
                lot.Id,
                fromStatus,
                lot.StatusId,
                "FinanceReject",
                reason.Trim(),
                userName));
            await SaveChangesAsync();
        }
        catch (DomainException)
        {
            throw;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error occurred while RejectSettlement");
            throw new RepositoryException("An error occurred while RejectSettlement.", ex);
        }
    }

    public async Task<SaleLotReceiptDto> GetReceipt(int id)
    {
        var lot = await _dbSet.FirstOrDefaultAsync(e => e.Id == id)
            ?? throw new DomainException("اللوط غير موجود");

        if (string.IsNullOrWhiteSpace(lot.DepositReceiptPath) || !File.Exists(lot.DepositReceiptPath))
            throw new DomainException("لا يوجد إيصال مرفوع لهذا اللوط");

        var bytes = await File.ReadAllBytesAsync(lot.DepositReceiptPath);
        var ext = Path.GetExtension(lot.DepositReceiptPath).TrimStart('.');
        var mime = lot.DepositReceiptIsPdf ? "application/pdf" : $"image/{ext}";
        return new SaleLotReceiptDto
        {
            Id = lot.Id,
            IsPdf = lot.DepositReceiptIsPdf,
            FileName = Path.GetFileName(lot.DepositReceiptPath),
            FileData = $"data:{mime};base64,{Convert.ToBase64String(bytes)}"
        };
    }

    public async Task<List<StatusHistoryReadDto>> GetHistory(string entityType, int entityId)
    {
        return await _context.Set<StatusHistory>()
            .Where(e => e.EntityType == entityType && e.EntityId == entityId)
            .OrderByDescending(e => e.CreatedDate)
            .Select(e => new StatusHistoryReadDto
            {
                Id = e.Id,
                EntityType = e.EntityType,
                EntityId = e.EntityId,
                SeizureRequestId = e.SeizureRequestId,
                FromStatusId = e.FromStatusId,
                ToStatusId = e.ToStatusId,
                Action = e.Action,
                Notes = e.Notes,
                UserName = e.UserName,
                CreatedDate = e.CreatedDate
            })
            .ToListAsync();
    }

    private async Task TryCloseSeizure(int seizureRequestId, string? userName)
    {
        var details = await _context.Set<SeizureRequestDetail>()
            .Include(d => d.LotItemAllocations!).ThenInclude(a => a.SaleLot)
            .Where(d => d.SeizureRequestId == seizureRequestId)
            .AsTracking()
            .ToListAsync();

        if (details.Count == 0)
            return;

        // Check if all details of the seizure request are fully allocated and their lots are settled
        var allDetailsFullyAllocated = details.All(d => d.RemainingQuantity == 0 && (d.LotItemAllocations != null && d.LotItemAllocations.Any()));
        var allLotsSettled = details
            .SelectMany(d => d.LotItemAllocations ?? Enumerable.Empty<LotItemAllocation>())
            .All(a => a.SaleLot != null && a.SaleLot.StatusId == (int)SaleLotStatus.Settled);

        if (allDetailsFullyAllocated && allLotsSettled)
        {
            foreach (var detail in details)
            {
                if (detail.StatusId == (int)SeizureStatus.Completed)
                    continue;

                var fromStatus = detail.StatusId;
                detail.StatusId = (int)SeizureStatus.Completed;
                _context.Set<StatusHistory>().Add(SeizureWorkflow.History(
                    SeizureWorkflow.EntitySeizureDetail,
                    detail.Id,
                    fromStatus,
                    detail.StatusId,
                    "Close",
                    "اكتمال الضبطية بعد التصديق على جميع لوطاتها من هيئة الشئون المالية",
                    userName,
                    seizureRequestId));
            }
        }
        else
        {
            // Check per-detail completion: if an individual detail has 0 remaining quantity and all its lots are settled
            foreach (var detail in details)
            {
                if (detail.StatusId == (int)SeizureStatus.Completed)
                    continue;

                var allocations = detail.LotItemAllocations?.ToList() ?? new List<LotItemAllocation>();
                if (detail.RemainingQuantity == 0 && allocations.Count > 0 && allocations.All(a => a.SaleLot != null && a.SaleLot.StatusId == (int)SaleLotStatus.Settled))
                {
                    var fromStatus = detail.StatusId;
                    detail.StatusId = (int)SeizureStatus.Completed;
                    _context.Set<StatusHistory>().Add(SeizureWorkflow.History(
                        SeizureWorkflow.EntitySeizureDetail,
                        detail.Id,
                        fromStatus,
                        detail.StatusId,
                        "Close",
                        "اكتمال الصنف بعد التصديق على لوطاته من هيئة الشئون المالية",
                        userName,
                        seizureRequestId));
                }
            }
        }
    }

    private static SaleLotReadDto MapLot(SaleLot lot)
    {
        var isPublicService = lot.SaleEntity?.IsPublicService ?? false;
        string statusName;
        if (lot.StatusId == (int)SaleLotStatus.DispatchedToSaleEntity)
        {
            statusName = isPublicService ? "جهاز الخدمات العامة" : "لدى جهة البيع";
        }
        else
        {
            statusName = SeizureWorkflow.LotStatusName(lot.StatusId);
        }

        return new SaleLotReadDto
        {
            Id = lot.Id,
            SaleEntityId = lot.SaleEntityId,
            SaleEntityName = lot.SaleEntity?.Name ?? string.Empty,
            IsPublicService = isPublicService,
            StatusId = lot.StatusId,
            StatusName = statusName,
            TotalAmountCollected = lot.TotalAmountCollected,
            HasReceipt = !string.IsNullOrWhiteSpace(lot.DepositReceiptPath),
            DepositReceiptIsPdf = lot.DepositReceiptIsPdf,
            RejectionReason = lot.RejectionReason,
            Notes = lot.Notes,
            CreatedDate = lot.CreatedDate,
            FinAccepetedDate = lot.FinAccepetedDate,
            Items = (lot.LotItemAllocations ?? Enumerable.Empty<LotItemAllocation>()).Select(a => new LotItemAllocationReadDto
            {
                Id = a.Id,
                SeizureRequestDetailId = a.SeizureRequestDetailId,
                SeizureRequestId = a.SeizureRequestDetail?.SeizureRequestId ?? 0,
                FirmName = a.SeizureRequestDetail?.SeizureRequest?.Firm?.Name ?? string.Empty,
                ItemName = a.SeizureRequestDetail?.ItemName.Length > 0
                    ? a.SeizureRequestDetail.ItemName
                    : (a.SeizureRequestDetail?.Item?.Name ?? string.Empty),
                MainCategory = a.SeizureRequestDetail?.Item?.ItemCategory?.Name ?? string.Empty,
                SubCategory = a.SeizureRequestDetail?.Item?.Name ?? string.Empty,
                CategoryName = a.SeizureRequestDetail?.Item?.ItemCategory?.Name ?? string.Empty,
                AllocatedQuantity = a.AllocatedQuantity,
                ItemQuantity = a.SeizureRequestDetail?.Quantity ?? 0,
                UnitOfMeasureName = a.SeizureRequestDetail?.UnitOfMeasure?.Name ?? string.Empty,
                UnitPrice = a.SeizureRequestDetail?.UnitPrice
            }).ToList()
        };
    }
}
