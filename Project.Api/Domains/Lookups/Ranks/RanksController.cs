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
    public class RanksController : BaseController<Rank, RankDto>
    {
        private readonly IRankService _service;

        public RanksController(IRankService service) : base(service)
        {
            _service = service;
        }



    }
}
