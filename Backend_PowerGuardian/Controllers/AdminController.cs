using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Backend_PowerGuardian.Models;
using Backend_PowerGuardian.Services;
using Backend_PowerGuardian.DTOs;
using Backend_PowerGuardian.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/admin")]
public class AdminController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _context;

    public AdminController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    [HttpGet("clientes")]
    public async Task<IActionResult> ObtenerClientes()
    {
        var usuarios = await _userManager.Users.ToListAsync();
        var filteredUsuarios = new List<ApplicationUser>();

        foreach (var usuario in usuarios)
        {
            if (await _userManager.IsInRoleAsync(usuario, "Cliente"))
            {
                filteredUsuarios.Add(usuario);
            }
        }

        var lista = filteredUsuarios.Select(u => new ClienteAdminDto
        {
            Id = u.Id,
            NombreCompleto = $"{u.Nombres} {u.ApellidoPaterno} {u.ApellidoMaterno}".Trim(),
            Email = u.Email,
            Telefono = u.PhoneNumber ?? "",
            FechaRegistro = u.FechaRegistro,
            Dispositivos = _context.Dispositivos.Count(d => d.UsuarioId == u.Id),
            Activo = u.IsActive
        }).ToList();

        return Ok(lista);
    }

    [HttpPost("clientes/{id}/desactivar")]
    public async Task<IActionResult> DesactivarCliente(string id)
    {
        var usuario = await _userManager.FindByIdAsync(id);
        if (usuario == null) return NotFound("Usuario no encontrado");

        usuario.IsActive = false;
        await _userManager.UpdateAsync(usuario);

        return Ok(new { message = "Usuario desactivado correctamente" });

        //todo: no permitir login a usuarios desactivados
    }

    [HttpPost("clientes/{id}/activar")]
    public async Task<IActionResult> ActivarCliente(string id)
    {
        var usuario = await _userManager.FindByIdAsync(id);
        if (usuario == null) return NotFound("Usuario no encontrado");

        usuario.IsActive = true;
        await _userManager.UpdateAsync(usuario);

        return Ok(new { message = "Usuario activado correctamente" });
    }

    [HttpPost("inventario")]
    public async Task<IActionResult> CrearInventario([FromBody] InventarioCreateDto dto)
    {
        var producto = await _context.Productos.FindAsync(dto.ProductoId);
        if (producto == null)
            return BadRequest("Producto no encontrado.");

        var inventario = new Inventario
        {
            ProductoId = dto.ProductoId,
            Estado = "disponible",
            FechaIngreso = DateTime.UtcNow,
            Notas = dto.Notas
        };

        _context.Inventarios.Add(inventario);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Unidad agregada al inventario correctamente." });
    }

    [HttpPut("inventario/{id}")]
    public async Task<IActionResult> EditarInventario(int id, [FromBody] InventarioCreateDto dto)
    {
        var inv = await _context.Inventarios.FindAsync(id);
        if (inv == null) return NotFound("Unidad no encontrada");

        inv.ProductoId = dto.ProductoId;
        inv.Notas = dto.Notas;
        await _context.SaveChangesAsync();

        return Ok(new { message = "Unidad actualizada correctamente." });
    }

    [HttpDelete("inventario/{id}")]
    public async Task<IActionResult> EliminarInventario(int id)
    {
        var inv = await _context.Inventarios.FindAsync(id);
        if (inv == null) return NotFound("Unidad no encontrada");

        _context.Inventarios.Remove(inv);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Unidad eliminada correctamente." });
    }

    [HttpGet("inventario")]
    public async Task<IActionResult> ListarInventario()
    {
        var inventarios = await _context.Inventarios
            .Include(i => i.Producto)
            .ToListAsync();

        var inventarioDtos = inventarios.Select(i => new InventarioDto
        {
            Id = i.Id,
            ProductoId = i.ProductoId,
            ProductoNombre = i.Producto.Nombre,
            Estado = i.Estado,
            FechaIngreso = i.FechaIngreso,
            Notas = i.Notas,

            Sku = _context.ProductoUnidades
                .Where(p => p.ProductoId == i.ProductoId && p.Usado && i.UsuarioId != null)
                .OrderByDescending(p => p.FechaCompra)
                .Select(p => p.SKU)
                .FirstOrDefault(),

            ClienteUsuario = _context.Users
                .Where(u => u.Id == i.UsuarioId)
                .Select(u => u.UserName)
                .FirstOrDefault()
        });

        return Ok(inventarioDtos);
    }

    [HttpPost("inventario/agregar-bulk")]
    public async Task<IActionResult> AgregarInventarioBulk([FromBody] AgregarInventarioBulkDto dto)
    {
        if (dto.Cantidad <= 0)
            return BadRequest("La cantidad debe ser mayor a cero.");

        var productoExiste = await _context.Productos.AnyAsync(p => p.Id == dto.ProductoId);
        if (!productoExiste)
            return BadRequest("Producto no encontrado.");

        var lista = new List<Inventario>();

        for (int i = 0; i < dto.Cantidad; i++)
        {
            lista.Add(new Inventario
            {
                ProductoId = dto.ProductoId,
                Estado = "disponible",
                FechaIngreso = DateTime.UtcNow,
                Notas = dto.Notas ?? ""
            });
        }

        _context.Inventarios.AddRange(lista);
        await _context.SaveChangesAsync();

        return Ok(new { message = $"{dto.Cantidad} unidades agregadas correctamente al inventario." });
    }

    [HttpGet("resenas")]
    public async Task<IActionResult> ObtenerResenas()
    {
        var resenas = await _context.Resenas
            .Include(r => r.ProductoUnidad)
                .ThenInclude(pu => pu.Producto)
            .Include(r => r.Usuario)
            .Select(r => new ResenaAdminDto
            {
                Id = r.Id,
                ProductoNombre = r.ProductoUnidad.Producto.Nombre,
                SKU = r.ProductoUnidad.SKU,
                Cliente = r.Usuario.Nombres + " " + r.Usuario.ApellidoPaterno,
                Calificacion = r.Calificacion,
                Comentario = r.Comentario,
                Fecha = r.Fecha
            })
            .ToListAsync();

        return Ok(resenas);
    }

    [HttpDelete("resenas/{id}")]
    public async Task<IActionResult> EliminarResena(int id)
    {
        var resena = await _context.Resenas.FindAsync(id);
        if (resena == null)
            return NotFound("Reseña no encontrada");

        _context.Resenas.Remove(resena);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Reseña eliminada" });
    }

    // Proveedores
    [HttpGet("proveedores")]
    public async Task<IActionResult> GetProveedores()
    {
        var proveedores = await _context.Proveedores
            .Select(p => new ProveedorDto
            {
                Id = p.Id,
                Nombre = p.Nombre,
                Correo = p.Correo,
                Telefono = p.Telefono,
                Direccion = p.Direccion,
                Pais = p.Pais,
                Activo = p.Activo
            }).ToListAsync();

        return Ok(proveedores);
    }

    [HttpPost("proveedores")]
    public async Task<IActionResult> CrearProveedor([FromBody] ProveedorDto dto)
    {
        var proveedor = new Proveedor
        {
            Nombre = dto.Nombre,
            Correo = dto.Correo,
            Telefono = dto.Telefono,
            Direccion = dto.Direccion,
            Pais = dto.Pais,
            Activo = dto.Activo
        };

        _context.Proveedores.Add(proveedor);
        await _context.SaveChangesAsync();
        return Ok();
    }

    [HttpPut("proveedores/{id}")]
    public async Task<IActionResult> EditarProveedor(int id, [FromBody] ProveedorDto dto)
    {
        var proveedor = await _context.Proveedores.FindAsync(id);
        if (proveedor == null) return NotFound();

        proveedor.Nombre = dto.Nombre;
        proveedor.Correo = dto.Correo;
        proveedor.Telefono = dto.Telefono;
        proveedor.Direccion = dto.Direccion;
        proveedor.Pais = dto.Pais;
        proveedor.Activo = dto.Activo;

        await _context.SaveChangesAsync();
        return Ok();
    }

    [HttpDelete("proveedores/{id}")]
    public async Task<IActionResult> EliminarProveedor(int id)
    {
        var proveedor = await _context.Proveedores.FindAsync(id);
        if (proveedor == null) return NotFound();

        _context.Proveedores.Remove(proveedor);
        await _context.SaveChangesAsync();
        return Ok();
    }

    [HttpPost("compras-proveedor")]
    public async Task<IActionResult> RegistrarCompraProveedor([FromBody] CompraProveedorDto dto)
    {
        if (!await _context.Proveedores.AnyAsync(p => p.Id == dto.ProveedorId))
            return BadRequest("Proveedor no válido.");

        var compra = new CompraProveedor
        {
            ProveedorId = dto.ProveedorId,
            Notas = dto.Notas,
            FechaCompra = DateTime.UtcNow,
            Detalles = dto.Detalles.Select(d => new DetalleCompraProveedor
            {
                ProductoId = d.ProductoId,
                Cantidad = d.Cantidad
            }).ToList()
        };

        _context.ComprasProveedor.Add(compra);
        await _context.SaveChangesAsync();

        // Crear unidades en inventario
        foreach (var detalle in dto.Detalles)
        {
            for (int i = 0; i < detalle.Cantidad; i++)
            {
                _context.Inventarios.Add(new Inventario
                {
                    ProductoId = detalle.ProductoId,
                    Estado = "disponible",
                    FechaIngreso = DateTime.UtcNow,
                    Notas = $"Agregado por compra ID {compra.Id}"
                });
            }
        }

        await _context.SaveChangesAsync();

        return Ok(new { message = "Compra registrada y unidades agregadas al inventario." });
    }

    [HttpGet("compras-proveedor/historial")]
    public async Task<IActionResult> ObtenerHistorialComprasProveedor()
    {
        var historial = await _context.ComprasProveedor
            .Include(cp => cp.Proveedor)
            .Include(cp => cp.Detalles!)
                .ThenInclude(d => d.Producto)
            .SelectMany(cp => cp.Detalles!.Select(detalle => new HistorialCompraProveedorDto
            {
                ProveedorNombre = cp.Proveedor.Nombre,
                Fecha = cp.FechaCompra,
                ProductoNombre = detalle.Producto.Nombre,
                Cantidad = detalle.Cantidad
            }))
            .OrderByDescending(h => h.Fecha)
            .ToListAsync();

        return Ok(historial);
    }

    // Materias Primas
    [HttpGet("materias-primas")]
    public async Task<ActionResult<IEnumerable<MateriaPrimaDto>>> Obtener()
    {
        var lista = await _context.MateriasPrimas
            .Select(m => new MateriaPrimaDto
            {
                Id = m.Id,
                Nombre = m.Nombre,
                UnidadMedida = m.UnidadMedida,
                CostoUnitario = m.CostoUnitario,
                Activo = m.Activo
            }).ToListAsync();

        return Ok(lista);
    }

    [HttpPost("materias-primas")]
    public async Task<IActionResult> Crear([FromBody] MateriaPrimaDto dto)
    {
        var mp = new MateriaPrima
        {
            Nombre = dto.Nombre,
            UnidadMedida = dto.UnidadMedida,
            CostoUnitario = dto.CostoUnitario,
            Activo = true
        };

        _context.MateriasPrimas.Add(mp);
        await _context.SaveChangesAsync();
        return Ok(new { message = "Materia prima registrada" });
    }

    [HttpPut("materias-primas/{id}")]
    public async Task<IActionResult> Editar(int id, [FromBody] MateriaPrimaDto dto)
    {
        var mp = await _context.MateriasPrimas.FindAsync(id);
        if (mp == null) return NotFound();

        mp.Nombre = dto.Nombre;
        mp.UnidadMedida = dto.UnidadMedida;
        mp.CostoUnitario = dto.CostoUnitario;
        mp.Activo = dto.Activo;

        await _context.SaveChangesAsync();
        return Ok(new { message = "Materia prima actualizada" });
    }

    [HttpDelete("materias-primas/{id}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        var mp = await _context.MateriasPrimas.FindAsync(id);
        if (mp == null) return NotFound();

        _context.MateriasPrimas.Remove(mp);
        await _context.SaveChangesAsync();
        return Ok(new { message = "Materia prima eliminada" });
    }

    // recetas
    [HttpGet("recetas/{productoId}")]
    public async Task<ActionResult<IEnumerable<RecetaProductoDto>>> ObtenerPorProducto(int productoId)
    {
        var recetas = await _context.RecetasProducto
            .Include(r => r.MateriaPrima)
            .Include(r => r.Producto)
            .Where(r => r.ProductoId == productoId)
            .Select(r => new RecetaProductoDto
            {
                Id = r.Id,
                ProductoId = r.ProductoId,
                ProductoNombre = r.Producto.Nombre,
                MateriaPrimaId = r.MateriaPrimaId,
                MateriaPrimaNombre = r.MateriaPrima.Nombre,
                UnidadMedida = r.MateriaPrima.UnidadMedida,
                Cantidad = r.Cantidad,
                CostoUnitario = r.MateriaPrima.CostoUnitario
            }).ToListAsync();

        return Ok(recetas);
    }

    [HttpPost("recetas")]
    public async Task<IActionResult> Crear([FromBody] RecetaProductoDto dto)
    {
        var receta = new RecetaProducto
        {
            ProductoId = dto.ProductoId,
            MateriaPrimaId = dto.MateriaPrimaId,
            Cantidad = dto.Cantidad
        };

        _context.RecetasProducto.Add(receta);
        await _context.SaveChangesAsync();
        return Ok(new { message = "Receta registrada" });
    }

    [HttpDelete("recetas/{id}")]
    public async Task<IActionResult> EliminarReceta(int id)
    {
        var receta = await _context.RecetasProducto.FindAsync(id);
        if (receta == null) return NotFound();

        _context.RecetasProducto.Remove(receta);
        await _context.SaveChangesAsync();
        return Ok(new { message = "Receta eliminada" });
    }

}

