using FluentValidation;
using MehmetHairDesigner.Server.Application.Interfaces;
using MehmetHairDesigner.Server.Application.Services;
using MehmetHairDesigner.Server.Application.Validators.Auth;
using MehmetHairDesigner.Server.Infrastructure.Entities;
using MehmetHairDesigner.Server.Infrastructure.Persistence;
using MehmetHairDesigner.Server.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using System.Text;

namespace MehmetHairDesigner.Server.WebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // 💾 DbContext
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("HairDesignerConnectionString")));

            // 👤 Identity
            builder.Services.AddIdentity<IdentityAppUser, IdentityRole<Guid>>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

            // 🔐 JWT Authentication
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
.AddJwtBearer(options =>
{
    var base64Key = builder.Configuration["Jwt:Key"];
    var keyBytes = Convert.FromBase64String(base64Key);

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Jwt:Audience"],
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
        ClockSkew = TimeSpan.Zero,
        RequireExpirationTime = true,
        NameClaimType = ClaimTypes.NameIdentifier,
        RoleClaimType = ClaimTypes.Role
    };

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            if (!string.IsNullOrEmpty(token))
                context.Token = token;

            return Task.CompletedTask;
        },
        OnChallenge = context =>
        {
            context.HandleResponse();
            context.Response.StatusCode = 401;
            context.Response.ContentType = "application/json";
            return context.Response.WriteAsync("{\"error\":\"Yetkisiz erişim - geçerli token gönderilmedi.\"}");
        }
    };
});

            builder.Services.AddAuthorization();

            // ✅ CORS Policy
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", corsBuilder =>
                {
                    corsBuilder
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                });
            });

            // 🧩 DI / Validators / Services
            builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();
            builder.Services.AddScoped<AppointmentService>();
            builder.Services.AddScoped<ITokenService, TokenService>();
            builder.Services.AddValidatorsFromAssemblyContaining<RegisterDtoValidator>();
            builder.Services.AddControllers();
            builder.Services.AddRouting(options => options.LowercaseUrls = false);
            builder.Services.AddScoped<INotificationRequestRepository, NotificationRequestRepository>();
            builder.Services.AddScoped<IMailService, MailService>();
            builder.Services.AddScoped<IBusySlotRepository, BusySlotRepository>();
            builder.Services.AddScoped<IBusySlotService, BusySlotService>();
            builder.Services.AddScoped<IHolidayRepository, HolidayRepository>();
            builder.Services.AddScoped<IHolidayService, HolidayService>();  
            builder.Services.AddScoped<IWorkingHourService, WorkingHourService>();
            builder.Services.AddScoped<IWorkingHourRepository, WorkingHourRepository>();

            // 🧪 Swagger
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "MehmetHairDesigner API", Version = "v1" });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT kullanmak için 'Bearer {token}' formatında girin",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header
                        },
                        new List<string>()
                    }
                });
            });

            var app = builder.Build();

            // 🧪 Swagger UI
           

            // 🔄 CORS aktif hale getiriliyor
            app.UseCors("AllowAll");

            // 🔐 JWT Auth Middleware
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSwagger();
            app.UseSwaggerUI();

            // 🎯 Routing
            app.MapControllers();
            Console.WriteLine("📢 Controllers mapped!");

            // Seed admin user/roles
            using (var scope = app.Services.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityAppUser>>();
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

                if (!roleManager.RoleExistsAsync("Customer").GetAwaiter().GetResult())
                {
                    roleManager.CreateAsync(new IdentityRole<Guid>("Customer")).GetAwaiter().GetResult();
                    Console.WriteLine("✅ 'Customer' rolü başarıyla oluşturuldu.");
                }

                AppDbContextSeed.SeedAsync(userManager, roleManager).GetAwaiter().GetResult();
            }

            app.Run();
        }
    }
}