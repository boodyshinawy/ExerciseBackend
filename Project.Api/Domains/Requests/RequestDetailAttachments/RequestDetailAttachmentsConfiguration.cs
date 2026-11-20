namespace Project.Api;

public class RequestDetailAttachmentsConfiguration : IInstaller
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IRequestDetailAttachmentService, RequestDetailAttachmentService>();
        services.AddScoped<IRequestDetailAttachmentRepository, RequestDetailAttachmentRepository>();
    }
}
