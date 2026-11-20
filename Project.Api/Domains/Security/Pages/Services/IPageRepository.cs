

namespace Project.Api;

public interface IPageRepository : IBaseRepository<Page>
{
    Task<List<MenuItemDto>> GetMenu(List<Page> pages);
    Task<List<Page>> GetMenuByRoles(List<string> roleIds);
    Task<IEnumerable<Page>> GetPageNameWithRoleName(string FirmId);
}