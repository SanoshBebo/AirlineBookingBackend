using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
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
        private readonly IMemoryCache _memoryCache;

        public IntegrationController(AirlineDbContext context, IMemoryCache memoryCache)
        {
            _context = context;
            _memoryCache = memoryCache;
        }


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
            List<object> scheduleReturnModels = new List<object>();
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


                var details = new
                {
                    scheduleId = schedule.ScheduleId,
                    flightName = schedule.FlightName,
                    sourceAirportId = schedule.SourceAirportId,
                    sourceAirportName = sourceAirportName,
                    destinationAirportId = schedule.DestinationAirportId,
                    destinationAirportName = destinationAirportName,
                    flightDuration = schedule.FlightDuration,
                    dateTime = schedule.DateTime,
                    capacity = flightcapacity,
                    seatAvailability = SeatAvailability,
                };

                scheduleReturnModels.Add(details);
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
         /*   List<ScheduleReturnModel> scheduleReturnModels = new List<ScheduleReturnModel>();*/
            List<object> scheduleReturnModels = new List<object>();

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


                var details = new
                {
                    scheduleId = schedule.ScheduleId,
                    flightName = schedule.FlightName,
                    sourceAirportId = schedule.SourceAirportId,
                    sourceAirportName = sourceAirportName,
                    destinationAirportId = schedule.DestinationAirportId,
                    destinationAirportName = destinationAirportName,
                    flightDuration = schedule.FlightDuration,
                    dateTime = schedule.DateTime,
                    capacity = flightcapacity,
                    seatAvailability = SeatAvailability,
                };

                scheduleReturnModels.Add(details);
            }
            return Ok(scheduleReturnModels);
        }


        [HttpGet("FlightSchedule/{id}")]
        public async Task<ActionResult<FlightSchedule>> GetFlightSchedule(int id)
        {
            if (_context.FlightSchedules == null)
            {
                return NotFound();
            }
            var flightSchedule = await _context.FlightSchedules.FindAsync(id);

            if (flightSchedule == null)
            {
                return NotFound();
            }

            return flightSchedule;
        }


        [Authorize]
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

                    var flightSchedule = _context.FlightSchedules.FirstOrDefault(s => s.FlightName == connectionTicket.FlightName && s.SourceAirportId == connectionTicket.SourceAirportId
                    && s.DestinationAirportId == connectionTicket.DestinationAirportId && s.DateTime == connectionTicket.DateTime);

                    var seat = _context.Seats.FirstOrDefault(s => s.ScheduleId == flightSchedule.ScheduleId && s.SeatNumber == connectionTicket.SeatNo);

                    seat.Status = "Booked";

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

        [Authorize]
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


        [Authorize]
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
                booking.Status = "Cancelled";


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
        [Authorize]
        public async Task<ActionResult<IEnumerable<SeatDto>>> GetSeatsForSchedule(int scheduleid)
        {
            if (_context.Seats == null)
            {
                return NotFound();
            }

            List<SeatDto> seats = new List<SeatDto>();

            // Check if seats for the schedule ID exist in memory cache
            foreach (var seat in await _context.Seats.Where(s => s.ScheduleId == scheduleid).ToListAsync())
            {
                var cacheKey = $"{scheduleid}_{seat.SeatNumber}";

                if (_memoryCache.TryGetValue(cacheKey, out bool isBooked))
                {
                    // If seat is present in cache, replace the seat status with the cached status
                    seats.Add(new SeatDto
                    {
                        scheduleId = seat.ScheduleId,
                        seatNumber = seat.SeatNumber,
                        status = isBooked ? "Booked" : seat.Status // Use cached status if available
                    });
                }
                else
                {
                    // If seat is not in cache, use the status from the database
                    seats.Add(new SeatDto
                    {
                        scheduleId = seat.ScheduleId,
                        seatNumber = seat.SeatNumber,
                        status = seat.Status
                    });
                }
            }

            if (!seats.Any())
            {
                return NotFound();
            }

            return Ok(seats);
        }




        [Authorize]
        [HttpPatch("changeseatstatus/{scheduleId}/{status}")]
        public async Task<IActionResult> ChangeSeatStatus([FromRoute] int scheduleId, [FromRoute] string status, [FromBody] List<string> seatNumbers)
        {
            if (seatNumbers.Count == 0)
            {
                return BadRequest("No Seats Selected");
            }

            var seatsToCache = new List<string>();

            foreach (var seatno in seatNumbers)
            {
                var seat = _context.Seats.FirstOrDefault(s => s.SeatNumber == seatno && s.ScheduleId == scheduleId);
                if (seat != null)
                {
            
                    if (status.ToLower() == "booked")
                    {
                        // Add seats with "Booked" status to memory cache
                        seatsToCache.Add($"{scheduleId}_{seatno}");
                    }
                    else
                    {
                    seat.Status = status;
                    }
                }
            }

            try
            {
                await _context.SaveChangesAsync();

                if (status == "Booked")
                {
                    // Add seats to memory cache with a 5-minute expiration
                    foreach (var seatKey in seatsToCache)
                    {
                        _memoryCache.Set(seatKey, true, TimeSpan.FromMinutes(5));
                    }
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                return NotFound();
            }

            return Ok($"Seats have been changed to {status} ");
        }

     












        [HttpGet("PowerBiData")]
        public async Task<IActionResult> PowerBIData()
        {
            try
            {
                ///////////////////////////////////////////////////////////////////////////////////
                // Retrieve all schedules
                var schedules = await _context.FlightSchedules.ToListAsync();

                // Create a list to store combined details
                var combinedScheduleDetails = new List<CombinedScheduleDetails>();

                // Iterate through each schedule
                foreach (var schedule in schedules)
                {
                    // Retrieve airport names for source and destination airports
                    var sourceAirportName = await GetAirportName(schedule.SourceAirportId);
                    var destinationAirportName = await GetAirportName(schedule.DestinationAirportId);

                    // Combine details into a single object
                    var combinedDetails = new CombinedScheduleDetails
                    {
                        ScheduleId = schedule.ScheduleId,
                        FlightName = schedule.FlightName,
                        SourceAirportName = sourceAirportName,
                        SourceAiportId = schedule.SourceAirportId,
                        DestinationAirportId = schedule.DestinationAirportId,
                        DestinationAirportName = destinationAirportName,
                        FlightDuration = schedule.FlightDuration,
                        DateTime = schedule.DateTime,
                        AirlineName = "AbhiramAirline"

                    };

                    // Add the combined details to the list
                    combinedScheduleDetails.Add(combinedDetails);
                }
                ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                var bookings = await _context.Bookings.ToListAsync();

                // Create a list to store combined details
                List<CombinedBookingDetails> combinedBookingDetailsList = new List<CombinedBookingDetails>();

                // Iterate through each booking
                foreach (var booking in bookings)
                {
                    // Fetch flight ticket details
                    var flightTickets = await _context.FlightTickets
                        .Where(ft => ft.BookingId == booking.BookingId)
                        .ToListAsync();

                    // Iterate through each flight ticket
                    foreach (var flightTicket in flightTickets)
                    {
                        // Fetch schedule details
                        var schedule = await _context.FlightSchedules.FirstOrDefaultAsync(fs => fs.ScheduleId == flightTicket.ScheduleId);

                        // Fetch airport names
                        var sourceAirportName = await GetAirportName(schedule.SourceAirportId);
                        var destinationAirportName = await GetAirportName(schedule.DestinationAirportId);

                        // Create CombinedBookingDetails instance
                        var combinedDetails = new CombinedBookingDetails
                        {
                            BookingId = booking.BookingId,
                            BookingType = booking.BookingType,
                            TicketNo = flightTicket.TicketNo,
                            Name = flightTicket.Name,
                            Age = flightTicket.Age,
                            Gender = flightTicket.Gender,
                            SourceAirportId = schedule.SourceAirportId,
                            SourceAirportName = sourceAirportName,
                            DestinationAirportId = schedule.DestinationAirportId,
                            DestinationAirportName = destinationAirportName,
                            FlightName = schedule.FlightName,
                            DateTime = schedule.DateTime,
                            AirlineName = "AbhiramAirline"
                            // Add other properties as needed
                        };

                        // Add the combined details to the list
                        combinedBookingDetailsList.Add(combinedDetails);
                    }
                    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    ///connectionflightTickets
                    var connectionFlightTickets = await _context.ConnectionFlightTickets.Where(cft => cft.BookingId == booking.BookingId).ToListAsync();
                    foreach (var connflightticket in connectionFlightTickets)
                    {
                        // Fetch airport names
                        var sourceAirportName = await GetAirportName(connflightticket.SourceAirportId);
                        var destinationAirportName = await GetAirportName(connflightticket.DestinationAirportId);
                        var combinedDetails = new CombinedBookingDetails
                        {
                            BookingId = booking.BookingId,
                            BookingType = booking.BookingType,
                            TicketNo = connflightticket.TicketNo,
                            Name = connflightticket.Name,
                            Age = connflightticket.Age,
                            Gender = connflightticket.Gender,
                            SourceAirportId = connflightticket.SourceAirportId,
                            SourceAirportName = sourceAirportName,
                            DestinationAirportId = connflightticket.DestinationAirportId,
                            DestinationAirportName = destinationAirportName,
                            FlightName = connflightticket.FlightName,
                            DateTime = connflightticket.DateTime,
                            AirlineName = connflightticket.AirlineName,
                            // Add other properties as needed
                        };
                        combinedBookingDetailsList.Add(combinedDetails);
                    }


                }
                ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                var powerBIData = new PowerBiData()
                {
                    FlightSchedules = combinedScheduleDetails,
                    CombinedBookingDetails = combinedBookingDetailsList
                };

                return Ok(powerBIData);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
           
        private async Task<string> GetAirportName(string airportId)
        {
            var airport = await _context.Airports.FirstOrDefaultAsync(a => a.AirportId == airportId);
            return airport?.AirportName ?? "Unknown Airport";
        }

    }

    public class SeatDto
    {
        public int scheduleId { get; set; }
        public string seatNumber { get; set; } = null!;
        public string? status { get; set; }
    }

    public class PowerBiData
    {
        public List<CombinedScheduleDetails> FlightSchedules { get; set; }
        public List<CombinedBookingDetails> CombinedBookingDetails { get; set; }
    }
    public class CombinedScheduleDetails
    {
        public int ScheduleId { get; set; }
        public string FlightName { get; set; }
        public string SourceAiportId { get; set; }
        public string DestinationAirportId { get; set; }
        public string SourceAirportName { get; set; }
        public string DestinationAirportName { get; set; }
        public TimeSpan FlightDuration { get; set; }
        public DateTime DateTime { get; set; }
        public string? AirlineName { get; set; }
    }
    public class CombinedBookingDetails
    {
        public Guid BookingId { get; set; }
        public string BookingType { get; set; }
        public int TicketNo { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; }
        public string SourceAirportId { get; set; }
        public string SourceAirportName { get; set; }
        public string DestinationAirportId { get; set; }
        public string DestinationAirportName { get; set; }
        public string FlightName { get; set; }
        public DateTime DateTime { get; set; }
        public string AirlineName { get; set; }
        // Add other properties as needed
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
