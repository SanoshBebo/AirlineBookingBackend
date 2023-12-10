using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SanoshAirlines.Models;
using SanoshAirlines.Models.RequestBodyModels;

namespace SanoshAirlines.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlightDetailsController : ControllerBase
    {
        private readonly AirlineDbContext _context;

        public FlightDetailsController(AirlineDbContext context)
        {
            _context = context;
        }

        // GET: api/FlightDetails
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FlightDetail>>> GetFlightDetails()
        {
          if (_context.FlightDetails == null)
          {
              return NotFound();
          }
            return await _context.FlightDetails.ToListAsync();
        }

        // GET: api/FlightDetails/5
        [HttpGet("{id}")]
        public async Task<ActionResult<FlightDetail>> GetFlightDetail(string id)
        {
          if (_context.FlightDetails == null)
          {
              return NotFound();
          }
            var flightDetail = await _context.FlightDetails.FindAsync(id);

            if (flightDetail == null)
            {
                return NotFound();
            }

            return flightDetail;
        }

        // PUT: api/FlightDetails/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFlightDetail(string id, FlightCapAndStatus flightDetail)
        {
            var Flight = _context.FlightDetails.FirstOrDefault(fd => fd.FlightName == id);

            if(Flight == null)
            {
                return BadRequest("Flight Not Found");
                

            }

            Flight.FlightCapacity = flightDetail.FlightCapacity;
            Flight.IsActive = flightDetail.IsActive;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FlightDetailExists(id))
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

        [HttpPost]
        public IActionResult PostFlightDetails(FlightCapAndStatus details)
        {
            var latestFlight = _context.FlightDetails.OrderByDescending(f => f.FlightName).FirstOrDefault();

            string flightName = "SanoshAirline1"; // Default value if no flights exist yet

            if (latestFlight != null)
            {
                string latestFlightName = latestFlight.FlightName;
                int flightNumber;
                if (int.TryParse(latestFlightName.Replace("SanoshAirline", ""), out flightNumber))
                {
                    flightNumber++;
                    flightName = "SanoshAirline" + flightNumber;
                }
                else
                {
                    throw new InvalidOperationException("Invalid flight name format");
                }
            }

            FlightDetail newFlight = new FlightDetail
            {
                FlightName = flightName,
                FlightCapacity = details.FlightCapacity,
                IsActive = details.IsActive
            };

            _context.FlightDetails.Add(newFlight);
            _context.SaveChanges();
            return Ok();
        }


        private bool FlightDetailExists(string id)
        {
            return (_context.FlightDetails?.Any(e => e.FlightName == id)).GetValueOrDefault();
        }
    }
}
