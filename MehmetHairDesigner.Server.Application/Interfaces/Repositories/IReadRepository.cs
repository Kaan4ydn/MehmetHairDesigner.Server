using MehmetHairDesigner.Server.Domain.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MehmetHairDesigner.Server.Application.Interfaces.Repositories
{
    public interface IReadRepository<TEntity> where TEntity : class, IBaseEntity, new()
    {
        IQueryable<TEntity> GetAll();
        IQueryable<TEntity> GetAllWithTracking();
        TEntity? GetByExpression(Expression<Func<TEntity, bool>> expression);
        Task<TEntity?> GetByExpressionAsync(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default);
        TEntity? GetByExpressionWithTracking(Expression<Func<TEntity, bool>> expression);
        Task<TEntity?> GetByExpressionWithTrackingAsync(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default);
        bool Any(Expression<Func<TEntity, bool>> expression);
        Task<bool> AnyAsync(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default);
        Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default, bool isTrackingActive = true);
        TEntity? GetFirst();
        Task<TEntity?> GetFirstAsync(CancellationToken cancellationToken = default);
        IQueryable<TEntity> Where(Expression<Func<TEntity, bool>> expression);
        IQueryable<TEntity> WhereWithTracking(Expression<Func<TEntity, bool>> expression);
        TEntity? FirstOrDefault(Expression<Func<TEntity, bool>> expression, bool isTrackingActive = true);
        TEntity First(Expression<Func<TEntity, bool>> expression, bool isTrackingActive = true);
        Task<TEntity> FirstAsync(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default, bool isTrackingActive = true);
    }
}
