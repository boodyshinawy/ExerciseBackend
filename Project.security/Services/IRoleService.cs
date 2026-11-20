namespace Project.Security;

public interface IRoleService
{
    IQueryable<ApplicationRole> Roles { get; }

    Task<ApplicationRole?> FindByIdAsync(string id);

    Task<ApplicationRole?> FindByNameAsync(string name);

    Task<AuthResult> CreateAsync(ApplicationRole role);

    Task<AuthResult> UpdateAsync(ApplicationRole role);
}
