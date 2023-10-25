using Microsoft.AspNetCore.Identity;

namespace Simbir.GoAPI.Data.Entities;

public class ApplicationUser : IdentityUser<long>
{
    public double Balance { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime RefreshTokenExpiryTime { get; set; }
}
