using MehmetHairDesigner.Server.Application.Interfaces.Repositories;
using MehmetHairDesigner.Server.Domain.Abstraction;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MehmetHairDesigner.Server.Infrastructure.Repositories
{
    public class ReadRepository<TEntity> : IReadRepository<TEntity> where TEntity : class, IBaseEntity, new()
    {
        private readonly DbContext _context;
        private DbSet<TEntity> Entity;

        public ReadRepository(DbContext context)
        {
            _context = context;
            Entity = _context.Set<TEntity>();
        }

        public IQueryable<TEntity> GetAll()
        {
            return Entity.AsNoTracking().AsQueryable();
        }

        public IQueryable<TEntity> GetAllWithTracking()
        {
            return Entity.AsQueryable();
        }
        public TEntity? GetByExpression(Expression<Func<TEntity, bool>> expression)
        {
            return Entity.Where(expression).AsNoTracking().FirstOrDefault();
        }

        public async Task<TEntity?> GetByExpressionAsync(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await Entity.Where(expression).AsNoTracking().FirstOrDefaultAsync(cancellationToken);
        }
        public TEntity? GetByExpressionWithTracking(Expression<Func<TEntity, bool>> expression)
        {
            return Entity.Where(expression).FirstOrDefault();
        }
        public async Task<TEntity?> GetByExpressionWithTrackingAsync(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await Entity.Where(expression).FirstOrDefaultAsync(cancellationToken);
        }
        public bool Any(Expression<Func<TEntity, bool>> expression)
        {
            return Entity.Any(expression);
        }
        public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await Entity.AnyAsync(expression, cancellationToken);
        }
        public async Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default(CancellationToken), bool isTrackingActive = true)
        {
            return (!isTrackingActive) ? (await Entity.Where(expression).AsNoTracking().FirstOrDefaultAsync(cancellationToken)) : (await Entity.Where(expression).FirstOrDefaultAsync(cancellationToken));
        }
        public TEntity? GetFirst()
        {
            return Entity.AsNoTracking().FirstOrDefault();
        }

        public async Task<TEntity?> GetFirstAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return await Entity.AsNoTracking().FirstOrDefaultAsync(cancellationToken);
        }

        public IQueryable<TEntity> Where(Expression<Func<TEntity, bool>> expression)
        {
            return Entity.AsNoTracking().Where(expression).AsQueryable();
        }

        public IQueryable<TEntity> WhereWithTracking(Expression<Func<TEntity, bool>> expression)
        {
            return Entity.Where(expression).AsQueryable();
        }
        public TEntity? FirstOrDefault(Expression<Func<TEntity, bool>> expression, bool isTrackingActive = true)
        {
            if (isTrackingActive)
            {
                return Entity.FirstOrDefault(expression);
            }

            return Entity.AsNoTracking().FirstOrDefault(expression);
        }
        //public IQueryable<KeyValuePair<bool, int>> CountBy(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default(CancellationToken))
        //{
        //	return Entity.CountBy(expression);
        //}
        public TEntity First(Expression<Func<TEntity, bool>> expression, bool isTrackingActive = true)
        {
            if (isTrackingActive)
            {
                return Entity.First(expression);
            }

            return Entity.AsNoTracking().First(expression);
        }

        public async Task<TEntity> FirstAsync(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default(CancellationToken), bool isTrackingActive = true)
        {
            if (isTrackingActive)
            {
                return await Entity.FirstAsync(expression, cancellationToken);
            }

            return await Entity.AsNoTracking().FirstAsync(expression, cancellationToken);
        }
    }
}
