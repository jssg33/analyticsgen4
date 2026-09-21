using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Enterprise.Models;

namespace somecontrollers.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SysLogController : ControllerBase
    {
        private readonly EnterpriseContext _context;

        public SysLogController(EnterpriseContext context)
        {
            _context = context;
        }

        // GET: api/SysLog
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Syslog>>> Get()
        {
            return await _context.Syslogs
                .OrderByDescending(x => x.LogDate)
                .ToListAsync();
        }

        // GET: api/SysLog/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Syslog>> Get(int id)
        {
            var sysLog = await _context.Syslogs.FindAsync(id);

            if (sysLog == null)
            {
                return NotFound();
            }

            return sysLog;
        }

        // POST: api/SysLog
        [HttpPost]
        public async Task<ActionResult<Syslog>> Post(Syslog sysLog)
        {
            if (sysLog.LogDate == default)
            {
                sysLog.LogDate = DateTime.UtcNow;
            }

            _context.Syslogs.Add(sysLog);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(Get),
                new { id = sysLog.Id },
                sysLog);
        }

        // PUT: api/SysLog/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, Syslog sysLog)
        {
            if (id != sysLog.Id)
            {
                return BadRequest();
            }

            _context.Entry(sysLog).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SysLogExists(id))
                {
                    return NotFound();
                }

                throw;
            }

            return NoContent();
        }

        // DELETE: api/SysLog/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var sysLog = await _context.Syslogs.FindAsync(id);

            if (sysLog == null)
            {
                return NotFound();
            }

            _context.Syslogs.Remove(sysLog);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SysLogExists(int id)
        {
            return _context.Syslogs.Any(e => e.Id == id);
        }
    }
}
