using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using MehmetHairDesigner.Server.Infrastructure.Entities;
using System;

namespace MehmetHairDesigner.Server.Infrastructure.Persistence
{
    public class AppDbContext : IdentityDbContext<IdentityAppUser, IdentityRole<Guid>, Guid>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // İleride DbSet<Product> gibi tablolar buraya eklenir.
    }
}