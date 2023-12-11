using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SanoshAirlines.Models;

namespace SanoshAirlines.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlightSchedulesController : ControllerBase
    {
        private readonly AirlineDbContext _context;

        public FlightSchedulesController(AirlineDbContext context)
        {
            _context = context;
        }

        // GET: api/FlightSchedules
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FlightSchedule>>> GetFlightSchedules()
        {
            if (_context.FlightSchedules == null)
            {
                return NotFound();
            }
            return await _context.FlightSchedules.ToListAsync();
        }


        // GET: api/FlightSchedules
        [HttpGet("{source}/{destination}/{date}")]
        public async Task<ActionResult<IEnumerable<FlightSchedule>>> GetDirectFlightSchedule([FromRoute] string source, [FromRoute] string destination, [FromRoute] DateTime date)
        {
            if (_context.FlightSchedules == null)
            {
                return NotFound();
            }
            var schedules = await _context.FlightSchedules.Where(s => s.SourceAirportId == source && s.DestinationAirportId == destination && s.DateTime.Date == date.Date).ToListAsync();
            return Ok(schedules);
        }



        // GET: api/FlightSchedules
        [HttpGet("connectingflight/{source}/{destination}/{date}")]
        public async Task<ActionResult<IEnumerable<FlightSchedule>>> GetConnectingFlightSchedule([FromRoute] string source, [FromRoute] string destination, [FromRoute] DateTime date)
        {
            if (_context.FlightSchedules == null)
            {
                return NotFound();
            }
            var schedules = await _context.FlightSchedules.Where(s => s.SourceAirportId == source && s.DestinationAirportId != destination && s.DateTime.Date == date.Date).ToListAsync();
            return Ok(schedules);
        }


        // GET: api/FlightSchedules
        [HttpGet("{scheduleid}")]
        public async Task<ActionResult<IEnumerable<FlightSchedule>>> GetExactFlightSchedule([FromRoute]int scheduleid)
        {
            if (_context.FlightSchedules == null)
            {
                return NotFound();
            }
     
            var schedule = await _context.FlightSchedules.FirstOrDefaultAsync(s => s.ScheduleId == scheduleid);
            return Ok(schedule);
        }



        // PUT: api/FlightSchedules/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFlightSchedule([FromRoute]int id,[FromBody] FlightSchedule flightSchedule)
        {
            if (id != flightSchedule.ScheduleId)
            {
                return BadRequest();
            }

            _context.Entry(flightSchedule).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FlightScheduleExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/FlightSchedules
        [HttpPost("{months}")]
        public IActionResult AddFlightSchedule([FromBody] FlightSchedule schedule, [FromRoute] int months)
        {
            if (!ModelState.IsValid || months <= 0)
            {
                return BadRequest("Invalid input or months should be a positive number.");
            }

            if (schedule.SourceAirportId == schedule.DestinationAirportId)
            {
                return BadRequest("Source and Destination cannot be the same");
            }

            var sourceAirport = _context.Airports.Find(schedule.SourceAirportId);
            var destinationAirport = _context.Airports.Find(schedule.DestinationAirportId);

            if (sourceAirport == null || destinationAirport == null)
            {
                return BadRequest("Source or Destination airport does not exist.");
            }

            var existingFlight = _context.FlightSchedules.FirstOrDefault(f =>
               f.FlightName == schedule.FlightName &&
               f.DateTime == schedule.DateTime);

            if (existingFlight != null)
            {
                return BadRequest("Flight already scheduled at this datetime.");
            }

            DateTime departureDateTime = schedule.DateTime;
            string flightName = schedule.FlightName;
            string sourceAirportId = schedule.SourceAirportId;
            string destinationAirportId = schedule.DestinationAirportId;

            List<FlightSchedule> schedulesToAdd = new List<FlightSchedule>();

            try
            {
                for (int i = 0; i < months * DateTime.DaysInMonth(departureDateTime.Year, departureDateTime.Month); i++)
                {

                    var addedSchedule = new FlightSchedule
                    {
                        DateTime = departureDateTime.AddDays(i),
                        FlightName = flightName,
                        SourceAirportId = sourceAirportId,
                        DestinationAirportId = destinationAirportId,
                        FlightDuration = schedule.FlightDuration,
                    };

                    _context.FlightSchedules.Add(addedSchedule);
                    _context.SaveChanges(); 

                    var flight = _context.FlightDetails.FirstOrDefault(f => f.FlightName == addedSchedule.FlightName);
                    if (flight != null)
                    {
                        int totalSeats = flight.FlightCapacity;
                        var seatsToAdd = new List<Seat>();

                        for (int seatIndex = 1; seatIndex <= totalSeats; seatIndex++)
                        {
                            char seatRow = (char)('A' + ((seatIndex - 1) % 6));
                            int seatNumber = (seatIndex - 1) / 6 + 1;

                            string seatId = $"{seatRow}{seatNumber}";

                            seatsToAdd.Add(new Seat
                            {
                                ScheduleId = addedSchedule.ScheduleId, 
                                SeatNumber = seatId,
                                Status = "Available"
                            });
                        }

                        _context.Seats.AddRange(seatsToAdd);
                    }
                }

                _context.SaveChanges();

                return Ok("Flight schedules and seats added successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }

        }


        // DELETE: api/FlightSchedules
        [HttpDelete]
        public async Task<IActionResult> DeleteFlightSchedules([FromBody] List<int> ids)
        {
            if (_context.FlightSchedules == null || ids == null || ids.Count == 0)
            {
                return BadRequest("Invalid input or empty list of IDs.");
            }

            var flightSchedules = await _context.FlightSchedules.Where(fs => ids.Contains(fs.ScheduleId)).ToListAsync();

            if (flightSchedules == null || flightSchedules.Count == 0)
            {
                return NotFound("No FlightSchedules found for the given IDs.");
            }


            _context.FlightSchedules.RemoveRange(flightSchedules);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool FlightScheduleExists(int id)
        {
            return (_context.FlightSchedules?.Any(e => e.ScheduleId == id)).GetValueOrDefault();
        }

    }
}
