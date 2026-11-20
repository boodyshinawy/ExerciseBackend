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
    public class ItemsController : BaseController<Item, ItemDto>
    {
        private readonly IItemService _service;

        public ItemsController(IItemService service) : base(service)
        {
            _service = service;
        }

        [HttpGet("ItemWithCategory/{categoryId}")]
        public async Task<IActionResult> ItemWithCategory(int categoryId)
        {
            var items = await _service.ItemWithCategory(categoryId);
            return items.Success
                ? this.AppSuccess(items)
                : this.AppFailed<List<ItemWithCategory>>(items.Message);

        }



    }
}
