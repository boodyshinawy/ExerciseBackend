namespace Project.Api;

public class PagesConfiguration : IInstaller
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IPageService, PageService>();
        services.AddScoped<IPageRepository, PageRepository>();
    }
}
