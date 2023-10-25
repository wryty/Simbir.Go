using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Simbir.GoAPI.Data.Entities;
using Simbir.GoAPI.Data;
using Simbir.GoAPI.Models.Identity;
using Simbir.GoAPI.Services.Identity;
using System.Data;
using Simbir.GoAPI.Models;

namespace Simbir.GoAPI.Controllers;



[ApiController]
[Route("/api/Admin")]
[Authorize(AuthenticationSchemes = "Bearer", Roles = RoleConsts.Administrator)]
public class AdminRentController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly DataContext _context;
    private readonly ITokenService _tokenService;
    private readonly IConfiguration _configuration;

    public AdminRentController(ITokenService tokenService, DataContext context, UserManager<ApplicationUser> userManager, IConfiguration configuration)
    {
        _tokenService = tokenService;
        _context = context;
        _userManager = userManager;
        _configuration = configuration;
    }


    [HttpGet("Rent/{rentId}")]
    public async Task<IActionResult> GetRentById(long rentId)
    {
        var rent = await _context.Rents.FindAsync(rentId);

        if (rent == null)
        {
            return NotFound("Rent not found");
        }

        return Ok(rent);
    }

    [HttpGet("UserHistory/{userId}")]
    public IActionResult GetUserRentHistory(long userId)
    {
        var user = _context.Users.Find(userId);

        if (user == null)
        {
            return NotFound("User not found");
        }

        var userRentHistory = _context.Rents.Where(rent => rent.UserId == userId).ToList();

        return Ok(userRentHistory);
    }


    [HttpPost("Rent")]
    public async Task<IActionResult> CreateRent([FromBody] RentRequest request)
    {
        var transport = await _context.Transports.FindAsync(request.TransportId);
        var user = await _context.Users.FindAsync(request.UserId);

        if (transport == null || user == null)
        {
            return NotFound("Transport or user not found");
        }

        if (!DateTime.TryParse(request.TimeStart, out var startTime) || startTime <= DateTime.UtcNow)
        {
            return BadRequest("Invalid start time");
        }

        if (!string.IsNullOrEmpty(request.TimeEnd) && (!DateTime.TryParse(request.TimeEnd, out var endTime) || endTime <= DateTime.UtcNow))
        {
            return BadRequest("Invalid end time");
        }

        var newRent = new Rent
        {
            TransportId = request.TransportId,
            UserId = request.UserId,
            TimeStart = request.TimeStart,
            TimeEnd = request.TimeEnd,
            PriceOfUnit = request.PriceOfUnit,
            PriceType = request.PriceType,
            FinalPrice = request.FinalPrice
        };

        _context.Rents.Add(newRent);
        await _context.SaveChangesAsync();

        return Ok("Rent created successfully");
    }


    [HttpPost("Rent/End/{rentId}")]
    public async Task<IActionResult> EndRentAdmin(long rentId, long lat, long lon)
    {

        var rent = await _context.Rents.FindAsync(rentId);

        if (rent == null)
        {
            return NotFound("Rent not found");
        }

        if (rent.TimeEnd != null)
        {
            return BadRequest("Rent has already ended");
        }
        if (lat < -90 || lat > 90 || lon < -180 || lon > 180)
        {
            return BadRequest("Invalid latitude or longitude");
        }

        rent.TimeEnd = DateTime.UtcNow.ToString("o");

        var transport = _context.Transports.Find(rent.TransportId);

        if (transport == null)
        {
            return NotFound("Error");
        }




        DateTime startTime = DateTime.Parse(rent.TimeStart);
        DateTime endTime = DateTime.UtcNow;

        TimeSpan duration = endTime - startTime;
        double totalCost;

        if (rent.PriceType == "Minutes")
        {
            totalCost = duration.TotalMinutes * rent.PriceOfUnit;
        }
        else if (rent.PriceType == "Days")
        {
            totalCost = duration.TotalDays * rent.PriceOfUnit;
        }
        else
        {
            return BadRequest("Invalid PriceType");
        }

        rent.FinalPrice = totalCost;

        transport.Latitude = lat;
        transport.Longitude = lon;

        await _context.SaveChangesAsync();


        return Ok("Rent ended successfully");
    }


    [HttpPut("Rent/{id}")]
    public async Task<IActionResult> UpdateRent(long id, [FromBody] RentRequest request)
    {
        var rent = await _context.Rents.FindAsync(id);

        if (rent == null)
        {
            return NotFound("Rent not found");
        }
        rent.TransportId = request.TransportId;
        rent.UserId = request.UserId;
        rent.TimeStart = request.TimeStart;
        rent.TimeEnd = request.TimeEnd;
        rent.PriceOfUnit = request.PriceOfUnit;
        rent.PriceType = request.PriceType;
        rent.FinalPrice = request.FinalPrice;

        await _context.SaveChangesAsync();

        return Ok("Rent updated successfully");
    }

    [HttpDelete("Rent/{rentId}")]
    public async Task<IActionResult> DeleteRent(long rentId)
    {
        var rent = await _context.Rents.FindAsync(rentId);

        if (rent == null)
        {
            return NotFound("Rent not found");
        }

        _context.Rents.Remove(rent);
        await _context.SaveChangesAsync();

        return Ok("Rent deleted successfully");
    }
}