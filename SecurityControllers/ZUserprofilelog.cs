using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Enterprise.Models;

namespace somecontrollers.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserProfileLogController : ControllerBase
    {
        private readonly EnterpriseContext _context;

        public UserProfileLogController(EnterpriseContext context)
        {
            _context = context;
        }

        // GET: api/UserProfileLog
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserProfileLog>>> Get()
        {
            return await _context.UserProfileLogs
                .OrderByDescending(x => x.DateCreated)
                .ToListAsync();
        }

        // GET: api/UserProfileLog/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserProfileLog>> Get(int id)
        {
            var profileLog = await _context.UserProfileLogs.FindAsync(id);

            if (profileLog == null)
            {
                return NotFound();
            }

            return profileLog;
        }

        // GET: api/UserProfileLog/User/123
        [HttpGet("User/{uid}")]
        public async Task<ActionResult<IEnumerable<UserProfileLog>>> GetByUid(int uid)
        {
            return await _context.UserProfileLogs
                .Where(x => x.Uid == uid)
                .OrderByDescending(x => x.DateCreated)
                .ToListAsync();
        }

        // POST: api/UserProfileLog
        [HttpPost]
        public async Task<ActionResult<UserProfileLog>> Post(UserProfileLog profileLog)
        {
            if (profileLog.DateCreated == default)
            {
                profileLog.DateCreated = DateTime.UtcNow;
            }

            _context.UserProfileLogs.Add(profileLog);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(Get),
                new { id = profileLog.Id },
                profileLog);
        }

        // PUT: api/UserProfileLog/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, UserProfileLog profileLog)
        {
            if (id != profileLog.Id)
            {
                return BadRequest();
            }

            _context.Entry(profileLog).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserProfileLogExists(id))
                {
                    return NotFound();
                }

                throw;
            }

            return NoContent();
        }

        // DELETE: api/UserProfileLog/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var profileLog = await _context.UserProfileLogs.FindAsync(id);

            if (profileLog == null)
            {
                return NotFound();
            }

            _context.UserProfileLogs.Remove(profileLog);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserProfileLogExists(int id)
        {
            return _context.UserProfileLogs.Any(e => e.Id == id);
        }
    }
}
