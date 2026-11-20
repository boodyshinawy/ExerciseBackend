namespace Project.Api;

public class UnitOfMeasuresConfiguration : IInstaller
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IUnitOfMeasureService, UnitOfMeasureService>();
        services.AddScoped<IUnitOfMeasureRepository, UnitOfMeasureRepository>();
    }
}
