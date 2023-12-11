using System;
using System.Collections.Generic;

namespace SanoshAirlines.Models;

public partial class FlightDetail
{
    public int FlightId { get; set; }

    public string FlightName { get; set; } = null!;

    public int FlightCapacity { get; set; }

    public bool IsActive { get; set; }

  /*  public virtual ICollection<FlightSchedule> FlightSchedules { get; set; } = new List<FlightSchedule>();*/
}
