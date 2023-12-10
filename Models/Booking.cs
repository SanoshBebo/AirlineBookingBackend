using System;
using System.Collections.Generic;

namespace SanoshAirlines.Models;

public partial class Booking
{
    public Guid BookingId { get; set; }

    public string? Status { get; set; }

    public Guid? UserId { get; set; }

    public string? BookingType { get; set; }

 /*   public virtual ICollection<ConnectionFlightTicket> ConnectionFlightTickets { get; set; } = new List<ConnectionFlightTicket>();

    public virtual ICollection<FlightTicket> FlightTickets { get; set; } = new List<FlightTicket>();

    public virtual User? User { get; set; }*/
}
