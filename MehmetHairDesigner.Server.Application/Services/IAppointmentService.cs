using MehmetHairDesigner.Server.Application.DTOs;
using MehmetHairDesigner.Server.Domain.Entities;

public interface IAppointmentService
{
    Task CreateAppointmentAsync(Appointment appointment);
    Task CreateForRegisteredUserAsync(Guid userId, CreateAppointmentDto dto);
    Task CreateForGuestAsync(CreateAppointmentGuestDto dto);

    Task<List<AvailabilitySlotDto>> GetAvailabilityAsync(Guid barberId, DateTime date, ServiceType serviceType);

    Task<Dictionary<string, List<AvailabilitySlotDto>>> GetAvailabilityForRangeAsync(Guid barberId, ServiceType serviceType, int days);

    Task<List<Appointment>> GetAppointmentsByBarberAndDate2(Guid barberId, DateTime date);

    Task CreateManualAppointmentAsync(ManualAppointmentDto dto);

    Task<Appointment?> GetAppointmentDetailsAsync(Guid id);

    Task<bool> AdminCancelAppointmentAsync(Guid appointmentId, string reason);

    Task<bool> IsSlotAvailableAsync(Guid barberId, DateTime requestedStart, ServiceType serviceType);


}