using MehmetHairDesigner.Server.Application.DTOs;
using MehmetHairDesigner.Server.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MehmetHairDesigner.Server.Application.Services
{
    public interface IHolidayService
    {
        Task AddHolidayAsync(AddHolidayDto dto);
        Task DeleteHolidayAsync(Guid id);
        Task<List<Holiday>> GetHolidaysAsync(Guid barberId);
    }
}
