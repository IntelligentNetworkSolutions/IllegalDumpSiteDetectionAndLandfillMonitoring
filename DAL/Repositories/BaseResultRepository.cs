﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DAL.ApplicationStorage;
using DAL.Interfaces.Repositories;
using Entities.Intefaces;
using Microsoft.EntityFrameworkCore;
using SD;

namespace DAL.Repositories
{
    public abstract class BaseResultRepository<TEntity, TId> : IBaseResultRepository<TEntity, TId> where TEntity : class, IBaseEntity<TId> where TId : IEquatable<TId>
    {
        private readonly ApplicationDbContext _dbContext;

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
            catch(Exception ex)
            {
                return ResultDTO<TEntity?>.ExceptionFail(ex.Message, ex);
            }
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
            catch(Exception ex)
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
