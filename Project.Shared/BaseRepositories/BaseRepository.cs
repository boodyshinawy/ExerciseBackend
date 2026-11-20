
using System.Linq.Expressions;

namespace Project.Shared
{
    public class BaseRepository<TEntity> : IBaseRepository<TEntity>
        where TEntity : BaseEntity
    {
        protected readonly ApplicationDbContext _context;
        protected DbSet<TEntity> _dbSet;

        public BaseRepository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<TEntity>();
        }

        public virtual async Task<IEnumerable<TEntity>> Get()
        {
            try
            {
                return await _dbSet.ToListAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while retrieving entities.");
                throw new RepositoryException("An error occurred while retrieving entities.", ex);
            }
        }

        public virtual async Task<IEnumerable<TEntity>> Get(int page, int pageSize)
        {
            try
            {
                return await _dbSet.Paging(page, pageSize);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while retrieving paginated entities.");
                throw new RepositoryException("An error occurred while retrieving paginated entities.", ex);
            }
        }

        public virtual async Task<TEntity> Get(int id)
        {
            try
            {
                return await _dbSet.FirstOrDefaultAsync(e => e.Id.Equals(id));
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error occurred while retrieving entity with ID {id}.");
                throw new RepositoryException($"An error occurred while retrieving entity with ID {id}.", ex);
            }
        }

        public virtual async Task<TEntity> Add(TEntity entity)
        {
            try
            {
                //entity.Id = Guid.NewGuid().ToString();
                ArgumentNullException.ThrowIfNull(entity);
                EntityEntry<TEntity> entityEntry = await _dbSet.AddAsync(entity);
                await SaveChangesAsync();
                return entityEntry.Entity;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while adding an entity.");
                throw new RepositoryException("An error occurred while adding an entity.", ex);
            }
        }

        public virtual async Task Add(IEnumerable<TEntity> entities)
        {
            try
            {
                ArgumentNullException.ThrowIfNull(entities);
                //foreach (var entity in entities)
                //{
                //    if (string.IsNullOrEmpty(entity.Id))
                //    {
                //        entity.Id = Guid.NewGuid().ToString();
                //    }
                //}

                await _dbSet.AddRangeAsync(entities);
                await SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while adding multiple entities.");
                throw new RepositoryException("An error occurred while adding multiple entities.", ex);
            }
        }

        public virtual async Task<TEntity> Update(TEntity entity)
        {
            try
            {
                TEntity entityFromDb = await Get(entity.Id);
                ArgumentNullException.ThrowIfNull(entityFromDb);
                if (entityFromDb.IsDeleted)
                    throw new ArgumentNullException($"{nameof(entity)} was deleted.");
                // Check if entityFromDb implements IAuditableEntity to preserve state
                StateType? oldState = null;
                StateType? newState = null;

                if (entityFromDb is IAuditableEntity auditableEntity)
                {
                    oldState = auditableEntity.OldState;
                    newState = auditableEntity.NewState;
                }
                foreach (var property in typeof(TEntity).GetProperties())
                {
                    if (property.Name == nameof(IAuditableEntity.OldState) || property.Name == nameof(IAuditableEntity.NewState))
                        continue;
                    var value = property.GetValue(entity);
                    var entityProperty = entityFromDb.GetType().GetProperty(property.Name);

                    if (entityProperty != null && entityProperty.CanWrite && value != null)
                    {
                        if (IsZeroOrEmpty(value))
                        {
                            continue;
                        }
                        entityProperty.SetValue(entityFromDb, value);
                    }
                }
                // Restore OldState and NewState if entityFromDb implements IAuditableEntity
                if (entityFromDb is IAuditableEntity auditableEntityRestored)
                {
                    auditableEntityRestored.OldState = oldState ?? auditableEntityRestored.OldState;
                    auditableEntityRestored.NewState = newState ?? auditableEntityRestored.NewState;
                }

                _dbSet.Update(entityFromDb);
                await SaveChangesAsync();
                return entityFromDb;
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error occurred while updating entity with ID {entity.Id}.");
                throw new RepositoryException($"An error occurred while updating entity with ID {entity.Id}.", ex);
            }
        }

