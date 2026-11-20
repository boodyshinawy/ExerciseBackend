namespace Project.Api;

public class SeizureRequestRepository : BaseRepository<SeizureRequest>, ISeizureRequestRepository
{
    public SeizureRequestRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async override Task<SeizureRequest> Add(SeizureRequest entity)
    {
        entity.RequestDate = DateTime.SpecifyKind(entity.RequestDate, DateTimeKind.Utc);
        return await base.Add(entity);
    }

    public async override Task<SeizureRequest> Update(SeizureRequest entity)
    {
        entity.RequestDate = DateTime.SpecifyKind(entity.RequestDate, DateTimeKind.Utc);
        return await base.Update(entity);
    }
    public async Task<List<FirmDto>> GetFirms()
    {
        try
        {
            var firms = await _context.Set<Firm>().Select(ca => new FirmDto
            {
                FirmId = ca.Id,
                FirmName = ca.Name,
            })
             .ToListAsync();
            return firms;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error occurred while GetFirms", ex);
            throw new RepositoryException("An error occurred while GetFirms.", ex);
        }
    }

    public async Task<List<SeizureRequestReadDto>> GetSeizureRequests(bool all,string firmCode,List<string> roleMembers)
    {
        try
        {
            List<SeizureRequestReadDto>? seizureRequest;
            if (all)
            {
                seizureRequest = await _dbSet.Include(e => e.Firm).Include(e => e.SeizureRequestDetails).ThenInclude(e => e.Status).Where(e => e.SeizureRequestDetails.Any(e => e.StatusId == (int)SeizureStatus.Registration) || e.SeizureRequestDetails.Count == 0).Select(ca => new SeizureRequestReadDto
                {
                    Id = ca.Id,
                    FirmId = ca.FirmId,
                    FirmName = ca.Firm != null ? ca.Firm.Name : string.Empty,
                    RequestDate = ca.RequestDate,
                    HasSeizuresDetail = ca.SeizureRequestDetails != null ? ca.SeizureRequestDetails.Count() : 0,
                    RoleMembers= roleMembers,
                    HasPendingRequests= ca.SeizureRequestDetails!=null && ca.SeizureRequestDetails.Any(e=>e.StatusId==(int)SeizureStatus.Registration),
                    SeizuresDone= ca.SeizureRequestDetails!=null ?ca.SeizureRequestDetails.Count(e=>e.StatusId==(int)SeizureStatus.Completed):0,
                    SeizuresRevision= ca.SeizureRequestDetails!=null ?ca.SeizureRequestDetails.Count(e=>e.StatusId >= (int)SeizureStatus.SubCommittee && e.StatusId <= (int)SeizureStatus.FinanceAuthority):0,
                    SeizuresPending= ca.SeizureRequestDetails!=null ?ca.SeizureRequestDetails.Count(e=>e.StatusId == (int)SeizureStatus.Registration):0,
                    CaseNumber = ca.CaseNumber,
                    MahdrNumber = ca.MahdrNumber,
                    ReturnReasonFromSubCommite = ca.ReturnReasonFromSubCommite,
                    ReturnReasonFromPermanentCommite = ca.ReturnReasonFromPermanentCommite,
                    StatusId = ca.SeizureRequestDetails != null && ca.SeizureRequestDetails.Any()
                        ? ca.SeizureRequestDetails.Min(d => d.StatusId)
                        : (int)SeizureStatus.Registration,
                    StatusName = ca.SeizureRequestDetails != null && ca.SeizureRequestDetails.Any()
                        ? ca.SeizureRequestDetails.OrderBy(d => d.StatusId).Select(d => d.Status!.Name).FirstOrDefault()
                        : "قيد التسجيل"
                    
                })
                    .ToListAsync();
        }
            else
        {
            seizureRequest = await _dbSet.Include(e => e.Firm).Include(e => e.SeizureRequestDetails).ThenInclude(e => e.Status).Where(e => e.FirmId == firmCode&& (e.SeizureRequestDetails.Any(e => e.StatusId == (int)SeizureStatus.Registration)||e.SeizureRequestDetails.Count==0)).Select(ca => new SeizureRequestReadDto
            {
                Id = ca.Id,
                FirmId = ca.FirmId,
                FirmName = ca.Firm != null ? ca.Firm.Name : string.Empty,
                RequestDate = ca.RequestDate,
                CaseNumber = ca.CaseNumber,
                MahdrNumber = ca.MahdrNumber,
                ReturnReasonFromSubCommite = ca.ReturnReasonFromSubCommite,
                ReturnReasonFromPermanentCommite = ca.ReturnReasonFromPermanentCommite,
                AmanaAcceptNumber = ca.AmanaAcceptNumber,
                AmanaAcceptDate = ca.AmanaAcceptDate,
                SubCommitteeNumber = ca.SubCommitteeNumber,
                SubCommitteeDate = ca.SubCommitteeDate,
                PermanentCommitteeNumber = ca.PermanentCommitteeNumber,
                PermanentCommitteeDate = ca.PermanentCommitteeDate,
                HasSeizuresDetail = ca.SeizureRequestDetails != null ? ca.SeizureRequestDetails.Count() : 0,
                RoleMembers = roleMembers,
                HasPendingRequests = ca.SeizureRequestDetails != null && ca.SeizureRequestDetails.Any(e => e.StatusId == (int)SeizureStatus.Registration),
                SeizuresDone = ca.SeizureRequestDetails != null ? ca.SeizureRequestDetails.Count(e => e.StatusId == (int)SeizureStatus.Completed) : 0,
                SeizuresRevision = ca.SeizureRequestDetails != null ? ca.SeizureRequestDetails.Count(e => e.StatusId >= (int)SeizureStatus.SubCommittee && e.StatusId <= (int)SeizureStatus.FinanceAuthority) : 0,
                SeizuresPending = ca.SeizureRequestDetails != null ? ca.SeizureRequestDetails.Count(e => e.StatusId == (int)SeizureStatus.Registration) : 0,
                StatusId = ca.SeizureRequestDetails != null && ca.SeizureRequestDetails.Any()
                    ? ca.SeizureRequestDetails.Min(d => d.StatusId)
                    : (int)SeizureStatus.Registration,
                StatusName = ca.SeizureRequestDetails != null && ca.SeizureRequestDetails.Any()
                    ? ca.SeizureRequestDetails.OrderBy(d => d.StatusId).Select(d => d.Status!.Name).FirstOrDefault()
                    : "قيد التسجيل"
            })
              .ToListAsync();
        }

        return seizureRequest;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error occurred while GetSeizureRequests", ex);
            throw new RepositoryException("An error occurred while GetSeizureRequests.", ex);
        }
    }

