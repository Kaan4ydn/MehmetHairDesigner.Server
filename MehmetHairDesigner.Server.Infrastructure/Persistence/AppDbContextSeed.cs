using Microsoft.AspNetCore.Identity;
using System;
using System.Threading.Tasks;
using MehmetHairDesigner.Server.Infrastructure.Entities;
public static class AppDbContextSeed
{
    public static async Task SeedAsync(UserManager<IdentityAppUser> userManager, RoleManager<IdentityRole<Guid>> roleManager)
    {
        var adminEmail = "admin@admin.com";
        var adminRole = "Admin";

        if (!await roleManager.RoleExistsAsync(adminRole))
        {
            await roleManager.CreateAsync(new IdentityRole<Guid>(adminRole));
        }

        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            adminUser = new IdentityAppUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                FullName = "Admin User"
            };
            await userManager.CreateAsync(adminUser, "Admin123!"); // 🔐 Gerçek projede güçlü parola
            await userManager.AddToRoleAsync(adminUser, adminRole);
        }
    }
}
