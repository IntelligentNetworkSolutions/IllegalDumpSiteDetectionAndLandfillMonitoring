using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Entities.Intefaces;
using SD;

namespace DAL.Interfaces.Repositories
{
    public interface IBaseResultRepository<TEntity, TId> where TEntity : class, IBaseEntity<TId> where TId : IEquatable<TId>
    {
        IQueryable<TEntity> Table { get; }

        Task<ResultDTO<int>> SaveChangesAsync(CancellationToken cancellationToken = default);

        Task<ResultDTO<TEntity?>> GetById(TId id, bool track = false, string? includeProperties = null);
        Task<ResultDTO<TEntity?>> GetFirstOrDefault(Expression<Func<TEntity, bool>>? filter = null, bool track = false, string? includeProperties = null);
        Task<ResultDTO<IEnumerable<TEntity>>> GetAll(Expression<Func<TEntity, bool>>? filter = null,
                                            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
                                             bool track = false, string? includeProperties = null, int? limit = null);

        Task<ResultDTO> Create(TEntity entity, bool saveChanges = true, CancellationToken cancellationToken = default);
        Task<ResultDTO<TEntity>> CreateAndReturnEntity(TEntity entity, bool saveChanges = true, CancellationToken cancellationToken = default);
        Task<ResultDTO> CreateRange(IEnumerable<TEntity> entities, bool saveChanges = true, CancellationToken cancellationToken = default);

        Task<ResultDTO> Update(TEntity entity, bool saveChanges = true, CancellationToken cancellationToken = default);
        Task<ResultDTO> UpdateRange(IEnumerable<TEntity> entities, bool saveChanges = true, CancellationToken cancellationToken = default);

        Task<ResultDTO> Delete(TEntity entity, bool saveChanges = true, CancellationToken cancellationToken = default);
        Task<ResultDTO> DeleteRange(IEnumerable<TEntity> entities, bool saveChanges = true, CancellationToken cancellationToken = default);
    }
}
