


namespace Project.Api;

public interface IItemRepository : IBaseRepository<Item>
{
    Task<List<ItemWithCategory>> ItemWithCategory(int categoryId);
}