using MehmetHairDesigner.Server.Application.DTOs;
using MehmetHairDesigner.Server.Application.Interfaces;
using MehmetHairDesigner.Server.Application.Interfaces.Repositories;
using MehmetHairDesigner.Server.Application.Services;
using MehmetHairDesigner.Server.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class AppointmentService : IAppointmentService
{
    private readonly IAppointmentRepository _repo;
    private readonly INotificationRequestRepository _notificationRequestRepo;
    private readonly IMailService _mailService;
    private readonly IHolidayRepository _holidayRepo;
    private readonly IWorkingHourRepository _workingHourRepo;
    private readonly IBusySlotRepository _busySlotRepo;
    private readonly INotificationService  _notificationService;

    public AppointmentService(IAppointmentRepository repo , INotificationRequestRepository notificationRequestRepo, IMailService mailService, IHolidayRepository holidayRepo, IWorkingHourRepository workingHourRepo, IBusySlotRepository busySlotRepo, INotificationService notificationService)
    {
        _repo = repo;
        _notificationRequestRepo = notificationRequestRepo;
        _mailService = mailService;
        _holidayRepo = holidayRepo;
        _workingHourRepo = workingHourRepo;
        _busySlotRepo = busySlotRepo;
        _notificationService = notificationService;
    }

    public async Task<bool> UserHasAppointment(Guid userId, DateTime date)
    {
        return await _repo.HasAppointmentForDayAsync(userId, date);
    }

    public async Task CreateAppointmentAsync(Appointment appointment)
    {
        appointment.EndTime = appointment.ServiceType switch
        {
            ServiceType.Sac => appointment.StartTime.AddMinutes(30),
            ServiceType.Sakal => appointment.StartTime.AddMinutes(15),
            ServiceType.SacVeSakal => appointment.StartTime.AddMinutes(45),
            _ => appointment.StartTime
        };

        await _repo.AddAsync(appointment);
        await _repo.SaveChangesAsync();
    }

    public async Task<List<AvailabilitySlotDto>> GetAvailabilityAsync(Guid barberId, DateTime date, ServiceType serviceType)
    {
        var appointments = await _repo.GetAppointmentsByBarberAndDate(barberId, date);
        var start = date.Date.AddHours(9);
        var end = date.Date.AddHours(21);

        var serviceDuration = serviceType switch
        {
            ServiceType.Sac => TimeSpan.FromMinutes(30),
            ServiceType.Sakal => TimeSpan.FromMinutes(15),
            ServiceType.SacVeSakal => TimeSpan.FromMinutes(45),
            _ => TimeSpan.FromMinutes(15)
        };

        var slotInterval = TimeSpan.FromMinutes(15); // slotlar yine 15 dk görünsün
        var result = new List<AvailabilitySlotDto>();

        for (var time = start; time <= end - serviceDuration; time += slotInterval)
        {
            bool overlaps = appointments.Any(a =>
                time < a.EndTime && a.StartTime < time + serviceDuration);

            result.Add(new AvailabilitySlotDto
            {
                Time = time,
                IsAvailable = !overlaps
            });
        }

        return result;
    }

    public async Task CreateForRegisteredUserAsync(Guid userId, CreateAppointmentDto dto)
    {
        var appointment = new Appointment
        {
            UserId = userId,
            BarberId = dto.BarberId,
            StartTime = dto.StartTime,
            ServiceType = dto.ServiceType,
            Notes = dto.Notes ,
            Status = "Pending"
        };

        await CreateAppointmentAsync(appointment);
    }

    public async Task CreateForGuestAsync(CreateAppointmentGuestDto dto)
    {

        var user = await _repo.GetUserByFullNameAndPhoneAsync(dto.FullName, dto.PhoneNumber);


        if (user == null) {
            var guestUser = new AppUser
            {
                FullName = dto.FullName!,
                PhoneNumber = dto.PhoneNumber!,
                Roles = new List<string> { "Guest" }
            };

            await _repo.AddUserAsync(guestUser);
            await _repo.SaveChangesAsync();

            var appointmentForNewGuest = new Appointment
            {
                UserId = guestUser.Id,
                BarberId = dto.BarberId,
                StartTime = dto.StartTime,
                ServiceType = dto.ServiceType,
                Notes = dto.Notes
            };

            await CreateAppointmentAsync(appointmentForNewGuest);
        }
        else {
            var appointment = new Appointment
            {
                UserId = user.Id,
                BarberId = dto.BarberId,
                StartTime = dto.StartTime,
                ServiceType = dto.ServiceType,
                Notes = dto.Notes
            };

            await CreateAppointmentAsync(appointment);
        }

            
    }

    public async Task<bool> IsSlotAvailableAsync(Guid barberId, DateTime requestedStart, ServiceType serviceType)
    {
        var requestedEnd = serviceType switch
        {
            ServiceType.Sac => requestedStart.AddMinutes(30),
            ServiceType.Sakal => requestedStart.AddMinutes(15),
            ServiceType.SacVeSakal => requestedStart.AddMinutes(45),
            _ => requestedStart
        };

        // 1. Holiday kontrolü
        var isHoliday = await _holidayRepo.IsHolidayAsync(barberId, requestedStart.Date);
        if (isHoliday)
            return false;

        // 2. Working hours kontrolü
        var workingHours = await _workingHourRepo.GetByBarberAndDayAsync(barberId, requestedStart.DayOfWeek);
        if (workingHours == null || requestedStart.TimeOfDay < workingHours.Start || requestedEnd.TimeOfDay > workingHours.End)
            return false;

        // 3. Busy slot kontrolü
        var busySlots = await _busySlotRepo.GetByDateAsync(barberId, requestedStart.Date);
        if (busySlots.Any(b => requestedStart < b.EndTime && b.StartTime < requestedEnd))
            return false;

        // 4. Appointment çakýþma kontrolü
        var appointments = await _repo.GetAppointmentsByBarberAndDate(barberId, requestedStart.Date);
        if (appointments.Any(existing => requestedStart < existing.EndTime && existing.StartTime < requestedEnd))
            return false;

        return true;
    }
    public async Task<Dictionary<string, List<AvailabilitySlotDto>>> GetAvailabilityForRangeAsync(Guid barberId, ServiceType serviceType, int days)
    {
        var response = new Dictionary<string, List<AvailabilitySlotDto>>();

        for (int i = 0; i < days; i++)
        {
            var date = DateTime.Today.AddDays(i);
            var slots = await GetAvailabilityAsync(barberId, date, serviceType);
            response.Add(date.ToString("yyyy-MM-dd"), slots);
        }

        return response;
    }

    public async Task<bool> CancelAppointmentAsync(Guid appointmentId, Guid userId)
    {
        var appointment = await _repo.GetByIdAsync(appointmentId);
        if (appointment == null || appointment.UserId != userId)
            return false;

        // Bildirimden önce bilgileri al
        var date = appointment.StartTime.Date;
        var time = appointment.StartTime.TimeOfDay;
        var serviceType = appointment.ServiceType;

        _repo.Delete(appointment);
        await _repo.SaveChangesAsync();

        var barberId = appointment.BarberId;

        // Bildirim kontrolü
        await _notificationService.NotifyIfSlotAvailable(barberId, date, time, serviceType);


        return true;
    }

    public async Task CreateManualAppointmentAsync(ManualAppointmentDto dto)
    {
        var user = await _repo.GetUserByFullNameAndPhoneAsync(dto.FullName, dto.PhoneNumber);

        bool isAvailable = await IsSlotAvailableAsync(dto.BarberId, dto.StartTime, (ServiceType)dto.ServiceType);
        if (!isAvailable)
            throw new Exception("Seçilen saat dolu.");

        if (dto.StartTime <= DateTime.Now)
            throw new Exception("Geçmiþ tarihe randevu alýnamaz.");

        // Kullanýcý yoksa: yeni guest oluþtur
        if (user == null)
        {
            var guestDto = new CreateAppointmentGuestDto
            {
                FullName = dto.FullName,
                PhoneNumber = dto.PhoneNumber,
                BarberId = dto.BarberId,
                StartTime = dto.StartTime,
                ServiceType = (ServiceType)dto.ServiceType,
                Notes = dto.Notes
            };

            await CreateForGuestAsync(guestDto);
            return;
        }

        // Kullanýcý varsa ve guest rolündeyse
        if (user.Roles.Contains("Guest"))
        {
            var guestDto = new CreateAppointmentGuestDto
            {
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber ?? dto.PhoneNumber, // null ise DTO'dan al
                BarberId = dto.BarberId,
                StartTime = dto.StartTime,
                ServiceType = (ServiceType)dto.ServiceType,
                Notes = dto.Notes
            };

            await CreateForGuestAsync(guestDto); // ama yeni user oluþturma!
            return;
        }

        // Kullanýcý varsa ve User rolündeyse
        var registeredDto = new CreateAppointmentDto
        {
            BarberId = dto.BarberId,
            StartTime = dto.StartTime,
            ServiceType = (ServiceType)dto.ServiceType,
            Notes = dto.Notes
        };

        await CreateForRegisteredUserAsync(user.Id, registeredDto);
    }






    public async Task<List<Appointment>> GetAppointmentsByBarberAndDate2(Guid barberId, DateTime date)
    {
        return await _repo.GetAppointmentsByBarberAndDate2(barberId, date);
    }
}