namespace Project.Api;

public class ItemCategoriesConfiguration : IInstaller
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IItemCategoryService, ItemCategoryService>();
        services.AddScoped<IItemCategoryRepository, ItemCategoryRepository>();
    }
}
