using Microsoft.AspNetCore.Mvc;

using System.Threading.Tasks;

namespace Project.Shared
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class BaseSettingController<TEntity, TDto> : BaseController<TEntity, TDto>
        where TEntity : BaseSettingEntity
        where TDto : BaseSettingDto
    {
        private readonly IBaseSettingService<TEntity, TDto> _baseSettingService;

        public BaseSettingController(IBaseSettingService<TEntity, TDto> service, IBaseSettingService<TEntity, TDto> baseSettingService)
            : base(service)
        {
            _baseSettingService = baseSettingService;
        }

        [HttpGet("Search/{searchText}")]
        public virtual async Task<IActionResult> Search(string searchText)
        {
            try
            {
                var searchResult = await _baseSettingService.Search(searchText);
                return this.AppSuccess<IEnumerable<TDto>>(searchResult);
            }
            catch (Exception ex)
            {
                return this.AppInternalServerError<TEntity>(ex.Message);
            }
        }
    }
}
