using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Simbir.GoAPI.Data.Entities;
using Simbir.GoAPI.Data;
using System.IdentityModel.Tokens.Jwt;
using Simbir.GoAPI.Services.Identity;
using Microsoft.EntityFrameworkCore;
using Simbir.GoAPI.Web.Extensions;
using System.Security.Claims;
using System.Security.Principal;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using System.Web;
using Simbir.GoAPI.Models;
using Simbir.GoAPI.Models.Identity;

namespace Simbir.GoAPI.Controllers;




[ApiController]
[Route("/api/Transport")]
public class TransportController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly DataContext _context;
    private readonly ITokenService _tokenService;
    private readonly IConfiguration _configuration;

    public TransportController(ITokenService tokenService, DataContext context, UserManager<ApplicationUser> userManager, IConfiguration configuration)
    {
        _tokenService = tokenService;
        _context = context;
        _userManager = userManager;
        _configuration = configuration;
    }

    [Authorize]
    [HttpPost("/api/Transport")]
    public async Task<ActionResult> CreateTransport([FromBody] TransportRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var user = await _userManager.FindByNameAsync(User.Identity.Name);

        if (user == null)
        {
            return NotFound("User not found");
        }

        var transport = new Transport
        {
            CanBeRented = request.CanBeRented,
            TransportType = request.TransportType,
            Model = request.Model,
            Color = request.Color,
            Identifier = request.Identifier,
            Description = request.Description,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            MinutePrice = request.MinutePrice,
            DayPrice = request.DayPrice,
            OwnerId = user.Id
        };

        _context.Transports.Add(transport);
        await _context.SaveChangesAsync();

        return Ok("Transport created successfully");
    }

    [Authorize]
    [HttpPut("/api/Transport/{id}")]
    public async Task<ActionResult> UpdateTransport(long id, [FromBody] TransportRequest request)
    {
        var user = await _userManager.FindByNameAsync(User.Identity.Name);

        if (user == null)
        {
            return NotFound("User not found");
        }

        var transport = await _context.Transports.FindAsync(id);

        if (transport == null)
        {
            return NotFound("Transport not found");
        }

        if (transport.OwnerId != user.Id)
        {
            return Forbid("You don't have permission to update this transport");
        }

        transport.CanBeRented = request.CanBeRented;
        transport.Model = request.Model;
        transport.Color = request.Color;
        transport.Identifier = request.Identifier;
        transport.Description = request.Description;
        transport.Latitude = request.Latitude;
        transport.Longitude = request.Longitude;
        transport.MinutePrice = request.MinutePrice;
        transport.DayPrice = request.DayPrice;

        _context.Transports.Update(transport);
        await _context.SaveChangesAsync();

        return Ok("Transport updated successfully");
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTransport(long id)
    {
        var transport = await _context.Transports.FindAsync(id);

        var user = await _userManager.FindByNameAsync(User.Identity.Name);

        if (transport == null)
        {
            return NotFound("Transport not found");
        }

        if (transport.OwnerId != user.Id)
        {
            return Forbid("You don't have permission to update this transport");
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

    [HttpGet("{id}")]
    public IActionResult GetTransportByIdAdmin(long id)
    {
        return Ok(_context.Transports.Find(id));
    }

}
