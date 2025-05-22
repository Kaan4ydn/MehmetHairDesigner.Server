using MehmetHairDesigner.Server.Domain.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MehmetHairDesigner.Server.Application.Interfaces.Repositories
{
    public interface IWriteRepository<TEntity> where TEntity : class, IBaseEntity, new()
    {
        void Add(TEntity entity);
        Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);
        Task AddRangeAsync(ICollection<TEntity> entities, CancellationToken cancellationToken = default);
        void AddRange(ICollection<TEntity> entities);
        void Delete(TEntity entity);
        Task DeleteByExpressionAsync(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default);
        Task DeleteByIdAsync(string id);
        void DeleteRange(ICollection<TEntity> entities);
        void Update(TEntity entity);
        void UpdateRange(ICollection<TEntity> entities);
    }
}
