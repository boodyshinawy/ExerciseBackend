using Project.Security;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Project.Api;

public class SeizureRequestDetailRepository : BaseRepository<SeizureRequestDetail>, ISeizureRequestDetailRepository
{
    public SeizureRequestDetailRepository(ApplicationDbContext context) : base(context)
    {
    }
    public async Task<List<SeizureRequestDetailReadDto>> GetRequestDetailForRequest(int seizureRequestId, List<string> roleMembers)
    {
        try
        {
            var isBorderGuard = roleMembers.Contains(RoleMember.BorderGuardOffice.ToString());
            var isFinance = roleMembers.Contains(RoleMember.FinanceAuthority.ToString());
            var isAdmin = roleMembers.Contains(RoleMember.Admin.ToString());

            // Border guard works on registration items. Other workflow roles see the full seizure.
            IQueryable<SeizureRequestDetail> query = _dbSet
                .Where(e => e.SeizureRequestId == seizureRequestId)
                .Include(e => e.RequestDetailAttachments)
                .Include(e => e.Item).ThenInclude(e => e.ItemCategory);

            var isSubCommittee = roleMembers.Contains(RoleMember.SubCommittee.ToString());
            var isMainCommittee = roleMembers.Contains(RoleMember.PermanentCommittee.ToString());
            var isSeller = roleMembers.Contains(RoleMember.ServicesAgency.ToString());

            if (isBorderGuard && !isAdmin && !isSubCommittee && !isMainCommittee && !isSeller && !isFinance)
                query = query.Where(e => e.StatusId == (int)SeizureStatus.Registration);

            return await query.OrderByDescending(e => e.CreatedDate).Select(ca => new SeizureRequestDetailReadDto
            {
                Id = ca.Id,
                SeizureRequestId = ca.SeizureRequestId,
                ItemId = ca.ItemId,
                Quantity = ca.Quantity,
                RemainingQuantity = ca.RemainingQuantity,
                IsLocked = ca.StatusId >= (int)SeizureStatus.SalesEntity,
                ItemName = !string.IsNullOrWhiteSpace(ca.ItemName) ? ca.ItemName : (ca.Item != null ? ca.Item.Name : string.Empty),
                SubCategoryName = ca.Item != null ? ca.Item.Name : string.Empty,
                UnitPrice = ca.UnitPrice,
                TotalPrice = ca.Quantity * (ca.UnitPrice ?? 0m),
                UnitOfMeasureId = ca.UnitOfMeasureId,
                UnitOfMeasureName = ca.UnitOfMeasure != null ? ca.UnitOfMeasure.Name : string.Empty,
                StatusId = ca.StatusId,
                CategoryName = ca.Item.ItemCategory.Name,
                CategoryId = ca.Item.ItemCategory.Id,
                CategoryColor = ca.Item.ItemCategory.Color,
                Photos = ca.RequestDetailAttachments != null ? ca.RequestDetailAttachments.Count() : 0,
                RoleMembers = roleMembers,
                HasFinanceDecision = false,
                HasFinanceRecieved = false,
            }).ToListAsync();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error occurred while GetRequestDetailForRequest", ex);
            throw new RepositoryException("An error occurred while GetRequestDetailForRequest.", ex);
        }
    }
    public async Task<LookupsForRequest> GetLookupsForRequest()
    {
        try
        {

            var Ranks = _context.Set<Rank>().Select(e => new RankDto
            {
                Id = e.Id,
                Name = e.Name,
            }).ToList();

            var ItemCategories = _context.Set<ItemCategory>().Select(e => new ItemCategoryDto
            {
                Id = e.Id,
                Name = e.Name,
                Color = e.Color
            }).ToList();

            var Items = _context.Set<Item>().Select(e => new ItemDto
            {
                Id = e.Id,
                Name = e.Name,
                ItemCategoryId = e.ItemCategoryId

            }).ToList();

            var UnitOfMeasures = _context.Set<UnitOfMeasure>().Where(e => !e.IsDeleted).Select(e => new UnitOfMeasureDto
            {
                Id = e.Id,
                Name = e.Name
            }).ToList();

            var lookupsForRequest = new LookupsForRequest
            {
                Ranks = Ranks,
                ItemCategories = ItemCategories,
                Items = Items,
                UnitOfMeasures = UnitOfMeasures
            };
            return lookupsForRequest;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error occurred while lookupsForRequest", ex);
            throw new RepositoryException("An error occurred while lookupsForRequest.", ex);
        }
    }
    public async Task<SeizureRequestDetailReadDto> GetSpecificRequestDetail(int id, List<string> roleMembers)
    {
        try
        {
               var  requestDetail = await _dbSet.Where(e => e.Id == id).Include(e => e.RequestDetailAttachments)
                .Include(e => e.Item).ThenInclude(e => e.ItemCategory).OrderByDescending(e => e.CreatedDate).Select(ca => new SeizureRequestDetailReadDto
                {
                    Id = ca.Id,
                    SeizureRequestId = ca.SeizureRequestId,
                    ItemId = ca.ItemId,
                    Quantity = ca.Quantity,
                    RemainingQuantity = ca.RemainingQuantity,
                    IsLocked = ca.StatusId >= (int)SeizureStatus.SalesEntity,
                    ItemName = !string.IsNullOrWhiteSpace(ca.ItemName) ? ca.ItemName : (ca.Item != null ? ca.Item.Name : string.Empty),
                SubCategoryName = ca.Item != null ? ca.Item.Name : string.Empty,
                    UnitPrice = ca.UnitPrice,
                    TotalPrice = ca.Quantity * (ca.UnitPrice ?? 0m),
                    UnitOfMeasureId = ca.UnitOfMeasureId,
                    UnitOfMeasureName = ca.UnitOfMeasure != null ? ca.UnitOfMeasure.Name : string.Empty,
                    StatusId = ca.StatusId,
                    CategoryName = ca.Item.ItemCategory.Name,
                    CategoryId = ca.Item.ItemCategory.Id,
                    CategoryColor = ca.Item.ItemCategory.Color,
                    Photos = ca.RequestDetailAttachments != null ? ca.RequestDetailAttachments.Count() : 0,
                    RoleMembers = roleMembers,
                    HasFinanceDecision = false,
                    HasFinanceRecieved = false,
                })
                .FirstOrDefaultAsync();


            return requestDetail;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error occurred while GetSpecificRequestDetail", ex);
            throw new RepositoryException("An error occurred while GetSpecificRequestDetail.", ex);
        }
    }
    public async Task<SearchReadDto> SearchReadDto(string firmId,List<string> roleMembers)
    {
        try
        {

            // DbContext is not thread-safe: run queries sequentially (not Task.WhenAll).
            var statusDtos = await _context.Set<Status>().Select(e => new StatusDto
            {
                Id = e.Id,
                Name = e.Name,
            }).ToListAsync();

            var categories = await _context.Set<ItemCategory>().Select(e => new ItemCategoryDto
            {
                Id = e.Id,
                Name = e.Name,
                Color = e.Color
            }).ToListAsync();

            var items = await _context.Set<Item>().Select(e => new ItemDto
            {
                Id = e.Id,
                Name = e.Name,
                ItemCategoryId = e.ItemCategoryId
            }).ToListAsync();

            var firms = await _context.Set<Firm>().Select(e => new FirmDto
            {
                FirmId = e.Id,
                FirmName = e.Name,
            }).ToListAsync();

            List<SeizureRequestDetailReadDto> requestDetails = new List<SeizureRequestDetailReadDto>();
            var currentDate = DateTime.UtcNow;
            var lastWeek=currentDate.AddDays(-7);
            bool isFirmScoped = roleMembers.All(r => r == RoleMember.BorderGuardOffice.ToString() || r == RoleMember.SubCommittee.ToString());
            if (isFirmScoped)
            {
                requestDetails = await _dbSet.Where(e => e.SeizureRequest.FirmId == firmId && e.CreatedDate >= lastWeek && e.CreatedDate <= currentDate).Include(e => e.Status).Include(e=>e.SeizureRequest).Include(e => e.RequestDetailAttachments)
                .Include(e => e.Item).ThenInclude(e => e.ItemCategory).OrderByDescending(e => e.CreatedDate).Select(ca => new SeizureRequestDetailReadDto
                {
                    Id = ca.Id,
                    FirmName = ca.SeizureRequest != null && ca.SeizureRequest.Firm != null ? ca.SeizureRequest.Firm.Name : string.Empty,
                    SeizureRequestId = ca.SeizureRequestId,
                    ItemId = ca.ItemId,
                    Quantity = ca.Quantity,
                    UnitPrice = ca.UnitPrice,
                    TotalPrice = ca.Quantity * (ca.UnitPrice ?? 0m),
                    UnitOfMeasureId = ca.UnitOfMeasureId,
                    UnitOfMeasureName = ca.UnitOfMeasure != null ? ca.UnitOfMeasure.Name : string.Empty,
                    ItemName = !string.IsNullOrWhiteSpace(ca.ItemName) ? ca.ItemName : (ca.Item != null ? ca.Item.Name : string.Empty),
                    SubCategoryName = ca.Item != null ? ca.Item.Name : string.Empty,
                    StatusId = ca.StatusId,
                    StatusName=ca.Status.Name,
                    CategoryName = ca.Item.ItemCategory.Name,
                    CategoryId = ca.Item.ItemCategory.Id,
                    CategoryColor = ca.Item.ItemCategory.Color,
                    Photos = ca.RequestDetailAttachments != null ? ca.RequestDetailAttachments.Count() : 0,
                    CreatedDate = ca.CreatedDate,
                    MahdrNumber = ca.SeizureRequest != null ? ca.SeizureRequest.MahdrNumber : null,
                    CaseNumber = ca.SeizureRequest != null ? ca.SeizureRequest.CaseNumber : null,
                    AmanaAcceptNumber = ca.SeizureRequest != null ? ca.SeizureRequest.AmanaAcceptNumber : null,
                    AmanaAcceptDate = ca.SeizureRequest != null ? ca.SeizureRequest.AmanaAcceptDate : null,
                    SubCommitteeNumber = ca.SeizureRequest != null ? ca.SeizureRequest.SubCommitteeNumber : null,
                    SubCommitteeDate = ca.SeizureRequest != null ? ca.SeizureRequest.SubCommitteeDate : null,
                    PermanentCommitteeNumber = ca.SeizureRequest != null ? ca.SeizureRequest.PermanentCommitteeNumber : null,
                    PermanentCommitteeDate = ca.SeizureRequest != null ? ca.SeizureRequest.PermanentCommitteeDate : null,
                    SubCommitteeOpinion = ca.SeizureRequest != null ? ca.SeizureRequest.SubCommitteeOpinion : null,
                    TechnicalReport = ca.SeizureRequest != null ? ca.SeizureRequest.TechnicalReport : null,
                    ReturnReasonFromPermanentCommite = ca.SeizureRequest != null ? ca.SeizureRequest.ReturnReasonFromPermanentCommite : null,
                    ReturnReasonFromSubCommite = ca.SeizureRequest != null ? ca.SeizureRequest.ReturnReasonFromSubCommite : null,
                    RoleMembers = roleMembers,
                    HasFinanceDecision = false,
                    HasFinanceRecieved = false,
                })
                .ToListAsync();

            }
            else
            {
                requestDetails = await _dbSet.Where(e => e.CreatedDate >= lastWeek && e.CreatedDate <= currentDate && e.StatusId >= (int)SeizureStatus.SubCommittee && e.StatusId <= (int)SeizureStatus.Completed).Include(e => e.Status).Include(e => e.SeizureRequest).Include(e => e.RequestDetailAttachments)
                .Include(e => e.Item).ThenInclude(e => e.ItemCategory).OrderByDescending(e => e.CreatedDate).Select(ca => new SeizureRequestDetailReadDto
                {
                    Id = ca.Id,
                    FirmName = ca.SeizureRequest != null && ca.SeizureRequest.Firm != null ? ca.SeizureRequest.Firm.Name : string.Empty,
                    SeizureRequestId = ca.SeizureRequestId,
                    ItemId = ca.ItemId,
                    Quantity = ca.Quantity,
                    UnitPrice = ca.UnitPrice,
                    TotalPrice = ca.Quantity * (ca.UnitPrice ?? 0m),
                    UnitOfMeasureId = ca.UnitOfMeasureId,
                    UnitOfMeasureName = ca.UnitOfMeasure != null ? ca.UnitOfMeasure.Name : string.Empty,
                    ItemName = !string.IsNullOrWhiteSpace(ca.ItemName) ? ca.ItemName : (ca.Item != null ? ca.Item.Name : string.Empty),
                    SubCategoryName = ca.Item != null ? ca.Item.Name : string.Empty,
                    StatusId = ca.StatusId,
                    StatusName=ca.Status.Name,
                    CategoryName = ca.Item.ItemCategory.Name,
                    CategoryId = ca.Item.ItemCategory.Id,
                    CategoryColor = ca.Item.ItemCategory.Color,
                    Photos = ca.RequestDetailAttachments != null ? ca.RequestDetailAttachments.Count() : 0,
                    CreatedDate = ca.CreatedDate,
                    MahdrNumber = ca.SeizureRequest != null ? ca.SeizureRequest.MahdrNumber : null,
                    CaseNumber = ca.SeizureRequest != null ? ca.SeizureRequest.CaseNumber : null,
                    AmanaAcceptNumber = ca.SeizureRequest != null ? ca.SeizureRequest.AmanaAcceptNumber : null,
                    AmanaAcceptDate = ca.SeizureRequest != null ? ca.SeizureRequest.AmanaAcceptDate : null,
                    SubCommitteeNumber = ca.SeizureRequest != null ? ca.SeizureRequest.SubCommitteeNumber : null,
                    SubCommitteeDate = ca.SeizureRequest != null ? ca.SeizureRequest.SubCommitteeDate : null,
                    PermanentCommitteeNumber = ca.SeizureRequest != null ? ca.SeizureRequest.PermanentCommitteeNumber : null,
                    PermanentCommitteeDate = ca.SeizureRequest != null ? ca.SeizureRequest.PermanentCommitteeDate : null,
                    SubCommitteeOpinion = ca.SeizureRequest != null ? ca.SeizureRequest.SubCommitteeOpinion : null,
                    TechnicalReport = ca.SeizureRequest != null ? ca.SeizureRequest.TechnicalReport : null,
                    ReturnReasonFromPermanentCommite = ca.SeizureRequest != null ? ca.SeizureRequest.ReturnReasonFromPermanentCommite : null,
                    ReturnReasonFromSubCommite = ca.SeizureRequest != null ? ca.SeizureRequest.ReturnReasonFromSubCommite : null,
                    RoleMembers = roleMembers,
                    HasFinanceDecision = false,
                    HasFinanceRecieved = false,
                })
                .ToListAsync();
            }
            var searchReadDto = new SearchReadDto
            {
                Requests = requestDetails,
                Firms = firms,
                Categories = categories,
                Items = items,
                ItemCategories = items,
                StatusDtos = statusDtos,
                RoleMembers=roleMembers
            };
            return searchReadDto;

        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error occurred while SearchReadDto", ex);
            throw new RepositoryException("An error occurred while SearchReadDto.", ex);
        }
    }
    public async override Task<SeizureRequestDetail> Remove(int id, bool physicalDelete = false)
    {
        var entity = await _dbSet.Include(e => e.RequestDetailAttachments).AsTracking().FirstOrDefaultAsync(e => e.Id == id);
        foreach (var attachment in entity?.RequestDetailAttachments)
        {
            attachment.IsDeleted = true;
        }
        entity.IsDeleted = true;
        await SaveChangesAsync();
        return entity;
    }
    public async Task ChangeRequestStatus(ChangeRequestStatus changeRequestStatus, string? userName)
    {
        try
        {
            var request = await _dbSet.AsTracking().FirstOrDefaultAsync(e => e.Id == changeRequestStatus.Id);
            if (request == null)
                throw new DomainException("المضبوط غير موجود");

            if (!Enum.IsDefined(typeof(SeizureStatus), changeRequestStatus.StatusId))
                throw new DomainException("حالة غير صالحة");

            var fromStatus = request.StatusId;
            var toStatus = changeRequestStatus.StatusId;
            if (fromStatus == toStatus)
                return;

            var isReturn = toStatus < fromStatus;

            if (isReturn)
            {
                if (!SeizureWorkflow.TryReturn(fromStatus, toStatus, out var returnError))
                    throw new DomainException(returnError ?? "إرجاع غير مسموح");
                if (string.IsNullOrWhiteSpace(changeRequestStatus.Reason))
                    throw new DomainException("سبب الإرجاع إلزامي");
            }
            else
            {
                if (!SeizureWorkflow.TryForward(fromStatus, toStatus, out var forwardError))
                    throw new DomainException(forwardError ?? "انتقال غير مسموح");
            }

            request.StatusId = toStatus;
            if (isReturn && toStatus == (int)SeizureStatus.Registration)
            {
                var parentRequest = await _context.Set<SeizureRequest>().AsTracking().FirstOrDefaultAsync(e => e.Id == request.SeizureRequestId);
                if (parentRequest != null)
                {
                    parentRequest.ReturnReasonFromSubCommite = changeRequestStatus.Reason?.Trim();
                }
            }
            else if (isReturn && toStatus == (int)SeizureStatus.SubCommittee)
            {
                var parentRequest = await _context.Set<SeizureRequest>().AsTracking().FirstOrDefaultAsync(e => e.Id == request.SeizureRequestId);
                if (parentRequest != null)
                {
                    parentRequest.ReturnReasonFromPermanentCommite = changeRequestStatus.Reason?.Trim();
                }
            }

            _context.Set<StatusHistory>().Add(SeizureWorkflow.History(
                SeizureWorkflow.EntitySeizureDetail,
                request.Id,
                fromStatus,
                toStatus,
                isReturn ? "Return" : "Forward",
                changeRequestStatus.Reason,
                userName,
                request.SeizureRequestId));
            await SaveChangesAsync();
        }
        catch (DomainException)
        {
            throw;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error occurred while ChangeRequestStatus", ex);
            throw new RepositoryException("An error occurred while ChangeRequestStatus.", ex);
        }
    }

