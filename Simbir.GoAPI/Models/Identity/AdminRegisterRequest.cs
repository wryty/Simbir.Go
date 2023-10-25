using System.ComponentModel.DataAnnotations;
namespace Simbir.GoAPI.Models.Identity;

public class AdminRegisterRequest
{
    [Required]
    [Display(Name = "UserName")]
    public string UserName { get; set; } = null!;

    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Пароль")]
    public string Password { get; set; } = null!;

    [Required]
    [Display(Name = "isAdmin")]
    public bool IsAdmin { get; set; } = false!;

    [Required]
    [Display(Name = "Balance")]
    public double Balance { get; set; } = 0.0!;
}