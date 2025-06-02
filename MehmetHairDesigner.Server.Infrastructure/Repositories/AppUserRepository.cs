using MehmetHairDesigner.Server.Application.Interfaces;
using MehmetHairDesigner.Server.Domain.Entities;
using MehmetHairDesigner.Server.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

public class AppUserRepository : IAppUserRepository
{
    private readonly AppDbContext _context;

    public AppUserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<AppUser>> SearchUsersAsync(string keyword)
    {
        return await _context.AppUsers
            .Where(u => u.FullName.Contains(keyword))
            .OrderBy(u => u.FullName)
            .Take(10)
            .ToListAsync();
    }
}