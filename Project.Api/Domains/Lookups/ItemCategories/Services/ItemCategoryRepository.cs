namespace Project.Api;

public class ItemCategoryRepository : BaseRepository<ItemCategory>, IItemCategoryRepository
{
    public ItemCategoryRepository(ApplicationDbContext context) : base(context)
    {
    }
    public async Task<List<CategoryWithItems>> CategoriesWithItems()
    {
        try
        {
            var categories= await _dbSet.Include(e=>e.Items).OrderBy(e=>e.Id).Select(ca=>new CategoryWithItems
            {
                Id=ca.Id,
                Name=ca.Name,
                Color=ca.Color,
                Items=ca.Items.Select(e=>e.Name).ToList()??new List<string>()
            })
             .ToListAsync();
            return categories;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error occurred while CategoryWithItems", ex);
            throw new RepositoryException("An error occurred while CategoryWithItems.", ex);
        }
    }




}
