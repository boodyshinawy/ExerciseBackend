using System.Data;
using System.Data.Common;

namespace Project.Shared;

public static class MigrationManager
{
    public static async Task<WebApplication> MigrateAndSeedAsync(
        this WebApplication webApp,
        Func<ApplicationDbContext, IUserService, Task> seedAsync)
    {


        using IServiceScope scope = webApp.Services.CreateScope();
        ApplicationDbContext appContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        IUserService userService = scope.ServiceProvider.GetRequiredService<IUserService>();

        await appContext.Database.MigrateAsync();
        await seedAsync(appContext, userService);

        return webApp;
    }

}
