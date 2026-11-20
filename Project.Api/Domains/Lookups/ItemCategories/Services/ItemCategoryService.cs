using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;


using System.Linq;
using System.Threading.Tasks;

namespace Project.Api
{
    public class ItemCategoryService : BaseService<ItemCategory, ItemCategoryDto>, IItemCategoryService
    {
        private readonly IItemCategoryRepository _itemCategoryRepository;

        public ItemCategoryService(
            IValidator<ItemCategory> validator,
            IItemCategoryRepository repository,
            IMapper mapper,
            IStringLocalizer<SharedService> localizer)
            : base(repository, validator, mapper, localizer)
        {
            _itemCategoryRepository = repository;
        }
        public async Task<OperationResult<List<CategoryWithItems>>> CategoryWithItems()
        {
            try
            {
                var categoriesWithItems = await _itemCategoryRepository.CategoriesWithItems();
                //List<PagesOnRole>? pageNameWithRoleNameDto = _mapper.Map<List<PagesOnRole>>(pageNameWithRoleName);
                return OperationResult<List<CategoryWithItems>>.Succeeded(_localizer["GetCategoriesWithItems"], categoriesWithItems);

            }
            catch (RepositoryException ex)
            {
                return OperationResult<List<CategoryWithItems>>.InternalServerError(_localizer["UnexpectedErrorInRepository"]);
            }
            catch (Exception ex)
            {
                return OperationResult<List<CategoryWithItems>>.InternalServerError(_localizer["AnErrorOccurredPages"]);
            }

        }

    }


}
