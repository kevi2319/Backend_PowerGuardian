using Backend_PowerGuardian.Models;
using Backend_PowerGuardian.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

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

        public UserController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration,
            Data.ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _context = context;
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
        [JwtAuthorize]
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
        [JwtAuthorize]
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
        [JwtAuthorize]
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

            var unidad = await _context.ProductoUnidades.FirstOrDefaultAsync(u => u.SKU == model.SKU);
            if (unidad == null)
                return BadRequest("El SKU ingresado no existe");
            if (unidad.Usado)
                return BadRequest("El SKU ya fue utilizado para un registro");

            var user = new ApplicationUser
            {
                UserName = model.Username,
                Email = model.Email,
                Nombres = model.Nombres,
                ApellidoPaterno = model.ApellidoPaterno,
                ApellidoMaterno = model.ApellidoMaterno ?? "", // Evitar null
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

            if (!await _roleManager.RoleExistsAsync("Cliente"))
                await _roleManager.CreateAsync(new IdentityRole("Cliente"));

            await _userManager.AddToRoleAsync(user, "Cliente");

            // Marcar SKU como usado y guardar fecha de compra
            unidad.Usado = true;
            unidad.FechaCompra = DateTime.UtcNow;
            unidad.UserId = user.Id;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Usuario registrado correctamente" });
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
                new(JwtRegisteredClaimNames.Sub, user.Id),
                new(ClaimTypes.Name, user.UserName),
                new(JwtRegisteredClaimNames.Email, user.Email),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
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

    // DTOs para el registro y login

    public class RegisterModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string Nombres { get; set; }
        public string ApellidoPaterno { get; set; }
        public string ApellidoMaterno { get; set; } // Aquí puedes poner string? si lo deseas
        public DateTime? FechaNacimiento { get; set; }
        public string Pais { get; set; }
        public string Email { get; set; }
        public string Telefono { get; set; }
        public string SKU { get; set; }
    }

    public class LoginModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class UpdateProfileModel
    {
        public string? Nombres { get; set; }
        public string? ApellidoPaterno { get; set; }
        public string? ApellidoMaterno { get; set; }
        public string? Email { get; set; }
        public string? Telefono { get; set; }
        public string? Pais { get; set; }
        public DateTime? FechaNacimiento { get; set; }
    }

    public class ChangePasswordModel
    {
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
