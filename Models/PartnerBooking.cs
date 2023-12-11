using System;
using System.Collections.Generic;

namespace SanoshAirlines.Models;

public partial class PartnerBooking
{
    public int TicketNo { get; set; }

    public string AirlineName { get; set; } = null!;

    public Guid BookingId { get; set; }

    public string FlightName { get; set; } = null!;

    public string? SourceAirportId { get; set; }

    public string? DestinationAirportId { get; set; }

    public string Status { get; set; } = null!;

    public string SeatNo { get; set; } = null!;

    public string Name { get; set; } = null!;

    public int Age { get; set; }

    public string Gender { get; set; } = null!;

    public DateTime DateTime { get; set; }

    /*public virtual Airport? DestinationAirport { get; set; }

    public virtual Airport? SourceAirport { get; set; }*/
}
