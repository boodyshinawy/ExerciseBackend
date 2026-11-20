using Microsoft.AspNetCore.Mvc.Filters;

namespace Project.Api;

public class HasPermissionAttribute : TypeFilterAttribute
{
    public HasPermissionAttribute( string module) : base(typeof(PermissionRequirementFilter))
    {
        Arguments = new object[] { module };
    }
}

public class PermissionRequirementFilter : IAuthorizationFilter
{
    private readonly string _module;
    private readonly IPageRoleRepository _pageRoleRepository;

    public PermissionRequirementFilter(string module,
        IPageRoleRepository pageRoleRepository)
    {
        _module = module;
        _pageRoleRepository = pageRoleRepository;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var userId = context.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            context.Result = new ForbidResult();
            return;
        }
        var roleClaims = context.HttpContext.User.Claims.Where(c => c.Type == CustomClaimTypes.RolesId).ToList();
        var roleIds = roleClaims.Select(c => c.Value).ToList();
        if (!roleIds.Any())
        {
            context.Result = new ForbidResult();
            return;
        }
        var task = Task.Run(async () =>
                await _pageRoleRepository.GetPagesBasedOnRoles(roleIds)
                                );

        var pagesWithActions = task.Result.ToList();

        if (!pagesWithActions.Any(p => p.Page.Label == _module))
        {
            context.Result = new ForbidResult();
            return;
        }

        var actionName = context.ActionDescriptor.EndpointMetadata.OfType<ModuleActionAttribute>().FirstOrDefault()?.ActionName;

        if (!string.IsNullOrEmpty(actionName))
        {
            if (!pagesWithActions.Any(p => p.Page.Label == _module))
            //&& p.ActionsName.Any(a => a == actionName)))
            {
                context.Result = new ForbidResult();
            }
        }
        else
        {
            var method = context.HttpContext.Request.Method;

            if (method == "GET" && !pagesWithActions.Any(p => p.Page.Label == _module && p.ReadAction))
            {
                context.Result = new ForbidResult();
            }

            else if (method == "POST" && !pagesWithActions.Any(p => p.Page.Label == _module && p.CreateAction))
            {
                context.Result = new ForbidResult();
            }

            //else if (method == "PUT" && !pagesWithActions.Any(p => p.Page.Label == _module && p.ActionsName.Any(a => a == Actions.Update)))
            else if (method == "PUT" && !pagesWithActions.Any(p => p.Page.Label == _module && p.UpdateAction))
            {
                context.Result = new ForbidResult();
            }

            else if (method == "DELETE" && !pagesWithActions.Any(p => p.Page.Label == _module && p.DeleteAction))
            {
                context.Result = new ForbidResult();
            }
        }
    }
}
