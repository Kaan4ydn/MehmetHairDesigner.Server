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
        private readonly AppointmentService _appointmentService;
        private readonly INotificationRequestRepository _notificationRequestRepo;
        private readonly AppDbContext _context;


        public AppointmentController(
    AppointmentService appointmentService,
    INotificationRequestRepository notificationRequestRepo,
    AppDbContext context    )
        {
            _appointmentService = appointmentService;
            _notificationRequestRepo = notificationRequestRepo;

            Console.WriteLine("✅ AppointmentController yüklendi.");
            _context = context;
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
                return BadRequest("Aynı gün içerisinde zaten bir randevunuz var.");
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
        public async Task<IActionResult> NotifyWhenAvailable([FromBody] NotifyRequestDto dto)
        {
            var entity = new NotificationRequest
            {
                UserId = User.Identity?.IsAuthenticated == true
                    ? Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier))
                    : null,
                PhoneNumber = dto.PhoneNumber,
                RequestedDate = dto.Date.Date,
                ServiceType = dto.ServiceType
            };

            await _notificationRequestRepo.AddAsync(entity);
            await _notificationRequestRepo.SaveChangesAsync();

            return Ok("Uygun saat açıldığında size haber verilecek.");
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

        [AllowAnonymous]
        [HttpDelete("cancel-guest")]
        public async Task<IActionResult> CancelGuestAppointment([FromBody] CancelGuestAppointmentDto dto)
        {
            bool result = await _appointmentService.CancelGuestAppointmentAsync(
                dto.FullName, dto.PhoneNumber);

            if (!result)
                return NotFound("Eşleşen randevu bulunamadı.");

            return Ok("Randevunuz iptal edildi.");
        }

    }
}
