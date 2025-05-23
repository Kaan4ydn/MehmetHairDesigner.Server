using MehmetHairDesigner.Server.Domain.Entities;
using MehmetHairDesigner.Server.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using MehmetHairDesigner.Server.Application.Interfaces;

namespace MehmetHairDesigner.Server.Infrastructure.Repositories
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly AppDbContext _context;

        public AppointmentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Appointment>> GetAppointmentsByBarberAndDate(Guid barberId, DateTime date)
        {
            return await _context.Appointments
                .Where(a => a.BarberId == barberId && a.StartTime.Date == date.Date)
                .ToListAsync();
        }

        public async Task AddAsync(Appointment appointment)
        {
            await _context.Appointments.AddAsync(appointment);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}