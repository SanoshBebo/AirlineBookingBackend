using System;
using System.Collections.Generic;

namespace SanoshAirlines.Models;

public partial class Airport
{
    public string AirportId { get; set; } = null!;

    public string City { get; set; } = null!;

    public string AirportName { get; set; } = null!;

    public string State { get; set; } = null!;

   /* public virtual ICollection<ConnectionFlightTicket> ConnectionFlightTicketDestinationAirports { get; set; } = new List<ConnectionFlightTicket>();

    public virtual ICollection<ConnectionFlightTicket> ConnectionFlightTicketSourceAirports { get; set; } = new List<ConnectionFlightTicket>();

    public virtual ICollection<FlightSchedule> FlightScheduleDestinationAirports { get; set; } = new List<FlightSchedule>();

    public virtual ICollection<FlightSchedule> FlightScheduleSourceAirports { get; set; } = new List<FlightSchedule>();

    public virtual ICollection<PartnerBooking> PartnerBookingDestinationAirports { get; set; } = new List<PartnerBooking>();

    public virtual ICollection<PartnerBooking> PartnerBookingSourceAirports { get; set; } = new List<PartnerBooking>();*/
}
