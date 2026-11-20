

namespace Project.Api;

public interface IItemCategoryService : IBaseService<ItemCategory, ItemCategoryDto>
{
    Task<OperationResult<List<CategoryWithItems>>> CategoryWithItems();
}
