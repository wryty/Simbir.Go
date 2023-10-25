using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Simbir.GoAPI.Data;
using Simbir.GoAPI.Data.Entities;
using Simbir.GoAPI.Models;
using Simbir.GoAPI.Models.Identity;
using Simbir.GoAPI.Services.Identity;
using System.Data;

namespace Simbir.GoAPI.Controllers;


[ApiController]
[Route("/api/Admin/Transport")]
[Authorize(AuthenticationSchemes = "Bearer", Roles = RoleConsts.Administrator)]
public class AdminTransportController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly DataContext _context;
    private readonly ITokenService _tokenService;
    private readonly IConfiguration _configuration;

    public AdminTransportController(ITokenService tokenService, DataContext context, UserManager<ApplicationUser> userManager, IConfiguration configuration)
    {
        _tokenService = tokenService;
        _context = context;
        _userManager = userManager;
        _configuration = configuration;
    }



    [HttpGet]
    public IActionResult GetTransportList(int start, int count, string transportType)
    {
        IQueryable<Transport> query = _context.Transports;

        if (!string.IsNullOrEmpty(transportType) && transportType != "All")
        {
            query = query.Where(t => t.TransportType == transportType);
        }

        var transports = query.Skip(start).Take(count).ToList();

        return Ok(transports);
    }

    [HttpGet("{id}")]
    public IActionResult GetTransportListById(long id)
    {
        return Ok(_context.Transports.Find(id));
    }


    [HttpPost]
    public async Task<IActionResult> CreateTransportAdmin([FromBody] AdminTransportRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        /*if (transportType == null)
        {
            return BadRequest("Invalid transport type");
        }*/

        var transport = new Transport
        {
            OwnerId = request.OwnerId,
            CanBeRented = request.CanBeRented,
            TransportType = request.TransportType,
            Model = request.Model,
            Color = request.Color,
            Identifier = request.Identifier,
            Description = request.Description,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            MinutePrice = request.MinutePrice,
            DayPrice = request.DayPrice
        };

        _context.Transports.Add(transport);
        await _context.SaveChangesAsync();

        return Ok("Transport created successfully");
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTransportAdmin(long id, [FromBody] AdminTransportRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var existingTransport = await _context.Transports.FindAsync(id);

        if (existingTransport == null)
        {
            return NotFound("Transport not found");
        }

        /*var transportType = await _context.TransportTypes
            .FirstOrDefaultAsync(tt => tt.Name == request.TransportType);

        if (transportType == null)
        {
            return BadRequest("Invalid transport type");
        }*/

        existingTransport.OwnerId = request.OwnerId;
        existingTransport.CanBeRented = request.CanBeRented;
        existingTransport.TransportType = request.TransportType;
        existingTransport.Model = request.Model;
        existingTransport.Color = request.Color;
        existingTransport.Identifier = request.Identifier;
        existingTransport.Description = request.Description;
        existingTransport.Latitude = request.Latitude;
        existingTransport.Longitude = request.Longitude;
        existingTransport.MinutePrice = request.MinutePrice;
        existingTransport.DayPrice = request.DayPrice;

        _context.Transports.Update(existingTransport);
        await _context.SaveChangesAsync();

        return Ok("Transport updated successfully");
    }


    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTransportAdmin(long id)
    {
        var transport = await _context.Transports.FindAsync(id);
        if (transport == null)
        {
            return NotFound("Transport not found");
        }

        _context.Transports.Remove(transport);

        var result = await _context.SaveChangesAsync();

        if (result > 0)
        {
            return Ok("Transport deleted successfully");
        }
        else
        {
            return BadRequest("Failed to delete transport");
        }
    }

}