        public virtual async Task<TEntity> UpdateWithSave(TEntity entity, bool save = true)
        {
            try
            {
                TEntity entityFromDb = await Get(entity.Id);
                ArgumentNullException.ThrowIfNull(entityFromDb);
                if (entityFromDb.IsDeleted)
                    throw new ArgumentNullException($"{nameof(entity)} was deleted.");

                // Check if entityFromDb implements IAuditableEntity to preserve state
                StateType? oldState = null;
                StateType? newState = null;
                if (entityFromDb is IAuditableEntity auditableEntity)
                {
                    oldState = auditableEntity.OldState;
                    newState = auditableEntity.NewState;
                }
                foreach (var property in typeof(TEntity).GetProperties())
                {
                    if (property.Name == nameof(IAuditableEntity.OldState) || property.Name == nameof(IAuditableEntity.NewState))
                        continue;
                    var value = property.GetValue(entity);
                    var entityProperty = entityFromDb.GetType().GetProperty(property.Name);

                    if (entityProperty != null && entityProperty.CanWrite && value != null)
                    {
                        if (IsZeroOrEmpty(value))
                        {
                            continue;
                        }
                        entityProperty.SetValue(entityFromDb, value);
                    }
                }
                // Restore OldState and NewState if entityFromDb implements IAuditableEntity
                if (entityFromDb is IAuditableEntity auditableEntityRestored)
                {
                    auditableEntityRestored.OldState = oldState ?? auditableEntityRestored.OldState;
                    auditableEntityRestored.NewState = newState ?? auditableEntityRestored.NewState;
                }

                _dbSet.Update(entityFromDb);
                if (save)
                {
                    await SaveChangesAsync();
                }
                return entityFromDb;
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error occurred while updating entity with ID {entity.Id} with save flag set to {save}.");
                throw new RepositoryException($"An error occurred while updating entity with ID {entity.Id} with save flag set to {save}.", ex);
            }
        }

        public virtual async Task<TEntity> Update(int id, TEntity entity)
        {
            try
            {
                TEntity entityFromDb = await Get(id);
                ArgumentNullException.ThrowIfNull(entityFromDb);
                if (entityFromDb.IsDeleted)
                    throw new ArgumentNullException($"{nameof(entity)} was deleted.");

                entity.Id = id;
                // Check if entityFromDb implements IAuditableEntity to preserve state
                StateType? oldState = null;
                StateType? newState = null;
                if (entityFromDb is IAuditableEntity auditableEntity)
                {
                    oldState = auditableEntity.OldState;
                    newState = auditableEntity.NewState;
                }
                foreach (var property in typeof(TEntity).GetProperties())
                {
                    if (property.Name == nameof(IAuditableEntity.OldState) || property.Name == nameof(IAuditableEntity.NewState))
                        continue;
                    var value = property.GetValue(entity);
                    var entityProperty = entityFromDb.GetType().GetProperty(property.Name);

                    if (entityProperty != null && entityProperty.CanWrite && value != null)
                    {
                        if (IsZeroOrEmpty(value)|| property.Name == nameof(BaseEntity.TimeStamp))
                        {
                            continue;
                        }
                        entityProperty.SetValue(entityFromDb, value);
                    }
                }
                // Restore OldState and NewState if entityFromDb implements IAuditableEntity
                if (entityFromDb is IAuditableEntity auditableEntityRestored)
                {
                    auditableEntityRestored.OldState = oldState ?? auditableEntityRestored.OldState;
                    auditableEntityRestored.NewState = newState ?? auditableEntityRestored.NewState;
                }

                _dbSet.Update(entityFromDb);
                await SaveChangesAsync();
                return entityFromDb;
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error occurred while updating entity with ID {id}.");
                throw new RepositoryException($"An error occurred while updating entity with ID {id}.", ex);
            }
        }

        public virtual async Task<TEntity> Remove(TEntity entity, bool physicalDelete = false)
        {
            try
            {
                EntityEntry<TEntity> entityEntry;
                if (physicalDelete)
                    entityEntry = await Task.Run(() => _dbSet.Remove(entity));
                else
                {
                    entity.IsDeleted = true;
                    entityEntry = _dbSet.Update(entity);
                }
                await SaveChangesAsync();
                return entityEntry.Entity;
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error occurred while removing entity with ID {entity.Id}.");
                throw new RepositoryException($"An error occurred while removing entity with ID {entity.Id}.", ex);
            }
        }

        public virtual async Task<TEntity> Remove(int id, bool physicalDelete = false)
        {
            try
            {
                TEntity entityFromDb = await Get(id);
                ArgumentNullException.ThrowIfNull(entityFromDb);
                return await Remove(entityFromDb, physicalDelete);
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error occurred while removing entity with ID {id}.");
                throw new RepositoryException($"An error occurred while removing entity with ID {id}.", ex);
            }
        }

        public virtual async Task<IEnumerable<TEntity>> Remove(IEnumerable<int> ids)
        {
            try
            {
                if (ids == null || !ids.Any())
                    throw new ArgumentNullException($"{nameof(ids)} was not provided.");

                List<TEntity> entities = await _dbSet.Where(e => ids.Contains(e.Id)).ToListAsync();
                await RemoveEntities(entities);
                return entities;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while removing entities by IDs.");
                throw new RepositoryException("An error occurred while removing entities by IDs.", ex);
            }
        }

