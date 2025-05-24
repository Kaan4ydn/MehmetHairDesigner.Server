using MehmetHairDesigner.Server.Application.DTOs;
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


        public AdminController(IBusySlotService busySlotService, IWorkingHourService workingHourService)
        {
            _busySlotService = busySlotService;
            _workingHourService = workingHourService;
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
        [HttpGet("busyslots")]
        public async Task<IActionResult> GetBusySlotsByDate([FromQuery] Guid barberId, [FromQuery] DateTime date)
        {
            var slots = await _busySlotService.GetBusySlotsByDate(barberId, date);
            return Ok(slots);
        }



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


        [HttpGet("dashboard")]
        public IActionResult Dashboard()
        {
            return Ok("Sadece admin erişebilir.");
        }
    }
}