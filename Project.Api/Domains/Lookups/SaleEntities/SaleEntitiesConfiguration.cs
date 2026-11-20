namespace Project.Api;

public class SaleEntitiesConfiguration : IInstaller
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ISaleEntityService, SaleEntityService>();
        services.AddScoped<ISaleEntityRepository, SaleEntityRepository>();
    }
}
