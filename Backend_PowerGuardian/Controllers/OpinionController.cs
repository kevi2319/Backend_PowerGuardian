using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend_PowerGuardian.Data;
using Backend_PowerGuardian.Models;

namespace Backend_PowerGuardian.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OpinionController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public OpinionController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Opinion>>> GetOpiniones()
        {
            return await _context.Opinion
                .OrderByDescending(o => o.Fecha)
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Opinion>> GetOpinion(int id)
        {
            var opinion = await _context.Opinion.FindAsync(id);

            if (opinion == null)
                return NotFound();

            return opinion;
        }

        [HttpPost]
        public async Task<IActionResult> PostOpinion([FromBody] Opinion opinion)
        {
            _context.Opinion.Add(opinion);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Opinión registrada correctamente." });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutOpinion(int id, [FromBody] Opinion opinion)
        {
            if (id != opinion.Id)
                return BadRequest();

            _context.Entry(opinion).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OpinionExists(id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOpinion(int id)
        {
            var opinion = await _context.Opinion.FindAsync(id);
            if (opinion == null)
                return NotFound();

            _context.Opinion.Remove(opinion);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool OpinionExists(int id)
        {
            return _context.Opinion.Any(e => e.Id == id);
        }
    }
}
