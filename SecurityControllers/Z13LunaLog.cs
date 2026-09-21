using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Enterprise.Models;

namespace somecontrollers.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LunaLogController : ControllerBase
    {
        private readonly EnterpriseContext _context;

        public LunaLogController(EnterpriseContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Lunalog>>> Get()
        {
            return await _context.LunaLogs
                .OrderByDescending(x => x.AccessTime)
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Lunalog>> Get(int id)
        {
            var log = await _context.LunaLogs.FindAsync(id);

            if (log == null)
                return NotFound();

            return log;
        }

        [HttpPost]
        public async Task<ActionResult<Lunalog>> Post(Lunalog lunaLog)
        {
            if (lunaLog.AccessTime == null)
                lunaLog.AccessTime = DateTime.UtcNow;

            _context.LunaLogs.Add(lunaLog);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(Get),
                new { id = lunaLog.Id },
                lunaLog);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, Lunalog lunaLog)
        {
            if (id != lunaLog.Id)
                return BadRequest();

            _context.Entry(lunaLog).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LunaLogExists(id))
                    return NotFound();

                throw;
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var log = await _context.LunaLogs.FindAsync(id);

            if (log == null)
                return NotFound();

            _context.LunaLogs.Remove(log);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool LunaLogExists(int id)
        {
            return _context.LunaLogs.Any(e => e.Id == id);
        }
    }
}
