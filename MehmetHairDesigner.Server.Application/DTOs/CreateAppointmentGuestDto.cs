using MehmetHairDesigner.Server.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MehmetHairDesigner.Server.Application.DTOs
{
    public class CreateAppointmentGuestDto
    {
        public Guid BarberId { get; set; }
        public DateTime StartTime { get; set; }
        public ServiceType ServiceType { get; set; }

        // Zorunlu alanlar
        public string FullName { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;

        public string? Notes { get; set; }
    }
}
