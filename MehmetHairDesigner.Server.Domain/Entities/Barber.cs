using System;
using System.Collections.Generic;
using MehmetHairDesigner.Server.Domain.Abstraction;
using MehmetHairDesigner.Server.Domain.Entities;

namespace MehmetHairDesigner.Server.Domain.Entities
{
    public class Barber : BaseEntity
    {
        public string FullName { get; set; }  // Mehmet, Emre gibi

        // Navigation
        public ICollection<Appointment> Appointments { get; set; }
    }
}