using MehmetHairDesigner.Server.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MehmetHairDesigner.Server.Application.Interfaces
{
    public interface IWorkingHourRepository
    {
        Task<List<WorkingHour>> GetByBarberAsync(Guid barberId);
        Task ReplaceAllAsync(Guid barberId, List<WorkingHour> hours);

        Task<WorkingHour?> GetByBarberAndDayAsync(Guid barberId, DayOfWeek day);

        Task<WorkingHour?> GetWorkingHoursForDay(Guid barberId, DayOfWeek dayOfWeek);
        Task SaveChangesAsync();
    }
}
