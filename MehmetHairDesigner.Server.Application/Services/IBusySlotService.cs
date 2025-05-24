using MehmetHairDesigner.Server.Application.DTOs;
using MehmetHairDesigner.Server.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MehmetHairDesigner.Server.Application.Services
{
    public interface IBusySlotService
    {
        Task AddBusySlotAsync(CreateBusySlotDto dto);
        Task DeleteBusySlotAsync(Guid id);
        Task<List<BusySlot>> GetBusySlotsByDate(Guid barberId, DateTime date);
    }
}
