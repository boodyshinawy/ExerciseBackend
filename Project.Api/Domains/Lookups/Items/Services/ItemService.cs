using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;


using System.Linq;
using System.Threading.Tasks;

namespace Project.Api
{
    public class ItemService : BaseService<Item, ItemDto>, IItemService
    {
        private readonly IItemRepository _itemRepository;

        public ItemService(
            IValidator<Item> validator,
            IItemRepository repository,
            IMapper mapper,
            IStringLocalizer<SharedService> localizer)
            : base(repository, validator, mapper, localizer)
        {
            _itemRepository = repository;
        }


        public async Task<OperationResult<List<ItemWithCategory>>> ItemWithCategory(int categoryId)
        {
            try
            {
                var itemsWithCategory = await _itemRepository.ItemWithCategory(categoryId);
                //List<PagesOnRole>? pageNameWithRoleNameDto = _mapper.Map<List<PagesOnRole>>(pageNameWithRoleName);
                return OperationResult<List<ItemWithCategory>>.Succeeded(_localizer["ItemsWithCategory"], itemsWithCategory);

            }
            catch (RepositoryException ex)
            {
                return OperationResult<List<ItemWithCategory>>.InternalServerError(_localizer["UnexpectedErrorInRepository"]);
            }
            catch (Exception ex)
            {
                return OperationResult<List<ItemWithCategory>>.InternalServerError(_localizer["AnErrorOccurredPages"]);
            }

        }


    }


}
