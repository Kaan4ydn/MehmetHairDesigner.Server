using MehmetHairDesigner.Server.Application.Interfaces;
using MehmetHairDesigner.Server.Domain.Entities;
using MehmetHairDesigner.Server.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace MehmetHairDesigner.Server.Infrastructure.Repositories
{
    public class WorkingHourRepository : IWorkingHourRepository
    {
        private readonly AppDbContext _context;

        public WorkingHourRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<WorkingHour>> GetByBarberAsync(Guid barberId)
        {
            return await _context.WorkingHours
                .Where(x => x.BarberId == barberId)
                .ToListAsync();
        }

        public async Task ReplaceAllAsync(Guid barberId, List<WorkingHour> hours)


        {
            var existing = await _context.WorkingHours.Where(x => x.BarberId == barberId).ToListAsync();
            _context.WorkingHours.RemoveRange(existing);
            await _context.WorkingHours.AddRangeAsync(hours);
        }

        public async Task<WorkingHour?> GetByBarberAndDayAsync(Guid barberId, DayOfWeek day)
        {
            return await _context.WorkingHours
                .FirstOrDefaultAsync(w => w.BarberId == barberId && w.Day == day);
        }

        public async Task<WorkingHour?> GetWorkingHoursForDay(Guid barberId, DayOfWeek dayOfWeek)
        {
            return await _context.WorkingHours
                .FirstOrDefaultAsync(x => x.BarberId == barberId && x.Day == dayOfWeek);
        }
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
