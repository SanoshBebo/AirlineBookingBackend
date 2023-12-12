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
    public class SeatsController : ControllerBase
    {
        private readonly AirlineDbContext _context;

        public SeatsController(AirlineDbContext context)
        {
            _context = context;
        }

  
        // GET: api/Seats/5
        [HttpGet("{scheduleid}")]
        public async Task<ActionResult<Seat>> GetSeatsForSchedule(int scheduleid)
        {
          if (_context.Seats == null)
          {
              return NotFound();
          }
            var seats = await _context.Seats.Where(s=> s.ScheduleId == scheduleid).ToListAsync();

            if (seats == null)
            {
                return NotFound();
            }

            return Ok(seats);
        }

        // PUT: api/Seats/5/bookedoravailable   
        //to book seats
        [HttpPut("{scheduleId}/{status}")]
        public async Task<IActionResult> ChangeSeatStatus([FromRoute]int scheduleId, [FromRoute] string status, [FromBody]List<string> seatNumbers)
        {
            if(seatNumbers.Count == 0)
            {
                return BadRequest("No Seats Selected");
            }

            foreach (var seatno in seatNumbers)
            {
                var seat = _context.Seats.FirstOrDefault(s => s.SeatNumber == seatno && s.ScheduleId == scheduleId);
                if(seat != null)
                {
                    if(seat.Status == status)
                    {
                        return BadRequest($"Seat Is Already {status}");
                    }
                    seat.Status = status;
                }
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return NotFound();
            }

            return Ok($"Seats have been changed to {status} ");
        }
    }
}
