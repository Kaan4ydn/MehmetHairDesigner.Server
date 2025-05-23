using MehmetHairDesigner.Server.Application.DTOs;
using MehmetHairDesigner.Server.Application.Interfaces;
using MehmetHairDesigner.Server.Domain.Entities;

public class AppointmentService : IAppointmentService
{
    private readonly IAppointmentRepository _repo;

    public AppointmentService(IAppointmentRepository repo)
    {
        _repo = repo;
    }

    public async Task CreateAppointmentAsync(Appointment appointment)
    {
        appointment.EndTime = appointment.ServiceType switch
        {
            ServiceType.Sac => appointment.StartTime.AddMinutes(30),
            ServiceType.Sakal => appointment.StartTime.AddMinutes(15),
            ServiceType.SacVeSakal => appointment.StartTime.AddMinutes(45),
            _ => appointment.StartTime
        };

        await _repo.AddAsync(appointment);
        await _repo.SaveChangesAsync();
    }

    public async Task<List<AvailabilitySlotDto>> GetAvailabilityAsync(Guid barberId, DateTime date)
    {
        var appointments = await _repo.GetAppointmentsByBarberAndDate(barberId, date);
        var start = date.Date.AddHours(9);
        var end = date.Date.AddHours(21);
        var duration = TimeSpan.FromMinutes(15);

        var result = new List<AvailabilitySlotDto>();
        for (var time = start; time < end; time += duration)
        {
            bool isBusy = appointments.Any(a =>
                time < a.EndTime && a.StartTime < time.Add(duration));

            result.Add(new AvailabilitySlotDto
            {
                Time = time,
                IsAvailable = !isBusy
            });
        }

        return result;
    }
}