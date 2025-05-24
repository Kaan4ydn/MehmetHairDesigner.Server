using MehmetHairDesigner.Server.Application.DTOs;
using MehmetHairDesigner.Server.Domain.Entities;

public interface IAppointmentService
{
    Task CreateAppointmentAsync(Appointment appointment);
    Task CreateForRegisteredUserAsync(Guid userId, CreateAppointmentDto dto);
    Task CreateForGuestAsync(CreateAppointmentGuestDto dto);

    Task<List<AvailabilitySlotDto>> GetAvailabilityAsync(Guid barberId, DateTime date, ServiceType serviceType);
}