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
    public class ItemCategoriesController : BaseController<ItemCategory, ItemCategoryDto>
    {
        private readonly IItemCategoryService _service;

        public ItemCategoriesController(IItemCategoryService service) : base(service)
        {
            _service = service;
        }
        [HttpGet("CategoryWithItems")]
        public async Task<IActionResult> CategoryWithItems()
        {
            var pagesResult = await _service.CategoryWithItems();
            return pagesResult.Success
                ? this.AppSuccess(pagesResult)
                : this.AppFailed<List<PagesOnRole>>(pagesResult.Message);

        }


    }
}
