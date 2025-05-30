using MehmetHairDesigner.Server.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MehmetHairDesigner.Server.Application.Interfaces
{
    public interface INotificationRequestRepository
    {
        Task AddAsync(NotificationRequest entity);
        Task SaveChangesAsync();
        Task<List<NotificationRequest>> GetPendingRequestsAsync(Guid barberId, DateTime date, TimeSpan time, ServiceType serviceType);


    }
}
