
namespace Project.Api;
public interface IPageRoleService : IBaseService<PageRole, PageRoleDto>
{
    Task<OperationResult<List<PageRoleDto>>> GetActions(string path, List<string> rolesIds);
    Task<OperationResult<CategorizedMenuItemsDto>> GetMenuItems(List<string> rolesId);
    Task<OperationResult<List<PagesOnRole>>> GetPageNamesInRole(string roleId);
    Task<OperationResult<List<PagesOnRole>>> SavePermissions(List<PagesOnRole> pagesOnRole);
}
