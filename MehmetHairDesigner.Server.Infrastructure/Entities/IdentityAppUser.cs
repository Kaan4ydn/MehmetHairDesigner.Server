using Microsoft.AspNetCore.Identity;
using MehmetHairDesigner.Server.Domain.Entities;

namespace MehmetHairDesigner.Server.Infrastructure.Entities
{
    public class IdentityAppUser : IdentityUser<Guid>
    {
        public string FullName { get; set; }

        public AppUser ToDomainUser(List<string>? roles = null)
{
    return new AppUser
    {
        Id = Guid.Parse(Id.ToString()),
        FullName = FullName,
        Email = Email,
        Roles = roles ?? new List<string>()
    };
}
    }
}
