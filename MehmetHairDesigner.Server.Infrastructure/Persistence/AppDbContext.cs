using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using MehmetHairDesigner.Server.Infrastructure.Entities;
using MehmetHairDesigner.Server.Domain.Entities;
using System;

namespace MehmetHairDesigner.Server.Infrastructure.Persistence
{
    public class AppDbContext : IdentityDbContext<IdentityAppUser, IdentityRole<Guid>, Guid>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

          public DbSet<Appointment> Appointments { get; set; }

        // Berberler
        public DbSet<Barber> Barbers { get; set; }
        public DbSet<AppUser> AppUsers { get; set; }

        public DbSet<WorkingHour> WorkingHours { get; set; }

        public DbSet<BusySlot> BusySlots { get; set; }

        public DbSet<NotificationRequest> NotificationRequests { get; set; }

        public DbSet<Holiday> Holidays { get; set; }


    }
}