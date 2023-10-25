using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Simbir.GoAPI.Data.Entities;
using Simbir.GoAPI.Data;
using Simbir.GoAPI.Services.Identity;
using Microsoft.AspNetCore.Authorization;
using System.Data;

namespace Simbir.GoAPI.Controllers;

[ApiController]
[Route("/api/Payment")]
public class PaymentController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly DataContext _context;
    private readonly ITokenService _tokenService;
    private readonly IConfiguration _configuration;

    public PaymentController(ITokenService tokenService, DataContext context, UserManager<ApplicationUser> userManager, IConfiguration configuration)
    {
        _tokenService = tokenService;
        _context = context;
        _userManager = userManager;
        _configuration = configuration;
    }

    [Authorize]
    [HttpPost("Hesoyam/{accountId}")]
    public async Task<IActionResult> AddBalance(long accountId)
    {
        var user = await _userManager.FindByIdAsync(accountId.ToString());

        if (user == null)
        {
            return NotFound("User not found");
        }

        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser.Id != accountId && !User.IsInRole("Administrator"))
        {
            return Forbid("You don't have permission to add balance to this account");
        }

        user.Balance += 250000;

        var result = await _userManager.UpdateAsync(user);
        await _context.SaveChangesAsync();

        if (result.Succeeded)
        {
            return Ok($"Successfully added 250000 to the balance of the user with id {accountId}");
        }
        else
        {
            return BadRequest("Failed to add balance");
        }
    }
}