    public async Task ChangeSeizuresUnderRequestToSubCommittee(int id, string? userName)
    {
        try
        {
            var request = await _dbSet.Include(e => e.SeizureRequestDetails).AsTracking().FirstOrDefaultAsync(e => e.Id == id);
            if (request == null)
                throw new DomainException("الضبطية غير موجودة");

            var pending = request.SeizureRequestDetails?
                .Where(e => e.StatusId == (int)SeizureStatus.Registration)
                .ToList() ?? new List<SeizureRequestDetail>();

            if (pending.Count == 0)
                throw new DomainException("لا توجد مضبوطات قيد التسجيل لإرسالها");

            foreach (var item in pending)
            {
                item.StatusId = (int)SeizureStatus.SubCommittee;
                _context.Set<StatusHistory>().Add(SeizureWorkflow.History(
                    SeizureWorkflow.EntitySeizureDetail,
                    item.Id,
                    (int)SeizureStatus.Registration,
                    (int)SeizureStatus.SubCommittee,
                    "Forward",
                    "إرسال إلى اللجنة الفرعية",
                    userName,
                    request.Id));
            }
            await SaveChangesAsync();
        }
        catch (DomainException)
        {
            throw;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error occurred while ChangeSeizuresUnderRequestToSubCommittee", ex);
            throw new RepositoryException("An error occurred while ChangeSeizuresUnderRequestToSubCommittee.", ex);
        }
    }

