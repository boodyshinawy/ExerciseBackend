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
    public class StatusesController : BaseController<Status, StatusDto>
    {
        private readonly IStatusService _service;

        public StatusesController(IStatusService service) : base(service)
        {
            _service = service;
        }



    }
}
