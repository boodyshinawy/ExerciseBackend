namespace Project.Api;

public class PageRolesConfiguration : IInstaller
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IPageRoleService, PageRoleService>();
        services.AddScoped<IPageRoleRepository, PageRoleRepository>();
    }
}
