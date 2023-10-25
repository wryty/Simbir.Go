namespace Simbir.GoAPI.Data.Entities;

public class Rent
{
    public long Id { get; set; }
    public long TransportId { get; set; }
    public long UserId { get; set; }
    public string TimeStart { get; set; } // Формат ISO 8601 (например, "2023-10-25T12:00:00Z")
    public string? TimeEnd { get; set; } // Формат ISO 8601 (может быть null)
    public double PriceOfUnit { get; set; }
    public string PriceType { get; set; } // "Minutes" или "Days"
    public double? FinalPrice { get; set; } // Может быть null

}