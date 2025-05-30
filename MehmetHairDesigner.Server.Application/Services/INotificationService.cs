using MehmetHairDesigner.Server.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MehmetHairDesigner.Server.Application.Services
{
    public interface INotificationService
    {
        Task NotifyIfSlotAvailable(Guid barberId, DateTime date, TimeSpan time, ServiceType serviceType);
    }
}
