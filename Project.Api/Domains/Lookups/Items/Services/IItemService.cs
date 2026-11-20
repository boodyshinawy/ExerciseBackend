

namespace Project.Api;

public interface IItemService : IBaseService<Item, ItemDto>
{
    Task<OperationResult<List<ItemWithCategory>>> ItemWithCategory(int categoryId);
}
