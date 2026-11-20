

namespace Project.Api;

public interface IItemCategoryRepository : IBaseRepository<ItemCategory>
{
    Task<List<CategoryWithItems>> CategoriesWithItems();
}