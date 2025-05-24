using MehmetHairDesigner.Server.Application.DTOs;
using MehmetHairDesigner.Server.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MehmetHairDesigner.Server.Application.Services;


namespace MehmetHairDesigner.Server.Application.Services
{
    public class WorkingHourService : IWorkingHourService
    {
        private readonly IWorkingHourRepository _repo;

        public WorkingHourService(IWorkingHourRepository repo)
        {
            _repo = repo;
        }

        public async Task SetWorkingHoursAsync(SetWorkingHoursDto dto)
        {
            var list = dto.Days.Select(d => new WorkingHour
            {
                Id = Guid.NewGuid(),
                BarberId = dto.BarberId,
                Day = d.Day,
                Start = d.Start,
                End = d.End
            }).ToList();

            await _repo.ReplaceAllAsync(dto.BarberId, list);
            await _repo.SaveChangesAsync();
        }

        public async Task<List<WorkingHour>> GetWorkingHoursAsync(Guid barberId)
        {
            return await _repo.GetByBarberAsync(barberId);
        }
    }
}
