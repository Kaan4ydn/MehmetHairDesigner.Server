using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using MehmetHairDesigner.Server.Infrastructure.Persistence;
using MehmetHairDesigner.Server.Application.Services;
using MehmetHairDesigner.Server.Infrastructure.Entities;
using MehmetHairDesigner.Server.Application.Validators.Auth;
using FluentValidation;
using MehmetHairDesigner.Server.Application.Interfaces;
using MehmetHairDesigner.Server.Infrastructure.Repositories;

namespace MehmetHairDesigner.Server.WebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("HairDesignerConnectionString")));

            builder.Services.AddIdentity<IdentityAppUser, IdentityRole<Guid>>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

           builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var key = builder.Configuration["Jwt:Key"];

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
            ClockSkew = TimeSpan.Zero,
            RequireExpirationTime = true
        };

        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine($"JWT AUTH ERROR: {context.Exception.Message}");
                return Task.CompletedTask;
            },

            OnTokenValidated = context =>
    {
        Console.WriteLine($"✅ Token doğrulandı: {context.SecurityToken}");
        return Task.CompletedTask;
    }
        };
    });
            builder.Services.AddAuthorization();
            builder.Services.AddControllers();
            builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();
builder.Services.AddScoped<AppointmentService>();
            
            builder.Services.AddValidatorsFromAssemblyContaining<RegisterDtoValidator>();

            builder.Services.AddScoped<ITokenService, TokenService>();

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

            // 🛠 Swagger UI
            
                app.UseSwagger();
                app.UseSwaggerUI();
            

            // JWT middleware'leri
            app.UseAuthentication(); // 🔑 önce authentication
            app.UseAuthorization();  // 🔐 sonra authorization

            app.MapControllers();
            Console.WriteLine("📢 Controllers mapped!");

          using (var scope = app.Services.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityAppUser>>();
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

                // await yerine:
                AppDbContextSeed.SeedAsync(userManager, roleManager).GetAwaiter().GetResult();
            }


            app.Run();
        }
    }
}