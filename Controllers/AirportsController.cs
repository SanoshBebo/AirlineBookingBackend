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
    public class AirportsController : ControllerBase
    {
        private readonly AirlineDbContext _context;

        public AirportsController(AirlineDbContext context)
        {
            _context = context;
        }

        // GET: api/Airports
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Airport>>> GetAirports()
        {
          if (_context.Airports == null)
          {
              return BadRequest();
          }
            return await _context.Airports.ToListAsync();
        }

        // GET: api/Airports/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Airport>> GetAirport(string id)
        {
          if (_context.Airports == null)
          {
              return NotFound();
          }
            var airport = await _context.Airports.FindAsync(id);

            if (airport == null)
            {
                return NotFound();
            }

            return airport;
        }

        // PUT: api/Airports/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAirport(string id, Airport airport)
        {
            if (id != airport.AirportId)
            {
                return BadRequest();
            }

            _context.Entry(airport).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AirportExists(id))
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

        // POST: api/Airports
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Airport>> PostAirport(Airport airport)
        {
          if (_context.Airports == null)
          {
              return Problem("Entity set 'AirlineDbContext.Airports'  is null.");
          }
            _context.Airports.Add(airport);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (AirportExists(airport.AirportId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetAirport", new { id = airport.AirportId }, airport);
        }

        // DELETE: api/Airports/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAirport(string id)
        {
            if (_context.Airports == null)
            {
                return NotFound();
            }
            var airport = await _context.Airports.FindAsync(id);
            if (airport == null)
            {
                return NotFound();
            }

            _context.Airports.Remove(airport);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AirportExists(string id)
        {
            return (_context.Airports?.Any(e => e.AirportId == id)).GetValueOrDefault();
        }
    }
}
