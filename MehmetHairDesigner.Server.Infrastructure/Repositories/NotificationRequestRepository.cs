using MehmetHairDesigner.Server.Application.Interfaces;
using MehmetHairDesigner.Server.Domain.Entities;
using MehmetHairDesigner.Server.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace MehmetHairDesigner.Server.Infrastructure.Repositories
{
    public class NotificationRequestRepository : INotificationRequestRepository
    {
        private readonly AppDbContext _context;

        public NotificationRequestRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(NotificationRequest entity)
        {
            await _context.NotificationRequests.AddAsync(entity);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<List<NotificationRequest>> GetPendingRequestsAsync(DateTime date, TimeSpan time, ServiceType serviceType)
        {
            return await _context.NotificationRequests
                .Where(x =>
                    x.RequestedDate.Date == date.Date &&
                    (!x.RequestedStart.HasValue || x.RequestedStart <= time) &&
                    (!x.RequestedEnd.HasValue || x.RequestedEnd > time) &&
                    x.ServiceType == serviceType)
                .ToListAsync();
        }
    }
}
