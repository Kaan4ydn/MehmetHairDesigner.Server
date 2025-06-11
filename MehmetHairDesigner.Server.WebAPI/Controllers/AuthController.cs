using Google.Apis.Auth;
using MehmetHairDesigner.Server.Application.DTOs;
using MehmetHairDesigner.Server.Application.Services;
using MehmetHairDesigner.Server.Domain.Entities;
using MehmetHairDesigner.Server.Infrastructure.Entities;
using MehmetHairDesigner.Server.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Numerics;
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
        private readonly AppDbContext _context;

        public AuthController(UserManager<IdentityAppUser> userManager,
                              SignInManager<IdentityAppUser> signInManager,
                              ITokenService tokenService, AppDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _context = context;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                return BadRequest(errors);
            }

            var user = new IdentityAppUser
            {
                FullName = dto.FullName,
                UserName = dto.Email,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,


            };

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            await _userManager.AddToRoleAsync(user, dto.Role);


            return Ok("Kayıt başarılı.");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return Unauthorized("Kullanıcı bulunamadı.");

            var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, false);
            if (!result.Succeeded)
                return Unauthorized("Hatalı şifre.");

            var roles = await _userManager.GetRolesAsync(user);

            var token = _tokenService.CreateToken(user.ToDomainUser(roles.ToList()));

            return Ok(new { token });
        }


        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("userinfo")]
        public IActionResult GetCurrentUserInfo()
        {
            var id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var name = User.Identity?.Name;
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var roles = User.FindAll(ClaimTypes.Role).Select(r => r.Value).ToList();

            return Ok(new
            {
                Id = id,
                FullName = name,
                Email = email,
                Roles = roles
            });
        }

        [HttpPost("google-login")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginDto dto)
        {
            try
            {
                var payload = await GoogleJsonWebSignature.ValidateAsync(dto.IdToken);
                var email = payload.Email;

                var user = await _userManager.FindByEmailAsync(email);
                bool isNewUser = false;

                if (user == null)
                {
                    user = new IdentityAppUser
                    {
                        Email = email,
                        UserName = email,
                        FullName = payload.Name,
                        PhoneNumber = "" // Google'dan gelmez, frontend'den sonra alınacak
                    };

                    var result = await _userManager.CreateAsync(user);
                    if (!result.Succeeded)
                        return BadRequest(result.Errors);

                    await _userManager.AddToRoleAsync(user, "Customer");
                    isNewUser = true;
                }

                var roles = await _userManager.GetRolesAsync(user);
                var token = _tokenService.CreateToken(user.ToDomainUser(roles.ToList()));

                return Ok(new
                {
                    token,
                    isNewUser,
                    PhoneNumberRequired = string.IsNullOrWhiteSpace(user.PhoneNumber)
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    error = "Google doğrulama başarısız",
                    detail = ex.Message
                });
            }
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("add-phone")]
        public async Task<IActionResult> AddPhone([FromBody] AddPhoneDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized("Kullanıcı kimliği bulunamadı.");

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return NotFound("Kullanıcı bulunamadı.");

            user.PhoneNumber = dto.PhoneNumber;

            var appUser = new AppUser
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Roles = new List<string> { "User" }
            };

            await _context.AppUsers.AddAsync(appUser);
            await _context.SaveChangesAsync();





            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return BadRequest(result.Errors);



            return Ok("Telefon numarası başarıyla güncellendi.");
        }

    }
}