    public async Task<List<WorkflowSeizureDto>> GetSeizuresByStatus(int statusId)
    {
        try
        {
            var requests = await _dbSet
                .Include(e => e.Firm)
                .Include(e => e.SeizureRequestDetails!).ThenInclude(d => d.Status)
                .Include(e => e.SeizureRequestDetails!).ThenInclude(d => d.UnitOfMeasure)
                .Include(e => e.SeizureRequestDetails!).ThenInclude(d => d.Item!).ThenInclude(i => i.ItemCategory)
                .Include(e => e.SeizureRequestDetails!).ThenInclude(d => d.LotItemAllocations!).ThenInclude(a => a.SaleLot!).ThenInclude(l => l.SaleEntity)
                .Include(e => e.SeizureRequestDetails!).ThenInclude(d => d.RequestDetailAttachments)
                .Where(e => e.SeizureRequestDetails!.Any(d => d.StatusId == statusId))
                .OrderByDescending(e => e.RequestDate)
                .ToListAsync();

            return requests.Select(r => MapSeizure(r, statusId)).ToList();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error occurred while GetSeizuresByStatus");
            throw new RepositoryException("An error occurred while GetSeizuresByStatus.", ex);
        }
    }

    public async Task<List<WorkflowSeizureDto>> GetSeizuresByStatusScoped(int statusId, string? firmCode, bool isGlobal)
    {
        try
        {
            var query = _dbSet
                .Include(e => e.Firm)
                .Include(e => e.SeizureRequestDetails!).ThenInclude(d => d.Status)
                .Include(e => e.SeizureRequestDetails!).ThenInclude(d => d.UnitOfMeasure)
                .Include(e => e.SeizureRequestDetails!).ThenInclude(d => d.Item!).ThenInclude(i => i.ItemCategory)
                .Include(e => e.SeizureRequestDetails!).ThenInclude(d => d.LotItemAllocations!).ThenInclude(a => a.SaleLot!).ThenInclude(l => l.SaleEntity)
                .Include(e => e.SeizureRequestDetails!).ThenInclude(d => d.RequestDetailAttachments)
                .Where(e => e.SeizureRequestDetails!.Any(d => d.StatusId == statusId))
                .AsQueryable();

            if (!isGlobal && !string.IsNullOrWhiteSpace(firmCode))
                query = query.Where(e => e.FirmId == firmCode);

            var requests = await query.OrderByDescending(e => e.RequestDate).ToListAsync();
            return requests.Select(r => MapSeizure(r, statusId)).ToList();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error occurred while GetSeizuresByStatusScoped");
            throw new RepositoryException("An error occurred while GetSeizuresByStatusScoped.", ex);
        }
    }

    public async Task BulkSubmitToSubCommittee(List<int> seizureRequestIds, string? firmCode, string? userName)
    {
        if (seizureRequestIds == null || seizureRequestIds.Count == 0)
            throw new DomainException("يجب اختيار ضبطية واحدة على الأقل");

        var uniqueIds = seizureRequestIds.Distinct().ToList();
        var requests = await _dbSet
            .Include(e => e.SeizureRequestDetails)
            .Where(e => uniqueIds.Contains(e.Id))
            .AsTracking()
            .ToListAsync();

        foreach (var request in requests)
        {
            // Unit ownership check
            if (!string.IsNullOrWhiteSpace(firmCode) && request.FirmId != firmCode)
                throw new DomainException($"الضبطية ({request.Id}) لا تنتمي إلى وحدتك");

            var pending = request.SeizureRequestDetails?
                .Where(e => e.StatusId == (int)SeizureStatus.Registration)
                .ToList() ?? new List<SeizureRequestDetail>();

            if (pending.Count == 0)
                throw new DomainException($"الضبطية ({request.Id}) لا تحتوي على أصناف قيد التسجيل");

            foreach (var item in pending)
            {
                item.StatusId = (int)SeizureStatus.SubCommittee;
                _context.Set<StatusHistory>().Add(SeizureWorkflow.History(
                    SeizureWorkflow.EntitySeizureDetail,
                    item.Id,
                    (int)SeizureStatus.Registration,
                    (int)SeizureStatus.SubCommittee,
                    "Forward",
                    "إرسال مجمع إلى اللجنة الفرعية",
                    userName,
                    request.Id));
            }
        }
        await SaveChangesAsync();
    }


