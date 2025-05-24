using MehmetHairDesigner.Server.Domain.Abstraction;

namespace MehmetHairDesigner.Server.Domain.Entities
{
    public class AppUser : BaseEntity
    {
        public string FullName { get; set; }
        public string? Email { get; set; } // guest için null olabilir
        public string PhoneNumber { get; set; }

        public List<string> Roles { get; set; } = new();
    }
}