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
    public class HolidayRepository : IHolidayRepository
    {
        private readonly AppDbContext _context;

        public HolidayRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Holiday holiday)
        {
            await _context.Holidays.AddAsync(holiday);
        }

        public async Task DeleteAsync(Guid id)
        {
            var holiday = await _context.Holidays.FindAsync(id);
            if (holiday != null)
                _context.Holidays.Remove(holiday);
        }

        public async Task<List<Holiday>> GetByBarberAsync(Guid barberId)
        {
            return await _context.Holidays
                .Where(x => x.BarberId == barberId)
                .ToListAsync();
        }

        public async Task<bool> IsHolidayAsync(Guid barberId, DateTime date)
        {
            return await _context.Holidays
                .AnyAsync(h => h.BarberId == barberId && h.Date.Date == date.Date);
        }
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