    public async Task ReturnSeizure(ReturnSeizureDto dto, string? userName)
    {
        if (string.IsNullOrWhiteSpace(dto.Reason))
            throw new DomainException("سبب الإرجاع إلزامي");

        var request = await _dbSet.Include(e => e.SeizureRequestDetails).AsTracking().FirstOrDefaultAsync(e => e.Id == dto.SeizureRequestId)
            ?? throw new DomainException("الضبطية غير موجودة");

        var details = request.SeizureRequestDetails?.ToList() ?? new List<SeizureRequestDetail>();
        if (details.Count == 0)
            throw new DomainException("لا توجد مضبوطات");

        var candidates = details
            .Where(d => SeizureWorkflow.TryReturn(d.StatusId, dto.TargetStatusId, out _))
            .ToList();

        if (candidates.Count == 0)
            throw new DomainException("لا توجد مضبوطات قابلة للإرجاع في هذه المرحلة");

        if (candidates.Any(d => SeizureWorkflow.IsImmutable(d.StatusId)))
            throw new DomainException("لا يمكن إرجاع الضبطية بعد اعتماد اللجنة الدائمة");

        if (dto.TargetStatusId == (int)SeizureStatus.Registration)
        {
            request.ReturnReasonFromSubCommite = dto.Reason.Trim();
        }
        else if (dto.TargetStatusId == (int)SeizureStatus.SubCommittee)
        {
            request.ReturnReasonFromPermanentCommite = dto.Reason.Trim();
        }

        foreach (var detail in candidates)
        {
            var fromStatus = detail.StatusId;
            detail.StatusId = dto.TargetStatusId;
            _context.Set<StatusHistory>().Add(SeizureWorkflow.History(
                SeizureWorkflow.EntitySeizureDetail,
                detail.Id,
                fromStatus,
                dto.TargetStatusId,
                "Return",
                dto.Reason.Trim(),
                userName,
                request.Id));
        }

        await SaveChangesAsync();
    }
    public async Task ForwardSeizure(ForwardSeizureDto dto, int targetStatusId, string? userName)
    {
        var request = await _dbSet.Include(e => e.SeizureRequestDetails).AsTracking().FirstOrDefaultAsync(e => e.Id == dto.SeizureRequestId)
            ?? throw new DomainException("الضبطية غير موجودة");

        var details = request.SeizureRequestDetails?.ToList() ?? new List<SeizureRequestDetail>();
        if (details.Count == 0)
            throw new DomainException("لا توجد مضبوطات");

        if (targetStatusId == (int)SeizureStatus.PermanentCommittee)
        {
            if (string.IsNullOrWhiteSpace(dto.SubCommitteeOpinion))
                throw new DomainException("رأي اللجنة الفرعية إلزامي");
            if (string.IsNullOrWhiteSpace(dto.SubCommitteeNumber))
                throw new DomainException("رقم اللجنة الفرعية إلزامي");
            if (!dto.SubCommitteeDate.HasValue)
                throw new DomainException("تاريخ انعقاد اللجنة الفرعية إلزامي");
            if (details.Any(d => d.UnitPrice == null || d.UnitPrice <= 0))
                throw new DomainException("يجب تسعير جميع اصناف الضبطية اولا");

            var subCommDateUtc = dto.SubCommitteeDate.Value.Kind == DateTimeKind.Unspecified
                ? DateTime.SpecifyKind(dto.SubCommitteeDate.Value, DateTimeKind.Utc)
                : dto.SubCommitteeDate.Value.ToUniversalTime();

            request.TechnicalReport = dto.TechnicalReport?.Trim();
            request.SubCommitteeOpinion = dto.SubCommitteeOpinion.Trim();
            request.SubCommitteeNumber = dto.SubCommitteeNumber.Trim();
            request.SubCommitteeDate = subCommDateUtc;
        }

        var expectedFrom = targetStatusId switch
        {
            (int)SeizureStatus.PermanentCommittee => (int)SeizureStatus.SubCommittee,
            (int)SeizureStatus.SalesEntity => (int)SeizureStatus.PermanentCommittee,
            (int)SeizureStatus.ServicesAgency => (int)SeizureStatus.PermanentCommittee,
            _ => throw new DomainException("انتقال غير مسموح")
        };

        var candidates = details.Where(d => d.StatusId == expectedFrom).ToList();
        if (candidates.Count == 0)
            throw new DomainException("لا توجد مضبوطات في الحالة المطلوبة لهذا الإجراء");

        foreach (var detail in candidates)
        {
            if (!SeizureWorkflow.TryForward(detail.StatusId, targetStatusId, out var error))
                throw new DomainException(error ?? "انتقال غير مسموح");

            var fromStatus = detail.StatusId;
            detail.StatusId = targetStatusId;
            if (targetStatusId == (int)SeizureStatus.SalesEntity || targetStatusId == (int)SeizureStatus.ServicesAgency)
                detail.RemainingQuantity = detail.Quantity;

            _context.Set<StatusHistory>().Add(SeizureWorkflow.History(
                SeizureWorkflow.EntitySeizureDetail,
                detail.Id,
                fromStatus,
                targetStatusId,
                targetStatusId == (int)SeizureStatus.SalesEntity ? "Approve" : "Forward",
                dto.SubCommitteeOpinion,
                userName,
                request.Id));
        }

        await SaveChangesAsync();
    }

