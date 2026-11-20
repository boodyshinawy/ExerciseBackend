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
    public class RequestDetailAttachmentsController : BaseController<RequestDetailAttachment, RequestDetailAttachmentDto>
    {
        private readonly IRequestDetailAttachmentService _service;

        public RequestDetailAttachmentsController(IRequestDetailAttachmentService service) : base(service)
        {
            _service = service;
        }
        [HttpPost("CreateRequestDetailAttachmensts/{seizureRequestDetailId}")]
        public async Task<IActionResult> CreateRequestDetailAttachmensts(int seizureRequestDetailId, [FromForm] List<RequestDetailAttachmentUploading> requestDetailAttachmentUploadings )
        {

            var firmId = User.Claims.FirstOrDefault(c => c.Type == CustomClaimTypes.FirmId)?.Value;

            var seizureRequests = await _service.CreateRequestDetailAttachmensts(seizureRequestDetailId,requestDetailAttachmentUploadings,firmId);
            return seizureRequests.Success
                ? this.AppSuccess(seizureRequests)
                : this.AppFailed<string>(seizureRequests.Message);

        }

        [HttpDelete("DeleteRequestDetailAttachmensts/{attachmentId}")]
        public async Task<IActionResult> DeleteRequestDetailAttachmensts(int attachmentId)
        {

            var seizureRequests = await _service.DeleteRequestDetailAttachmensts(attachmentId);
            return seizureRequests.Success
                ? this.AppSuccess(seizureRequests)
                : this.AppFailed<string>(seizureRequests.Message);

        }

        [HttpGet("GetRequestDetailAttachmensts/{seizureRequestDetailId}")]
        public async Task<IActionResult> GetRequestDetailAttachmensts(int seizureRequestDetailId)
        {

            var seizureRequests = await _service.GetRequestDetailAttachmensts(seizureRequestDetailId);
            return seizureRequests.Success
                ? this.AppSuccess(seizureRequests)
                : this.AppFailed<string>(seizureRequests.Message);

        }


    }
}
