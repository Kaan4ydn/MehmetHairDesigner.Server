using MehmetHairDesigner.Server.Domain.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MehmetHairDesigner.Server.Domain.Entities
{
    public class NotificationRequest : BaseEntity
    {
        public Guid? UserId { get; set; }
        public Guid BarberId { get; set; }

        public string Email { get; set; }

        public DateTime RequestedDate { get; set; }
        public TimeSpan? RequestedStart { get; set; }
        public TimeSpan? RequestedEnd { get; set; }

        public ServiceType ServiceType { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

}
