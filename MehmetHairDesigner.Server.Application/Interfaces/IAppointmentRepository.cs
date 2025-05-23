using MehmetHairDesigner.Server.Domain.Entities;

namespace MehmetHairDesigner.Server.Application.Interfaces
{
    public interface IAppointmentRepository
    {
        Task<List<Appointment>> GetAppointmentsByBarberAndDate(Guid barberId, DateTime date);
        Task AddAsync(Appointment appointment);
        Task SaveChangesAsync();
    }
}