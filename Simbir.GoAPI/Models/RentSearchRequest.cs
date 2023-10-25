namespace Simbir.GoAPI.Models;

public class RentSearchRequest
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double Radius { get; set; }
    public string Type { get; set; } // "Car", "Bike", "Scooter" или "All"
}