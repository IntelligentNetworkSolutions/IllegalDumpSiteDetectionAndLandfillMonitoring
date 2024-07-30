using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DAL.ApplicationStorage;
using DAL.Interfaces.Repositories;
using Entities.Intefaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using SD;

namespace DAL.Repositories
{
    public abstract class BaseResultRepository<TEntity, TId> : IBaseResultRepository<TEntity, TId> where TEntity : class, IBaseEntity<TId> where TId : IEquatable<TId>
    {
        protected internal readonly ApplicationDbContext _dbContext;

        public BaseResultRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(_dbContext));
        }

        public virtual IQueryable<TEntity> Table => _dbContext.Set<TEntity>().AsQueryable();

        public virtual async Task<ResultDTO<int>> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                int result = await _dbContext.SaveChangesAsync(cancellationToken);
                if (result <= 0)
                    return ResultDTO<int>.Fail("No Rows were updated");

                return ResultDTO<int>.Ok(result);
            }
            catch (Exception ex)
            {
                return ResultDTO<int>.ExceptionFail(ex.Message, ex);
            }
        }

        #region Create
        public virtual async Task<ResultDTO> Create(TEntity entity, bool saveChanges = true, CancellationToken cancellationToken = default)
        {
            await _dbContext.Set<TEntity>().AddAsync(entity);
            if (saveChanges)
            {
                ResultDTO<int> resSave = await SaveChangesAsync(cancellationToken);
                if (!resSave.IsSuccess)
                    return ResultDTO.Fail(resSave.ErrMsg);

                return ResultDTO.Ok();
            }

            return ResultDTO.Ok();
        }

        public virtual async Task<ResultDTO<TEntity>> CreateAndReturnEntity(TEntity entity, bool saveChanges = true, CancellationToken cancellationToken = default)
        {
            await _dbContext.Set<TEntity>().AddAsync(entity);
            if (saveChanges)
            {
                ResultDTO<int> resSave = await SaveChangesAsync(cancellationToken);
                if (!resSave.IsSuccess)
                    return ResultDTO<TEntity>.Fail(resSave.ErrMsg);

                return ResultDTO<TEntity>.Ok(entity);
            }

            return ResultDTO<TEntity>.Ok(entity);
        }

        public virtual async Task<ResultDTO> CreateRange(IEnumerable<TEntity> entities, bool saveChanges = true, CancellationToken cancellationToken = default)
        {
            await _dbContext.Set<TEntity>().AddRangeAsync(entities);
            if (saveChanges)
            {
                ResultDTO<int> resSave = await SaveChangesAsync(cancellationToken);
                if (!resSave.IsSuccess)
                    return ResultDTO.Fail(resSave.ErrMsg);

                return ResultDTO.Ok();
            }

            return ResultDTO.Ok();
        }
        #endregion

        #region Update
        public virtual async Task<ResultDTO> Update(TEntity entity, bool saveChanges = true, CancellationToken cancellationToken = default)
        {
            _dbContext.Set<TEntity>().Update(entity);
            if (saveChanges)
            {
                ResultDTO<int> resSave = await SaveChangesAsync(cancellationToken);
                if (!resSave.IsSuccess)
                    return ResultDTO.Fail(resSave.ErrMsg);

                return ResultDTO.Ok();
            }

            return ResultDTO.Ok();
        }

        public virtual async Task<ResultDTO> UpdateRange(IEnumerable<TEntity> entities, bool saveChanges = true, CancellationToken cancellationToken = default)
        {
            _dbContext.Set<TEntity>().UpdateRange(entities);
            if (saveChanges)
            {
                ResultDTO<int> resSave = await SaveChangesAsync(cancellationToken);
                if (!resSave.IsSuccess)
                    return ResultDTO.Fail(resSave.ErrMsg);

                return ResultDTO.Ok();
            }

            return ResultDTO.Ok();
        }
        #endregion

        #region Delete
        public virtual async Task<ResultDTO> Delete(TEntity entity, bool saveChanges = true, CancellationToken cancellationToken = default)
        {
            switch (entity)
            {
                case null:
                    throw new ArgumentNullException(nameof(entity));

                // TODO: Add
                //case ISoftDeletedEntity softDeletedEntity:
                //    softDeletedEntity.IsActive = false;
                //    await UpdateEntityAsync(entity, saveChanges);
                //    break;

                default:
                    _dbContext.Set<TEntity>().Remove(entity);
                    break;
            }

            if (saveChanges)
            {
                ResultDTO<int> resSave = await SaveChangesAsync(cancellationToken);
                if (!resSave.IsSuccess)
                    return ResultDTO.Fail(resSave.ErrMsg);

                return ResultDTO.Ok();
            }

            return ResultDTO.Ok();
        }

        public virtual async Task<ResultDTO> DeleteRange(IEnumerable<TEntity> entities, bool saveChanges = true, CancellationToken cancellationToken = default)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            // TODO: Add
            //if (entities.OfType<ISoftDeletedEntity>().Any())
            //{
            //    foreach (var entity in entities)
            //    {
            //        if (entity is ISoftDeletedEntity softDeletedEntity)
            //        {
            //            softDeletedEntity.IsActive = false;
            //            await UpdateEntityAsync(entity, saveChanges);
            //        }
            //    }
            //}
            _dbContext.Set<TEntity>().RemoveRange(entities);

            if (saveChanges)
            {
                ResultDTO<int> resSave = await SaveChangesAsync(cancellationToken);
                if (!resSave.IsSuccess)
                    return ResultDTO.Fail(resSave.ErrMsg);

                return ResultDTO.Ok();
            }

            return ResultDTO.Ok();
        }
        #endregion

        #region Read
        public virtual async Task<ResultDTO<TEntity?>> GetById(TId id, bool track = false, string? includeProperties = null)
        {
            try
            {
                IQueryable<TEntity> query = _dbContext.Set<TEntity>();

                if (!track)
                    query = query.AsNoTracking();

                if (!string.IsNullOrWhiteSpace(includeProperties))
                {
                    string[] splitIncludeProps = includeProperties.Split(new string[] { ",", ", " },
                                                                            StringSplitOptions.TrimEntries
                                                                            | StringSplitOptions.RemoveEmptyEntries);
                    foreach (string includeProp in splitIncludeProps)
                        query = query.Include(includeProp);
                }

                TEntity? entity = await query.FirstOrDefaultAsync(e => e.Id.Equals(id));
                return ResultDTO<TEntity?>.Ok(entity);
            }
            catch (Exception ex)
            {
                return ResultDTO<TEntity?>.ExceptionFail(ex.Message, ex);
            }
        }

        public virtual async Task<ResultDTO<TEntity?>> GetByIdInclude(TId id, bool track = false,
            Expression<Func<TEntity, object>>[]? includeProperties = null)
        {
            try
            {
                IQueryable<TEntity> query = _dbContext.Set<TEntity>();

                if (!track)
                    query = query.AsNoTracking();

                if (includeProperties != null && includeProperties.Length > 0)
                    foreach (Expression<Func<TEntity, object>> includeProperty in includeProperties)
                        query = query.Include(includeProperty);

                TEntity? entity = await query.FirstOrDefaultAsync(e => e.Id.Equals(id));
                return ResultDTO<TEntity?>.Ok(entity);
            }
            catch (Exception ex)
            {
                return ResultDTO<TEntity?>.ExceptionFail(ex.Message, ex);
            }
        }

        public virtual async Task<ResultDTO<TEntity?>> GetByIdIncludeThenAll(TId id, bool track = false,
            (Expression<Func<TEntity, object>> Include, Expression<Func<object, object>>[]? ThenInclude)[]? includeProperties = null)
        {
            try
            {
                IQueryable<TEntity> query = _dbContext.Set<TEntity>();

                if (!track)
                    query = query.AsNoTracking();

                if (includeProperties != null && includeProperties.Length > 0)
                {
                    foreach ((Expression<Func<TEntity, object>> Include, Expression<Func<object, object>>[]? ThenInclude) includeProperty
                                in includeProperties)
                    {
                        ValidateIncludeThenIncludeChain(includeProperty.Include, includeProperty.ThenInclude);

                        query = includeProperties.Aggregate(query, (current, includeProp) =>
                        {
                            if (includeProp.ThenInclude is null)
                                return current.Include(includeProp.Include);

                            return includeProp.ThenInclude.Aggregate(current.Include(includeProp.Include),
                                (currentQuery, thenIncludeProp) => currentQuery.ThenInclude(thenIncludeProp));
                        });
                    }
                }

                TEntity? entity = await query.FirstOrDefaultAsync(e => e.Id.Equals(id));
                return ResultDTO<TEntity?>.Ok(entity);
            }
            catch (Exception ex)
            {
                return ResultDTO<TEntity?>.ExceptionFail(ex.Message, ex);
            }
        }

        private void ValidateIncludeThenIncludeChain(Expression<Func<TEntity, object>> include, Expression<Func<object, object>>[]? thenIncludes)
        {
            if (thenIncludes == null || thenIncludes.Length == 0)
                return;

            Type includeType = GetPropertyType(include);
            Type previousType = includeType;

            foreach (Expression<Func<object, object>> thenInclude in thenIncludes)
            {
                Type thenIncludeType = GetPropertyType(thenInclude);

                if (!IsValidThenInclude(previousType, thenIncludeType))
                    throw new InvalidOperationException($"The ThenInclude property '{thenIncludeType.Name}' is not a valid property of '{previousType.Name}'");

                previousType = thenIncludeType;
            }
        }

        private Type GetPropertyType(LambdaExpression expression)
        {
            MemberExpression? memberExpression = RemoveConvert(expression.Body) as MemberExpression;
            if (memberExpression == null)
                throw new InvalidOperationException("Invalid expression");

            PropertyInfo? propertyInfo = memberExpression.Member as PropertyInfo;
            if (propertyInfo == null)
                throw new InvalidOperationException("Expression does not refer to a property");

            return propertyInfo.PropertyType;
        }

        private Expression RemoveConvert(Expression expression)
        {
            while (expression.NodeType == ExpressionType.Convert || expression.NodeType == ExpressionType.ConvertChecked)
            {
                expression = ((UnaryExpression)expression).Operand;
            }
            return expression;
        }

        private bool IsValidThenInclude(Type includeType, Type thenIncludeType)
        {
            // Check if includeType is a collection
            if (includeType.IsGenericType)
            {
                Type elementType = includeType.GetGenericArguments().First();
                return elementType.GetProperties().Any(p => p.PropertyType == thenIncludeType);
            }

            return includeType.GetProperties().Any(p => p.PropertyType == thenIncludeType);
        }


        public virtual async Task<ResultDTO<TEntity?>> GetFirstOrDefault(Expression<Func<TEntity, bool>>? filter = null, bool track = false, string? includeProperties = null)
        {
            try
            {
                IQueryable<TEntity> query = _dbContext.Set<TEntity>();

                if (!track)
                    query = query.AsNoTracking();

                if (filter is not null)
                    query = query.Where(filter);

                if (!string.IsNullOrWhiteSpace(includeProperties))
                {
                    string[] splitIncludeProps = includeProperties.Split(new string[] { ",", ", " },
                                                                            StringSplitOptions.TrimEntries
                                                                            | StringSplitOptions.RemoveEmptyEntries);
                    foreach (string includeProp in splitIncludeProps)
                        query = query.Include(includeProp);
                }

                TEntity? entity = await query.FirstOrDefaultAsync();
                return ResultDTO<TEntity?>.Ok(entity);
            }
            catch (Exception ex)
            {
                return ResultDTO<TEntity?>.ExceptionFail(ex.Message, ex);
            }
        }

        public virtual async Task<ResultDTO<IEnumerable<TEntity>>> GetAll(Expression<Func<TEntity, bool>>? filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null, bool track = false, string? includeProperties = null, int? limit = null)
        {
            try
            {
                IQueryable<TEntity> query = _dbContext.Set<TEntity>();

                if (!track)
                    query = query.AsNoTracking();

                if (filter is not null)
                    query = query.Where(filter);

                if (!string.IsNullOrWhiteSpace(includeProperties))
                {
                    string[] splitIncludeProps = includeProperties.Split(new string[] { ",", ", " },
                                                                            StringSplitOptions.TrimEntries
                                                                            | StringSplitOptions.RemoveEmptyEntries);
                    foreach (string includeProp in splitIncludeProps)
                        query = query.Include(includeProp);
                }

                if (orderBy is not null)
                    query = orderBy(query);

                if (limit is not null)
                    query = query.Take(limit.Value);

                IEnumerable<TEntity> entities = await query.ToListAsync();

                return ResultDTO<IEnumerable<TEntity>>.Ok(entities);
            }
            catch (Exception ex)
            {
                return ResultDTO<IEnumerable<TEntity>>.ExceptionFail(ex.Message, ex);
            }
        }
        #endregion
    }
}