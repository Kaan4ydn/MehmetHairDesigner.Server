using MehmetHairDesigner.Server.Domain.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MehmetHairDesigner.Server.Domain.Entities
{
    public class Holiday : BaseEntity
    {
        public Guid BarberId { get; set; }
        public DateTime Date { get; set; }
        public string? Reason { get; set; }
    }
}
