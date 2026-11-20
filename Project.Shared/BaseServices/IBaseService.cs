namespace Project.Shared
{
    public interface IBaseService<TEntity, TDto>
        where TEntity : BaseEntity
        where TDto : BaseDto
    {
        Task<OperationResult<TDto>> Create(TDto entity);
        Task<OperationResult<IEnumerable<TEntity>>> Create(IEnumerable<TDto> dtoEntities);

        Task<OperationResult<TDto>> Read(int id);
        Task<OperationResult<IEnumerable<TDto>>> Read();
        Task<OperationResult<IEnumerable<TDto>>> Read(int page, int pageSize);

        Task<OperationResult<TEntity>> Update(TEntity entity);
        Task<OperationResult<TDto>> Update(int id, TDto entityDto);
        Task<OperationResult<TDto>> UpdateWithDto(TDto dto);

        Task<OperationResult<TEntity>> Delete(int id, bool pysicalDelete = false);
        Task<OperationResult<TEntity>> Delete(TEntity entity, bool pysicalDelete = false);
        Task<OperationResult<IEnumerable<TEntity>>> Delete(IEnumerable<TEntity> entities, bool pysicalDelete = false);
        Task<OperationResult<IEnumerable<TEntity>>> Delete(IEnumerable<int> ids);
    }
}
