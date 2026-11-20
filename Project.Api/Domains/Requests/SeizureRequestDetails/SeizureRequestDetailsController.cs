using Microsoft.AspNetCore.Mvc;
using Project.Security;
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
    public class SeizureRequestDetailsController : BaseController<SeizureRequestDetail, SeizureRequestDetailDto>
    {
        private readonly ISeizureRequestDetailService _service;
        private readonly ISaleLotService _saleLotService;

        public SeizureRequestDetailsController(ISeizureRequestDetailService service, ISaleLotService saleLotService) : base(service)
        {
            _service = service;
            _saleLotService = saleLotService;
        }

        [HttpPost]
        [Authorize(Roles = "BorderGuardOffice,SubCommittee,Admin")]
        public async override Task<IActionResult> Post(SeizureRequestDetailDto dto)
        {
            var roleMembersData = User.Claims.FirstOrDefault(c => c.Type == CustomClaimTypes.RoleMembers)?.Value;
            var roleMembers = JsonConvert.DeserializeObject<List<string>>(roleMembersData ?? "[]");
            var requestsDetails = await _service.CreateRequestDetailForRequest(dto, roleMembers);
            return requestsDetails.Success
                ? this.AppSuccess(requestsDetails)
                : this.AppFailed<SeizureRequestDetailReadDto>(requestsDetails.Message);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "BorderGuardOffice,SubCommittee,Admin")]
        public async override Task<IActionResult> Put(int id, SeizureRequestDetailDto dto)
        {
            var updatedResult = await _service.Update(id, dto);
            return updatedResult.Success
                ? this.AppSuccess(updatedResult)
                : this.AppFailed(updatedResult.Message);
        }

        [HttpGet("SearchReadDto")]
        [Authorize(Roles = "BorderGuardOffice,SubCommittee,PermanentCommittee,ServicesAgency,FinanceAuthority,Admin")]
        public async Task<IActionResult> SearchReadDto()
        {
            var roleMembersData = User.Claims.FirstOrDefault(c => c.Type == CustomClaimTypes.RoleMembers)?.Value;
            var firmId = User.Claims.FirstOrDefault(c => c.Type == CustomClaimTypes.FirmId)?.Value;
            var roleMembers = JsonConvert.DeserializeObject<List<string>>(roleMembersData ?? "[]");
            var requestsDetails = await _service.SearchReadDto(firmId, roleMembers);
            return requestsDetails.Success
                ? this.AppSuccess(requestsDetails)
                : this.AppFailed<SearchReadDto>(requestsDetails.Message);
        }

        [HttpPost("SearchResult")]
        [Authorize(Roles = "BorderGuardOffice,SubCommittee,PermanentCommittee,ServicesAgency,FinanceAuthority,Admin")]
        public async Task<IActionResult> SearchResult([FromForm] SearchParams searchParams)
        {
            var roleMembersData = User.Claims.FirstOrDefault(c => c.Type == CustomClaimTypes.RoleMembers)?.Value;
            var firmId = User.Claims.FirstOrDefault(c => c.Type == CustomClaimTypes.FirmId)?.Value;
            var roleMembers = JsonConvert.DeserializeObject<List<string>>(roleMembersData ?? "[]");
            var requestsDetails = await _service.SearchResult(firmId, searchParams, roleMembers);
            return requestsDetails.Success
                ? this.AppSuccess(requestsDetails)
                : this.AppFailed<SearchReadDto>(requestsDetails.Message);
        }

        [HttpPost("GeneratePdf")]
        [Authorize(Roles = "BorderGuardOffice,SubCommittee,PermanentCommittee,ServicesAgency,FinanceAuthority,Admin")]
        public async Task<IActionResult> GeneratePdf([FromForm] SearchParams searchParams)
        {
            var roleMembersData = User.Claims.FirstOrDefault(c => c.Type == CustomClaimTypes.RoleMembers)?.Value;
            var firmId = User.Claims.FirstOrDefault(c => c.Type == CustomClaimTypes.FirmId)?.Value;
            var roleMembers = JsonConvert.DeserializeObject<List<string>>(roleMembersData ?? "[]");
            var requestsDetails = await _service.GeneratePdf(firmId, searchParams, roleMembers);
            return requestsDetails.Success
                ? File(requestsDetails.Data, "application/pdf", "VacationRecievementDocument.pdf")
                : this.AppFailed<SearchReadDto>(requestsDetails.Message);
        }

        [HttpGet("GetRequestDetailForRequest/{seizureRequestId}")]
        [Authorize(Roles = "BorderGuardOffice,SubCommittee,PermanentCommittee,ServicesAgency,FinanceAuthority,Admin")]
        public async Task<IActionResult> GetRequestDetailForRequest(int seizureRequestId)
        {
            var roleMembersData = User.Claims.FirstOrDefault(c => c.Type == CustomClaimTypes.RoleMembers)?.Value;
            var roleMembers = JsonConvert.DeserializeObject<List<string>>(roleMembersData ?? "[]");
            var requestsDetails = await _service.GetRequestDetailForRequest(seizureRequestId, roleMembers);
            return requestsDetails.Success
                ? this.AppSuccess(requestsDetails)
                : this.AppFailed<List<PagesOnRole>>(requestsDetails.Message);
        }

        [HttpGet("GetLookupsForRequest")]
        [Authorize(Roles = "BorderGuardOffice,SubCommittee,PermanentCommittee,ServicesAgency,FinanceAuthority,Admin")]
        public async Task<IActionResult> GetLookupsForRequest(int seizureRequestId)
        {
            var lookups = await _service.GetLookupsForRequest();
            return lookups.Success
                ? this.AppSuccess(lookups)
                : this.AppFailed<List<LookupsForRequest>>(lookups.Message);
        }

        [HttpPost("ChangeRequestStatus")]
        [Authorize(Roles = "SubCommittee,PermanentCommittee,Admin")]
        public async Task<IActionResult> ChangeRequestStatus(ChangeRequestStatus changeRequestStatus)
        {
            var userName = User.FindFirstValue(ClaimTypes.Name);
            var request = await _service.ChangeRequestStatus(changeRequestStatus, userName);
            return request.Success
                ? this.AppSuccess(request)
                : this.AppFailed<string>(request.Message);
        }

        [HttpPost("FinanceRecievement/{id}")]
        [Authorize(Roles = "ServicesAgency,FinanceAuthority,Admin")]
        public async Task<IActionResult> FinanceRecievement(int id, FinanceAction financeAction)
        {
            var request = await _service.FinanceAction(id, financeAction, false);
            return request.Success
                ? this.AppSuccess(request)
                : this.AppFailed<string>(request.Message);
        }

        [HttpPost("FinancePrice/{id}")]
        [Authorize(Roles = "FinanceAuthority,Admin")]
        public async Task<IActionResult> FinancePrice(int id, FinanceAction financeAction)
        {
            var request = await _service.FinanceAction(id, financeAction, true);
            return request.Success
                ? this.AppSuccess(request)
                : this.AppFailed<string>(request.Message);
        }

        /// <summary>
        /// Sub-Committee or Permanent-Committee updates the unit price for a seized item.
        /// </summary>
        [HttpPatch("{id}/price")]
        [Authorize(Roles = "SubCommittee,PermanentCommittee,Admin")]
        public async Task<IActionResult> PatchUnitPrice(int id, [FromBody] PatchUnitPriceDto dto)
        {
            var userName = User.FindFirstValue(ClaimTypes.Name);
            var result = await _saleLotService.PatchUnitPrice(id, dto.UnitPrice, userName);
            return result.Success ? this.AppSuccess(result) : this.AppFailed<string>(result.Message);
        }

        [HttpGet("GetDashboardData")]
        [Authorize(Roles = "BorderGuardOffice,FinanceAuthority,Admin")]
        public async Task<IActionResult> GetDashboardData()
        {
            var roleMembersData = User.Claims.FirstOrDefault(c => c.Type == CustomClaimTypes.RoleMembers)?.Value;
            var firmId = User.Claims.FirstOrDefault(c => c.Type == CustomClaimTypes.FirmId)?.Value;
            var roleMembers = string.IsNullOrWhiteSpace(roleMembersData)
                ? new List<string>()
                : JsonConvert.DeserializeObject<List<string>>(roleMembersData) ?? new List<string>();

            if ((User.IsInRole("BorderGuardOffice") || User.IsInRole("مكتب حرس حدود")) && 
                !roleMembers.Any(r => string.Equals(r, RoleMember.BorderGuardOffice.ToString(), StringComparison.OrdinalIgnoreCase) || string.Equals(r, "مكتب حرس حدود", StringComparison.OrdinalIgnoreCase)))
            {
                roleMembers.Add(RoleMember.BorderGuardOffice.ToString());
            }

            var requestsDetails = await _service.GetDashboardData(firmId, roleMembers);
            return requestsDetails.Success
                ? this.AppSuccess(requestsDetails)
                : this.AppFailed<DashboardData>(requestsDetails.Message);
        }

        [HttpGet("SeizuresOnDuration/{dateDuration}")]
        [Authorize(Roles = "BorderGuardOffice,SubCommittee,PermanentCommittee,ServicesAgency,FinanceAuthority,Admin")]
        public async Task<IActionResult> SeizuresOnDuration(string dateDuration)
        {
            var roleMembersData = User.Claims.FirstOrDefault(c => c.Type == CustomClaimTypes.RoleMembers)?.Value;
            var firmId = User.Claims.FirstOrDefault(c => c.Type == CustomClaimTypes.FirmId)?.Value;
            var roleMembers = JsonConvert.DeserializeObject<List<string>>(roleMembersData ?? "[]") ?? new List<string>();
            var dateDurationEnum = (DateDuration)Enum.Parse(typeof(DateDuration), dateDuration);

            if ((User.IsInRole("BorderGuardOffice") || User.IsInRole("مكتب حرس حدود")) && 
                !roleMembers.Any(r => string.Equals(r, RoleMember.BorderGuardOffice.ToString(), StringComparison.OrdinalIgnoreCase) || string.Equals(r, "مكتب حرس حدود", StringComparison.OrdinalIgnoreCase)))
            {
                roleMembers.Add(RoleMember.BorderGuardOffice.ToString());
            }

            var requestsDetails = await _service.SeizuresOnDuration(firmId, roleMembers, dateDurationEnum);
            return requestsDetails.Success
                ? this.AppSuccess(requestsDetails)
                : this.AppFailed<List<SeizureRequestDetailReadDto>>(requestsDetails.Message);
        }
    }
}
