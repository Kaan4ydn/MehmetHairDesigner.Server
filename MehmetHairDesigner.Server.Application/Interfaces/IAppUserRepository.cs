using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MehmetHairDesigner.Server.Application.Interfaces
{
    using MehmetHairDesigner.Server.Domain.Entities;

    public interface IAppUserRepository
    {
        Task<List<AppUser>> SearchUsersAsync(string keyword);
    }
}
