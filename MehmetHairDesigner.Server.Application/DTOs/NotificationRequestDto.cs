using MehmetHairDesigner.Server.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MehmetHairDesigner.Server.Application.DTOs
{
    public class NotifyRequestDto
    {

        public Guid BarberId { get; set; }
        public DateTime Date { get; set; }                       // 🔹 Zorunlu
        public TimeSpan? StartTime { get; set; }                 // 🔸 Opsiyonel
        public TimeSpan? EndTime { get; set; }                   // 🔸 Opsiyonel

        public ServiceType ServiceType { get; set; }
        
    }
}
