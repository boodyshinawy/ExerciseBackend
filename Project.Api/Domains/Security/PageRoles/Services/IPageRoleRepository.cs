


namespace Project.Api;
public interface IPageRoleRepository : IBaseRepository<PageRole>
{
    Task<List<PageRole>> GetActionsBasedOnRolesAndPath(string path, List<string> roleIds);
    Task<List<Page>> GetMenuOnRoles(List<string> roleIds);

    //Task<List<PageActionDto>> GetPageWithActions(List<string> roleIds);
    Task<IEnumerable<PageRole>> GetPageNamesInRole(string roleId);
    Task<IEnumerable<PageRole>> GetPagesBasedOnRoles(List<string> roleIds);
}