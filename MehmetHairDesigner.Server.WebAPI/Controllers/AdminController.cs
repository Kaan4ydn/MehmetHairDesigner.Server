using MehmetHairDesigner.Server.Application.DTOs;
using MehmetHairDesigner.Server.Application.Interfaces;
using MehmetHairDesigner.Server.Application.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace MehmetHairDesigner.Server.WebAPI.Controllers
{
    [Authorize(Roles = "Admin", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly IBusySlotService _busySlotService;
        private readonly IWorkingHourService _workingHourService;
        private readonly IHolidayService _holidayService;
        private readonly IAppointmentRepository _appointmentRepo;
        private readonly IMailService _mailService;


        public AdminController(IBusySlotService busySlotService, IWorkingHourService workingHourService, IHolidayService holidayService, IAppointmentRepository appointmentRepo, IMailService mailService)
        {
            _busySlotService = busySlotService;
            _workingHourService = workingHourService;
            _holidayService = holidayService;
            _appointmentRepo = appointmentRepo;
            _mailService = mailService;
        }

        #region BusySlots

        /// <summary>
        /// Admin takvimde meşgul saat oluşturur.
        /// </summary>
        [HttpPost("busyslot")]
        public async Task<IActionResult> CreateBusySlot([FromBody] CreateBusySlotDto dto)
        {
            await _busySlotService.AddBusySlotAsync(dto);
            return Ok("Saat meşgul olarak işaretlendi.");
        }

        /// <summary>
        /// Admin takvimdeki meşgul saati siler.
        /// </summary>
        [HttpDelete("busyslot/{id}")]
        public async Task<IActionResult> DeleteBusySlot(Guid id)
        {
            await _busySlotService.DeleteBusySlotAsync(id);
            return Ok("Meşgul saat kaldırıldı.");
        }

        /// <summary>
        /// Belirli berbere ait günün meşgul saatlerini getirir.
        /// </summary>
       



        #endregion

        #region WorkingHours

        [HttpPost("working-hours")]
        public async Task<IActionResult> SetWorkingHours([FromBody] SetWorkingHoursDto dto)
        {
            await _workingHourService.SetWorkingHoursAsync(dto);
            return Ok("Çalışma saatleri güncellendi.");
        }

        [HttpGet("working-hours")]
        public async Task<IActionResult> GetWorkingHours([FromQuery] Guid barberId)
        {
            var result = await _workingHourService.GetWorkingHoursAsync(barberId);
            return Ok(result);
        }

        #endregion

        #region Holiday

        [HttpPost("holiday")]
        public async Task<IActionResult> AddHoliday([FromBody] AddHolidayDto dto)
        {
            await _holidayService.AddHolidayAsync(dto);
            return Ok("Tatil günü eklendi.");
        }

        [HttpDelete("holiday/{id}")]
        public async Task<IActionResult> DeleteHoliday(Guid id)
        {
            await _holidayService.DeleteHolidayAsync(id);
            return Ok("Tatil günü silindi.");
        }

        [HttpPut("appointment/{id}/approve")]
        public async Task<IActionResult> ApproveAppointment(Guid id)
        {
            var appointment = await _appointmentRepo.GetByIdAsync(id);
            if (appointment == null)
                return NotFound("Randevu bulunamadı.");

            appointment.Status = "booked";
            await _appointmentRepo.SaveChangesAsync();

            await _mailService.SendAsync(
                appointment.User.Email,
                "Randevunuz Onaylandı",
                $"📅 Randevunuz {appointment.StartTime:dd MMMM HH:mm} tarihinde onaylandı. Görüşmek üzere!");

            return Ok("Randevu onaylandı.");
        }

        [HttpPut("appointment/{id}/reject")]
        public async Task<IActionResult> RejectAppointment(Guid id)
        {
            var appointment = await _appointmentRepo.GetByIdAsync(id);
            if (appointment == null)
                return NotFound("Randevu bulunamadı.");

            string email = appointment.User.Email;

            _appointmentRepo.Delete(appointment);
            await _appointmentRepo.SaveChangesAsync();

            await _mailService.SendAsync(
                email,
                "Randevunuz Reddedildi",
                $"❌ Üzgünüz, {appointment.StartTime:dd MMMM HH:mm} tarihli randevunuz reddedildi. Yeni bir zaman seçebilirsiniz.");

            return Ok("Randevu reddedildi ve kullanıcıya bilgilendirme gönderildi.");
        }

        [HttpGet("pending")]
        public async Task<IActionResult> GetPendingAppointments()
        {
            var pending = await _appointmentRepo.GetPendingAppointmentsAsync();

            var result = pending.Select(a => new AppointmentDto
            {
                Id = a.Id,
                StartTime = a.StartTime,
                Status = a.Status,
                BarberName = a.Barber.FullName,
                UserName = a.User.FullName
            }).ToList();

            return Ok(result);
        }


        #endregion


        [HttpGet("dashboard")]
        public IActionResult Dashboard()
        {
            return Ok("Sadece admin erişebilir.");
        }
    }
}