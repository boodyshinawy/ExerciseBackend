namespace Project.Api;

public class RanksConfiguration : IInstaller
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IRankService, RankService>();
        services.AddScoped<IRankRepository, RankRepository>();
    }
}
