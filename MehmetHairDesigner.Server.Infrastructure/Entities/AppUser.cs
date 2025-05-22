using Microsoft.AspNetCore.Identity;
using System;

namespace MehmetHairDesigner.Server.Infrastructure.Entities
{
    public class AppUser : IdentityUser<Guid>
    {
        public string FullName { get; set; }
    }
}
