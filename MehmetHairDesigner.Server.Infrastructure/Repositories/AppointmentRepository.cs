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

        public async Task<bool> HasAppointmentForDayAsync(Guid userId, DateTime day)
        {
            return await _context.Appointments
                .AnyAsync(a => a.UserId == userId && a.StartTime > DateTime.UtcNow);
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

        public async Task AddUserAsync(AppUser user)
        {
            await _context.AppUsers.AddAsync(user);
        }

        public async Task<Appointment?> GetByIdAsync(Guid id)
        {
            return await _context.Appointments
       .Include(a => a.User)
       .FirstOrDefaultAsync(a => a.Id == id);
        }

        public void Delete(Appointment appointment)
        {
            _context.Appointments.Remove(appointment);
        }

        public async Task<Appointment?> GetGuestAppointmentAsync(string fullName, string phoneNumber)
        {
            return await _context.Appointments
                .Include(a => a.User) // navigation property varsa
                .FirstOrDefaultAsync(a =>
                    a.User.FullName == fullName &&
                    a.User.PhoneNumber == phoneNumber );
        }

        public async Task<List<Appointment>> GetAppointmentsByBarberAndDate2(Guid barberId, DateTime date)
        {
            return await _context.Appointments
                .Where(a => a.BarberId == barberId && a.StartTime.Date == date.Date)
                .ToListAsync();
        }


        public async Task<List<Appointment>> GetAppointmentsForDate(Guid barberId, DateTime date)
        {
            return await _context.Appointments
                .Where(x => x.BarberId == barberId && x.StartTime.Date == date.Date)
                .ToListAsync();
        }

        public Task<Appointment> GetLatestFutureAppointmentForUser(Guid userId)
        {
            return _context.Appointments
                .Where(a => a.UserId == userId && a.StartTime > DateTime.UtcNow)
                .OrderBy(a => a.StartTime)
                .FirstOrDefaultAsync();
        }

        public async Task<List<Appointment>> GetPendingAppointmentsAsync()
        {
            return await _context.Appointments
                .Include(a => a.User)
                .Include(a => a.Barber)
                .Where(a => a.Status == "pending")
                .OrderBy(a => a.StartTime)
                .ToListAsync();
        }




        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }


    }
}