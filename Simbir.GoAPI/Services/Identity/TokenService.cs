using Microsoft.AspNetCore.Identity;
using Simbir.GoAPI.Web.Extensions;
using Simbir.GoAPI.Data.Entities;
using System.IdentityModel.Tokens.Jwt;

namespace Simbir.GoAPI.Services.Identity;

public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;

    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string CreateToken(ApplicationUser user, List<IdentityRole<long>> roles)
    {
        var token = user
            .CreateClaims(roles)
            .CreateJwtToken(_configuration);
        var tokenHandler = new JwtSecurityTokenHandler();

        return tokenHandler.WriteToken(token);
    }
}