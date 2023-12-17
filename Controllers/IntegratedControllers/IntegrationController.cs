using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SanoshAirlines.Models;
using SanoshAirlines.Models.RequestBodyModels;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace SanoshAirlines.Controllers.IntegratedControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IntegrationController : ControllerBase
    {
        private readonly AirlineDbContext _context;

        public IntegrationController(AirlineDbContext context)
        {
            _context = context;
        }

        // GET: api/FlightSchedules
        [HttpGet("directflight/{source}/{destination}/{date}")]
        public async Task<ActionResult<IEnumerable<FlightSchedule>>> GetFlightSchedule([FromRoute] string source, [FromRoute] string destination, [FromRoute] DateTime date)
        {
            if (_context.FlightSchedules == null)
            {
                return NotFound();
            }

            DateTime startDate = date.Date;
            DateTime endDate = startDate.AddDays(1);


            var schedules = await _context.FlightSchedules
            .Where(schedule =>
            schedule.SourceAirportId == source &&
                    schedule.DestinationAirportId == destination &&
                    schedule.DateTime >= startDate &&
                    schedule.DateTime < endDate)
                  .ToListAsync();


            if (!schedules.Any())
            {
                return NotFound();
            }
            List<ScheduleReturnModel> scheduleReturnModels = new List<ScheduleReturnModel>();
            foreach (var schedule in schedules)
            {
                // Get SourceAirportName
                var sourceAirport = _context.Airports.FirstOrDefault(a => a.AirportId == schedule.SourceAirportId);
                string sourceAirportName = sourceAirport?.AirportName ?? "Unknown";

                // Get DestinationAirportName
                var destinationAirport = _context.Airports.FirstOrDefault(a => a.AirportId == schedule.DestinationAirportId);
                string destinationAirportName = destinationAirport?.AirportName ?? "Unknown";

                var flight = _context.FlightDetails.FirstOrDefault(a => a.FlightName == schedule.FlightName);
                int flightcapacity = flight.FlightCapacity;


                // Fetch seat count for the current schedule
                int SeatAvailability = _context.Seats.Count(s => s.ScheduleId == schedule.ScheduleId && s.Status == "Available");


                ScheduleReturnModel scheduleReturnModel = new ScheduleReturnModel
                {
                    ScheduleId = schedule.ScheduleId,
                    FlightName = schedule.FlightName,
                    SourceAirportId = schedule.SourceAirportId,
                    SourceAirportName = sourceAirportName,
                    DestinationAirportId = schedule.DestinationAirportId,
                    DestinationAirportName = destinationAirportName,
                    FlightDuration = schedule.FlightDuration,
                    DateTime = schedule.DateTime,
                    Capacity = flightcapacity,
                    SeatAvailability = SeatAvailability,
                };
                scheduleReturnModels.Add(scheduleReturnModel);
            }
            return Ok(scheduleReturnModels);

        }


        [HttpGet("connectingflight/{source}/{destination}/{date}")]
        public async Task<ActionResult<IEnumerable<FlightSchedule>>> GetConnectingFlightSchedule([FromRoute] string source, [FromRoute] string destination, [FromRoute] DateTime date)
        {
            if (_context.FlightSchedules == null)
            {
                return NotFound();
            }
            var schedules = await _context.FlightSchedules.Where(s => s.SourceAirportId == source && s.DestinationAirportId != destination && s.DateTime.Date == date.Date).ToListAsync();

            if (!schedules.Any())
            {
                return NotFound();
            }
            List<ScheduleReturnModel> scheduleReturnModels = new List<ScheduleReturnModel>();
            foreach (var schedule in schedules)
            {
                // Get SourceAirportName
                var sourceAirport = _context.Airports.FirstOrDefault(a => a.AirportId == schedule.SourceAirportId);
                string sourceAirportName = sourceAirport?.AirportName ?? "Unknown";

                // Get DestinationAirportName
                var destinationAirport = _context.Airports.FirstOrDefault(a => a.AirportId == schedule.DestinationAirportId);
                string destinationAirportName = destinationAirport?.AirportName ?? "Unknown";

                var flight = _context.FlightDetails.FirstOrDefault(a => a.FlightName == schedule.FlightName);
                int flightcapacity = flight.FlightCapacity;


                // Fetch seat count for the current schedule
                int SeatAvailability = _context.Seats.Count(s => s.ScheduleId == schedule.ScheduleId && s.Status == "Available");


                ScheduleReturnModel scheduleReturnModel = new ScheduleReturnModel
                {
                    ScheduleId = schedule.ScheduleId,
                    FlightName = schedule.FlightName,
                    SourceAirportId = schedule.SourceAirportId,
                    SourceAirportName = sourceAirportName,
                    DestinationAirportId = schedule.DestinationAirportId,
                    DestinationAirportName = destinationAirportName,
                    FlightDuration = schedule.FlightDuration,
                    DateTime = schedule.DateTime,
                    Capacity = flightcapacity,
                    SeatAvailability = SeatAvailability,
                };
                scheduleReturnModels.Add(scheduleReturnModel);
            }
            return Ok(scheduleReturnModels);
        }





        [HttpPost("partnerbooking")]
        public async Task<IActionResult> PostPartnerBookings(List<PartnerTicketReturn> connectionTickets)
        {
            try
            {
                if (_context.PartnerBookings == null)
                {
                    return Problem("Entity set 'ApplicationDbContext.PartnerBookings' is null.");
                }

                var partnerBookings = new List<PartnerBooking>();

                foreach (var connectionTicket in connectionTickets)
                {
                    var partnerBooking = new PartnerBooking
                    {
                        TicketNo = connectionTicket.TicketNo,
                        FlightName = connectionTicket.FlightName,
                        BookingId = connectionTicket.BookingId,
                        SourceAirportId = connectionTicket.SourceAirportId,
                        DestinationAirportId = connectionTicket.DestinationAirportId,
                        AirlineName = connectionTicket.AirlineName,
                        SeatNo = connectionTicket.SeatNo,
                        Name = connectionTicket.Name,
                        Age = connectionTicket.Age,
                        Gender = connectionTicket.Gender,
                        DateTime = connectionTicket.DateTime,
                        Status = "Booked",
                    };

                    partnerBookings.Add(partnerBooking);
                }

                _context.PartnerBookings.AddRange(partnerBookings);
                await _context.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpPatch("cancelticketsinpartnerbooking/{bookingid}")]
        public async Task<ActionResult<BookingModel>> CancelTicketsInPartnerBooking([FromRoute] Guid bookingid, [FromBody] List<string> Names)
        {
            if (_context.Bookings == null)
            {
                return Problem("Entity set 'AirlineDbContext.Bookings' is null.");
            }

            var bookings = await _context.PartnerBookings.Where(pb => pb.BookingId == bookingid && Names.Contains(pb.Name)).ToListAsync();

            if (bookings == null)
            {
                return BadRequest("Bookings Does Not Exist");
            }

            foreach (var booking in bookings)
            {
                booking.Status = "Cancelled";
                var schedule = await _context.FlightSchedules.FirstOrDefaultAsync(fs => fs.FlightName == booking.FlightName && fs.SourceAirportId == booking.SourceAirportId &&
                fs.DestinationAirportId == booking.DestinationAirportId && fs.DateTime == booking.DateTime
                );

                var seat = await _context.Seats.FirstOrDefaultAsync(s => s.ScheduleId == schedule.ScheduleId && s.SeatNumber == booking.SeatNo);

                seat.Status = "Available";

            }

            await _context.SaveChangesAsync();
            return Ok("Your Tickets Has Been Cancelled");
        }



        [HttpPatch("cancelpartnerbooking/{bookingid}")]
        public async Task<ActionResult<BookingModel>> CancelPartnerBooking([FromRoute] Guid bookingid)
        {
            if (_context.Bookings == null)
            {
                return Problem("Entity set 'AirlineDbContext.Bookings' is null.");
            }

            var bookings = await _context.PartnerBookings.Where(pb => pb.BookingId == bookingid).ToListAsync();

            if (bookings == null)
            {
                return BadRequest("BookingID Does Not Exist");
            }

            foreach (var booking in bookings)
            {
                booking.Status = "Canceled";


                var schedule = await _context.FlightSchedules.FirstOrDefaultAsync(fs => fs.FlightName == booking.FlightName && fs.SourceAirportId == booking.SourceAirportId &&
                fs.DestinationAirportId == booking.DestinationAirportId && fs.DateTime == booking.DateTime
                );

                var seat = await _context.Seats.FirstOrDefaultAsync(s => s.ScheduleId == schedule.ScheduleId && s.SeatNumber == booking.SeatNo);

                seat.Status = "Available";

            }

            await _context.SaveChangesAsync();
            return Ok("Your Booking Has Been Cancelled");
        }


        [HttpGet("seats/{scheduleid}")]
        public async Task<ActionResult<Seat>> GetSeatsForSchedule(int scheduleid)
        {
            if (_context.Seats == null)
            {
                return NotFound();
            }
            var seats = await _context.Seats.Where(s => s.ScheduleId == scheduleid).ToListAsync();

            if (seats == null)
            {
                return NotFound();
            }

            return Ok(seats);
        }




    }


    public class PartnerTicketReturn
    {
        public Guid BookingId { get; set; }
        public int TicketNo { get; set; }
        public string? Name { get; set; }
        public int Age { get; set; }
        public string? Gender { get; set; }
        public string? SeatNo { get; set; }
        public string? FlightName { get; set; }
        public string? SourceAirportId { get; set; }
        public string? DestinationAirportId { get; set; }
        public string? AirlineName { get; set; }
        public DateTime DateTime { get; set; }
    }

    public class ScheduleReturnModel
    {
        public int ScheduleId { get; set; }

        public string FlightName { get; set; } = null!;

        public string SourceAirportId { get; set; } = null!;
        public string? SourceAirportName { get; set; }

        public string DestinationAirportId { get; set; } = null!;
        public string? DestinationAirportName { get; set; }

        public TimeSpan FlightDuration { get; set; }

        public DateTime DateTime { get; set; }
        public int Capacity { get; set; }

        public int SeatAvailability { get; set; }
    }
}
