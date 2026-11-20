namespace Project.Shared;

public interface IBaseRepository<TEntity> where TEntity : BaseEntity
{
    Task<TEntity> Add(TEntity entity);
    Task Add(IEnumerable<TEntity> entities);
    Task<TEntity> Get(int id);
    Task<IEnumerable<TEntity>> Get();
    Task<IEnumerable<TEntity>> Get(int page, int pageSize);
    Task<TEntity> Update(TEntity entity);
    Task<TEntity> Update(int id, TEntity entity);
    Task<TEntity> UpdateWithSave(TEntity entity, bool Save = true);
    Task<TEntity> Remove(int id, bool pysicalDelete = false);
    Task<TEntity> Remove(TEntity entity, bool pysicalDelete = false);
    Task<IEnumerable<TEntity>> Remove(IEnumerable<TEntity> entities, bool pysicalDelete = false);
    Task<IEnumerable<TEntity>> Remove(IEnumerable<int> ids);
    Task<IDbContextTransaction> GetTransaction();
    bool HasChildRecords<TEntity>(int id);
}