    public async Task BulkApprove(BulkApproveDto dto, string? userName)
    {
        if (dto.SeizureRequestIds == null || dto.SeizureRequestIds.Count == 0)
            throw new DomainException("يجب اختيار ضبطية واحدة على الأقل");

        var acceptDateUtc = dto.AmanaAcceptDate.HasValue
            ? (dto.AmanaAcceptDate.Value.Kind == DateTimeKind.Unspecified
                ? DateTime.SpecifyKind(dto.AmanaAcceptDate.Value, DateTimeKind.Utc)
                : dto.AmanaAcceptDate.Value.ToUniversalTime())
            : (DateTime?)null;

        var permCommDateUtc = dto.PermanentCommitteeDate.HasValue
            ? (dto.PermanentCommitteeDate.Value.Kind == DateTimeKind.Unspecified
                ? DateTime.SpecifyKind(dto.PermanentCommitteeDate.Value, DateTimeKind.Utc)
                : dto.PermanentCommitteeDate.Value.ToUniversalTime())
            : (DateTime?)null;

        foreach (var id in dto.SeizureRequestIds.Distinct())
        {
            var request = await _dbSet.Include(e => e.SeizureRequestDetails).AsTracking().FirstOrDefaultAsync(e => e.Id == id)
                ?? throw new DomainException("الضبطية غير موجودة");

            if (!string.IsNullOrWhiteSpace(dto.AmanaAcceptNumber))
                request.AmanaAcceptNumber = dto.AmanaAcceptNumber.Trim();
            if (acceptDateUtc.HasValue)
                request.AmanaAcceptDate = acceptDateUtc;

            if (!string.IsNullOrWhiteSpace(dto.PermanentCommitteeNumber))
                request.PermanentCommitteeNumber = dto.PermanentCommitteeNumber.Trim();
            if (permCommDateUtc.HasValue)
                request.PermanentCommitteeDate = permCommDateUtc;

            var details = request.SeizureRequestDetails?.ToList() ?? new List<SeizureRequestDetail>();
            if (details.Count == 0)
                throw new DomainException("لا توجد مضبوطات");

            var candidates = details.Where(d => d.StatusId == (int)SeizureStatus.PermanentCommittee).ToList();
            if (candidates.Count == 0)
                throw new DomainException("لا توجد مضبوطات في الحالة المطلوبة لهذا الإجراء");

            foreach (var detail in candidates)
            {
                if (!SeizureWorkflow.TryForward(detail.StatusId, (int)SeizureStatus.SalesEntity, out var error))
                    throw new DomainException(error ?? "انتقال غير مسموح");

                var fromStatus = detail.StatusId;
                detail.StatusId = (int)SeizureStatus.SalesEntity;
                detail.RemainingQuantity = detail.Quantity;

                _context.Set<StatusHistory>().Add(SeizureWorkflow.History(
                    SeizureWorkflow.EntitySeizureDetail,
                    detail.Id,
                    fromStatus,
                    (int)SeizureStatus.SalesEntity,
                    "Approve",
                    !string.IsNullOrWhiteSpace(dto.AmanaAcceptNumber) ? $"تصديق أمانة: {dto.AmanaAcceptNumber}" : null,
                    userName,
                    request.Id));
            }

            await SaveChangesAsync();
        }
    }

