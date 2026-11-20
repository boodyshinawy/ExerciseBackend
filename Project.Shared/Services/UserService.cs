namespace Project.Shared;

public class UserService : IUserService
{
    private readonly ApplicationDbContext _context;
    private readonly IPasswordHasher _passwordHasher;

    public UserService(ApplicationDbContext context, IPasswordHasher passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    public IQueryable<ApplicationUser> Users => _context.Users.Where(u => !u.IsDeleted);

    public Task<ApplicationUser?> FindByNameAsync(string userName) =>
        _context.Users.Include(u => u.Firm).FirstOrDefaultAsync(u => u.UserName == userName && !u.IsDeleted);

    public Task<ApplicationUser?> FindByIdAsync(string id) =>
        _context.Users.Include(u => u.Firm).FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted);

    public async Task<AuthResult> CreateAsync(ApplicationUser user, string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            return AuthResult.Failed;

        user.Id = string.IsNullOrEmpty(user.Id) ? Guid.NewGuid().ToString() : user.Id;
        user.SecurityStamp = Guid.NewGuid().ToString();
        user.PasswordHash = _passwordHasher.HashPassword(password);

        _context.Users.Add(user);
        return await _context.SaveChangesAsync() > 0 ? AuthResult.Success : AuthResult.Failed;
    }

    public async Task<AuthResult> UpdateAsync(ApplicationUser user)
    {
        _context.Users.Update(user);
        return await _context.SaveChangesAsync() > 0 ? AuthResult.Success : AuthResult.Failed;
    }

    public async Task<AuthResult> DeleteAsync(ApplicationUser user)
    {
        _context.Users.Remove(user);
        return await _context.SaveChangesAsync() > 0 ? AuthResult.Success : AuthResult.Failed;
    }

    public Task<bool> CheckPasswordAsync(ApplicationUser user, string password) =>
        Task.FromResult(_passwordHasher.VerifyPassword(password, user.PasswordHash));

    public async Task<AuthResult> ChangePasswordAsync(ApplicationUser user, string newPassword)
    {
        user.PasswordHash = _passwordHasher.HashPassword(newPassword);
        user.SecurityStamp = Guid.NewGuid().ToString();
        return await UpdateAsync(user);
    }

    public Task<bool> IsLockedOutAsync(ApplicationUser user) =>
        Task.FromResult(user.LockoutEnd.HasValue && user.LockoutEnd > DateTimeOffset.UtcNow);

    public async Task<AuthResult> SetLockoutEndDateAsync(ApplicationUser user, DateTimeOffset? lockoutEnd)
    {
        user.LockoutEnd = lockoutEnd;
        return await UpdateAsync(user);
    }

    public async Task<List<string>> GetRoleNamesAsync(ApplicationUser user) =>
        await _context.UserRoles
            .Where(ur => ur.UserId == user.Id)
            .Join(_context.Roles, ur => ur.RoleId, r => r.Id, (_, role) => role.Name)
            .ToListAsync();
}
