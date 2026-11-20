namespace Project.Api;

public class StatusesConfiguration : IInstaller
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IStatusService, StatusService>();
        services.AddScoped<IStatusRepository, StatusRepository>();
    }
}
