using MehmetHairDesigner.Server.Application.Interfaces.Repositories;
using MehmetHairDesigner.Server.Application.Interfaces.UnitOfWorks;
using MehmetHairDesigner.Server.Infrastructure.Persistence;
using MehmetHairDesigner.Server.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MehmetHairDesigner.Server.Infrastructure.UnitOfWorks
{
    public class UnitOfWork(AppDbContext appDbContext) : IUnitOfWork
    {
        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken)) => await appDbContext.SaveChangesAsync(cancellationToken);
        public int SaveChanges() => appDbContext.SaveChanges();
        IReadRepository<T> IUnitOfWork.GetReadRepository<T>() => new ReadRepository<T>(appDbContext);
        IWriteRepository<T> IUnitOfWork.GetWriteRepository<T>() => new WriteRepository<T>(appDbContext);
        public async ValueTask DisposeAsync() => await appDbContext.DisposeAsync();
    }
}
