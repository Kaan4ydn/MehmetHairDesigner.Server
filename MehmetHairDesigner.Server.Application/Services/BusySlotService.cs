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
    public class BusySlotService : IBusySlotService
    {
        private readonly IBusySlotRepository _repo;

        public BusySlotService(IBusySlotRepository repo)
        {
            _repo = repo;
        }

        public async Task AddBusySlotAsync(CreateBusySlotDto dto)
        {
            var slot = new BusySlot
            {
                Id = Guid.NewGuid(),
                BarberId = dto.BarberId,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                Reason = dto.Reason
            };

            await _repo.AddAsync(slot);
            await _repo.SaveChangesAsync();
        }

        public async Task DeleteBusySlotAsync(Guid id)
        {
            await _repo.DeleteAsync(id);
            await _repo.SaveChangesAsync();
        }

        public async Task<List<BusySlot>> GetBusySlotsByDate(Guid barberId, DateTime date)
        {
            return await _repo.GetByDateAsync(barberId, date);
        }
    }
}
