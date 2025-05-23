namespace MehmetHairDesigner.Server.Domain.Entities
{
    public class AppUser
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }

        public List<string> Roles { get; set; } = new();
    }
}