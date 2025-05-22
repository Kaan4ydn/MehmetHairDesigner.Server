using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MehmetHairDesigner.Server.Infrastructure.Persistence;
using MehmetHairDesigner.Server.Domain.Entities;


namespace MehmetHairDesigner.Server.WebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // 💾 In-Memory veritabanı ekleniyor (geçici olarak verileri bellekte tutar)
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseInMemoryDatabase("HairDesignerDb"));

            // 🔐 Identity servisleri ekleniyor (Kullanıcı işlemleri için)
            builder.Services.AddIdentity<AppUser, IdentityRole<Guid>>(options =>
            {
                // Şifre kurallarını sadeleştiriyoruz (test süreci için)
                options.Password.RequireDigit = false;               // Rakam zorunluluğu yok
                options.Password.RequireNonAlphanumeric = false;     // Özel karakter gerekmez
                options.Password.RequireUppercase = false;           // Büyük harf gerekmez
                options.Password.RequiredLength = 6;                 // Minimum 6 karakter
            })
            .AddEntityFrameworkStores<AppDbContext>()               // Verileri EF Core ile sakla
            .AddDefaultTokenProviders();                            // Doğrulama token'ı üretmeyi sağlar (e-posta doğrulama vb.)

            // 🌐 API controller'ları ekleniyor
            builder.Services.AddControllers();

            // 🔍 Swagger/OpenAPI servisi
            builder.Services.AddOpenApi();

            var app = builder.Build();

            // 🛠 Geliştirme ortamında Swagger UI açılıyor
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            // 🔐 [Authorize] kullanılan endpoint’lerin kimlik doğrulamasını etkinleştirir
            app.UseAuthorization();

            // 🎯 Controller'lara gelen istekleri yönlendir
            app.MapControllers();

            app.Run();
        }
    }
}