    private static SeizureRequestDetailReadDto MapDetail(SeizureRequestDetail ca, SeizureRequest? parent = null)
    {
        var request = parent ?? ca.SeizureRequest;
        var allocations = (ca.LotItemAllocations ?? Enumerable.Empty<LotItemAllocation>())
            .Select(a => new LotItemAllocationReadDto
            {
                Id = a.Id,
                SaleLotId = a.SaleLotId,
                SaleEntityName = a.SaleLot?.SaleEntity?.Name ?? string.Empty,
                IsPublicService = a.SaleLot?.SaleEntity?.IsPublicService ?? false,
                SeizureRequestDetailId = a.SeizureRequestDetailId,
                SeizureRequestId = ca.SeizureRequestId,
                FirmName = request?.Firm?.Name ?? ca.SeizureRequest?.Firm?.Name ?? string.Empty,
                ItemName = ca.ItemName.Length > 0 ? ca.ItemName : (ca.Item?.Name ?? string.Empty),
                MainCategory = ca.Item?.ItemCategory?.Name ?? string.Empty,
                CategoryName = ca.Item?.ItemCategory?.Name ?? string.Empty,
                SubCategory = ca.Item != null ? ca.Item.Name : string.Empty,
                SubCategoryName = ca.Item != null ? ca.Item.Name : string.Empty,
                AllocatedQuantity = a.AllocatedQuantity,
                ItemQuantity = ca.Quantity,
                UnitOfMeasureName = ca.UnitOfMeasure?.Name ?? string.Empty,
                UnitPrice = ca.UnitPrice,
                StatusId = a.SaleLot?.StatusId ?? ca.StatusId,
                StatusName = a.SaleLot?.SaleEntity?.Name ?? ca.Status?.Name ?? "مكتمل"
            })
            .ToList();

        var allocated = allocations.Any()
            ? allocations.Sum(a => a.AllocatedQuantity)
            : (ca.StatusId == (int)SeizureStatus.Completed ? ca.Quantity : ca.Quantity - ca.RemainingQuantity);

        if (allocated <= 0 && ca.StatusId == (int)SeizureStatus.Completed)
            allocated = ca.Quantity;

        return new SeizureRequestDetailReadDto
        {
            Id = ca.Id,
            SeizureRequestId = ca.SeizureRequestId,
            ItemId = ca.ItemId,
            Quantity = ca.Quantity,
            RemainingQuantity = ca.RemainingQuantity,
            AllocatedQuantity = allocated,
            UnitOfMeasureId = ca.UnitOfMeasureId,
            UnitOfMeasureName = ca.UnitOfMeasure?.Name ?? string.Empty,
            IsLocked = SeizureWorkflow.IsImmutable(ca.StatusId),
            ItemName = ca.ItemName.Length > 0 ? ca.ItemName : (ca.Item?.Name ?? string.Empty),
            SubCategory = ca.Item != null ? ca.Item.Name : string.Empty,
            SubCategoryName = ca.Item != null ? ca.Item.Name : string.Empty,
            UnitPrice = ca.UnitPrice,
            TotalPrice = ca.TotalPrice,
            StatusId = ca.StatusId,
            StatusName = ca.Status?.Name,
            CategoryName = ca.Item?.ItemCategory?.Name ?? string.Empty,
            CategoryId = ca.Item?.ItemCategory?.Id ?? 0,
            CategoryColor = ca.Item?.ItemCategory?.Color ?? string.Empty,
            Photos = ca.RequestDetailAttachments?.Count ?? 0,
            MahdrNumber = request?.MahdrNumber,
            CaseNumber = request?.CaseNumber,
            AmanaAcceptNumber = request?.AmanaAcceptNumber,
            AmanaAcceptDate = request?.AmanaAcceptDate,
            SubCommitteeNumber = request?.SubCommitteeNumber,
            SubCommitteeDate = request?.SubCommitteeDate,
            PermanentCommitteeNumber = request?.PermanentCommitteeNumber,
            PermanentCommitteeDate = request?.PermanentCommitteeDate,
            SubCommitteeOpinion = request?.SubCommitteeOpinion,
            TechnicalReport = request?.TechnicalReport,
            ReturnReasonFromPermanentCommite = request?.ReturnReasonFromPermanentCommite,
            ReturnReasonFromSubCommite = request?.ReturnReasonFromSubCommite,
            HasFinanceDecision = false,
            HasFinanceRecieved = false,
            LotItemAllocations = allocations
        };
    }

