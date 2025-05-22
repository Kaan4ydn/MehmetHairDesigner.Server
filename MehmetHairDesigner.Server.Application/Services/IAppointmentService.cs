using MehmetHairDesigner.Server.Domain.Entities;

public interface IAppointmentService
{
    Task CreateAppointmentAsync(Appointment appointment);
}