namespace Project.Api;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class PageRolesController : BaseController<PageRole, PageRoleDto>
{
    private readonly IPageRoleService _service;
    public PageRolesController(IPageRoleService service) : base(service)
    {
        _service = service;
    }

    [HttpGet("GetPageNamesInRole/{roleId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetPageNamesInRole(string roleId)
    {
        var pagesResult = await _service.GetPageNamesInRole(roleId);
        return pagesResult.Success
            ? this.AppSuccess(pagesResult)
            : this.AppFailed<List<PagesOnRole>>(pagesResult.Message);
    }

    [HttpPost("SavePermission")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> SavePermission(List<PagesOnRole> entities)
    {
        var pagesResult = await _service.SavePermissions(entities);
        return pagesResult.Success
            ? this.AppSuccess(pagesResult)
            : this.AppFailed<List<PagesOnRole>>(pagesResult.Message);
    }

    [HttpGet("GetMenuItems")]
    [Authorize]
    public async Task<IActionResult> GetMenuItems()
    {
        var roleClaims = User.Claims.Where(c => c.Type == CustomClaimTypes.RolesId).ToList();
        var roleIds = roleClaims.Select(c => c.Value).ToList();
        var pagesResult = await _service.GetMenuItems(roleIds);
        return pagesResult.Success
            ? this.AppSuccess(pagesResult)
            : this.AppFailed<List<PagesOnRole>>(pagesResult.Message);
    }

    [HttpPost("GetActions")]
    [Authorize]
    public async Task<IActionResult> GetActions([FromBody] string path)
    {
        var roleClaims = User.Claims.Where(c => c.Type == CustomClaimTypes.RolesId).ToList();
        var roleIds = roleClaims.Select(c => c.Value).ToList();
        var ActionResult = await _service.GetActions(path, roleIds);
        return ActionResult.Success
            ? this.AppSuccess(ActionResult)
            : this.AppFailed<List<PagesOnRole>>(ActionResult.Message);
    }
}
