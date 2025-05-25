using MehmetHairDesigner.Server.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MehmetHairDesigner.Server.Application.Interfaces
{
    public interface IBusySlotRepository
    {
        Task AddAsync(BusySlot slot);
        Task DeleteAsync(Guid id);
        Task<List<BusySlot>> GetByDateAsync(Guid barberId, DateTime date);

        Task SaveChangesAsync();
    }
}
