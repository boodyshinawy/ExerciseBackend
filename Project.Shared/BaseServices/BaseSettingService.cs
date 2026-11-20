using Microsoft.Extensions.Localization;

namespace Project.Shared;

public abstract class BaseSettingService<TEntity, TDto> : BaseService<TEntity, TDto>, IBaseSettingService<TEntity, TDto>
    where TEntity : BaseSettingEntity
    where TDto : BaseSettingDto
{
    private readonly IBaseSettingRepository<TEntity> _baseSettingRepository;
    private readonly IBaseRepository<TEntity> _baseRepository;

    public BaseSettingService(IBaseSettingRepository<TEntity> baseSettingRepository, IValidator<TEntity> validator, IMapper mapper, IStringLocalizer<SharedService> localizer, IBaseRepository<TEntity> baseRepository)
        : base(baseRepository,validator, mapper, localizer)
    {
        _baseSettingRepository = baseSettingRepository;
        _baseRepository = baseRepository;
    }

    public async Task<OperationResult<IEnumerable<TDto>>> Search(string searchText)
    {
        try
        {
            var entities = await _baseSettingRepository.Search(searchText);
            var dtos = _mapper.Map<IEnumerable<TDto>>(entities);
            return OperationResult<IEnumerable<TDto>>.Succeeded(_localizer["EntitiesRetrievedSuccessfully"], dtos);

        }
        catch (Exception ex)
        {
            return OperationResult<IEnumerable<TDto>>.InternalServerError(_localizer["UnexpectedError", ex.Message]);
        }
    }
}
