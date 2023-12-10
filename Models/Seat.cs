using System;
using System.Collections.Generic;

namespace SanoshAirlines.Models;

public partial class Seat
{
    public int ScheduleId { get; set; }

    public string SeatNumber { get; set; } = null!;

    public string? Status { get; set; }

/*    public virtual FlightSchedule Schedule { get; set; } = null!;
*/}
