using Microsoft.Extensions.Localization;
using Project.Shared;

namespace Project.Api;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IMapper _mapper;
    private readonly IStringLocalizer<SharedController> _localizer;
    private readonly ApplicationDbContext _context;
    private readonly IRoleService _roleService;

    public UsersController(IUserService userService,
                       IMapper mapper,
                       IStringLocalizer<SharedController> localizer,
                       ApplicationDbContext context,
                       IRoleService roleService)
    {
        _userService = userService;
        _mapper = mapper;
        _localizer = localizer;
        _context = context;
        _roleService = roleService;
    }

    [HttpGet("UsersInTenant")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetUsersByTenantId()
    {
        try
        {
            var users = await _userService.Users
                .Include(u => u.Firm)
                .ToListAsync();
            var usersWithRoles = new List<UserDto>();

            foreach (var user in users)
            {
                var userRoles = await _context.UserRoles.Where(ur => ur.UserId == user.Id).ToListAsync();
                var roles = new List<RoleDto>();

                foreach (var userRole in userRoles)
                {
                    var role = await _roleService.FindByIdAsync(userRole.RoleId);
                    if (role != null)
                    {
                        roles.Add(new RoleDto
                        {
                            Id = role.Id,
                            Name = role.Name,
                            NameAr = role.NameAr,
                            Color = role.Color
                        });
                    }
                }

                usersWithRoles.Add(new UserDto
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Name = user.Name,
                    FirmId = user.FirmId,
                    FirmName = user.Firm?.Name,
                    Roles = roles,
                    Status = user.Status
                });
            }

            return this.AppSuccess<List<UserDto>>(_localizer["SuccessProcess"], usersWithRoles);
        }
        catch (Exception)
        {
            return this.AppFailed<ApplicationRole>(_localizer["UnexpectedError"]);
        }
    }

    [HttpGet("GetUser")]
    [Authorize]
    public async Task<IActionResult> GetUser()
    {
        ApplicationUser? user = await _userService.FindByNameAsync(User.Identity!.Name!);

        if (user == null || user.IsDeleted)
            return this.AppNotFound<ApplicationUser>(_localizer["NotFound"]);

        var rolesName = await _userService.GetRoleNamesAsync(user);
        var roles = new List<ApplicationRole>();
        foreach (var role in rolesName)
        {
            var roleData = await _roleService.FindByNameAsync(role);
            if (roleData != null)
            {
                roles.Add(roleData);
            }
        }

        var roleMembersData = User.Claims.FirstOrDefault(c => c.Type == CustomClaimTypes.RoleMembers)?.Value;
        var roleMembers = JsonConvert.DeserializeObject<List<string>>(roleMembersData ?? "[]");
        var userInformation = new UserInformation
        {
            Name = user.Name,
            RoleMembers = roleMembers,
            firmName = user.Firm?.Name ?? string.Empty,
            RolesWithFlags = roles.Select(e => new RoleWithFlag
            {
                Name = e.Name,
            }).ToList()
        };

        return this.AppSuccess<UserInformation>(_localizer["SuccessProcess"], userInformation);
    }

    [HttpPost("UpdateUserBasedOnName")]
    [Authorize]
    public async Task<IActionResult> UpdateUser([FromBody] UserDto model)
    {
        var user = await _userService.FindByNameAsync(User.Identity!.Name!);
        if (user == null || user.IsDeleted)
            return this.AppNotFound<ApplicationUser>(_localizer["NotFound"]);

        user.Name = model.Name;
        user.UserName = model.UserName;
        user.Status = model.Status;

        var result = await _userService.UpdateAsync(user);
        if (result.Succeeded)
            return this.AppSuccess<ApplicationUser>(_localizer["SuccessProcess"]);

        return this.AppFailed<dynamic>(_localizer["FailedToUpdateUser"]);
    }

    [HttpPost("ChangePassword")]
    [Authorize]
    public async Task<IActionResult> ChangePassword(ChangePassword changePassword)
    {
        if (changePassword.NewPassword != changePassword.ConfirmPassword)
            return this.AppFailed<ApplicationUser>(_localizer["PasswordsDoNotMatch"]);

        ApplicationUser? user = await _userService.FindByNameAsync(User.Identity!.Name!);
        if (user == null || user.IsDeleted)
            return this.AppNotFound<ApplicationUser>(_localizer["NotFound"]);

        var result = await _userService.ChangePasswordAsync(user, changePassword.NewPassword);

        if (result.Succeeded)
            return this.AppSuccess<ApplicationUser>(_localizer["SuccessProcess"]);

        return this.AppFailed<dynamic>(_localizer["FailedToChangePassword"]);
    }

    [HttpGet("GetUserById/{userId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetUserById(string userId)
    {
        ApplicationUser? user = await _userService.FindByIdAsync(userId);
        if (user == null || user.IsDeleted)
            return this.AppNotFound<ApplicationUser>(_localizer["NotFound"]);

        return this.AppSuccess<ApplicationUser>(_localizer["SuccessProcess"], user);
    }

    [HttpPost("UpdateUser/{userId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateUser(string userId, [FromBody] UserDto model)
    {
        using var transaction = _context.Database.BeginTransaction();

        try
        {
            var user = await _userService.FindByIdAsync(userId);
            if (user == null || user.IsDeleted)
                return this.AppNotFound<ApplicationUser>(_localizer["NotFound"]);

            user.Name = model.Name ?? user.Name;
            user.UserName = model.UserName ?? user.UserName;
            user.Status = model.Status ?? user.Status;
            user.FirmId = model.FirmId ?? user.FirmId;

            var updateResult = await _userService.UpdateAsync(user);
            if (!updateResult.Succeeded)
                return this.AppFailed<dynamic>(_localizer["FailedToUpdateUser"]);

            if (model.Status == "Blocked")
            {
                var lockoutResult = await _userService.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);
                if (!lockoutResult.Succeeded)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = _localizer["FailedToLockUser"] });
                }
            }
            else if (model.Status == "Active")
            {
                var unlockResult = await _userService.SetLockoutEndDateAsync(user, null);
                if (!unlockResult.Succeeded)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = _localizer["FailedToUnlockUser"] });
                }
            }

            var userRoles = await _context.UserRoles
                .Where(ur => ur.UserId == user.Id)
                .ToListAsync();

            _context.UserRoles.RemoveRange(userRoles);
            await _context.SaveChangesAsync();

            if (model.Roles != null && model.Roles.Any())
            {
                foreach (var roleDto in model.Roles)
                {
                    var role = await _roleService.FindByIdAsync(roleDto.Id);
                    if (role != null)
                    {
                        _context.UserRoles.Add(new UserRole
                        {
                            UserId = user.Id,
                            RoleId = role.Id
                        });
                        await _context.SaveChangesAsync();
                    }
                }
            }

            var firm = await _context.Set<Firm>().FirstOrDefaultAsync(f => f.Id == user.FirmId);
            var userDto = new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                UserName = user.UserName,
                Status = user.Status,
                Roles = model.Roles ?? new List<RoleDto>(),
                FirmId = user.FirmId,
                FirmName = firm?.Name
            };

            transaction.Commit();
            return this.AppSuccess<UserDto>(_localizer["SuccessProcess"], userDto);
        }
        catch (Exception)
        {
            transaction.Rollback();
            return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = _localizer["UnexpectedError"] });
        }
    }

    [HttpDelete("deleteUser/{userId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteUser(string userId)
    {
        using var transaction = _context.Database.BeginTransaction();

        try
        {
            var user = await _userService.FindByIdAsync(userId);
            if (user == null)
            {
                return StatusCode(StatusCodes.Status404NotFound, new Response { Status = "Error", Message = _localizer["UserNotFound"] });
            }

            var userRoles = await _context.UserRoles
                .Where(ur => ur.UserId == user.Id)
                .ToListAsync();

            _context.UserRoles.RemoveRange(userRoles);
            await _context.SaveChangesAsync();

            var deleteUserResult = await _userService.DeleteAsync(user);
            if (!deleteUserResult.Succeeded)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = _localizer["FailedToDeleteUser"] });
            }

            transaction.Commit();
            return Ok(new Response { Status = "Success", Message = _localizer["UserDeletedSuccessfully"] });
        }
        catch (Exception)
        {
            transaction.Rollback();
            return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = _localizer["UnexpectedError"] });
        }
    }

    [HttpPost("ChangePassword/{userId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ChangePassword(string userId, ChangePassword changePassword)
    {
        if (changePassword.NewPassword != changePassword.ConfirmPassword)
            return this.AppFailed<ApplicationUser>(_localizer["PasswordsDoNotMatch"]);

        ApplicationUser? user = await _userService.FindByIdAsync(userId);
        if (user == null || user.IsDeleted)
            return this.AppNotFound<ApplicationUser>(_localizer["NotFound"]);

        var result = await _userService.ChangePasswordAsync(user, changePassword.NewPassword);

        if (result.Succeeded)
            return this.AppSuccess<ApplicationUser>(_localizer["SuccessProcess"]);

        return this.AppFailed<dynamic>(_localizer["FailedToChangePassword"]);
    }
}
