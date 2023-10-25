﻿using Simbir.GoAPI.Models;

namespace Simbir.GoAPI.Data.Entities;

public class Transport
{
    public long Id { get; set; }
    public bool CanBeRented { get; set; }
    public string TransportType { get; set; }
    public string Model { get; set; }
    public string Color { get; set; }
    public string Identifier { get; set; }
    public string Description { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double? MinutePrice { get; set; }
    public double? DayPrice { get; set; }
    public long OwnerId { get; set; }
    public ApplicationUser Owner { get; set; }

}
