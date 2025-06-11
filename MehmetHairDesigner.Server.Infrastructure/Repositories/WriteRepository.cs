using MehmetHairDesigner.Server.Application.Interfaces.Repositories;
using MehmetHairDesigner.Server.Domain.Abstraction;
using MehmetHairDesigner.Server.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MehmetHairDesigner.Server.Infrastructure.Repositories
{
    public class WriteRepository<TEntity> : IWriteRepository<TEntity> where TEntity : class, IBaseEntity, new()
    {
        private readonly AppDbContext _context;
        private DbSet<TEntity> Entity;

        public WriteRepository(AppDbContext context)
        {
            _context = context;
            Entity = _context.Set<TEntity>();
        }



        public void Add(TEntity entity)
        {
            Entity.Add(entity);
        }

        public async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default(CancellationToken))
        {
            await Entity.AddAsync(entity, cancellationToken);
        }

        public async Task AddRangeAsync(ICollection<TEntity> entities, CancellationToken cancellationToken = default(CancellationToken))
        {
            await Entity.AddRangeAsync(entities, cancellationToken);
        }
        public void AddRange(ICollection<TEntity> entities)
        {
            Entity.AddRange(entities);
        }
        public void Delete(TEntity entity)
        {
            Entity.Remove(entity);
        }

        public async Task DeleteByExpressionAsync(Expression<Func<TEntity, bool>> expression, CancellationToken cancellationToken = default(CancellationToken))
        {
            TEntity? entity = await Entity.Where(expression).AsNoTracking().FirstOrDefaultAsync(cancellationToken);
            if (entity is not null)
            {
                Entity.Remove(entity);
            }
        }

        public async Task DeleteByIdAsync(string id)
        {
            TEntity? entity = await Entity.FindAsync(id);
            if (entity is not null)
            {
                Entity.Remove(entity);
            }
        }

        public void DeleteRange(ICollection<TEntity> entities)
        {
            Entity.RemoveRange(entities);
        }
        public void Update(TEntity entity)
        {
            Entity.Update(entity);
        }

        public void UpdateRange(ICollection<TEntity> entities)
        {
            Entity.UpdateRange(entities);
        }
    }
}
