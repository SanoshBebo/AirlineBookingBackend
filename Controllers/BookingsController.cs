using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Humanizer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SanoshAirlines.Models;
using SanoshAirlines.Models.RequestBodyModels;
using Microsoft.AspNetCore.Authorization;
using System.Globalization;

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

        [Authorize]
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
                return Ok("No Bookings");
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
   
                    var sourceAirport = await _context.Airports
                    .FirstOrDefaultAsync(a => a.AirportId == ticket.SourceAirportId);

                    var destinationAirport = await _context.Airports
                        .FirstOrDefaultAsync(a => a.AirportId == ticket.DestinationAirportId);

                    var bookingTicketData = new
                    {
                        Ticket = ticket,
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
        [Authorize]
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

            var bookingData = new List<object>();

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

                var sourceAirport = await _context.Airports
                .FirstOrDefaultAsync(a => a.AirportId == ticket.SourceAirportId);

                var destinationAirport = await _context.Airports
                    .FirstOrDefaultAsync(a => a.AirportId == ticket.DestinationAirportId);

                var bookingTicketData = new
                {
                    Ticket = ticket,
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

            return Ok(bookingData);
        }


        // GET: api/Bookings/5
        [Authorize]
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


        [Authorize]
        [HttpPost("sendBookingTickets/{email}")]
        public async Task<ActionResult<ConnectionFlightTicket>> SendBookingTickets([FromRoute] string email, [FromBody] List<ConnectionFlightTicketDto> tickets)
        {

            string fromMail = "businessreports8@gmail.com";
            string fromPassword = "dmvibdlolcdpmavr";

            MailMessage message = new MailMessage();
            message.From = new MailAddress(fromMail);
            message.Subject = "Booking Details!";
            message.To.Add(new MailAddress($"{email}"));
            message.Body = EmailTemplates.GetFlightTickets(email,tickets);
            message.IsBodyHtml = true;

            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(fromMail, fromPassword),
                EnableSsl = true,
            };

            smtpClient.Send(message);

            return Ok();
        }

        [Authorize]
        [HttpPost("cancelBookingTickets/{email}")]
        public async Task<ActionResult<ConnectionFlightTicket>> CancelBookingTickets([FromRoute] string email, [FromBody] List<ConnectionFlightTicketDto> tickets)
        {

            string fromMail = "businessreports8@gmail.com";
            string fromPassword = "dmvibdlolcdpmavr";

            MailMessage message = new MailMessage();
            message.From = new MailAddress(fromMail);
            message.Subject = "Ticket Cancellation";
            message.To.Add(new MailAddress($"{email}"));
            message.Body = EmailTemplates.CancelFlightTickets(email, tickets);
            message.IsBodyHtml = true;

            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(fromMail, fromPassword),
                EnableSsl = true,
            };

            smtpClient.Send(message);

            return Ok();
        }

        [Authorize]
        [HttpPost("cancelBookingEmail/{email}")]
        public async Task<ActionResult<ConnectionFlightTicket>> CancelBookingViaEmail([FromRoute] string email, [FromBody] List<ConnectionFlightTicketDto> tickets)
        {

            string fromMail = "businessreports8@gmail.com";
            string fromPassword = "dmvibdlolcdpmavr";

            MailMessage message = new MailMessage();
            message.From = new MailAddress(fromMail);
            message.Subject = "Booking Cancelled";
            message.To.Add(new MailAddress($"{email}"));
            message.Body = EmailTemplates.CancelBooking(email, tickets);
            message.IsBodyHtml = true;

            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(fromMail, fromPassword),
                EnableSsl = true,
            };

            smtpClient.Send(message);

            return Ok();
        }


        // POST: api/Bookings
       /* [Authorize]*/
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
                                    Status = "Booked",
                                };

                                var seat = _context.Seats.FirstOrDefault(s => s.SeatNumber == passengerInfo.SeatNo && s.ScheduleId == booking.ScheduleId);
                             
                                seat.Status = "Booked";
                                
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
                                    Status = "Booked",

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


        [Authorize]
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

                booking.Status = "Cancelled";
                var isDirectFlight = _context.FlightTickets.FirstOrDefault(ft => ft.BookingId == booking.BookingId);
                var isConnectingFlight = _context.ConnectionFlightTickets.FirstOrDefault(cft => cft.BookingId == booking.BookingId);
                if (isDirectFlight != null)
                {
                    var FirstFlightTickets = await _context.FlightTickets.Where(ft => ft.BookingId == booking.BookingId).ToListAsync();

                    foreach (var Ticket in FirstFlightTickets)
                    {
                    Ticket.Status = "Cancelled";
                        var Seat = _context.Seats.FirstOrDefault(s => s.ScheduleId == Ticket.ScheduleId && s.SeatNumber == Ticket.SeatNo);
                        Seat.Status = "Available";
                    }
                }
                
                if (isConnectingFlight != null)
                {

                var ConnectingFlightTickets = await _context.ConnectionFlightTickets.Where(ft => ft.BookingId == booking.BookingId).ToListAsync();

                foreach (var Ticket in ConnectingFlightTickets)
                {
                    Ticket.Status = "Cancelled";
                }

                await _context.SaveChangesAsync();
                    return Ok(new { message = "Your Booking Has been Cancelled", connectingFlightTickets = ConnectingFlightTickets });
                }
            await _context.SaveChangesAsync();
            return Ok("Your Booking Has Been Cancelled");
        }




        [Authorize]
        [HttpPatch("canceltickets/{bookingid}")]
        public async Task<ActionResult<BookingModel>> CancelTicketsInABooking([FromRoute] Guid bookingid, [FromBody] List<string> Names)
        {
            if (_context.Bookings == null)
            {
                return Problem("Entity set 'AirlineDbContext.Bookings' is null.");
            }

            var booking = await _context.Bookings.FindAsync(bookingid);
            bool allTicketsCancelled = false; // Initialize the flag

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
                    Ticket.Status = "Cancelled";
                    var Seat = _context.Seats.FirstOrDefault(s => s.ScheduleId == Ticket.ScheduleId && s.SeatNumber == Ticket.SeatNo);
                    Seat.Status = "Available";
                }

                allTicketsCancelled = await _context.FlightTickets
                .Where(ft => ft.BookingId == booking.BookingId)
                .AllAsync(ft => ft.Status == "Cancelled");


            }

            if (isConnectingFlight != null)
            {
                var ConnectingFlightTickets = await _context.ConnectionFlightTickets.Where(cft => cft.BookingId == booking.BookingId && Names.Contains(cft.Name)).ToListAsync();
                foreach (var Ticket in ConnectingFlightTickets)
                {
                    Ticket.Status = "Cancelled";
                }

                await _context.SaveChangesAsync();

                    bool allConnectingTicketsCancelled = await _context.ConnectionFlightTickets
                .Where(cft => cft.BookingId == booking.BookingId)
                .AllAsync(cft => cft.Status == "Cancelled");

                allTicketsCancelled = allConnectingTicketsCancelled && allTicketsCancelled;

                if (allTicketsCancelled)
                {
                    booking.Status = "Cancelled";
                }

                await _context.SaveChangesAsync();

                return Ok(new { message = "Your Ticket Has been Cancelled", connectingFlightTickets = ConnectingFlightTickets });
            }

            if (allTicketsCancelled)
            {
                booking.Status = "Cancelled";
            }

            await _context.SaveChangesAsync();

            return Ok("Your Ticket Has Been Cancelled");
        }

      

        private bool BookingExists(Guid id)
        {
            return (_context.Bookings?.Any(e => e.BookingId == id)).GetValueOrDefault();
        }
    }

    public partial class ConnectionFlightTicketDto
    {
       
        public int TicketNo { get; set; }

        public Guid BookingId { get; set; }

        public string FlightName { get; set; } = null!;

        public string? SourceAirportId { get; set; }

        public string? DestinationAirportId { get; set; }

        public string SeatNo { get; set; } = null!;

        public string Name { get; set; } = null!;

        public int Age { get; set; }

        public string Gender { get; set; } = null!;

        public DateTime DateTime { get; set; }

    }
}
