namespace Project.Api;

public class ItemRepository : BaseRepository<Item>, IItemRepository
{
    public ItemRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<List<ItemWithCategory>> ItemWithCategory(int categoryId)
    {
        try
        {
            var items = await _dbSet.Include(e => e.ItemCategory).Where(e=>e.ItemCategoryId==categoryId).OrderBy(e => e.Id).Select(ca => new ItemWithCategory
            {
                Id = ca.Id,
                Name = ca.Name,
                Catgory =  new ItemCategoryDto{
                Id=ca.ItemCategory.Id,
                Name=ca.ItemCategory.Name,
                Color=ca.ItemCategory.Color
                }
            })
             .ToListAsync();
            return items;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error occurred while ItemWithCategory", ex);
            throw new RepositoryException("An error occurred while ItemWithCategory.", ex);
        }
    }


}
