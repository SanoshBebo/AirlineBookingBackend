using System;
using System.Collections.Generic;

namespace SanoshAirlines.Models;

public partial class FlightSchedule
{
    public int ScheduleId { get; set; }

    public string FlightName { get; set; } = null!;

    public string SourceAirportId { get; set; } = null!;

    public string DestinationAirportId { get; set; } = null!;

    public TimeSpan FlightDuration { get; set; }

    public DateTime DateTime { get; set; }

   /* public virtual Airport DestinationAirport { get; set; } = null!;

    public virtual FlightDetail FlightNameNavigation { get; set; } = null!;

    public virtual ICollection<FlightTicket> FlightTickets { get; set; } = new List<FlightTicket>();

    public virtual ICollection<Seat> Seats { get; set; } = new List<Seat>();

    public virtual Airport SourceAirport { get; set; } = null!;*/
}
