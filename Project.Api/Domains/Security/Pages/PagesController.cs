using Microsoft.AspNetCore.Mvc;
using Project.Shared;

using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Project.Api
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class PagesController : BaseController<Page, PageDto>
    {
        private readonly IPageService _service;

        public PagesController(IPageService service) : base(service)
        {
            _service = service;
        }



    }
}
