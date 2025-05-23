using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MehmetHairDesigner.Server.WebAPI.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        [HttpGet("dashboard")]
        public IActionResult Dashboard()
        {
            return Ok("Sadece admin erişebilir.");
        }
    }
}