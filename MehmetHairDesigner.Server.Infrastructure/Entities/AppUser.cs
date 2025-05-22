using Microsoft.AspNetCore.Identity;
using System;

namespace MehmetHairDesigner.Server.Domain.Entities
{
    public class AppUser : IdentityUser<Guid>
    {
        public string FullName { get; set; }
    }
}
