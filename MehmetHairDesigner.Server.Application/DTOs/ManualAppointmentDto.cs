using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MehmetHairDesigner.Server.Application.DTOs
{
    public class ManualAppointmentDto
    {
        public Guid? UserId { get; set; } // Kullanıcı UI'dan seçilmişse dolu olur (registered)
        public string FullName { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;

        public Guid BarberId { get; set; }
        public DateTime StartTime { get; set; }
        public int ServiceType { get; set; } // Enum: Sac = 1, Sakal = 2, SacVeSakal = 3

        public string? Notes { get; set; } // İsteğe bağlı açıklama
    }
}
