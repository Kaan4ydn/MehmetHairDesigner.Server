using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MehmetHairDesigner.Server.Application.DTOs;
using MehmetHairDesigner.Server.Application.Services;
using MehmetHairDesigner.Server.Infrastructure.Entities;
using System.Security.Claims;

namespace MehmetHairDesigner.Server.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityAppUser> _userManager;
        private readonly SignInManager<IdentityAppUser> _signInManager;
        private readonly ITokenService _tokenService;

        public AuthController(UserManager<IdentityAppUser> userManager,
                              SignInManager<IdentityAppUser> signInManager,
                              ITokenService tokenService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
        }

        // ✅ Kayıt olma
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            var user = new IdentityAppUser
            {
                FullName = dto.FullName,
                UserName = dto.Email,
                Email = dto.Email
            };

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok("Kayıt başarılı.");
        }

        // ✅ Giriş yapma ve token alma
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return Unauthorized("Kullanıcı bulunamadı.");

            var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, false);

            if (!result.Succeeded)
                return Unauthorized("Hatalı şifre.");

            var token = _tokenService.CreateToken(user.ToDomainUser());

            return Ok(new { token });
        }

        // ✅ Token ile kullanıcı bilgisi alma
        [Authorize]
        [HttpGet("userinfo")]
        public IActionResult GetCurrentUserInfo()
        {
            var id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var name = User.Identity?.Name;
            var email = User.FindFirst(ClaimTypes.Email)?.Value;

            return Ok(new
            {
                Id = id,
                FullName = name,
                Email = email
            });
        }
    }
}
