using MehmetHairDesigner.Server.Application.DTOs;
using MehmetHairDesigner.Server.Application.Interfaces;
using MehmetHairDesigner.Server.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MehmetHairDesigner.Server.Application.Services
{
    public class HolidayService : IHolidayService
    {
        private readonly IHolidayRepository _repo;

        public HolidayService(IHolidayRepository repo)
        {
            _repo = repo;
        }

        public async Task AddHolidayAsync(AddHolidayDto dto)
        {
            var entity = new Holiday
            {
                Id = Guid.NewGuid(),
                BarberId = dto.BarberId,
                Date = dto.Date.Date,
                Reason = dto.Reason
            };

            await _repo.AddAsync(entity);
            await _repo.SaveChangesAsync();
        }

        public async Task DeleteHolidayAsync(Guid id)
        {
            await _repo.DeleteAsync(id);
            await _repo.SaveChangesAsync();
        }

        public async Task<List<Holiday>> GetHolidaysAsync(Guid barberId)
        {
            return await _repo.GetByBarberAsync(barberId);
        }
    }
}