    private static WorkflowSeizureDto MapSeizure(SeizureRequest r, int statusId)
    {
        var items = (r.SeizureRequestDetails ?? Enumerable.Empty<SeizureRequestDetail>()).ToList();
        var queueItems = items.Where(d => d.StatusId == statusId).ToList();
        var representative = queueItems.FirstOrDefault() ?? items.FirstOrDefault();
        var mappedItems = items.Select(d => MapDetail(d, r)).ToList();
        var allAllocations = mappedItems
            .SelectMany(d => d.LotItemAllocations ?? Enumerable.Empty<LotItemAllocationReadDto>())
            .ToList();

        return new WorkflowSeizureDto
        {
            Id = r.Id,
            FirmId = r.FirmId,
            FirmName = r.Firm?.Name ?? string.Empty,
            RequestDate = r.RequestDate,
            CaseNumber = r.CaseNumber,
            MahdrNumber = r.MahdrNumber,
            ReturnReasonFromSubCommite = r.ReturnReasonFromSubCommite,
            ReturnReasonFromPermanentCommite = r.ReturnReasonFromPermanentCommite,
            AmanaAcceptNumber = r.AmanaAcceptNumber,
            AmanaAcceptDate = r.AmanaAcceptDate,
            SubCommitteeNumber = r.SubCommitteeNumber,
            SubCommitteeDate = r.SubCommitteeDate,
            PermanentCommitteeNumber = r.PermanentCommitteeNumber,
            PermanentCommitteeDate = r.PermanentCommitteeDate,
            StatusId = representative?.StatusId ?? statusId,
            StatusName = representative?.Status?.Name,
            TechnicalReport = r.TechnicalReport,
            SubCommitteeOpinion = r.SubCommitteeOpinion,
            IsLocked = items.Any(d => SeizureWorkflow.IsImmutable(d.StatusId)),
            ItemCount = items.Count,
            DistinctSubcategoriesCount = items.Select(d => d.ItemId).Distinct().Count(),
            Items = mappedItems,
            Allocations = allAllocations
        };
    }

    public async Task<bool> HasImmutableDetails(int requestId)
    {
        return await _context.Set<SeizureRequestDetail>()
            .AnyAsync(d => d.SeizureRequestId == requestId && d.StatusId >= (int)SeizureStatus.SalesEntity);
    }
}