        public virtual async Task<IEnumerable<TEntity>> Remove(IEnumerable<TEntity> entities, bool physicalDelete = false)
        {
            try
            {
                if (entities == null || !entities.Any())
                    throw new ArgumentNullException($"{nameof(entities)} was not provided.");

                if (physicalDelete)
                {
                    await RemoveEntities(entities, physicalDelete);
                }
                else
                {
                    await RemoveEntities(entities);
                }

                return entities;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while removing multiple entities.");
                throw new RepositoryException("An error occurred while removing multiple entities.", ex);
            }
        }

        private async Task RemoveEntities(IEnumerable<TEntity> entities, bool physicalDelete = false)
        {
            try
            {
                if (!physicalDelete)
                {
                    IEnumerable<int> ids = entities.Select(e => e.Id);
                    await _dbSet.Where(e => ids.Contains(e.Id)).ExecuteUpdateAsync(e => e.SetProperty(x => x.IsDeleted, true));
                }
                else
                {
                    await Task.Run(() => _dbSet.RemoveRange(entities));
                    await SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while removing entities.");
                throw new RepositoryException("An error occurred while removing entities.", ex);
            }
        }

        public async Task<IDbContextTransaction> GetTransaction()
        {

            try
            {
                return await _context.Database.BeginTransactionAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while starting a database transaction.");
                throw new RepositoryException("An error occurred while starting a database transaction.", ex);
            }
        }

        protected async Task SaveChangesAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                Log.Error(ex, "Database update error occurred.");
                throw new RepositoryException("An error occurred while updating the database.", ex);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An unexpected error occurred while saving changes.");
                throw new RepositoryException("An unexpected error occurred while saving changes.", ex);
            }
        }
        public bool HasChildRecords<TEntity>(int id)
        {
            var entityType = typeof(TEntity);
            var keyProperty = _context.Model.FindEntityType(entityType).FindPrimaryKey().Properties.First();
            var keyValue = Convert.ChangeType(id, keyProperty.ClrType);

            var dependentTables = _context.Model.GetEntityTypes()
                .Where(e => e.GetForeignKeys().Any(fk => fk.PrincipalEntityType.ClrType == entityType))
                .Select(e => e.ClrType)
                .ToList();


            foreach (var dependentTable in dependentTables)
            {
                var method = typeof(DbContext).GetMethods()
                    .Where(m => m.Name == "Set" && m.IsGenericMethodDefinition && m.GetParameters().Length == 0)
                    .Single()
                    .MakeGenericMethod(dependentTable);

                var dbSet = method.Invoke(_context, null);
                var query = dbSet as IQueryable;
                var dependentEntityType = _context.Model.FindEntityType(dependentTable);
                var foreignKeyProperty = dependentEntityType
                    .GetForeignKeys()
                    .First(fk => fk.PrincipalEntityType.ClrType == entityType)
                    .Properties
                    .First();


                var parameter = Expression.Parameter(dependentTable, "e");
                var propertyAccess = Expression.Property(parameter, foreignKeyProperty.Name);
                var equalsExpression = Expression.Equal(propertyAccess, Expression.Constant(keyValue));

                // Adding condition for IsDeleted flag being true
                var isDeletedProperty = dependentEntityType.FindProperty("IsDeleted");
                var isDeletedAccess = Expression.Property(parameter, isDeletedProperty.Name);
                var isNotDeletedExpression = Expression.Equal(isDeletedAccess, Expression.Constant(false));

                // Combining the equality check for the foreign key and the IsDeleted flag
                var combinedExpression = Expression.AndAlso(equalsExpression, isNotDeletedExpression);

                var lambda = Expression.Lambda(combinedExpression, parameter);

                var anyMethod = typeof(Queryable).GetMethods()
                    .First(m => m.Name == "Any" && m.GetParameters().Length == 2)
                    .MakeGenericMethod(dependentTable);

                var anyCall = Expression.Call(anyMethod, query.Expression, lambda);

                var result = query.Provider.Execute<bool>(anyCall);
                if (result)
                {
                    return true;
                }
            }

            return false;
        }

        private static bool IsZeroOrEmpty(object value)
        {
            if (value == null)
            {
                return false;
            }

            if (value is DateTime dateTime)
            {
                return dateTime == default;
            }

            var numericType = value.GetType();
            if (numericType.IsValueType && numericType.IsPrimitive)
            {
                return Convert.ToDouble(value) == 0;
            }

            if (value is string stringValue)
            {
                return stringValue == "";
            }

            return false;
        }

        public void Dispose() => _context.Dispose();
    }

    // Custom exception class for repository-specific errors
    public class RepositoryException : Exception
    {
        public RepositoryException(string message) : base(message) { }
        public RepositoryException(string message, Exception innerException) : base(message, innerException) { }
    }

    // Extension method
    public static class PaginationHelper
    {
        public static async Task<IEnumerable<TEntity>> Paging<TEntity>(this IQueryable<TEntity> query, int page, int pageSize)
            where TEntity : BaseEntity
        {
            return await query
                .OrderBy(e => e.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
    }
}
