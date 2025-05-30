using MehmetHairDesigner.Server.Application.DTOs;
using MehmetHairDesigner.Server.Application.Interfaces;
using MehmetHairDesigner.Server.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MehmetHairDesigner.Server.Application.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRequestRepository _repo;
        private readonly IMailService _mailService;
   
        private readonly IWorkingHourRepository _workingHourRepository;
        private readonly IAppointmentRepository _appointmentRepository;

        public NotificationService(INotificationRequestRepository repo, IMailService mailService, IWorkingHourService workingHourService, IWorkingHourRepository workingHourRepository, IAppointmentRepository appointmentRepository)
        {
            _repo = repo;
            _mailService = mailService;
            
            _workingHourRepository = workingHourRepository;
            _appointmentRepository = appointmentRepository;


        }   



        public async Task NotifyIfSlotAvailable(Guid barberId, DateTime date, TimeSpan time, ServiceType serviceType)
        {
            var pending = await _repo.GetPendingRequestsAsync(barberId, date, time, serviceType);

            // 1. Berberin o günkü çalışma saatlerini al
            var workingHours = await _workingHourRepository.GetWorkingHoursForDay(barberId, date.DayOfWeek);
            if (workingHours == null)
                return;

            var start = workingHours.Start;
            var end = workingHours.End;

            // 2. O güne ait tüm randevuları al
            var appointments = await _appointmentRepository.GetAppointmentsForDate(barberId, date);

            // 3. 15 dakikalık tüm slotları üret
            var allSlots = GenerateTimeSlots(start, end);

            // 4. Dolu olan slotları çıkar
            var busySlots = new HashSet<TimeSpan>();
            foreach (var app in appointments)
            {
                var appStart = app.StartTime.TimeOfDay;
                var appEnd = app.EndTime.TimeOfDay;

                var t = appStart;
                while (t < appEnd)
                {
                    busySlots.Add(t);
                    t = t.Add(TimeSpan.FromMinutes(15));
                }
            }

            // 5. Müsait olanları filtrele
            var slots = allSlots
                .Select(t => new AvailabilitySlotDto
                {
                    Time = date.Date.Add(t),
                    IsAvailable = !busySlots.Contains(t)
                })
                .ToList();

            foreach (var request in pending)
            {
                var rangeStart = request.RequestedStart ?? TimeSpan.Zero;
                var rangeEnd = request.RequestedEnd ?? TimeSpan.FromHours(23);

                var required = serviceType switch
                {
                    ServiceType.Sac => 1,
                    ServiceType.Sakal => 2,
                    ServiceType.SacVeSakal => 3,
                    _ => 2
                };

                var availableTimes = slots
                    .Where(s => s.IsAvailable)
                    .Select(s => s.Time.TimeOfDay)
                    .Where(t => t >= rangeStart && t + TimeSpan.FromMinutes(15 * required) <= rangeEnd)
                    .OrderBy(t => t)
                    .ToList();

                if (HasConsecutiveSlots(availableTimes, required) && !string.IsNullOrEmpty(request.Email))
                {
                    await _mailService.SendAsync(
                        request.Email,
                        "Randevu Boşluğu Oluştu!",
                        $"Seçtiğiniz {request.RequestedDate:dd.MM.yyyy} tarihli saat için boşluk oluştu.");
                }
            }
        }

        private bool HasConsecutiveSlots(List<TimeSpan> slots, int requiredCount)
        {
            for (int i = 0; i <= slots.Count - requiredCount; i++)
            {
                bool consecutive = true;
                for (int j = 0; j < requiredCount - 1; j++)
                {
                    if (slots[i + j + 1] != slots[i + j] + TimeSpan.FromMinutes(15))
                    {
                        consecutive = false;
                        break;
                    }
                }

                if (consecutive)
                    return true;
            }

            return false;
        }

        private List<TimeSpan> GenerateTimeSlots(TimeSpan start, TimeSpan end)
        {
            var list = new List<TimeSpan>();
            var current = start;
            while (current < end)
            {
                list.Add(current);
                current = current.Add(TimeSpan.FromMinutes(15));
            }
            return list;
        }
    }

}
