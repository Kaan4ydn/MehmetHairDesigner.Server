using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MehmetHairDesigner.Server.Application.DTOs
{
    public class AddHolidayDto
    {
        public Guid BarberId { get; set; }
        public DateTime Date { get; set; }
        public string? Reason { get; set; }
    }
}
