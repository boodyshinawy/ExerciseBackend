namespace Project.Api;

public class SaleLotsConfiguration : IInstaller
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ISaleLotService, SaleLotService>();
        services.AddScoped<ISaleLotRepository, SaleLotRepository>();
    }
}
