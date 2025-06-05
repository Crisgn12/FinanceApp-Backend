using Backend.DTO;
using Microsoft.AspNetCore.Identity;

namespace Backend.Services.Interfaces
{
    public interface ITokenService
    {

        TokenDTO GenerateToken(IdentityUser user, List<string> roles);

    }
}