    public async Task FinanceAction(int id, FinanceAction financeAction, bool financeDecision)
    {
        try
        {
            var request = await _dbSet.AsTracking().FirstOrDefaultAsync(e => e.Id == id);
            if (request == null)
            {
                throw new Exception(" not found ");
            }
            if (financeDecision)
            {
                request.StatusId = (int)SeizureStatus.Completed;
            }
            await SaveChangesAsync();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error occurred while ChangeRequestStatus", ex);
            throw new RepositoryException("An error occurred while ChangeRequestStatus.", ex);
        }
    }

    public async Task<List<SeizureRequestDetailReadDto>> SearchResult(string firmId,SearchParams searchParams, List<string> roleMembers)
    {
        try
        {
            bool isFirmScoped = roleMembers.All(r => r == RoleMember.BorderGuardOffice.ToString() || r == RoleMember.SubCommittee.ToString());
            bool isAdmin = roleMembers.Contains(RoleMember.Admin.ToString());
            if (isFirmScoped)
            {
                var query = _dbSet.Where(e => e.SeizureRequest.FirmId == firmId).Include(e=>e.Status).Include(e => e.SeizureRequest).Include(e => e.RequestDetailAttachments)
                .Include(e => e.Item).ThenInclude(e => e.ItemCategory).AsQueryable();

                if (!string.IsNullOrWhiteSpace(searchParams.PersonName))
                {
                    var pName = searchParams.PersonName.Trim();
                    query = query.Where(x => (x.ItemName != null && x.ItemName.Contains(pName)) || (x.Item != null && x.Item.Name.Contains(pName)));
                }
                if (searchParams.CategoryId.HasValue)
                {
                    query = query.Where(x => x.Item.ItemCategoryId == searchParams.CategoryId.Value);
                }
                if (searchParams.StatusId.HasValue)
                {
                    query = query.Where(x => x.StatusId == searchParams.StatusId);
                }
                if (searchParams.ItemId.HasValue)
                {
                    query = query.Where(x => x.ItemId == searchParams.ItemId);
                }
                if (searchParams.DelieveryDateFrom.HasValue)
                {
                    query = query.Where(x => x.CreatedDate >= searchParams.DelieveryDateFrom);
                }
                if (searchParams.DelieveryDateTo.HasValue)
                {
                    query = query.Where(x => x.CreatedDate <= searchParams.DelieveryDateTo);
                }
                if (!string.IsNullOrWhiteSpace(searchParams.AmanaAcceptNumber))
                {
                    var aNum = searchParams.AmanaAcceptNumber.Trim();
                    query = query.Where(x => x.SeizureRequest != null && x.SeizureRequest.AmanaAcceptNumber != null && x.SeizureRequest.AmanaAcceptNumber.Contains(aNum));
                }
                if (searchParams.AmanaAcceptDateFrom.HasValue)
                {
                    var aFrom = searchParams.AmanaAcceptDateFrom.Value.Kind == DateTimeKind.Unspecified
                        ? DateTime.SpecifyKind(searchParams.AmanaAcceptDateFrom.Value, DateTimeKind.Utc)
                        : searchParams.AmanaAcceptDateFrom.Value.ToUniversalTime();
                    query = query.Where(x => x.SeizureRequest != null && x.SeizureRequest.AmanaAcceptDate.HasValue && x.SeizureRequest.AmanaAcceptDate.Value >= aFrom);
                }
                if (searchParams.AmanaAcceptDateTo.HasValue)
                {
                    var aTo = searchParams.AmanaAcceptDateTo.Value.Kind == DateTimeKind.Unspecified
                        ? DateTime.SpecifyKind(searchParams.AmanaAcceptDateTo.Value, DateTimeKind.Utc)
                        : searchParams.AmanaAcceptDateTo.Value.ToUniversalTime();
                    query = query.Where(x => x.SeizureRequest != null && x.SeizureRequest.AmanaAcceptDate.HasValue && x.SeizureRequest.AmanaAcceptDate.Value <= aTo);
                }
                return await query.OrderByDescending(e => e.CreatedDate).Select(ca => new SeizureRequestDetailReadDto
                {
                    Id = ca.Id,
                    SeizureRequestId = ca.SeizureRequestId,
                    FirmName = ca.SeizureRequest != null && ca.SeizureRequest.Firm != null ? ca.SeizureRequest.Firm.Name : string.Empty,
                    ItemId = ca.ItemId,
                    Quantity = ca.Quantity,
                    UnitPrice = ca.UnitPrice,
                    TotalPrice = ca.Quantity * (ca.UnitPrice ?? 0m),
                    UnitOfMeasureId = ca.UnitOfMeasureId,
                    UnitOfMeasureName = ca.UnitOfMeasure != null ? ca.UnitOfMeasure.Name : string.Empty,
                    ItemName = !string.IsNullOrWhiteSpace(ca.ItemName) ? ca.ItemName : (ca.Item != null ? ca.Item.Name : string.Empty),
                    SubCategoryName = ca.Item != null ? ca.Item.Name : string.Empty,
                    StatusId = ca.StatusId,
                    StatusName=ca.Status.Name,
                    CategoryName = ca.Item.ItemCategory.Name,
                    CategoryId = ca.Item.ItemCategory.Id,
                    CategoryColor = ca.Item.ItemCategory.Color,
                    Photos = ca.RequestDetailAttachments != null ? ca.RequestDetailAttachments.Count() : 0,
                    CreatedDate = ca.CreatedDate,
                    MahdrNumber = ca.SeizureRequest != null ? ca.SeizureRequest.MahdrNumber : null,
                    CaseNumber = ca.SeizureRequest != null ? ca.SeizureRequest.CaseNumber : null,
                    AmanaAcceptNumber = ca.SeizureRequest != null ? ca.SeizureRequest.AmanaAcceptNumber : null,
                    AmanaAcceptDate = ca.SeizureRequest != null ? ca.SeizureRequest.AmanaAcceptDate : null,
                    SubCommitteeNumber = ca.SeizureRequest != null ? ca.SeizureRequest.SubCommitteeNumber : null,
                    SubCommitteeDate = ca.SeizureRequest != null ? ca.SeizureRequest.SubCommitteeDate : null,
                    PermanentCommitteeNumber = ca.SeizureRequest != null ? ca.SeizureRequest.PermanentCommitteeNumber : null,
                    PermanentCommitteeDate = ca.SeizureRequest != null ? ca.SeizureRequest.PermanentCommitteeDate : null,
                    SubCommitteeOpinion = ca.SeizureRequest != null ? ca.SeizureRequest.SubCommitteeOpinion : null,
                    TechnicalReport = ca.SeizureRequest != null ? ca.SeizureRequest.TechnicalReport : null,
                    ReturnReasonFromPermanentCommite = ca.SeizureRequest != null ? ca.SeizureRequest.ReturnReasonFromPermanentCommite : null,
                    ReturnReasonFromSubCommite = ca.SeizureRequest != null ? ca.SeizureRequest.ReturnReasonFromSubCommite : null
                })
                .ToListAsync();


            }
            else
            {
                var query = isAdmin
                    ? _dbSet.Include(e => e.Status).Include(e => e.SeizureRequest).Include(e => e.RequestDetailAttachments).Include(e => e.Item).ThenInclude(e => e.ItemCategory).AsQueryable()
                    : _dbSet.Where(e => e.StatusId >= (int)SeizureStatus.SubCommittee && e.StatusId <= (int)SeizureStatus.Completed).Include(e => e.Status).Include(e => e.SeizureRequest).Include(e => e.RequestDetailAttachments).Include(e => e.Item).ThenInclude(e => e.ItemCategory).AsQueryable();

                if (!string.IsNullOrEmpty(searchParams.FirmId))
                {
                    query = query.Where(x => x.SeizureRequest.FirmId == searchParams.FirmId);
                }
                if (!string.IsNullOrWhiteSpace(searchParams.PersonName))
                {
                    var pName = searchParams.PersonName.Trim();
                    query = query.Where(x => (x.ItemName != null && x.ItemName.Contains(pName)) || (x.Item != null && x.Item.Name.Contains(pName)));
                }
                if (searchParams.CategoryId.HasValue)
                {
                    query = query.Where(x => x.Item.ItemCategoryId == searchParams.CategoryId.Value);
                }
                if (searchParams.StatusId.HasValue)
                {
                    query = query.Where(x => x.StatusId == searchParams.StatusId);
                }
                if (searchParams.ItemId.HasValue)
                {
                    query = query.Where(x => x.ItemId == searchParams.ItemId);
                }
                if (searchParams.DelieveryDateFrom.HasValue)
                {
                    query = query.Where(x => x.CreatedDate >= searchParams.DelieveryDateFrom);
                }
                if (searchParams.DelieveryDateTo.HasValue)
                {
                    query = query.Where(x => x.CreatedDate <= searchParams.DelieveryDateTo);
                }
                if (!string.IsNullOrWhiteSpace(searchParams.AmanaAcceptNumber))
                {
                    var aNum = searchParams.AmanaAcceptNumber.Trim();
                    query = query.Where(x => x.SeizureRequest != null && x.SeizureRequest.AmanaAcceptNumber != null && x.SeizureRequest.AmanaAcceptNumber.Contains(aNum));
                }
                if (searchParams.AmanaAcceptDateFrom.HasValue)
                {
                    var aFrom = searchParams.AmanaAcceptDateFrom.Value.Kind == DateTimeKind.Unspecified
                        ? DateTime.SpecifyKind(searchParams.AmanaAcceptDateFrom.Value, DateTimeKind.Utc)
                        : searchParams.AmanaAcceptDateFrom.Value.ToUniversalTime();
                    query = query.Where(x => x.SeizureRequest != null && x.SeizureRequest.AmanaAcceptDate.HasValue && x.SeizureRequest.AmanaAcceptDate.Value >= aFrom);
                }
                if (searchParams.AmanaAcceptDateTo.HasValue)
                {
                    var aTo = searchParams.AmanaAcceptDateTo.Value.Kind == DateTimeKind.Unspecified
                        ? DateTime.SpecifyKind(searchParams.AmanaAcceptDateTo.Value, DateTimeKind.Utc)
                        : searchParams.AmanaAcceptDateTo.Value.ToUniversalTime();
                    query = query.Where(x => x.SeizureRequest != null && x.SeizureRequest.AmanaAcceptDate.HasValue && x.SeizureRequest.AmanaAcceptDate.Value <= aTo);
                }
                return await query.OrderByDescending(e => e.CreatedDate).Select(ca => new SeizureRequestDetailReadDto
                {
                    Id = ca.Id,
                    SeizureRequestId = ca.SeizureRequestId,
                    ItemId = ca.ItemId,
                    Quantity = ca.Quantity,
                    UnitPrice = ca.UnitPrice,
                    TotalPrice = ca.Quantity * (ca.UnitPrice ?? 0m),
                    UnitOfMeasureId = ca.UnitOfMeasureId,
                    UnitOfMeasureName = ca.UnitOfMeasure != null ? ca.UnitOfMeasure.Name : string.Empty,
                    FirmName = ca.SeizureRequest != null && ca.SeizureRequest.Firm != null ? ca.SeizureRequest.Firm.Name : string.Empty,
                    ItemName = !string.IsNullOrWhiteSpace(ca.ItemName) ? ca.ItemName : (ca.Item != null ? ca.Item.Name : string.Empty),
                    SubCategoryName = ca.Item != null ? ca.Item.Name : string.Empty,
                    StatusId = ca.StatusId,
                    StatusName=ca.Status.Name,
                    CategoryName = ca.Item.ItemCategory.Name,
                    CategoryId = ca.Item.ItemCategory.Id,
                    CategoryColor = ca.Item.ItemCategory.Color,
                    Photos = ca.RequestDetailAttachments != null ? ca.RequestDetailAttachments.Count() : 0,
                    CreatedDate = ca.CreatedDate,
                    MahdrNumber = ca.SeizureRequest != null ? ca.SeizureRequest.MahdrNumber : null,
                    CaseNumber = ca.SeizureRequest != null ? ca.SeizureRequest.CaseNumber : null,
                    AmanaAcceptNumber = ca.SeizureRequest != null ? ca.SeizureRequest.AmanaAcceptNumber : null,
                    AmanaAcceptDate = ca.SeizureRequest != null ? ca.SeizureRequest.AmanaAcceptDate : null,
                    SubCommitteeNumber = ca.SeizureRequest != null ? ca.SeizureRequest.SubCommitteeNumber : null,
                    SubCommitteeDate = ca.SeizureRequest != null ? ca.SeizureRequest.SubCommitteeDate : null,
                    PermanentCommitteeNumber = ca.SeizureRequest != null ? ca.SeizureRequest.PermanentCommitteeNumber : null,
                    PermanentCommitteeDate = ca.SeizureRequest != null ? ca.SeizureRequest.PermanentCommitteeDate : null,
                    SubCommitteeOpinion = ca.SeizureRequest != null ? ca.SeizureRequest.SubCommitteeOpinion : null,
                    TechnicalReport = ca.SeizureRequest != null ? ca.SeizureRequest.TechnicalReport : null,
                    ReturnReasonFromPermanentCommite = ca.SeizureRequest != null ? ca.SeizureRequest.ReturnReasonFromPermanentCommite : null,
                    ReturnReasonFromSubCommite = ca.SeizureRequest != null ? ca.SeizureRequest.ReturnReasonFromSubCommite : null
                })
                .ToListAsync();
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error occurred while GetRequestDetailForRequest", ex);
            throw new RepositoryException("An error occurred while GetRequestDetailForRequest.", ex);
        }
    }

    public async Task<DashboardData> GetDashboardData(string firmId, List<string> roleMembers)
    {
        try
        {
            var dashboardCards = new DashboardData();

            // Return all seizure_request_detail records regardless of status, excluding is_deleted = true
            IQueryable<SeizureRequestDetail> baseQuery = _dbSet.Where(e => !e.IsDeleted);

            var isBorderGuard = roleMembers != null && roleMembers.Any(r =>
                string.Equals(r, RoleMember.BorderGuardOffice.ToString(), StringComparison.OrdinalIgnoreCase) ||
                string.Equals(r, "BorderGuardOffice", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(r, "مكتب حرس حدود", StringComparison.OrdinalIgnoreCase));

            if (isBorderGuard)
            {
                baseQuery = baseQuery.Where(e => e.SeizureRequest != null && e.SeizureRequest.FirmId == firmId);
            }

            var seizuresStatus = await baseQuery
                .GroupBy(e => e.StatusId)
                .Select(g => new
                {
                    StatusId = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();

            var requests = await baseQuery
                .Include(e => e.Status)
                .Include(e => e.SeizureRequest).ThenInclude(sr => sr.Firm)
                .Include(e => e.RequestDetailAttachments)
                .Include(e => e.Item).ThenInclude(e => e.ItemCategory)
                .OrderByDescending(e => e.SeizureRequest != null ? e.SeizureRequest.RequestDate : e.CreatedDate)
                .Select(ca => new SeizureRequestDetailReadDto
                {
                    Id = ca.Id,
                    FirmName = ca.SeizureRequest != null && ca.SeizureRequest.Firm != null ? ca.SeizureRequest.Firm.Name : string.Empty,
                    SeizureRequestId = ca.SeizureRequestId,
                    ItemId = ca.ItemId,
                    Quantity = ca.Quantity,
                    UnitPrice = ca.UnitPrice,
                    TotalPrice = ca.Quantity * (ca.UnitPrice ?? 0m),
                    UnitOfMeasureId = ca.UnitOfMeasureId,
                    UnitOfMeasureName = ca.UnitOfMeasure != null ? ca.UnitOfMeasure.Name : string.Empty,
                    ItemName = !string.IsNullOrWhiteSpace(ca.ItemName) ? ca.ItemName : (ca.Item != null ? ca.Item.Name : string.Empty),
                    SubCategoryName = ca.Item != null ? ca.Item.Name : string.Empty,
                    StatusId = ca.StatusId,
                    StatusName = ca.Status != null ? ca.Status.Name : string.Empty,
                    CategoryName = ca.Item != null && ca.Item.ItemCategory != null ? ca.Item.ItemCategory.Name : string.Empty,
                    CategoryId = ca.Item != null && ca.Item.ItemCategory != null ? ca.Item.ItemCategory.Id : 0,
                    CategoryColor = ca.Item != null && ca.Item.ItemCategory != null ? ca.Item.ItemCategory.Color : string.Empty,
                    Photos = ca.RequestDetailAttachments != null ? ca.RequestDetailAttachments.Count() : 0,
                    RoleMembers = roleMembers,
                    HasFinanceDecision = false,
                    HasFinanceRecieved = false,
                    RequestDate = ca.SeizureRequest != null ? ca.SeizureRequest.RequestDate : ca.CreatedDate,
                    CreatedDate = ca.CreatedDate,
                })
                .ToListAsync();

            dashboardCards.EditingSeizures = seizuresStatus.Where(e => e.StatusId == (int)SeizureStatus.Registration).Sum(r => r.Count);
            dashboardCards.PendingSeizures = seizuresStatus.Where(e => e.StatusId != (int)SeizureStatus.Registration && e.StatusId != (int)SeizureStatus.Completed).Sum(r => r.Count);
            dashboardCards.AcceptedSeizures = seizuresStatus.Where(e => e.StatusId == (int)SeizureStatus.Completed).Sum(r => r.Count);
            dashboardCards.Seizures = requests;
            dashboardCards.RoleMembers = roleMembers;

            return dashboardCards;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error occurred while  GetDashboardData.");
            throw new RepositoryException("An error occurred while GetDashboardData.", ex);
        }
    }

    public async Task<List<SeizureRequestDetailReadDto>> SeizuresOnDuration(string firmId, List<string> roleMembers, DateDuration dateDuration)
    {
        try
        {
            var currentDate = DateTime.UtcNow;
            DateTime dateTime = DateTime.UtcNow;
            if (dateDuration == DateDuration.LastWeek)
            {
                dateTime = currentDate.AddDays(-7);
            }
            else if (dateDuration == DateDuration.LastMonth)
            {
                dateTime = currentDate.AddMonths(-1);
            }
            else
            {
                dateTime = currentDate.AddYears(-1);
            }

            IQueryable<SeizureRequestDetail> query = _dbSet.Where(e => !e.IsDeleted && e.SeizureRequest != null && e.SeizureRequest.RequestDate >= dateTime);

            var isBorderGuard = roleMembers != null && roleMembers.Any(r =>
                string.Equals(r, RoleMember.BorderGuardOffice.ToString(), StringComparison.OrdinalIgnoreCase) ||
                string.Equals(r, "BorderGuardOffice", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(r, "مكتب حرس حدود", StringComparison.OrdinalIgnoreCase));

            if (isBorderGuard)
            {
                query = query.Where(e => e.SeizureRequest != null && e.SeizureRequest.FirmId == firmId);
            }

            var requestDetails = await query
                .Include(e => e.Status)
                .Include(e => e.SeizureRequest).ThenInclude(sr => sr.Firm)
                .Include(e => e.RequestDetailAttachments)
                .Include(e => e.Item).ThenInclude(e => e.ItemCategory)
                .OrderByDescending(e => e.SeizureRequest != null ? e.SeizureRequest.RequestDate : e.CreatedDate)
                .Select(ca => new SeizureRequestDetailReadDto
                {
                    Id = ca.Id,
                    FirmName = ca.SeizureRequest != null && ca.SeizureRequest.Firm != null ? ca.SeizureRequest.Firm.Name : string.Empty,
                    SeizureRequestId = ca.SeizureRequestId,
                    ItemId = ca.ItemId,
                    Quantity = ca.Quantity,
                    UnitPrice = ca.UnitPrice,
                    TotalPrice = ca.Quantity * (ca.UnitPrice ?? 0m),
                    UnitOfMeasureId = ca.UnitOfMeasureId,
                    UnitOfMeasureName = ca.UnitOfMeasure != null ? ca.UnitOfMeasure.Name : string.Empty,
                    ItemName = !string.IsNullOrWhiteSpace(ca.ItemName) ? ca.ItemName : (ca.Item != null ? ca.Item.Name : string.Empty),
                    SubCategoryName = ca.Item != null ? ca.Item.Name : string.Empty,
                    StatusId = ca.StatusId,
                    StatusName = ca.Status != null ? ca.Status.Name : string.Empty,
                    CategoryName = ca.Item != null && ca.Item.ItemCategory != null ? ca.Item.ItemCategory.Name : string.Empty,
                    CategoryId = ca.Item != null && ca.Item.ItemCategory != null ? ca.Item.ItemCategory.Id : 0,
                    CategoryColor = ca.Item != null && ca.Item.ItemCategory != null ? ca.Item.ItemCategory.Color : string.Empty,
                    Photos = ca.RequestDetailAttachments != null ? ca.RequestDetailAttachments.Count() : 0,
                    RoleMembers = roleMembers,
                    HasFinanceDecision = false,
                    HasFinanceRecieved = false,
                    RequestDate = ca.SeizureRequest != null ? ca.SeizureRequest.RequestDate : ca.CreatedDate,
                    CreatedDate = ca.CreatedDate,
                })
                .ToListAsync();

            return requestDetails;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error occurred while  SeizuresOnDuration.");
            throw new RepositoryException("An error occurred while SeizuresOnDuration.", ex);
        }
    }
}



