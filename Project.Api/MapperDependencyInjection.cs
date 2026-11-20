namespace Project.Api;

public static class MapperDependencyInjection
{
    public static void AddMapper(this IServiceCollection services)
    {
        TypeAdapterConfig? config = TypeAdapterConfig.GlobalSettings.Default.PreserveReference(true).Config;
        //TypeAdapterConfig.GlobalSettings.ForDestinationType<IValidator>()
        //            .AfterMapping(dest => dest.Validate());
        var m = Assembly.GetExecutingAssembly();
        config.Scan(Assembly.GetExecutingAssembly());
        services.AddSingleton(config);

        services.AddScoped<IMapper, ServiceMapper>();
        //return services;
    }
}
public class EntityRegister : IRegister
{
    public void Register(TypeAdapterConfig config)
    {

        //config.NewConfig<Tenant, TenantDto>()
        //    .Map(dest => dest.Name, src => src.Name)
        //    .Map(dest => dest.Childern, src => src.Children);
        //config.NewConfig<Tenant, TenantReadingDto>()
        //   .Map(dest => dest.Key, src => src.Id)
        //   .Map(dest => dest.Data, src => new TenantDataDto
        //   {
        //       Name = src.Name,
        //       ParentId = src.ParentId,
        //       TypeId = src.TypeId,
        //       TypeName = src.Type.Name,
        //       Color = src.Type.Color
        //   })
        //   .Map(dest => dest.Children, src => src.Children.Adapt<List<TenantReadingDto>>()); // Ensure children are mapped as well
        //config.NewConfig<Tenant, TenantReadingSelectDto>()
        //    .Map(dest => dest.Key, src => src.Id)
        //    .Map(dest => dest.Label, src => src.Name)
        //    .Map(dest => dest.TypeName, src => src.Type.Name)
        //    .Map(dest => dest.Color, src => src.Type.Color)
        //    .Map(dest => dest.Children, src => src.Children.Adapt<List<TenantReadingSelectDto>>()); // Ensure children are mapped as well

        //config.NewConfig<Type, TypeDto>()
        //.Map(dest => dest.Name, src => src.Name);

        config
           .NewConfig<Page, PageRoleNameDto>()
           .Map(dest => dest.PageName, src => src.Label)
           .Map(dest => dest.RoleNames, src => src.PageRoles.Select(pr =>new RoleColor { RoleName = pr.Role.Name,Color=pr.Role.Color }).Distinct().ToList());
        config
              .NewConfig<PageRole, PagesOnRole>()
              .Map(dest => dest.PageName, src => src.Page.Label);



    }
}

