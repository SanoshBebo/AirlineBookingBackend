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

            var Bookings = await _context.Bookings.Where(b => b.UserId == userid).ToListAsync();
            var directFlights = new List<object>();
            var connectingFlights = new List<object>();

            if(Bookings == null)
            {
                return BadRequest("No Bookings");
            }

            var BookingData = new List<object>();

            foreach (var booking in Bookings)
            {
                var isDirectFlight = _context.FlightTickets.FirstOrDefault(ft => ft.BookingId == booking.BookingId);
                var isConnectingFlight = _context.ConnectionFlightTickets.FirstOrDefault(cft => cft.BookingId == booking.BookingId);
                if (isDirectFlight != null && isConnectingFlight !=null)
                {
                    var FirstFlightTickets = await _context.FlightTickets.Where(ft => ft.BookingId == booking.BookingId).ToListAsync();
                    
                    var ConnectingFlightTickets = await _context.ConnectionFlightTickets.Where(ft => ft.BookingId == booking.BookingId).ToListAsync();
                    
                    var BookingIdData = new
                    {
                        booking,
                        FirstFlightTickets,
                        ConnectingFlightTickets
                    };

                    BookingData.Add(BookingIdData);
                    
                }
                else if (isDirectFlight != null && isConnectingFlight == null)
                {
                    var FirstFlightTickets = await _context.FlightTickets.Where(ft => ft.BookingId == booking.BookingId).ToListAsync();

                    var BookingIdData = new
                    {
                        booking,
                        FirstFlightTickets,
                    };

                    BookingData.Add(BookingIdData);
                }
            }

            return Ok(BookingData);
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

                return Ok("Tickets Booked Successfully");
            }

            return BadRequest("No Details To Make Booking");

        }

        // POST: api/Bookings/PartnerBookings
        [HttpPost("partnerbookings")]
        public async Task<ActionResult<BookingModel>> PartnerBooking(PartnerBookingModel booking)
        {
            if (_context.Bookings == null)
            {
                return Problem("Entity set 'AirlineDbContext.Bookings' is null.");
            }

                    if (booking.PassengerInfos != null && booking.PassengerInfos.Count > 0)
                    {
                        foreach (var passengerInfo in booking.PassengerInfos)
                        {
                                var connectionTicket = new PartnerBooking
                                {
                                    TicketNo= booking.TicketNo,
                                    BookingId = booking.BookingId,
                                    FlightName = booking.FlightName,
                                    AirlineName = booking.AirlineName,
                                    SourceAirportId = booking.SourceAirportId,
                                    DestinationAirportId = booking.DestinationAirportId,
                                    SeatNo = passengerInfo.SeatNo,
                                    Name = passengerInfo.Name,
                                    Age = passengerInfo.Age,
                                    Gender = passengerInfo.Gender,
                                    DateTime = booking.DateTime,
                                    Status = booking.Status,
                                };
                        await _context.PartnerBookings.AddAsync(connectionTicket);
                        }
                    }

                await _context.SaveChangesAsync();

                return Ok("Tickets Booked Successfully");
        }

        [HttpPut("cancelbooking/{bookingid}")]
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


        [HttpPut("cancelpartnerbooking/{bookingid}")]
        public async Task<ActionResult<BookingModel>> CancelPartnerBooking([FromRoute] Guid bookingid)
        {
            if (_context.Bookings == null)
            {
                return Problem("Entity set 'AirlineDbContext.Bookings' is null.");
            }

            var bookings = await _context.PartnerBookings.Where(pb=> pb.BookingId == bookingid).ToListAsync();

            if (bookings == null)
            {
                return BadRequest("BookingID Does Not Exist");
            }

            foreach (var booking in bookings)
            {
                var schedule =await _context.FlightSchedules.FirstOrDefaultAsync(fs=> fs.FlightName == booking.FlightName && fs.SourceAirportId == booking.SourceAirportId && 
                fs.DestinationAirportId == booking.DestinationAirportId && fs.DateTime == booking.DateTime
                );
                                
                var seat = await _context.Seats.FirstOrDefaultAsync(s => s.ScheduleId == schedule.ScheduleId && s.SeatNumber == booking.SeatNo);

                seat.Status = "Available";

            }

            await _context.SaveChangesAsync();
            return Ok("Your Booking Has Been Cancelled");
        }


        [HttpPut("canceltickets/{bookingid}")]
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

        [HttpPut("cancelticketsinpartnerbooking/{bookingid}")]
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
                var schedule = await _context.FlightSchedules.FirstOrDefaultAsync(fs => fs.FlightName == booking.FlightName && fs.SourceAirportId == booking.SourceAirportId &&
                fs.DestinationAirportId == booking.DestinationAirportId && fs.DateTime == booking.DateTime
                );

                var seat = await _context.Seats.FirstOrDefaultAsync(s => s.ScheduleId == schedule.ScheduleId && s.SeatNumber == booking.SeatNo);

                seat.Status = "Available";

            }

            await _context.SaveChangesAsync();
            return Ok("Your Tickets Has Been Cancelled");
        }


        private bool BookingExists(Guid id)
        {
            return (_context.Bookings?.Any(e => e.BookingId == id)).GetValueOrDefault();
        }
    }
}
