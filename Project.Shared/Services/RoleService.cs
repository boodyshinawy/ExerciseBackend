namespace Project.Shared;

public class RoleService : IRoleService
{
    private readonly ApplicationDbContext _context;

    public RoleService(ApplicationDbContext context)
    {
        _context = context;
    }

    public IQueryable<ApplicationRole> Roles => _context.Roles.Where(r => !r.IsDeleted);

    public Task<ApplicationRole?> FindByIdAsync(string id) =>
        _context.Roles.FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted);

    public Task<ApplicationRole?> FindByNameAsync(string name) =>
        _context.Roles.FirstOrDefaultAsync(r => r.Name == name && !r.IsDeleted);

    public async Task<AuthResult> CreateAsync(ApplicationRole role)
    {
        role.Id = string.IsNullOrEmpty(role.Id) ? Guid.NewGuid().ToString() : role.Id;
        _context.Roles.Add(role);
        return await _context.SaveChangesAsync() > 0 ? AuthResult.Success : AuthResult.Failed;
    }

    public async Task<AuthResult> UpdateAsync(ApplicationRole role)
    {
        _context.Roles.Update(role);
        return await _context.SaveChangesAsync() > 0 ? AuthResult.Success : AuthResult.Failed;
    }
}
