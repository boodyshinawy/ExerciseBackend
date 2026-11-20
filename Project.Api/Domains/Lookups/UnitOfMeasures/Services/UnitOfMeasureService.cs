using Microsoft.Extensions.Localization;

namespace Project.Api;

public class UnitOfMeasureService : BaseService<UnitOfMeasure, UnitOfMeasureDto>, IUnitOfMeasureService
{
    private readonly IUnitOfMeasureRepository _unitOfMeasureRepository;

    public UnitOfMeasureService(
        IValidator<UnitOfMeasure> validator,
        IUnitOfMeasureRepository repository,
        IMapper mapper,
        IStringLocalizer<SharedService> localizer)
        : base(repository, validator, mapper, localizer)
    {
        _unitOfMeasureRepository = repository;
    }
}
