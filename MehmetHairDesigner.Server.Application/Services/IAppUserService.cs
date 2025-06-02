using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MehmetHairDesigner.Server.Application.DTOs;

public interface IAppUserService
{
    Task<List<AppUserDto>> SearchUsersAsync(string keyword);
}
