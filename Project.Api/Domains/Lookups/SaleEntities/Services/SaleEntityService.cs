namespace Project.Api;

public class SaleEntityService : BaseService<SaleEntity, SaleEntityDto>, ISaleEntityService
{
    public SaleEntityService(
        IValidator<SaleEntity> validator,
        ISaleEntityRepository repository,
        IMapper mapper,
        IStringLocalizer<SharedService> localizer)
        : base(repository, validator, mapper, localizer)
    {
    }
}
