using MehmetHairDesigner.Server.Application.DTOs;
using MehmetHairDesigner.Server.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MehmetHairDesigner.Server.Application.Interfaces;
using MehmetHairDesigner.Server.Application.Interfaces.UnitOfWorks;
using MehmetHairDesigner.Server.Application.Interfaces.Repositories;


namespace MehmetHairDesigner.Server.Application.Services
{
    public class WorkingHourService : IWorkingHourService
    {
        private readonly IWorkingHourRepository _repo;
        private readonly IReadRepository<WorkingHour> _readRepository;
        private readonly IWriteRepository<WorkingHour> _writeRepository;

        public WorkingHourService(IWorkingHourRepository repo, IReadRepository<WorkingHour> readRepository, IWriteRepository<WorkingHour> writeRepository)
        {
            _repo = repo;
            _readRepository = readRepository;
            _writeRepository = writeRepository;
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

        public async Task UpdateWorkingHourAsync(UpdateWorkingHourDto updateWorkingHourDto)
        {
            foreach (var dayDto in updateWorkingHourDto.Days)
            {
                var workingHour = await _readRepository
                    .GetByExpressionWithTrackingAsync(x =>
                        x.BarberId == updateWorkingHourDto.BarberId &&
                        x.Day == dayDto.Day);

                if (workingHour == null)
                    continue;

                workingHour.Start = dayDto.Start;
                workingHour.End = dayDto.End;

                _writeRepository.Update(workingHour);
            }

            await _repo.SaveChangesAsync();
        }


        public async Task DeleteAllWorkingHoursAsync(Guid barberId)
        {
            var workingHours = _readRepository
                .WhereWithTracking(x => x.BarberId == barberId)
                .ToList();

            if (workingHours.Any())
            {
                _writeRepository.DeleteRange(workingHours);
                await _repo.SaveChangesAsync();
            }
        }
    }
}
