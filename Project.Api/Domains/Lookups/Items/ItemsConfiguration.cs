namespace Project.Api;

public class ItemsConfiguration : IInstaller
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IItemService, ItemService>();
        services.AddScoped<IItemRepository, ItemRepository>();
    }
}
