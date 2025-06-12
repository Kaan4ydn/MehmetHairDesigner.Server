using MehmetHairDesigner.Server.Application.DTOs;
using MehmetHairDesigner.Server.Application.Interfaces;
using MehmetHairDesigner.Server.Application.Services;
using MehmetHairDesigner.Server.Domain.Entities;
using MehmetHairDesigner.Server.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Json;
using MehmetHairDesigner.Server.Infrastructure.Persistence;

namespace MehmetHairDesigner.Server.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AppointmentController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;
        private readonly INotificationRequestRepository _notificationRequestRepo;
        private readonly AppDbContext _context;
        private readonly IWorkingHourService _workingHourService;
        private readonly INotificationService _notificationService;
        private readonly IAppointmentRepository _appointmentRepo;
        private readonly IBusySlotService _busySlotService;
        private readonly IHolidayService _holidayService;



        public AppointmentController(
    IAppointmentService appointmentService,
    INotificationRequestRepository notificationRequestRepo,
    AppDbContext context,
        IWorkingHourService workingHourService,
        INotificationService notificationService,
        IAppointmentRepository appointmentRepo, IBusySlotService busySlotService , IHolidayService holidayService )
        {
            _appointmentService = appointmentService;
            _notificationRequestRepo = notificationRequestRepo;
            _workingHourService = workingHourService;
            _busySlotService = busySlotService;
            _holidayService = holidayService;

            Console.WriteLine("✅ AppointmentController yüklendi.");
            _context = context;
            _workingHourService = workingHourService;
            _notificationService = notificationService;
            _appointmentRepo = appointmentRepo;
        }

        /// <summary>
        /// Giriş yapmış kullanıcılar için randevu oluşturur.
        /// </summary>
        [Authorize]
        [HttpPost("registered")]
        public async Task<IActionResult> CreateAppointmentForRegistered([FromBody] CreateAppointmentDto dto)
        {
            Console.WriteLine("🔐 Registered user endpoint çağrıldı");

            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdStr, out var userId))
                return Unauthorized("Kullanıcı kimliği geçersiz.");

            var exists = await _context.AppUsers.AnyAsync(x => x.Id == userId);
            if (!exists)
            {
                var fullName = User.FindFirstValue(ClaimTypes.Name) ?? "Unknown";
                var email = User.FindFirstValue(ClaimTypes.Email);
                var phone = User.FindFirstValue("phone_number"); // varsa, Identity tarafında claim olarak ayarlanmalı

                var appUser = new AppUser
                {
                    Id = userId,
                    FullName = fullName,
                    Email = email,
                    PhoneNumber = phone,
                    Roles = new List<string> { "User" }
                };

                await _context.AppUsers.AddAsync(appUser);
                await _context.SaveChangesAsync();
            }

            bool isAvailable = await _appointmentService.IsSlotAvailableAsync(dto.BarberId, dto.StartTime, dto.ServiceType);
            if (!isAvailable)
                return BadRequest("Seçilen saatte berber meşgul.");

            if (await _appointmentService.UserHasAppointment(userId, dto.StartTime.Date))
            {
                return BadRequest("Zaten bir randevunuz var.");
            }

            if (dto.StartTime <= DateTime.Now)
            {
                return BadRequest("Geçmiş bir tarihe randevu alınamaz.");
            }

            await _appointmentService.CreateForRegisteredUserAsync(userId, dto);
            return Ok("Giriş yapmış kullanıcı için randevu başarıyla oluşturuldu.");
        }

        /// <summary>
        /// Giriş yapmamış kullanıcılar (guest) için randevu oluşturur.
        /// </summary>
        [AllowAnonymous]
        [HttpPost("guest")]
        public async Task<IActionResult> CreateAppointmentForGuest([FromBody] CreateAppointmentGuestDto dto)
        {
            Console.WriteLine("👤 Guest user endpoint çağrıldı");

            bool isAvailable = await _appointmentService.IsSlotAvailableAsync(dto.BarberId, dto.StartTime, dto.ServiceType);
            if (!isAvailable)
                return BadRequest("Seçilen saatte berber meşgul.");

            if (dto.StartTime <= DateTime.Now)
            {
                return BadRequest("Geçmiş bir tarihe randevu alınamaz.");
            }



            await _appointmentService.CreateForGuestAsync(dto);
            return Ok("Misafir kullanıcı için randevu başarıyla oluşturuldu.");
        }

        /// <summary>
        /// Belirtilen tarih ve berber için müsait saatleri döner.
        /// </summary>
        [HttpGet("availability")]
        public async Task<IActionResult> GetAvailability(
    [FromQuery] Guid barberId,
    [FromQuery] DateTime date,
    [FromQuery] ServiceType serviceType)
        {
            var result = await _appointmentService.GetAvailabilityAsync(barberId, date, serviceType);
            return Ok(result);
        }

        [HttpGet("available-slots")]
        public async Task<IActionResult> GetAvailableSlots(
     [FromQuery] Guid barberId,
     [FromQuery] ServiceType serviceType,
     [FromQuery] int days) // 1 = sadece bugün, 2 = bugün + yarın vs.
        {
            if (days <= 0 || days > 7)
                return BadRequest("Gün sayısı 1 ile 7 arasında olmalıdır.");

            var result = await _appointmentService.GetAvailabilityForRangeAsync(barberId, serviceType, days);
            return Ok(result);
        }

        [HttpPost("notify-when-available")]
        [Authorize]
        public async Task<IActionResult> NotifyWhenAvailable([FromBody] NotifyRequestDto dto)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            if (string.IsNullOrEmpty(email))
                return BadRequest("Kullanıcı e-posta bilgisi bulunamadı.");

            var entity = new NotificationRequest
            {
                UserId = userId,
                Email = email,
                BarberId = dto.BarberId,
                RequestedDate = dto.Date.Date,
                RequestedStart = dto.StartTime,
                RequestedEnd = dto.EndTime,
                ServiceType = dto.ServiceType
            };

            await _notificationRequestRepo.AddAsync(entity);
            await _notificationRequestRepo.SaveChangesAsync();

            return Ok("Uygun saat açıldığında size e-posta gönderilecek.");
        }

        [Authorize]
        [HttpDelete("{appointmentId}")]
        public async Task<IActionResult> CancelAppointment(Guid appointmentId)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdStr, out var userId))
                return Unauthorized("Kullanıcı kimliği geçersiz.");



            bool result = await _appointmentService.CancelAppointmentAsync(appointmentId, userId);
            if (!result)
                return NotFound("İlgili randevu bulunamadı veya size ait değil.");





            return Ok("Randevunuz iptal edildi.");
        }

        

        [HttpGet("working-hours")]
        public async Task<IActionResult> GetWorkingHours([FromQuery] Guid barberId)
        {
            var result = await _workingHourService.GetWorkingHoursAsync(barberId);
            return Ok(result);
        }

        [HttpGet("appointments")]
        public async Task<IActionResult> GetAppointmentsOfDay(Guid barberId, DateTime date)
        {
            var appointments = await _appointmentService.GetAppointmentsByBarberAndDate2(barberId, date.Date);
            return Ok(appointments);
        }

        [Authorize]
        [HttpGet("my-appointment")]
        public async Task<IActionResult> GetMyAppointment()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdStr, out var userId))
                return Unauthorized();

            var appt = await _appointmentRepo.GetLatestFutureAppointmentForUser(userId);
            if (appt == null)
                return NotFound();

            return Ok(appt);
        }

        [HttpGet("busyslots")]
        public async Task<IActionResult> GetBusySlotsByDate([FromQuery] Guid barberId, [FromQuery] DateTime date)
        {
            var slots = await _busySlotService.GetBusySlotsByDate(barberId, date);
            return Ok(slots);
        }


        [HttpGet("holiday")]
        public async Task<IActionResult> GetHolidays([FromQuery] Guid barberId)
        {
            var result = await _holidayService.GetHolidaysAsync(barberId);
            return Ok(result);
        }


    }
}
