using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MehmetHairDesigner.Server.Domain.Abstraction
{
    public interface IBaseEntity
    {
        Guid Id { get; set; }
        DateTimeOffset CreateAt { get; set; }
        DateTimeOffset? UpdateAt { get; set; }
        DateTimeOffset? DeleteAt { get; set; }
        bool IsDeleted { get; set; }
        bool IsActive { get; set; }
    }
}
