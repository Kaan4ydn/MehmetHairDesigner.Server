using MehmetHairDesigner.Server.Domain.Entities;
namespace MehmetHairDesigner.Server.Application.DTOs
{
    public class CreateAppointmentDto
    {
        public Guid BarberId { get; set; }
        public DateTime StartTime { get; set; }
        public ServiceType ServiceType { get; set; }

        // İsteğe bağlı not
        public string? Notes { get; set; }
    }
}