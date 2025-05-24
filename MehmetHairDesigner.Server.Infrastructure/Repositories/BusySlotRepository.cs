using MehmetHairDesigner.Server.Application.Interfaces;
using MehmetHairDesigner.Server.Domain.Entities;
using MehmetHairDesigner.Server.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MehmetHairDesigner.Server.Infrastructure.Repositories
{
    public class BusySlotRepository : IBusySlotRepository
    {
        private readonly AppDbContext _context;

        public BusySlotRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(BusySlot slot)
        {
            await _context.BusySlots.AddAsync(slot);
        }

        public async Task DeleteAsync(Guid id)
        {
            var slot = await _context.BusySlots.FindAsync(id);
            if (slot != null)
                _context.BusySlots.Remove(slot);
        }

        public async Task<List<BusySlot>> GetByDateAsync(Guid barberId, DateTime date)
        {
            return await _context.BusySlots
                .Where(x => x.BarberId == barberId && x.StartTime.Date == date.Date)
                .ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
