using Microsoft.Extensions.Localization;
using Project.Shared;

namespace Project.Api;

[Authorize]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly JwtOptions _jwtSettings;
    private readonly IJwtProvider _jwtProvider;
    private readonly ApplicationDbContext _context;
    private readonly IRoleService _roleService;
    private readonly IStringLocalizer<SharedController> _localizer;

    public AuthController(
        IUserService userService,
        IOptions<JwtOptions> jwtSettings,
        IJwtProvider jwtProvider,
        ApplicationDbContext context,
        IRoleService roleService,
        IStringLocalizer<SharedController> localizer)
    {
        _userService = userService;
        _jwtSettings = jwtSettings.Value;
        _jwtProvider = jwtProvider;
        _context = context;
        _roleService = roleService;
        _localizer = localizer;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        var user = await _userService.FindByNameAsync(model.Username);

        if (user == null)
        {
            return this.AppUnauthorized<ApplicationUser>(_localizer["UserNotFoundOrDeleted"]);
        }

        var invalidResult = await HandleInvalidPasswordOrLockoutAsync(user, model.Password);
        if (invalidResult != null)
        {
            return invalidResult;
        }

        await InvalidateUserSessionsAsync(user.Id);

        var newSessionId = Guid.NewGuid().ToString();
        await AddUserSessionAsync(user.Id, newSessionId);

        var authClaims = await GenerateUserClaims(user, newSessionId);

        var token = _jwtProvider.GenerateToken(authClaims, model.RememberMe);
        var refreshToken = _jwtProvider.GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.Now.AddDays(_jwtSettings.RefreshTokenLifetime).ToUniversalTime();
        await _userService.UpdateAsync(user);

        var roleClaims = authClaims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList();
        var roleMembersJson = authClaims.FirstOrDefault(c => c.Type == CustomClaimTypes.RoleMembers)?.Value;
        var roleMembers = string.IsNullOrEmpty(roleMembersJson)
            ? new List<string>()
            : JsonConvert.DeserializeObject<List<string>>(roleMembersJson) ?? new List<string>();

        return Ok(new
        {
            Success = true,
            Message = _localizer["LoginSuccessfuly"],
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            RefreshToken = refreshToken,
            Expiration = token.ValidTo,
            Roles = roleClaims,
            RoleMembers = roleMembers
        });
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterModel model)
    {
        using var transaction = _context.Database.BeginTransaction();

        try
        {
            var userExists = await _userService.FindByNameAsync(model.Username);
            if (userExists != null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = _localizer["UserAlreadyExists"] });
            }

            var firmId = model.FirmId;
            if (string.IsNullOrEmpty(firmId))
            {
                return this.AppFailed<ApplicationRole>(_localizer["TenantIdEmpty"]);
            }

            if (string.IsNullOrEmpty(model.Password))
            {
                return this.AppFailed<string>(_localizer["passwordEmpty"]);
            }
            if (model.Password != model.ConfirmPassword)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new Response { Status = "Error", Message = _localizer["PasswordAndConfirmPasswordDoNotMatch"] });
            }

            var user = new ApplicationUser
            {
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Username,
                Name = model.Name,
                FirmId = firmId,
                Status = model.Status
            };

            var result = await _userService.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = _localizer["UserCreationFailed"] });
            }

            var roles = await AssignRolesToUser(model.Roles, user);

            if (model.Status == "Blocked")
            {
                var lockoutResult = await _userService.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);
                if (!lockoutResult.Succeeded)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = _localizer["FailedToLockUser"] });
                }
            }

            var firm = await _context.Set<Firm>().FirstOrDefaultAsync(f => f.Id == user.FirmId);
            var userDto = new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                UserName = user.UserName,
                Status = user.Status,
                Roles = roles,
                FirmId = user.FirmId,
                FirmName = firm?.Name
            };

            transaction.Commit();
            return Ok(new { Status = "Success", Message = _localizer["UserCreatedSuccessfully"], Data = userDto });
        }
        catch (Exception)
        {
            transaction.Rollback();
            return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = _localizer["UnexpectedError"] });
        }
    }

    [AllowAnonymous]
    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken(TokenModel tokenModel)
    {
        if (tokenModel is null)
        {
            return BadRequest(_localizer["InvalidClientRequest"]);
        }

        string? accessToken = tokenModel.AccessToken;
        string? refreshToken = tokenModel.RefreshToken;

        var principal = _jwtProvider.GetPrincipalFromExpiredToken(accessToken);
        if (principal == null)
            return BadRequest(_localizer["InvalidAccessTokenOrRefreshToken"]);

        string username = principal.Identity!.Name!;

        var user = await _userService.FindByNameAsync(username);

        if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
        {
            return BadRequest(_localizer["InvalidAccessTokenOrRefreshToken"]);
        }

        var newAccessToken = _jwtProvider.GenerateToken(principal.Claims.ToList(), true);
        var newRefreshToken = _jwtProvider.GenerateRefreshToken();

        user.RefreshToken = newRefreshToken;
        await _userService.UpdateAsync(user);

        return new ObjectResult(new
        {
            accessToken = new JwtSecurityTokenHandler().WriteToken(newAccessToken),
            refreshToken = newRefreshToken
        });
    }

    [HttpGet("userclaims")]
    public async Task<IActionResult> GetUserClaims()
    {
        var roleName = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.UserData)?.Value;

        return Ok(roleName);
    }

    [HttpPost("revoke/{username}")]
    public async Task<IActionResult> Revoke(string username)
    {
        var user = await _userService.FindByNameAsync(username);
        if (user == null) return BadRequest(_localizer["InvalidUserName"]);

        user.RefreshToken = null;
        await _userService.UpdateAsync(user);

        return NoContent();
    }

    [HttpPost("revoke-all")]
    public async Task<IActionResult> RevokeAll()
    {
        var users = _userService.Users.ToList();
        foreach (var user in users)
        {
            user.RefreshToken = null;
            await _userService.UpdateAsync(user);
        }

        return NoContent();
    }

    [AllowAnonymous]
    [HttpGet("Test")]
    public async Task<IActionResult> Test()
    {
        return Ok("Success Connected to The Api With CICD 2");
    }

    private async Task<IActionResult?> HandleInvalidPasswordOrLockoutAsync(ApplicationUser user, string password)
    {
        if (await _userService.IsLockedOutAsync(user))
        {
            return this.AppUnauthorized<ApplicationUser>(_localizer["UserLockedOut"]);
        }

        if (!await _userService.CheckPasswordAsync(user, password))
        {
            return this.AppUnauthorized<ApplicationUser>(_localizer["InvalidUsernameOrPassword"]);
        }

        return null;
    }

    private async Task InvalidateUserSessionsAsync(string userId)
    {
        var sessions = _context.Set<UserSession>().AsTracking().Where(s => s.UserId == userId);
        foreach (var session in sessions)
        {
            session.IsActive = false;
        }
        await _context.SaveChangesAsync();
    }

    private async Task AddUserSessionAsync(string userId, string newSessionId)
    {
        var newSession = new UserSession
        {
            UserId = userId,
            SessionId = newSessionId,
            IsActive = true
        };

        _context.Set<UserSession>().Add(newSession);
        await _context.SaveChangesAsync();
    }

    private async Task<List<Claim>> GenerateUserClaims(ApplicationUser user, string newSessionId)
    {
        var authClaims = new List<Claim>
        {
            new(ClaimTypes.Name, user.UserName),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(ClaimTypes.NameIdentifier, user.Id),
            new(CustomClaimTypes.SessionId, newSessionId),
            new(CustomClaimTypes.FirmId, user.FirmId),
        };

        var userRoles = await _context.UserRoles
            .Where(ur => ur.UserId == user.Id)
            .ToListAsync();

        var dbRoles = await _context.Roles.Where(e => userRoles.Select(ur => ur.RoleId).Contains(e.Id)).ToListAsync();
        var all = dbRoles.Any(e => e.RoleMember != RoleMember.BorderGuardOffice.ToString() && e.RoleMember != RoleMember.SubCommittee.ToString());
        var roleMemebers = dbRoles
            .Select(e => e.RoleMember ?? e.Name)
            .Where(r => !string.IsNullOrWhiteSpace(r))
            .ToList();

        foreach (var userRole in userRoles)
        {
            authClaims.Add(new Claim(CustomClaimTypes.RolesId, userRole.RoleId));
        }

        foreach (var role in dbRoles)
        {
            if (!string.IsNullOrWhiteSpace(role.Name))
            {
                authClaims.Add(new Claim(ClaimTypes.Role, role.Name));
            }
        }

        authClaims.Add(new Claim(CustomClaimTypes.RoleMembers, JsonConvert.SerializeObject(roleMemebers)));
        authClaims.Add(new Claim(CustomClaimTypes.All, all.ToString()));

        return authClaims;
    }

    private async Task<List<RoleDto>> AssignRolesToUser(List<ApplicationRole> roles, ApplicationUser user)
    {
        var assignedRoles = new List<RoleDto>();

        if (roles != null && roles.Any())
        {
            foreach (var role in roles)
            {
                var roleSelected = await _roleService.FindByIdAsync(role.Id);
                if (roleSelected != null)
                {
                    var userRoleExists = await _context.UserRoles.AnyAsync(ur => ur.UserId == user.Id && ur.RoleId == roleSelected.Id);
                    if (userRoleExists)
                    {
                        continue;
                    }

                    _context.UserRoles.Add(new UserRole
                    {
                        UserId = user.Id,
                        RoleId = roleSelected.Id
                    });
                    await _context.SaveChangesAsync();

                    assignedRoles.Add(new RoleDto
                    {
                        Id = roleSelected.Id,
                        Name = roleSelected.Name,
                        NameAr = roleSelected.NameAr,
                        Color = roleSelected.Color
                    });
                }
            }
        }

        return assignedRoles;
    }
}
