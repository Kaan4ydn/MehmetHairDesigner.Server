using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MehmetHairDesigner.Server.Application.DTOs
{
    public class AppUserDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string? Email { get; set; }
        public string PhoneNumber { get; set; }
    }
}
