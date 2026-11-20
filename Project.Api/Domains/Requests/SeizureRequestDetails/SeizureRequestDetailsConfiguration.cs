namespace Project.Api;

public class SeizureRequestDetailsConfiguration : IInstaller
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ISeizureRequestDetailService, SeizureRequestDetailService>();
        services.AddScoped<ISeizureRequestDetailRepository, SeizureRequestDetailRepository>();
    }
}
