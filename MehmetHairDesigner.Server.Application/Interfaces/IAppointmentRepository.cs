using MehmetHairDesigner.Server.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MehmetHairDesigner.Server.Application.Interfaces
{
    public interface IAppointmentRepository
    {
        Task<List<Appointment>> GetAppointmentsByBarberAndDate(Guid barberId, DateTime date);
        Task AddAsync(Appointment appointment);
        Task AddUserAsync(AppUser user);

        Task<bool> HasAppointmentForDayAsync(Guid userId, DateTime day);

        Task<List<Appointment>> GetAppointmentsForDate(Guid barberId, DateTime date);

        Task<Appointment> GetByIdAsync(Guid id);
        void Delete(Appointment entity);

        Task<Appointment?> GetGuestAppointmentAsync(string fullName, string phoneNumber);

        Task<List<Appointment>> GetAppointmentsByBarberAndDate2(Guid barberId, DateTime date);

        Task<Appointment?> GetLatestFutureAppointmentForUser(Guid userId);

        Task<List<Appointment>> GetPendingAppointmentsAsync();
        Task SaveChangesAsync();

    }
}
