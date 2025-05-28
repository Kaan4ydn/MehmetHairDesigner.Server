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

        // ✅ Tüm berberleri getir
        [HttpGet]
        [Route("get-barber")]
        public async Task<IActionResult> GetAll()
        {
            var barbers = await _context.Barbers.ToListAsync();
            return Ok(barbers);
        }

        // ✅ Yeni berber oluştur (ileride admin koruması eklenecek)
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
    }
}