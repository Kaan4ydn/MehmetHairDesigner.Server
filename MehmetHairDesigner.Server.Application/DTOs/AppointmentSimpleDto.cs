using MehmetHairDesigner.Server.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MehmetHairDesigner.Server.Application.DTOs
{
    public class AppointmentSimpleDto
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public ServiceType ServiceType { get; set; }
    }
}
