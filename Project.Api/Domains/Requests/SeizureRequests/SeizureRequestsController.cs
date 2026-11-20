using Microsoft.AspNetCore.Mvc;
using Project.Shared;

using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Project.Api
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class SeizureRequestsController : BaseController<SeizureRequest, SeizureRequestDto>
    {
        private readonly ISeizureRequestService _service;
        private readonly ApplicationDbContext _context;

        public SeizureRequestsController(ISeizureRequestService service, ApplicationDbContext context) : base(service)
        {
            _service = service;
            _context = context;
        }

        [HttpGet("GetFirms")]
        [Authorize(Roles = "BorderGuardOffice,SubCommittee,PermanentCommittee,ServicesAgency,FinanceAuthority,Admin")]
        public async Task<IActionResult> GetFirms()
        {
            var firms = await _service.GetFirms();
            return firms.Success
                ? this.AppSuccess(firms)
                : this.AppFailed<List<FirmDto>>(firms.Message);
        }

        [HttpGet("ChangeSeizuresUnderRequestToSubCommittee/{id}")]
        [Authorize(Roles = "BorderGuardOffice,SubCommittee,Admin")]
        public async Task<IActionResult> ChangeSeizuresUnderRequestToSubCommittee(int id)
        {
            var userName = User.FindFirstValue(ClaimTypes.Name);
            var requests = await _service.ChangeSeizuresUnderRequestToSubCommittee(id, userName);
            return requests.Success
                ? this.AppSuccess(requests)
                : this.AppFailed<string>(requests.Message);
        }

        [HttpGet("GetSeizuresByStatus/{statusId}")]
        [Authorize(Roles = "BorderGuardOffice,SubCommittee,PermanentCommittee,ServicesAgency,FinanceAuthority,Admin")]
        public async Task<IActionResult> GetSeizuresByStatus(int statusId)
        {
            var result = await _service.GetSeizuresByStatus(statusId);
            return result.Success
                ? this.AppSuccess(result)
                : this.AppFailed<List<WorkflowSeizureDto>>(result.Message);
        }

        [HttpGet("GetSeizuresByStatusScoped/{statusId}")]
        [Authorize(Roles = "BorderGuardOffice,SubCommittee,PermanentCommittee,ServicesAgency,FinanceAuthority,Admin")]
        public async Task<IActionResult> GetSeizuresByStatusScoped(int statusId)
        {
            var firmCode = User.Claims.FirstOrDefault(c => c.Type == CustomClaimTypes.FirmId)?.Value;
            bool allFlag;
            bool.TryParse(User.Claims.FirstOrDefault(c => c.Type == CustomClaimTypes.All)?.Value, out allFlag);
            var result = await _service.GetSeizuresByStatusScoped(statusId, firmCode, allFlag);
            return result.Success
                ? this.AppSuccess(result)
                : this.AppFailed<List<WorkflowSeizureDto>>(result.Message);
        }

        [HttpPost("BulkSubmitToSubCommittee")]
        [Authorize(Roles = "BorderGuardOffice,Admin")]
        public async Task<IActionResult> BulkSubmitToSubCommittee([FromBody] BulkSubmitToSubCommitteeDto dto)
        {
            var firmCode = User.Claims.FirstOrDefault(c => c.Type == CustomClaimTypes.FirmId)?.Value;
            var userName = User.FindFirstValue(ClaimTypes.Name);
            var result = await _service.BulkSubmitToSubCommittee(dto, firmCode, userName);
            return result.Success ? this.AppSuccess(result) : this.AppFailed<string>(result.Message);
        }

        [HttpPost("ReturnSeizure")]
        [Authorize(Roles = "SubCommittee,PermanentCommittee,Admin")]
        public async Task<IActionResult> ReturnSeizure(ReturnSeizureDto dto)
        {
            var userName = User.FindFirstValue(ClaimTypes.Name);
            var result = await _service.ReturnSeizure(dto, userName);
            return result.Success ? this.AppSuccess(result) : this.AppFailed<string>(result.Message);
        }

        [HttpPost("ForwardToPermanentCommittee")]
        [Authorize(Roles = "SubCommittee,Admin")]
        public async Task<IActionResult> ForwardToPermanentCommittee(ForwardSeizureDto dto)
        {
            var userName = User.FindFirstValue(ClaimTypes.Name);
            var result = await _service.ForwardToPermanentCommittee(dto, userName);
            return result.Success ? this.AppSuccess(result) : this.AppFailed<string>(result.Message);
        }

        [HttpPost("BulkApprove")]
        [Authorize(Roles = "PermanentCommittee,Admin")]
        public async Task<IActionResult> BulkApprove(BulkApproveDto dto)
        {
            var userName = User.FindFirstValue(ClaimTypes.Name);
            var result = await _service.BulkApprove(dto, userName);
            return result.Success ? this.AppSuccess(result) : this.AppFailed<string>(result.Message);
        }

        [HttpGet("GetSeizureRequests")]
        [Authorize(Roles = "BorderGuardOffice,SubCommittee,PermanentCommittee,ServicesAgency,FinanceAuthority,Admin")]
        public async Task<IActionResult> GetSeizureRequests()
        {
            bool allFlag;
            var allFlagBool = bool.TryParse(User.Claims.FirstOrDefault(c => c.Type == CustomClaimTypes.All)?.Value, out allFlag);
            var roleMembersData = User.Claims.FirstOrDefault(c => c.Type == CustomClaimTypes.RoleMembers)?.Value;
            var roleMembers = JsonConvert.DeserializeObject<List<string>>(roleMembersData ?? "[]");
            if (!allFlagBool)
            {
                return this.AppFailed<ApplicationRole>("allFlagBoolEmpty");
            }
            var firmId = User.Claims.FirstOrDefault(c => c.Type == CustomClaimTypes.FirmId)?.Value;

            var seizureRequests = await _service.GetSeizureRequests(allFlag, firmId, roleMembers);
            return seizureRequests.Success
                ? this.AppSuccess(seizureRequests)
                : this.AppFailed<SeizureReadDto>(seizureRequests.Message);
        }

        [HttpPost]
        [Authorize(Roles = "BorderGuardOffice,Admin")]
        public override async Task<IActionResult> Post(SeizureRequestDto dto)
        {
            var userFirmId = User.Claims.FirstOrDefault(c => c.Type == CustomClaimTypes.FirmId)?.Value;
            if (User.IsInRole("BorderGuardOffice") || User.IsInRole("SubCommittee"))
            {
                if (!string.IsNullOrWhiteSpace(userFirmId))
                {
                    dto.FirmId = userFirmId;
                }
            }
            else if (string.IsNullOrWhiteSpace(dto.FirmId))
            {
                dto.FirmId = userFirmId ?? DataSeeder.SeedFirmCode;
            }

            dto.RequestDate = dto.RequestDate.Kind switch
            {
                DateTimeKind.Unspecified => DateTime.SpecifyKind(dto.RequestDate, DateTimeKind.Utc),
                DateTimeKind.Local => dto.RequestDate.ToUniversalTime(),
                _ => dto.RequestDate
            };
            var result = await _service.Create(dto);
            return result.Success
                ? this.AppSuccess(result)
                : this.AppFailed<SeizureRequestDto>(result.Message);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "BorderGuardOffice,Admin")]
        public override async Task<IActionResult> Put(int id, SeizureRequestDto dto)
        {
            var userFirmId = User.Claims.FirstOrDefault(c => c.Type == CustomClaimTypes.FirmId)?.Value;
            if (User.IsInRole("BorderGuardOffice") || User.IsInRole("SubCommittee"))
            {
                if (!string.IsNullOrWhiteSpace(userFirmId))
                {
                    dto.FirmId = userFirmId;
                }
            }

            dto.RequestDate = dto.RequestDate.Kind switch
            {
                DateTimeKind.Unspecified => DateTime.SpecifyKind(dto.RequestDate, DateTimeKind.Utc),
                DateTimeKind.Local => dto.RequestDate.ToUniversalTime(),
                _ => dto.RequestDate
            };
            var result = await _service.Update(id, dto);
            return result.Success
                ? this.AppSuccess(result)
                : this.AppFailed<SeizureRequestDto>(result.Message);
        }
    }
}
