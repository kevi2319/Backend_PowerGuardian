using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Backend_PowerGuardian.Data;
using Backend_PowerGuardian.Models;
using Microsoft.AspNetCore.Authorization;
using Backend_PowerGuardian.DTOs;

namespace Backend_PowerGuardian.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ManualesController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _env;

        public ManualesController(ApplicationDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        // GET: api/manuales
        [HttpGet]
        public async Task<IActionResult> Listar()
        {
            var manuales = await _db.Manuales
                .Where(m => !m.Eliminado)
                .ToListAsync();

            return Ok(manuales);
        }

        // POST: api/manuales
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Subir([FromForm] ManualUploadDto dto)
        {
            var archivo = dto.Archivo;
            if (archivo == null || archivo.Length == 0)
                return BadRequest("Archivo no válido");

            var folder = Path.Combine(_env.WebRootPath, "manuales");
            Directory.CreateDirectory(folder);

            var filePath = Path.Combine(folder, archivo.FileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await archivo.CopyToAsync(stream);
            }

            var manual = new Manual
            {
                Nombre = dto.Nombre,
                Descripcion = dto.Descripcion,
                UrlArchivo = $"/manuales/{archivo.FileName}"
            };

            _db.Manuales.Add(manual);
            await _db.SaveChangesAsync();

            return Ok(manual);
        }

        // DELETE: api/manuales/{id}/eliminar (soft delete)
        [HttpPut("{id}/eliminar")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var manual = await _db.Manuales.FindAsync(id);
            if (manual == null) return NotFound();

            manual.Eliminado = true;
            await _db.SaveChangesAsync();

            return Ok();
        }

    }

    // ...ManualUploadDto movido a DTOs/ManualUploadDto.cs...

}
