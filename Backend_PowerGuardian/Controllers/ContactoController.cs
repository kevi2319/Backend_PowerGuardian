using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend_PowerGuardian.Data;
using Backend_PowerGuardian.Models;

namespace Backend_PowerGuardian.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactoController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ContactoController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Contacto>>> GetContactos()
        {
            return await _context.Contactos
                .Include(c => c.Producto)
                .OrderByDescending(c => c.Fecha)
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Contacto>> GetContacto(int id)
        {
            var contacto = await _context.Contactos
                .Include(c => c.Producto)
                .FirstOrDefaultAsync(c => c.ProductoId == id);

            if (contacto == null)
                return NotFound();

            return contacto;
        }

        [HttpPost]
        public async Task<IActionResult> PostContacto([FromBody] Contacto contacto)
        {
            var producto = await _context.Productos.FindAsync(contacto.ProductoId);
            if (producto == null)
                return BadRequest(new { mensaje = "Producto no válido." });

            _context.Contactos.Add(contacto);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Mensaje enviado correctamente." });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutContacto(int id, Contacto contacto)
        {
            if (id != contacto.ProductoId)
                return BadRequest();

            var producto = await _context.Productos.FindAsync(contacto.ProductoId);
            if (producto == null)
                return BadRequest(new { mensaje = "Producto no válido." });

            _context.Entry(contacto).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ContactoExists(id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContacto(int id)
        {
            var contacto = await _context.Contactos.FindAsync(id);
            if (contacto == null)
                return NotFound();

            _context.Contactos.Remove(contacto);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ContactoExists(int id)
        {
            return _context.Contactos.Any(e => e.ProductoId == id);
        }
    }
}
