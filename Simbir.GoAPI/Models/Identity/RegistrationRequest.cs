using System.ComponentModel.DataAnnotations;

namespace Simbir.GoAPI.Models.Identity;

public class RegistrationRequest
{

    [Required]
    public string UserName { get; set; } = null!;

    [Required]
    public string Password { get; set; } = null!;
}