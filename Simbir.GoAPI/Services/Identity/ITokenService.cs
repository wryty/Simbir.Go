using Microsoft.AspNetCore.Identity;
using Simbir.GoAPI.Data.Entities;

namespace Simbir.GoAPI.Services.Identity;


public interface ITokenService
{
    string CreateToken(ApplicationUser user, List<IdentityRole<long>> role);
}