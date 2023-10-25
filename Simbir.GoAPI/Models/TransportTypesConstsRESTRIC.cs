using Simbir.GoAPI.Data.Entities;

namespace Simbir.GoAPI.Models;

public class VehicleType
{
    public long Id { get; set; }
    public string Type { get; set; }
    public List<Transport> Transports { get; set; }
}