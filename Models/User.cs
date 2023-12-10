using System;
using System.Collections.Generic;

namespace SanoshAirlines.Models;

public partial class User
{
    public Guid UserId { get; set; }

    public string? Name { get; set; }

    public string? Email { get; set; }

    public string? Password { get; set; }

    public string? Role { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
