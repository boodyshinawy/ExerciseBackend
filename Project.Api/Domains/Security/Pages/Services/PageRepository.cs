namespace Project.Api;

public class PageRepository : BaseRepository<Page>, IPageRepository
{
    public PageRepository(ApplicationDbContext context) : base(context)
    {
    }

    public  async Task<List<MenuItemDto>> GetMenu(List<Page> pages)
    {
        try
        {
            return BuildHierarchy(pages, 0);

        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error occurred while retrieving pages items.");
            throw new RepositoryException("An error occurred while retrieving entities.", ex);
        }
    }

    public async Task<List<Page>> GetMenuByRoles(List<string> roleIds)
    {
        try
        {
            if (roleIds == null || !roleIds.Any())
            {
                return new List<Page>();
            }

            return await _dbSet
                .Where(p => p.PageRoles.Any(pr => roleIds.Contains(pr.RoleId) && pr.ReadAction))
                .ToListAsync();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error occurred while GetMenuByRoles", ex);
            throw new RepositoryException("An error occurred while GetMenuByRoles entities.", ex);
        }
    }

    public  async Task<IEnumerable<Page>> GetPageNameWithRoleName(string firmId)
    {
        try
        {
            return await _dbSet
                .Include(p => p.PageRoles)
                .ThenInclude(pr => pr.Role)
                //.Where(p => p.PageRoles.Any(pr => pr.Role.FirmId == firmId))
                .ToListAsync();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error occurred while retrieving entities.");
            throw new RepositoryException("An error occurred while retrieving entities.", ex);
        }
    }

    private List<MenuItemDto> BuildHierarchy(List<Page> pages, int parentId)
    {
        return pages
            .Where(p => p.ParentId == parentId)
            .OrderBy(p => p.SortNumber)
            .Select(p => new MenuItemDto
            {
                Label = p.Label,
                Icon = p.Icon,
                To = p.To,
                Items = BuildHierarchy(pages, p.Id) // Recursive call
            })
            .ToList();
    }
}
