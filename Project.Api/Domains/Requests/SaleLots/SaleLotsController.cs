namespace Project.Api;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class SaleLotsController : BaseController<SaleLot, SaleLotDto>
{
    private readonly ISaleLotService _service;

    public SaleLotsController(ISaleLotService service) : base(service)
    {
        _service = service;
    }

    private (string? firmCode, bool isGlobal) GetUserScope()
    {
        var firmCode = User.Claims.FirstOrDefault(c => c.Type == CustomClaimTypes.FirmId)?.Value;
        bool allFlag;
        bool.TryParse(User.Claims.FirstOrDefault(c => c.Type == CustomClaimTypes.All)?.Value, out allFlag);

        var roleMembersData = User.Claims.FirstOrDefault(c => c.Type == CustomClaimTypes.RoleMembers)?.Value;
        var roleMembers = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(roleMembersData ?? "[]") ?? new List<string>();

        var isSubCommittee = User.IsInRole("SubCommittee") || roleMembers.Contains(RoleMember.SubCommittee.ToString());
        var isAdmin = User.IsInRole("Admin");

        // If SubCommittee and not Admin, strictly scope to user's firm
        var isGlobal = allFlag && (!isSubCommittee || isAdmin);

        return (firmCode, isGlobal);
    }

    [HttpGet("GetAllocatableItems")]
    [Authorize(Roles = "SubCommittee,PermanentCommittee,Admin")]
    public async Task<IActionResult> GetAllocatableItems()
    {
        var (firmCode, isGlobal) = GetUserScope();
        var result = await _service.GetAllocatableItemsScoped(firmCode, isGlobal);
        return result.Success ? this.AppSuccess(result) : this.AppFailed<List<AllocatableItemDto>>(result.Message);
    }

    [HttpGet("GetAllocatableItemsScoped")]
    [Authorize(Roles = "SubCommittee,PermanentCommittee,Admin")]
    public async Task<IActionResult> GetAllocatableItemsScoped()
    {
        var (firmCode, isGlobal) = GetUserScope();
        var result = await _service.GetAllocatableItemsScoped(firmCode, isGlobal);
        return result.Success ? this.AppSuccess(result) : this.AppFailed<List<AllocatableItemDto>>(result.Message);
    }

    [HttpGet("items-aggregated-summary")]
    [Authorize(Roles = "SubCommittee,PermanentCommittee,ServicesAgency,FinanceAuthority,Admin")]
    public async Task<IActionResult> GetItemsAggregatedSummary()
    {
        var (firmCode, isGlobal) = GetUserScope();
        var result = await _service.GetItemsAggregatedSummary(firmCode, isGlobal);
        return result.Success ? this.AppSuccess(result) : this.AppFailed<List<ItemAggregatedSummaryDto>>(result.Message);
    }

    [HttpGet("services-agency-queue")]
    [Authorize(Roles = "SubCommittee,ServicesAgency,Admin")]
    public async Task<IActionResult> GetServicesAgencyQueue()
    {
        var (firmCode, isGlobal) = GetUserScope();
        var result = await _service.GetServicesAgencyQueue(firmCode, isGlobal);
        return result.Success ? this.AppSuccess(result) : this.AppFailed<List<AllocatableItemDto>>(result.Message);
    }

    [HttpPost("AssignToServicesAgency")]
    [Authorize(Roles = "SubCommittee,PermanentCommittee,Admin")]
    public async Task<IActionResult> AssignToServicesAgency([FromBody] List<AssignToServicesAgencyItemDto> items)
    {
        var userName = User.FindFirstValue(ClaimTypes.Name);
        var result = await _service.AssignToServicesAgency(items, userName);
        return result.Success ? this.AppSuccess(result) : this.AppFailed<string>(result.Message);
    }

    [HttpPost("CreateLot")]
    [Authorize(Roles = "SubCommittee,PermanentCommittee,ServicesAgency,Admin")]
    public async Task<IActionResult> CreateLot(CreateSaleLotDto dto)
    {
        var userName = User.FindFirstValue(ClaimTypes.Name);
        var (firmCode, isGlobal) = GetUserScope();
        var result = await _service.CreateLot(dto, userName, firmCode, isGlobal);
        return result.Success ? this.AppSuccess(result) : this.AppFailed<SaleLotReadDto>(result.Message);
    }

    [HttpGet("GetLots")]
    [Authorize(Roles = "BorderGuardOffice,SubCommittee,PermanentCommittee,ServicesAgency,FinanceAuthority,Admin")]
    public async Task<IActionResult> GetLots([FromQuery] int? statusId, [FromQuery] bool? isPublicService, [FromQuery] int? saleEntityId)
    {
        var (firmCode, isGlobal) = GetUserScope();
        var roleMembersData = User.Claims.FirstOrDefault(c => c.Type == CustomClaimTypes.RoleMembers)?.Value;
        var roleMembers = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(roleMembersData ?? "[]") ?? new List<string>();

        // Requirement: If user role is ServicesAgency, only lots with sale_entity_id = 1 are returned
        if (roleMembers.Contains(RoleMember.ServicesAgency.ToString()) && !User.IsInRole("Admin"))
        {
            saleEntityId = 1;
        }

        var result = await _service.GetLots(statusId, isPublicService, saleEntityId, firmCode, isGlobal);
        return result.Success ? this.AppSuccess(result) : this.AppFailed<List<SaleLotReadDto>>(result.Message);
    }

    [HttpGet("GetLot/{id}")]
    [Authorize(Roles = "BorderGuardOffice,SubCommittee,PermanentCommittee,ServicesAgency,FinanceAuthority,Admin")]
    public async Task<IActionResult> GetLot(int id)
    {
        var (firmCode, isGlobal) = GetUserScope();
        var result = await _service.GetLot(id, firmCode, isGlobal);
        return result.Success ? this.AppSuccess(result) : this.AppFailed<SaleLotReadDto>(result.Message);
    }

    [HttpPost("SubmitSettlement/{id}")]
    [Authorize(Roles = "SubCommittee,PermanentCommittee,ServicesAgency,Admin")]
    public async Task<IActionResult> SubmitSettlement(int id, [FromForm] LotSettlementDto dto)
    {
        var userName = User.FindFirstValue(ClaimTypes.Name);
        var (firmCode, isGlobal) = GetUserScope();
        var result = await _service.SubmitSettlement(id, dto, userName, firmCode, isGlobal);
        return result.Success ? this.AppSuccess(result) : this.AppFailed<string>(result.Message);
    }

    [HttpPost("ApproveSettlement/{id}")]
    [Authorize(Roles = "SubCommittee,PermanentCommittee,FinanceAuthority,Admin")]
    public async Task<IActionResult> ApproveSettlement(int id)
    {
        var userName = User.FindFirstValue(ClaimTypes.Name);
        var result = await _service.ApproveSettlement(id, userName);
        return result.Success ? this.AppSuccess(result) : this.AppFailed<string>(result.Message);
    }

    [HttpPost("RejectSettlement/{id}")]
    [Authorize(Roles = "SubCommittee,PermanentCommittee,FinanceAuthority,Admin")]
    public async Task<IActionResult> RejectSettlement(int id, LotFinanceDecisionDto dto)
    {
        var userName = User.FindFirstValue(ClaimTypes.Name);
        var result = await _service.RejectSettlement(id, dto, userName);
        return result.Success ? this.AppSuccess(result) : this.AppFailed<string>(result.Message);
    }

    [HttpGet("GetReceipt/{id}")]
    [Authorize(Roles = "BorderGuardOffice,SubCommittee,PermanentCommittee,ServicesAgency,FinanceAuthority,Admin")]
    public async Task<IActionResult> GetReceipt(int id)
    {
        var result = await _service.GetReceipt(id);
        return result.Success ? this.AppSuccess(result) : this.AppFailed<SaleLotReceiptDto>(result.Message);
    }

    [HttpGet("GetHistory/{entityType}/{entityId}")]
    [Authorize(Roles = "BorderGuardOffice,SubCommittee,PermanentCommittee,ServicesAgency,FinanceAuthority,Admin")]
    public async Task<IActionResult> GetHistory(string entityType, int entityId)
    {
        var result = await _service.GetHistory(entityType, entityId);
        return result.Success ? this.AppSuccess(result) : this.AppFailed<List<StatusHistoryReadDto>>(result.Message);
    }
}
