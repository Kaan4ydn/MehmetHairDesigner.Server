using MehmetHairDesigner.Server.Application.DTOs;
using MehmetHairDesigner.Server.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MehmetHairDesigner.Server.Application.Services
{
    public class AppUserService : IAppUserService
    {
        private readonly IAppUserRepository _repo;

        public AppUserService(IAppUserRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<AppUserDto>> SearchUsersAsync(string keyword)
        {
            var users = await _repo.SearchUsersAsync(keyword);

            return users.Select(u => new AppUserDto
            {
                Id = u.Id,
                FullName = u.FullName,
                PhoneNumber = u.PhoneNumber,
                Email = u.Email
            }).ToList();
        }
    }
}
