using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Simbir.GoAPI.Data.Entities;
using Simbir.GoAPI.Data;
using Simbir.GoAPI.Models.Identity;
using Simbir.GoAPI.Services.Identity;
using System.Data;
using Simbir.GoAPI.Models;
using System.Drawing.Drawing2D;
using System.Globalization;

namespace Simbir.GoAPI.Controllers;


[ApiController]
[Route("/api/Rent")]
public class RentController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly DataContext _context;
    private readonly ITokenService _tokenService;
    private readonly IConfiguration _configuration;

    public RentController(ITokenService tokenService, DataContext context, UserManager<ApplicationUser> userManager, IConfiguration configuration)
    {
        _tokenService = tokenService;
        _context = context;
        _userManager = userManager;
        _configuration = configuration;
    }

    [HttpGet("Transport")]
    public IActionResult GetAvailableTransport([FromQuery] RentSearchRequest parameters)
    {
        var availableTransport = _context.Transports
               .Where(t => t.CanBeRented && (parameters.Type == "All" || t.TransportType == parameters.Type))
               .AsEnumerable()
               .Where(t => CalculateDistance(parameters.Latitude, parameters.Longitude, t.Latitude, t.Longitude) <= parameters.Radius)
               .ToList();

        return Ok(availableTransport);
    }


    [Authorize]
    [HttpGet("{rentId}")]
    public async Task<IActionResult> GetRentById(long rentId)
    {
        var rent = await _context.Rents.FindAsync(rentId);

        if (rent == null)
        {
            return NotFound("Rent not found");
        }

        var currentUser = await _userManager.GetUserAsync(User);
        var transport = await _context.Transports.FindAsync(rent.TransportId);

        if (rent.UserId != currentUser.Id && transport.OwnerId != currentUser.Id)
        {
            return Forbid("You don't have permission to access this rent");
        }

        return Ok(rent);
    }

    [Authorize]
    [HttpGet("MyHistory")]
    public async Task<IActionResult> GetMyRentHistory()
    {
        var currentUser = await _userManager.GetUserAsync(User);

        if (currentUser == null)
        {
            return NotFound("User not found");
        }

        var rentHistory = _context.Rents
            .Where(rent => rent.UserId == currentUser.Id)
            .ToList();

        return Ok(rentHistory);
    }


    [Authorize]
    [HttpGet("TransportHistory/{transportId}")]
    public async Task<IActionResult> GetTransportRentHistory(long transportId)
    {
        var currentUser = await _userManager.GetUserAsync(User);

        if (currentUser == null)
        {
            return NotFound("User not found");
        }

        var transport = await _context.Transports.FindAsync(transportId);

        if (transport == null)
        {
            return NotFound("Transport not found");
        }

        if (transport.OwnerId != currentUser.Id)
        {
            return Forbid("You don't have permission to access this transport's rent history");
        }

        var rentHistory = _context.Rents
            .Where(rent => rent.TransportId == transportId)
            .ToList();

        return Ok(rentHistory);
    }


    [Authorize]
    [HttpPost("New/{transportId}")]
    public async Task<IActionResult> RentTransport(long transportId, string rentType)
    {
        var currentUser = await _userManager.GetUserAsync(User);

        if (currentUser == null)
        {
            return NotFound("User not found");
        }

        var transport = await _context.Transports.FindAsync(transportId);

        if (transport == null)
        {
            return NotFound("Transport not found");
        }

        if (transport.OwnerId == currentUser.Id)
        {
            return BadRequest("You cannot rent your own transport");
        }

        if (!transport.CanBeRented)
        {
            return BadRequest("Transport alerady rented");
        }

        double pricePerUnit;
        if (rentType == "Minutes")
        {
            pricePerUnit = transport.MinutePrice ?? 0;
        }
        else if (rentType == "Days")
        {
            pricePerUnit = transport.DayPrice ?? 0;
        }
        else
        {
            return BadRequest("Invalid RentType");
        }
        var rent = new Rent
        {
            TransportId = transportId,
            UserId = currentUser.Id,
            TimeStart = DateTime.UtcNow.ToString("o"), // ISO 8601
            PriceOfUnit = pricePerUnit,
            PriceType = rentType.ToString(),
            TimeEnd = null,
            FinalPrice = null
        };

        transport.CanBeRented = false;
        _context.Rents.Add(rent);
        await _context.SaveChangesAsync();

        return Ok("Transport rented successfully");
    }


    [Authorize]
    [HttpPost("End/{rentId}")]
    public async Task<IActionResult> EndRent(long rentId, double lat, double lon)
    {
        var currentUser = await _userManager.GetUserAsync(User);

        if (currentUser == null)
        {
            return NotFound("User not found");
        }

        var rent = await _context.Rents.FindAsync(rentId);

        if (rent == null)
        {
            return NotFound("Rent not found");
        }

        if (rent.UserId != currentUser.Id)
        {
            return Forbid("You don't have permission to end this rent");
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




        DateTime startTime = DateTime.Parse(rent.TimeStart, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal).ToUniversalTime();
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
        transport.CanBeRented = true;

        await _context.SaveChangesAsync();


        return Ok("Rent ended successfully");
    }






    private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
    {
        const double EarthRadiusKm = 6371;

        var dLat = ToRadians(lat2 - lat1);
        var dLon = ToRadians(lon2 - lon1);

        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        var distance = EarthRadiusKm * c;

        return distance;
    }

    private double ToRadians(double degree)
    {
        return degree * (Math.PI / 180);
    }
}