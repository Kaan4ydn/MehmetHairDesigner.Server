using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MehmetHairDesigner.Server.Application.DTOs
{
    public class SetWorkingHoursDto
    {
        public Guid BarberId { get; set; }
        public List<DayWorkingHourDto> Days { get; set; } = new();
    }

    public class DayWorkingHourDto
    {
        public DayOfWeek Day { get; set; }
        public TimeSpan Start { get; set; }
        public TimeSpan End { get; set; }
    }
}
