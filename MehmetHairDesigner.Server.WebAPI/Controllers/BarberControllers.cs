using Microsoft.AspNetCore.Mvc;
using MehmetHairDesigner.Server.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using MehmetHairDesigner.Server.Infrastructure.Persistence;
using MehmetHairDesigner.Server.Application.DTOs;

namespace MehmetHairDesigner.Server.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BarberController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BarberController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("get-barber")]
        public async Task<IActionResult> GetAll()
        {
            var barbers = await _context.Barbers.ToListAsync();
            return Ok(barbers);
        }

        [HttpPost]
        [Route("post-barber")]
        public async Task<IActionResult> Create([FromBody] BarberCreateDto dto)
        {
            var barber = new Barber
            {
                Id = Guid.NewGuid(),
                FullName = dto.FullName,
                Appointments = new List<Appointment>() // boş başlat
            };

            _context.Barbers.Add(barber);
            await _context.SaveChangesAsync();

            return Ok(barber);
        }

        [HttpPut]
        [Route("update-barber/{barberId}")]
        public async Task<IActionResult> Update(Guid barberId, [FromBody] BarberCreateDto dto)
        {
            var barber = await _context.Barbers.FindAsync(barberId);

            if (barber == null)
                return NotFound("Berber bulunamadı.");

            barber.FullName = dto.FullName;

            _context.Barbers.Update(barber);
            await _context.SaveChangesAsync();

            return Ok(barber);
        }

        [HttpDelete]
        [Route("delete-barber/{barberId}")]
        public async Task<IActionResult> Delete(Guid barberId)
        {
            var barber = await _context.Barbers.FirstOrDefaultAsync(b => b.Id == barberId);

            if (barber == null)
                return NotFound("Berber bulunamadı.");

            _context.Barbers.Remove(barber);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}