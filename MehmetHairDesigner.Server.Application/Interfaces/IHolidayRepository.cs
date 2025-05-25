using MehmetHairDesigner.Server.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MehmetHairDesigner.Server.Application.Interfaces
{
    public interface IHolidayRepository
    {
        Task AddAsync(Holiday holiday);
        Task DeleteAsync(Guid id);
        Task<List<Holiday>> GetByBarberAsync(Guid barberId);

        Task<bool> IsHolidayAsync(Guid barberId, DateTime date);
        Task SaveChangesAsync();
    }
}
