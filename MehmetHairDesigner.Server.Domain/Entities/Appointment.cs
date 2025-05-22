using System;
using System.Collections.Generic;
namespace MehmetHairDesigner.Server.Domain.Entities
{
    public enum ServiceType
    {
        Sac = 1,
        Sakal = 2,
        SacVeSakal = 3
    }
    public class Appointment
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }           // Randevuyu alan kullanıcı
        public Guid BarberId { get; set; }         // Hangi berbere randevu alındı
        public DateTime StartTime { get; set; }    // Başlangıç
        public DateTime EndTime { get; set; }      // Bitiş (otomatik hesaplanacak)
        public ServiceType ServiceType { get; set; } // Saç / Sakal / Saç ve Sakal

        public string? Notes { get; set; }         // İsteğe bağlı not

        // Navigation
        public Barber Barber { get; set; }
    }



}

