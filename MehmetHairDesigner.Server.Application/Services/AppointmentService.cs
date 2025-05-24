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

    public AppointmentService(IAppointmentRepository repo , INotificationRequestRepository notificationRequestRepo, IMailService mailService)
    {
        _repo = repo;
        _notificationRequestRepo = notificationRequestRepo;
        _mailService = mailService;
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
            Notes = dto.Notes
        };

        await CreateAppointmentAsync(appointment);
    }

    public async Task CreateForGuestAsync(CreateAppointmentGuestDto dto)
    {
        var guestUser = new AppUser
        {
            FullName = dto.FullName!,
            PhoneNumber = dto.PhoneNumber!,
            Roles = new List<string> { "Guest" }
        };

        await _repo.AddUserAsync(guestUser);
        await _repo.SaveChangesAsync();

        var appointment = new Appointment
        {
            UserId = guestUser.Id,
            BarberId = dto.BarberId,
            StartTime = dto.StartTime,
            ServiceType = dto.ServiceType,
            Notes = dto.Notes
        };

        await CreateAppointmentAsync(appointment);
    }

    public async Task<bool> IsSlotAvailableAsync(Guid barberId, DateTime requestedStart, ServiceType serviceType)
    {
        var appointments = await _repo.GetAppointmentsByBarberAndDate(barberId, requestedStart.Date);

        var requestedEnd = serviceType switch
        {
            ServiceType.Sac => requestedStart.AddMinutes(30),
            ServiceType.Sakal => requestedStart.AddMinutes(15),
            ServiceType.SacVeSakal => requestedStart.AddMinutes(45),
            _ => requestedStart
        };

        // Çakýþma var mý kontrolü
        return !appointments.Any(existing =>
            requestedStart < existing.EndTime && existing.StartTime < requestedEnd);
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

        // Bildirim kontrolü
        var pending = await _notificationRequestRepo.GetPendingRequestsAsync(date, time, serviceType);
        foreach (var request in pending)
        {
            if (!string.IsNullOrEmpty(request.PhoneNumber)) // Mail yerine PhoneNumber’ý eposta gibi varsayýyoruz
            {
                await _mailService.SendAsync(
                    request.PhoneNumber,
                    "Randevu Boþluðu Oluþtu!",
                    $"Seçtiðiniz {request.RequestedDate:dd.MM.yyyy} tarihli saat için boþluk oluþtu.");
            }
        }

        return true;
    }

    public async Task<bool> CancelGuestAppointmentAsync(string fullName, string phoneNumber)
    {
        var appointment = await _repo.GetGuestAppointmentAsync(fullName, phoneNumber);
        if (appointment == null)
            return false;

        // Bildirimden önce bilgileri al
        var date = appointment.StartTime.Date;
        var time = appointment.StartTime.TimeOfDay;
        var serviceType = appointment.ServiceType;

        _repo.Delete(appointment);
        await _repo.SaveChangesAsync();

        // Bildirim kontrolü
        var pending = await _notificationRequestRepo.GetPendingRequestsAsync(date, time, serviceType);
        foreach (var request in pending)
        {
            if (!string.IsNullOrEmpty(request.PhoneNumber))
            {
                await _mailService.SendAsync(
                    request.PhoneNumber,
                    "Randevu Boþluðu Oluþtu!",
                    $"Seçtiðiniz {request.RequestedDate:dd.MM.yyyy} tarihli saat için boþluk oluþtu.");
            }
        }

        return true;
    }

}