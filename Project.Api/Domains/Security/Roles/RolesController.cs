using Project.Shared;

namespace Project.Api;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/[controller]")]
public class RolesController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IRoleService _roleService;
    private readonly ApplicationDbContext _context;
    private readonly IStringLocalizer<SharedController> _localizer;

    public RolesController(IUserService userService,
                           IRoleService roleService,
                           ApplicationDbContext context,
                           IStringLocalizer<SharedController> localizer)
    {
        _userService = userService;
        _roleService = roleService;
        _context = context;
        _localizer = localizer;
    }

    [HttpGet("GetRolesWithUsers")]
    public virtual async Task<IActionResult> GetRolesWithUsers()
    {
        try
        {
            var roles = _roleService.Roles;

            var rolesWithUserRoles = roles.GroupJoin(
                _context.UserRoles,
                role => role.Id,
                userRole => userRole.RoleId,
                (role, userRoles) => new { role, userRoles }
            );

            var rolesWithUsers = rolesWithUserRoles.SelectMany(
                temp => temp.userRoles.DefaultIfEmpty(),
                (temp, userRole) => new { temp.role, userRole }
            );

            var rolesWithUsersJoined = rolesWithUsers.GroupJoin(
                _userService.Users,
                temp => temp.userRole != null ? temp.userRole.UserId : null,
                user => user.Id,
                (temp, users) => new { temp.role, Users = users }
            );

            var rolesWithUsersFlat = rolesWithUsersJoined.SelectMany(
                temp => temp.Users.DefaultIfEmpty(),
                (temp, user) => new { temp.role, user }
            );

            var groupedRolesWithUsers = rolesWithUsersFlat.GroupBy(
                g => new { g.role.Id, g.role.Color, g.role.Name, g.role.NameAr, g.role.RoleMember }
            );

            var rolesWithUsersDto = await groupedRolesWithUsers.Select(group => new RoleWithUsersDto
            {
                Id = group.Key.Id,
                Color = group.Key.Color,
                Name = group.Key.Name,
                NameAr = group.Key.NameAr,
                RoleMember = group.Key.RoleMember,
                Users = group.Where(g => g.user != null).Select(g => g.user!.UserName).ToList()
            }).ToListAsync();

            return this.AppSuccess<List<RoleWithUsersDto>>(_localizer["SuccessProcess"], rolesWithUsersDto);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "An error occurred while retrieving roles and users");
            return this.AppFailed<ApplicationRole>(_localizer["UnexpectedError"]);
        }
    }

    [HttpGet("GetRolesBasedOnTenant")]
    public IActionResult GetRolesBasedOnTenant()
    {
        try
        {
            List<ApplicationRole> roles = _roleService.Roles.ToList();
            List<FirmDto>? firms = _context.Set<Firm>().Select(ca => new FirmDto
            {
                FirmId = ca.Id,
                FirmName = ca.Name,
            }).ToList();

            var securityLookup = new SecurityLookup
            {
                Roles = roles,
                Firms = firms
            };

            return this.AppSuccess<SecurityLookup>(_localizer["SuccessProcess"], securityLookup);
        }
        catch (Exception ex)
        {
            Log.Error("canot get all roles based on tenant", ex);
            return this.AppFailed<string>(_localizer["UnexpectedError"]);
        }
    }

    [HttpPost]
    [Route("UpdateRole")]
    public async Task<IActionResult> UpdateRole(RoleDto roleDto)
    {
        try
        {
            ApplicationRole? role = await _roleService.FindByIdAsync(roleDto.Id);
            if (role == null)
            {
                return this.AppFailed<RoleDto>(_localizer["UnexpectedError"]);
            }

            var cleanName = (roleDto.Name ?? string.Empty).Replace(" ", "");
            bool nameTaken = await _context.Set<ApplicationRole>().AnyAsync(r => r.Name == cleanName && r.Id != roleDto.Id);
            if (nameTaken)
            {
                return this.AppFailed<RoleDto>(_localizer["RoleAlreadyExists"]);
            }

            role.Name = cleanName;
            role.NameAr = roleDto.NameAr;
            role.Color = roleDto.Color;
            role.RoleMember = cleanName;
            await _roleService.UpdateAsync(role);
            roleDto.Name = cleanName;
            roleDto.RoleMember = cleanName;
            return this.AppSuccess<RoleDto>(_localizer["SuccessProcess"], roleDto);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "An error occurred while updating role");
            return this.AppFailed<RoleDto>(_localizer["UnexpectedError"]);
        }
    }

    [HttpPost("CreateRole")]
    public async Task<IActionResult> CreateRole(RoleDto roleDto)
    {
        var cleanName = (roleDto.Name ?? string.Empty).Replace(" ", "");
        if (string.IsNullOrEmpty(cleanName))
            return this.AppFailed<ApplicationRole>(_localizer["RoleNameEmpty"]);
        if (string.IsNullOrEmpty(roleDto.Color))
            return this.AppFailed<ApplicationRole>(_localizer["ColorEmpty"]);

        using var transaction = _context.Database.BeginTransaction();
        try
        {
            bool roleExist = await _context.Set<ApplicationRole>().AnyAsync(r => r.Name == cleanName);
            if (roleExist)
                return this.AppFailed<ApplicationRole>(_localizer["RoleAlreadyExists"]);

            // Calculate next incremental ID
            var existingIds = await _context.Set<ApplicationRole>()
                .Select(r => r.Id)
                .ToListAsync();

            int nextId = 1;
            if (existingIds.Any())
            {
                var numericIds = existingIds
                    .Select(id => int.TryParse(id, out var parsed) ? parsed : 0)
                    .ToList();

                if (numericIds.Any())
                {
                    nextId = numericIds.Max() + 1;
                }
            }

            var role = new ApplicationRole
            {
                Id = nextId.ToString(),
                Name = cleanName,
                NameAr = roleDto.NameAr,
                Color = roleDto.Color,
                RoleMember = cleanName
            };
            var result = await _roleService.CreateAsync(role);

            if (!result.Succeeded)
                return this.AppFailed<dynamic>(_localizer["FailedToCreateRole"]);

            var pages = await _context.Set<Page>().ToListAsync();
            foreach (var page in pages)
            {
                _context.Set<PageRole>().Add(new PageRole
                {
                    PageId = page.Id,
                    RoleId = role.Id,
                    CreateAction = false,
                    ReadAction = false,
                    UpdateAction = false,
                    DeleteAction = false
                });
            }
            await _context.SaveChangesAsync();

            transaction.Commit();
            return this.AppSuccess<ApplicationRole>(_localizer["SuccessProcess"], role);
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            Log.Error("there are error when creating with assigning pages to it ", ex);
            return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = _localizer["UnexpectedError"] });
        }
    }

    [HttpPost("RemoveRole")]
    public async Task<IActionResult> RemoveRole(string roleId)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var role = await _roleService.Roles.SingleOrDefaultAsync(r => r.Id == roleId);
            if (role == null || role.IsDeleted)
            {
                return this.AppNotFound<ApplicationRole>(_localizer["RoleNotFound"]);
            }

            var usersInRole = await _context.UserRoles
                .Where(ur => ur.RoleId == roleId)
                .Select(ur => ur.UserId)
                .ToListAsync();

            if (usersInRole.Any())
            {
                return this.AppFailed<string>(_localizer["RoleCannotBeDeletedBecauseItHasUsers"]);
            }

            var pageRoles = await _context.Set<PageRole>().Where(pr => pr.RoleId == roleId).ToListAsync();
            _context.Set<PageRole>().RemoveRange(pageRoles);

            _context.Roles.Remove(role);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return this.AppSuccess<ApplicationRole>(_localizer["SuccessProcess"]);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            return this.AppFailed<dynamic>(_localizer["FailedToRemoveRole"]);
        }
    }

    [HttpGet("GetAllRoles")]
    public IActionResult GetAllRoles()
    {
        List<ApplicationRole> roles = _roleService.Roles.ToList();
        return this.AppSuccess<List<ApplicationRole>>(_localizer["SuccessProcess"]);
    }
}
