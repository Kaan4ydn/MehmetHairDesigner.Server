using System;
using System.Collections.Generic;
using MehmetHairDesigner.Server.Domain.Entities;

namespace MehmetHairDesigner.Server.Domain.Entities
{
    public class Barber
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }  // Mehmet, Emre gibi

        // Navigation
        public ICollection<Appointment> Appointments { get; set; }
    }
}