namespace Project.Shared;

public interface IBaseSettingService<TEntity, TDto> : IBaseService<TEntity, TDto>
    where TEntity : BaseSettingEntity
    where TDto : BaseSettingDto
{
    Task<OperationResult<IEnumerable<TDto>>> Search(string searchText);
}
