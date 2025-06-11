using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MehmetHairDesigner.Server.Application.DTOs
{
    public class UpdateWorkingHourDto
    {
        public Guid BarberId { get; set; }
        public List<DayWorkingHourDto> Days { get; set; } = new();
    }
}
