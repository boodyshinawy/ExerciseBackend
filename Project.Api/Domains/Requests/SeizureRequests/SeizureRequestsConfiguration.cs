namespace Project.Api;

public class SeizureRequestsConfiguration : IInstaller
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ISeizureRequestService, SeizureRequestService>();
        services.AddScoped<ISeizureRequestRepository, SeizureRequestRepository>();
    }
}
