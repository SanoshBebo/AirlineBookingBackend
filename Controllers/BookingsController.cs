using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using Humanizer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SanoshAirlines.Models;
using SanoshAirlines.Models.RequestBodyModels;

namespace SanoshAirlines.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private readonly AirlineDbContext _context;

        public BookingsController(AirlineDbContext context)
        {
            _context = context;
        }

        // GET: api/Bookings
        [HttpGet("userbookings/{userid}")]
        public async Task<ActionResult<IEnumerable<Booking>>> GetBookingsOfUser([FromRoute] Guid userid)
        {
            if (_context.Bookings == null)
            {
                return NotFound();
            }

            var bookings = await _context.Bookings.Where(b => b.UserId == userid).ToListAsync();

            if (bookings == null || !bookings.Any())
            {
                return BadRequest("No Bookings");
            }

            var bookingData = new List<object>();

            foreach (var booking in bookings)
            {
                var firstFlightTickets = await _context.FlightTickets
                    .Where(ft => ft.BookingId == booking.BookingId)
                    .ToListAsync();

                var connectingFlightTickets = await _context.ConnectionFlightTickets
                    .Where(cft => cft.BookingId == booking.BookingId)
                    .ToListAsync();

                var bookingIdData = new List<object>();

                foreach (var ticket in firstFlightTickets)
                {
                    var flightSchedule = await _context.FlightSchedules
                        .FirstOrDefaultAsync(fs => fs.ScheduleId == ticket.ScheduleId);

                    var sourceAirport = await _context.Airports
                    .FirstOrDefaultAsync(a => a.AirportId == flightSchedule.SourceAirportId);

                    var destinationAirport = await _context.Airports
                        .FirstOrDefaultAsync(a => a.AirportId == flightSchedule.DestinationAirportId);


                    var bookingTicketData = new
                    {
                        Ticket = ticket,
                        FlightSchedule = flightSchedule,
                        SourceAirport = sourceAirport,
                        DestinationAirport = destinationAirport
                    };
                    bookingIdData.Add(bookingTicketData);
                }

                foreach (var ticket in connectingFlightTickets)
                {
                      var flightSchedule = await _context.FlightSchedules
                        .FirstOrDefaultAsync(fs => fs.FlightName == ticket.FlightName && fs.DateTime.Date == ticket.DateTime.Date && fs.SourceAirportId == ticket.SourceAirportId && fs.DestinationAirportId == ticket.DestinationAirportId);

                    var sourceAirport = await _context.Airports
                    .FirstOrDefaultAsync(a => a.AirportId == flightSchedule.SourceAirportId);

                    var destinationAirport = await _context.Airports
                        .FirstOrDefaultAsync(a => a.AirportId == flightSchedule.DestinationAirportId);

                    var bookingTicketData = new
                    {
                        Ticket = ticket,
                        FlightSchedule = flightSchedule,
                        SourceAirport = sourceAirport,
                        DestinationAirport = destinationAirport
                    };
                    bookingIdData.Add(bookingTicketData);
                }

                var bookingDataItem = new
                {
                    Booking = booking,
                    Tickets = bookingIdData
                };

                bookingData.Add(bookingDataItem);
            }

            return Ok(bookingData);
        }





        // GET: api/Bookings/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Booking>> GetBooking(Guid id)
        {
            if (_context.Bookings == null)
            {
                return NotFound();
            }
            var booking = await _context.Bookings.FindAsync(id);

            if (booking == null)
            {
                return NotFound();
            }

            return booking;
        }


        // GET: api/Bookings/5
        [HttpGet("getConnectionBooking/{id}")]
        public async Task<ActionResult<ConnectionFlightTicket>> GetConnectionBooking(Guid id)
        {
            if (_context.Bookings == null)
            {
                return NotFound();
            }
            var booking = await _context.Bookings.FindAsync(id);

            if (booking == null)
            {
                return NotFound();
            }

            var connectionBookingTickets = await _context.ConnectionFlightTickets.Where(cft => cft.BookingId == id).ToListAsync();


            return Ok(connectionBookingTickets);
        }



        // POST: api/Bookings
        [HttpPost]
        public async Task<ActionResult<BookingModel>> MakeBooking([FromBody] List<BookingModel> bookings)
        {
            if (_context.Bookings == null)
            {
                return Problem("Entity set 'AirlineDbContext.Bookings' is null.");
            }

            if (bookings.Count > 0)
            {

                var BookingId = Guid.NewGuid();

                var Booking = new Booking
                {
                    BookingId = BookingId,
                    Status = bookings[0].Status,
                    UserId = bookings[0].UserId,
                    BookingType = bookings[0].BookingType,
                };

                
                await _context.Bookings.AddAsync(Booking);

                foreach (var booking in bookings)
                {
                    if (booking.PassengerInfos != null && booking.PassengerInfos.Count > 0)
                    {
                        foreach (var passengerInfo in booking.PassengerInfos)
                        {
                            if (booking.AirlineName == "SanoshAirlines")
                            {
                                var ticket = new FlightTicket
                                {
                                    BookingId = BookingId,
                                    ScheduleId = booking.ScheduleId,
                                    SeatNo = passengerInfo.SeatNo,
                                    Age = passengerInfo.Age,
                                    Gender = passengerInfo.Gender,
                                    Name = passengerInfo.Name,
                                };
                                await _context.FlightTickets.AddAsync(ticket);
                            }
                            else
                            {
                                var connectionTicket = new ConnectionFlightTicket
                                {
                                    BookingId = BookingId,
                                    SeatNo = passengerInfo.SeatNo,
                                    Age = passengerInfo.Age,
                                    Gender = passengerInfo.Gender,
                                    Name = passengerInfo.Name,
                                    FlightName = booking.FlightName,
                                    AirlineName = booking.AirlineName,
                                    SourceAirportId = booking.SourceAirportId,
                                    DestinationAirportId = booking.DestinationAirportId,
                                    DateTime = booking.DateTime,
                                };



                                await _context.ConnectionFlightTickets.AddAsync(connectionTicket);
                            }
                        }
                    }
                }

                await _context.SaveChangesAsync();

                return Ok(BookingId);
            }

            return BadRequest("No Details To Make Booking");

        }



        [HttpPatch("cancelbooking/{bookingid}")]
        public async Task<ActionResult<BookingModel>> CancelBooking([FromRoute] Guid bookingid)
        {
            if (_context.Bookings == null)
            {
                return Problem("Entity set 'AirlineDbContext.Bookings' is null.");
            }

            var booking = await _context.Bookings.FindAsync(bookingid);

            if(booking == null)
            {
                return BadRequest("BookingID Does Not Exist");
            }

                booking.Status = "Canceled";
                var isDirectFlight = _context.FlightTickets.FirstOrDefault(ft => ft.BookingId == booking.BookingId);
                var isConnectingFlight = _context.ConnectionFlightTickets.FirstOrDefault(cft => cft.BookingId == booking.BookingId);
                if (isDirectFlight != null)
                {
                    var FirstFlightTickets = await _context.FlightTickets.Where(ft => ft.BookingId == booking.BookingId).ToListAsync();

                    foreach (var Ticket in FirstFlightTickets)
                    {
                        var Seat = _context.Seats.FirstOrDefault(s => s.ScheduleId == Ticket.ScheduleId && s.SeatNumber == Ticket.SeatNo);
                        Seat.Status = "Available";
                    }
                }
                
                if (isConnectingFlight != null)
                {
                    var ConnectingFlightTickets = await _context.ConnectionFlightTickets.Where(ft => ft.BookingId == booking.BookingId).ToListAsync();
                    await _context.SaveChangesAsync();
                    return Ok(new { message = "Your Booking Has been Cancelled", connectingFlightTickets = ConnectingFlightTickets });
                }
            await _context.SaveChangesAsync();
            return Ok("Your Booking Has Been Cancelled");
        }


      


        [HttpPatch("canceltickets/{bookingid}")]
        public async Task<ActionResult<BookingModel>> CancelTicketsInABooking([FromRoute] Guid bookingid, [FromBody] List<string> Names)
        {
            if (_context.Bookings == null)
            {
                return Problem("Entity set 'AirlineDbContext.Bookings' is null.");
            }

            var booking = await _context.Bookings.FindAsync(bookingid);

            if (booking == null)
            {
                return BadRequest("BookingID Does Not Exist");
            }

            var isDirectFlight = _context.FlightTickets.FirstOrDefault(ft => ft.BookingId == booking.BookingId);
            var isConnectingFlight = _context.ConnectionFlightTickets.FirstOrDefault(cft => cft.BookingId == booking.BookingId);

            if (isDirectFlight != null)
            {
                var FirstFlightTickets = await _context.FlightTickets.Where(ft => ft.BookingId == booking.BookingId && Names.Contains(ft.Name)).ToListAsync();

                foreach (var Ticket in FirstFlightTickets)
                {
                    var Seat = _context.Seats.FirstOrDefault(s => s.ScheduleId == Ticket.ScheduleId && s.SeatNumber == Ticket.SeatNo);
                    Seat.Status = "Available";
                }
            }

            if (isConnectingFlight != null)
            {
                var ConnectingFlightTickets = await _context.ConnectionFlightTickets.Where(cft => cft.BookingId == booking.BookingId && Names.Contains(cft.Name)).ToListAsync();
                await _context.SaveChangesAsync();
                return Ok(new { message = "Your Ticket Has been Cancelled", connectingFlightTickets = ConnectingFlightTickets });
            }

            await _context.SaveChangesAsync();
            return Ok("Your Ticket Has Been Cancelled");
        }

      

        private bool BookingExists(Guid id)
        {
            return (_context.Bookings?.Any(e => e.BookingId == id)).GetValueOrDefault();
        }
    }
}
