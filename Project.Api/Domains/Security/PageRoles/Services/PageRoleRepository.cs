
using Microsoft.EntityFrameworkCore;

namespace Project.Api;
public class PageRoleRepository : BaseRepository<PageRole>, IPageRoleRepository
{

    public PageRoleRepository(ApplicationDbContext context) : base(context)
    {
    }

    //public  async Task<List<PageActionDto>> GetPageWithActions(List<string> roleIds) => await _dbSet.Where(e => !e.IsDeleted && roleIds.Contains(e.RoleId)).Include(e => e.Actions.Where(o => !o.IsDeleted))
    //                                                           .Include(e => e.Page).Select(e => new PageActionDto { PageName = e.Page.Name, ActionsName = e.Actions.Select(e=>e.Name).ToList() }).ToListAsync();

    public async Task<IEnumerable<PageRole>> GetPageNamesInRole(string roleId)
    {
        try
        {
            return await _dbSet.Where(e=>e.RoleId==roleId)
                .Include(p => p.Page)
                .OrderBy(p => p.Page.SortNumber)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error occurred while GetPageNamesInRole",ex);
            throw new RepositoryException("An error occurred while retrieving entities.", ex);
        }
    }
    public async Task<IEnumerable<PageRole>> GetPagesBasedOnRoles(List<string> roleIds)
    {
        try
        {

            return await _dbSet.Where(e => roleIds.Contains(e.RoleId))
                           .Include(p => p.Page)
                           .ToListAsync();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error occurred while GetPagesBasedOnRoles", ex);
            throw new RepositoryException("An error occurred while GetPagesBasedOnRoles entities.", ex);
        }
    }

    public async Task<List<Page>> GetMenuOnRoles(List<string> roleIds)
    {
        try
        {

            if (roleIds == null || !roleIds.Any())
            {
                return new List<Page>();
            }

            return await _dbSet
                .Where(pr => roleIds.Contains(pr.RoleId)&&pr.ReadAction)
                .Include(pr => pr.Page) 
                .Select(pr => pr.Page)
                .Distinct() 
                .ToListAsync();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error occurred while GetMenuOnRoles", ex);
            throw new RepositoryException("An error occurred while GetMenuOnRoles entities.", ex);
        }
    }

    public async Task<List<PageRole>> GetActionsBasedOnRolesAndPath(string path,List<string> roleIds)
    {
        try
        {

            if (roleIds == null || !roleIds.Any())
            {
                return new List<PageRole>();
            }

            var pageRoles = await _dbSet
                .Include(pr => pr.Page)
                .Where(pr => roleIds.Contains(pr.RoleId) && pr.ReadAction && pr.Page.To==path)
                .ToListAsync();

            // Group by Page.To and select distinct PageRole entities
            var distinctPageRoles = pageRoles
                .GroupBy(pr => pr.Page.To)
                .Select(group => group.First())
                .ToList();

            return distinctPageRoles;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error occurred while GetMenuOnRoles", ex);
            throw new RepositoryException("An error occurred while GetMenuOnRoles entities.", ex);
        }
    }

}
