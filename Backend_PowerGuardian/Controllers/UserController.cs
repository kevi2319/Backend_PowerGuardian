using Backend_PowerGuardian.Models;
using Backend_PowerGuardian.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Backend_PowerGuardian.DTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Backend_PowerGuardian.Services;

namespace Backend_PowerGuardian.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly Data.ApplicationDbContext _context;
        private readonly IDispositivoService _dispositivoService;

        public UserController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration,
            Data.ApplicationDbContext context,
            IDispositivoService dispositivoService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _context = context;
            _dispositivoService = dispositivoService;
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> ObtenerUsuarioPorId(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            return Ok(new
            {
                user.Id,
                user.UserName,
                user.Email,
                user.Nombres,
                user.ApellidoPaterno,
                user.ApellidoMaterno,
                user.FechaNacimiento,
                user.Pais,
                user.PhoneNumber,
                Roles = await _userManager.GetRolesAsync(user)
            });
        }

        [HttpGet("debug-claims")]
        [AllowAnonymous]
        public IActionResult DebugClaims()
        {
            var authHeader = Request.Headers["Authorization"].FirstOrDefault();
            if (authHeader != null && authHeader.StartsWith("Bearer "))
            {
                var token = authHeader.Substring(7);
                var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
                var jsonToken = handler.ReadJwtToken(token);
                var claims = jsonToken.Claims.Select(c => new { c.Type, c.Value }).ToList();
                
                return Ok(new { 
                    TokenReceived = true,
                    AuthHeader = authHeader,
                    Claims = claims,
                    UserAuthenticated = User.Identity.IsAuthenticated,
                    UserClaims = User.Claims.Select(c => new { c.Type, c.Value }).ToList(),
                    UserIdentityName = User.Identity.Name,
                    HasSubClaim = claims.Any(c => c.Type == "sub"),
                    HasNameClaim = claims.Any(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"),
                    HasRoleClaim = claims.Any(c => c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role")
                });
            }
            
            return Ok(new { 
                TokenReceived = false,
                AuthHeader = authHeader,
                Headers = Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString())
            });
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> ObtenerPerfilActual()
        {
            var authHeader = Request.Headers["Authorization"].FirstOrDefault();
            var token = authHeader!.Substring(7);
            var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var jsonToken = handler.ReadJwtToken(token);
            var userId = jsonToken.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;

            var user = await _userManager.FindByIdAsync(userId!);
            if (user == null) return NotFound("Usuario no encontrado");

            return Ok(new
            {
                user.Id,
                user.UserName,
                user.Email,
                user.Nombres,
                user.ApellidoPaterno,
                user.ApellidoMaterno,
                user.FechaNacimiento,
                user.Pais,
                user.PhoneNumber,
                Roles = await _userManager.GetRolesAsync(user)
            });
        }

        [HttpPut("me")]
        [Authorize]
        public async Task<IActionResult> ActualizarPerfilActual([FromBody] UpdateProfileModel model)
        {
            var authHeader = Request.Headers["Authorization"].FirstOrDefault();
            var token = authHeader!.Substring(7);
            var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var jsonToken = handler.ReadJwtToken(token);
            var userId = jsonToken.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;

            var user = await _userManager.FindByIdAsync(userId!);
            if (user == null) return NotFound("Usuario no encontrado");

            if (!string.IsNullOrEmpty(model.Nombres))
                user.Nombres = model.Nombres;
            
            if (!string.IsNullOrEmpty(model.ApellidoPaterno))
                user.ApellidoPaterno = model.ApellidoPaterno;
            
            if (!string.IsNullOrEmpty(model.ApellidoMaterno))
                user.ApellidoMaterno = model.ApellidoMaterno;
            
            if (!string.IsNullOrEmpty(model.Email))
                user.Email = model.Email;
            
            if (!string.IsNullOrEmpty(model.Telefono))
                user.PhoneNumber = model.Telefono;
            
            if (!string.IsNullOrEmpty(model.Pais))
                user.Pais = model.Pais;

            if (model.FechaNacimiento.HasValue)
                user.FechaNacimiento = model.FechaNacimiento;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return BadRequest(result.Errors.Select(e => e.Description));

            return Ok(new { message = "Perfil actualizado correctamente" });
        }

        [HttpPost("me/password")]
        [Authorize]
        public async Task<IActionResult> CambiarPassword([FromBody] ChangePasswordModel model)
        {
            var authHeader = Request.Headers["Authorization"].FirstOrDefault();
            var token = authHeader!.Substring(7);
            var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var jsonToken = handler.ReadJwtToken(token);
            var userId = jsonToken.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;

            var user = await _userManager.FindByIdAsync(userId!);
            if (user == null) return NotFound("Usuario no encontrado");

            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if (!result.Succeeded)
                return BadRequest(result.Errors.Select(e => e.Description));

            return Ok(new { message = "Contraseña cambiada correctamente" });
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> ActualizarUsuario(string id, [FromBody] RegisterModel model)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            user.UserName = model.Username;
            user.Email = model.Email;
            user.Nombres = model.Nombres;
            user.ApellidoPaterno = model.ApellidoPaterno;
            user.ApellidoMaterno = model.ApellidoMaterno ?? ""; // Evitar null
            user.FechaNacimiento = model.FechaNacimiento;
            user.Pais = model.Pais;
            user.PhoneNumber = model.Telefono;

            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded ? Ok("Usuario actualizado correctamente") : BadRequest(result.Errors);
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if (await _userManager.FindByNameAsync(model.Username) != null)
                return BadRequest("Ya existe un usuario con ese nombre de usuario");

            if (model.Password != model.ConfirmPassword)
                return BadRequest("Las contraseñas no coinciden");

            if (await _userManager.FindByEmailAsync(model.Email) != null)
                return BadRequest("Ya existe un usuario con ese correo");

            if (await _userManager.Users.AnyAsync(u => u.PhoneNumber == model.Telefono))
                return BadRequest("Ya existe un usuario con ese teléfono");

            // Validar SKU
            if (string.IsNullOrWhiteSpace(model.SKU))
                return BadRequest("Debes ingresar un SKU válido");

            var unidad = await _context.ProductoUnidades
                .FirstOrDefaultAsync(u => u.SKU == model.SKU && !u.Usado);

            if (unidad == null)
                return BadRequest("El SKU ingresado no existe o ya fue utilizado.");

            // Crear usuario
            var user = new ApplicationUser
            {
                UserName = model.Username,
                Email = model.Email,
                Nombres = model.Nombres,
                ApellidoPaterno = model.ApellidoPaterno,
                ApellidoMaterno = model.ApellidoMaterno ?? "",
                FechaNacimiento = model.FechaNacimiento,
                Pais = model.Pais,
                PhoneNumber = model.Telefono
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                var errors = result.Errors?.Select(e => e.Description).ToArray();
                return BadRequest(errors != null ? string.Join("\n", errors) : "Error desconocido al registrar usuario.");
            }

            // Asegurar rol
            if (!await _roleManager.RoleExistsAsync("Cliente"))
                await _roleManager.CreateAsync(new IdentityRole("Cliente"));

            await _userManager.AddToRoleAsync(user, "Cliente");

            // Crear dispositivo desde el SKU
            var dispositivo = await _dispositivoService.CrearDesdeProductoUnidadAsync(unidad.Id, user.Id);
            if (dispositivo == null)
            {
                // Aquí puedes incluso revertir la creación del usuario si lo deseas
                return StatusCode(500, "Error al crear el dispositivo desde el SKU.");
            }

            // Marcar el SKU como usado
            unidad.Usado = true;
            unidad.FechaCompra = DateTime.UtcNow;
            unidad.UserId = user.Id;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Usuario registrado correctamente", dispositivoId = dispositivo.Id });
        }


        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Username) ??
                       await _userManager.FindByNameAsync(model.Username);

            if (user == null)
                return Unauthorized("Usuario o correo no encontrado");

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (!result.Succeeded)
                return Unauthorized("Credenciales inválidas");

            var token = await GenerateJwtToken(user);
            var roles = await _userManager.GetRolesAsync(user);
            return Ok(new {
                token,
                id = user.Id,
                role = roles.FirstOrDefault(),
                nombres = user.Nombres,
                apellidoPaterno = user.ApellidoPaterno
            });
        }

        private async Task<string> GenerateJwtToken(ApplicationUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                new Claim("id", user.Id),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, roles.FirstOrDefault() ?? "Cliente")
            };


            claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(30),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
