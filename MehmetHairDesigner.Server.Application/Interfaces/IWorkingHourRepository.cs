using MehmetHairDesigner.Server.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MehmetHairDesigner.Server.Application.Services
{
    public interface IWorkingHourRepository
    {
        Task<List<WorkingHour>> GetByBarberAsync(Guid barberId);
        Task ReplaceAllAsync(Guid barberId, List<WorkingHour> hours);
        Task SaveChangesAsync();
    }
}
