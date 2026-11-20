namespace Project.Api;

public class RolesConfiguration : IInstaller
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<RolesController>();
    }
}
