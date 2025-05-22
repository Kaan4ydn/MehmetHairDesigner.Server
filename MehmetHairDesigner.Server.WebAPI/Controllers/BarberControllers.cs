using Microsoft.AspNetCore.Mvc;
using MehmetHairDesigner.Server.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using MehmetHairDesigner.Server.Infrastructure.Persistence;

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
        public async Task<IActionResult> GetAll()
        {
            var barbers = await _context.Barbers.ToListAsync();
            return Ok(barbers);
        }

        // ✅ Yeni berber oluştur (ileride admin koruması eklenecek)
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Barber barber)
        {
            _context.Barbers.Add(barber);
            await _context.SaveChangesAsync();
            return Ok(barber);
        }
    }
}