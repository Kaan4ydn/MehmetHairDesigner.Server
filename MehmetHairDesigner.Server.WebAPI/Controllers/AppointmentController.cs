using Microsoft.AspNetCore.Mvc;
using MehmetHairDesigner.Server.Application.DTOs;
using MehmetHairDesigner.Server.Application.Services;
using MehmetHairDesigner.Server.Domain.Entities;
using System.Security.Claims;

namespace MehmetHairDesigner.Server.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AppointmentController : ControllerBase
    {
        private readonly AppointmentService _appointmentService;

        public AppointmentController(AppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

        // ✅ Randevu alma
        [HttpPost]
        public async Task<IActionResult> CreateAppointment([FromBody] CreateAppointmentDto dto)
        {
            var availability = await _appointmentService.GetAvailabilityAsync(dto.BarberId, dto.StartTime.Date);

            var isBusy = availability.Any(slot =>
                slot.Time == dto.StartTime && slot.IsAvailable == false);

            if (isBusy)
                return BadRequest("Seçilen saatte randevu alınamaz. Berber meşgul.");

            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Guid.TryParse(userIdStr, out var userId);

            var appointment = new Appointment
            {
                BarberId = dto.BarberId,
                StartTime = dto.StartTime,
                ServiceType = dto.ServiceType,
                UserId = userId,
                Notes = dto.GuestFullName + " - " + dto.GuestPhoneNumber
            };

            await _appointmentService.CreateAppointmentAsync(appointment);
            return Ok("Randevu başarıyla oluşturuldu.");
        }

        // ✅ Müsaitlik
        [HttpGet("availability")]
        public async Task<IActionResult> GetAvailability([FromQuery] Guid barberId, [FromQuery] DateTime date)
        {
            var result = await _appointmentService.GetAvailabilityAsync(barberId, date);
            return Ok(result);
        }
    }
}
