namespace Project.Security;

public interface IUserService
{
    IQueryable<ApplicationUser> Users { get; }

    Task<ApplicationUser?> FindByNameAsync(string userName);

    Task<ApplicationUser?> FindByIdAsync(string id);

    Task<AuthResult> CreateAsync(ApplicationUser user, string password);

    Task<AuthResult> UpdateAsync(ApplicationUser user);

    Task<AuthResult> DeleteAsync(ApplicationUser user);

    Task<bool> CheckPasswordAsync(ApplicationUser user, string password);

    Task<AuthResult> ChangePasswordAsync(ApplicationUser user, string newPassword);

    Task<bool> IsLockedOutAsync(ApplicationUser user);

    Task<AuthResult> SetLockoutEndDateAsync(ApplicationUser user, DateTimeOffset? lockoutEnd);

    Task<List<string>> GetRoleNamesAsync(ApplicationUser user);
}
