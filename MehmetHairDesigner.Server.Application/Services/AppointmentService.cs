using MehmetHairDesigner.Server.Domain.Entities;

public class AppointmentService : IAppointmentService
{
    public async Task CreateAppointmentAsync(Appointment appointment)
    {
        appointment.EndTime = appointment.ServiceType switch
        {
            ServiceType.Sac => appointment.StartTime.AddMinutes(30),
            ServiceType.Sakal => appointment.StartTime.AddMinutes(15),
            ServiceType.SacVeSakal => appointment.StartTime.AddMinutes(45),
            _ => appointment.StartTime
        };

        // Burada EF ile veritabanına kaydedebilirsin (örnek):
        // _context.Appointments.Add(appointment);
        // await _context.SaveChangesAsync();
    }
}