using MehmetHairDesigner.Server.Domain.Entities ;

namespace MehmetHairDesigner.Server.Application.Services
{
    public interface ITokenService
    {
        string CreateToken(AppUser user);
    }
}