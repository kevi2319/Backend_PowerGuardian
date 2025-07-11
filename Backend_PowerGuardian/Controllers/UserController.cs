using Backend_PowerGuardian.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Backend_PowerGuardian.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;

        public UserController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerUsuarioPorId(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

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
                user.PhoneNumber
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> ActualizarUsuario(string id, [FromBody] RegisterModel model)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            user.UserName = model.Username;
            user.Email = model.Email;
            user.Nombres = model.Nombres;
            user.ApellidoPaterno = model.ApellidoPaterno;
            user.ApellidoMaterno = model.ApellidoMaterno;
            user.FechaNacimiento = model.FechaNacimiento;
            user.Pais = model.Pais;
            user.PhoneNumber = model.Telefono;

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
                return Ok("Usuario actualizado correctamente");

            return BadRequest(result.Errors);
        }


        // Registro de usuario
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if (model.Password != model.ConfirmPassword)
                return BadRequest("Las contraseñas no coinciden");

            var existingEmail = await _userManager.FindByEmailAsync(model.Email);
            if (existingEmail != null)
                return BadRequest("Ya existe un usuario con ese correo");

            var existingPhone = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == model.Telefono);
            if (existingPhone != null)
                return BadRequest("Ya existe un usuario con ese teléfono");

            var user = new ApplicationUser
            {
                UserName = model.Username,
                Email = model.Email,
                Nombres = model.Nombres,
                ApellidoPaterno = model.ApellidoPaterno,
                ApellidoMaterno = model.ApellidoMaterno,
                FechaNacimiento = model.FechaNacimiento,
                Pais = model.Pais,
                PhoneNumber = model.Telefono
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
                return Ok("Usuario registrado correctamente");

            return BadRequest(result.Errors);
        }

        // Login de usuario (con generación de JWT)
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            //Dentro del user buscaremos el Email 
            var user = await _userManager.FindByEmailAsync(model.Username);

            // Si no encontró por email, intenta con username
            if (user == null)
                user = await _userManager.FindByNameAsync(model.Username);

            if (user == null)
                return Unauthorized("Usuario o correo no encontrado");

            //valida la contraseña
            var result = await _signInManager.PasswordSignInAsync(user, model.Password, false, false);

            if (result.Succeeded)
            {
                var token = GenerateJwtToken(user);
                return Ok(new { token, id = user.Id });
            }

            return Unauthorized("Invalid credentials");
        }

        // Método para generar el JWT
        private string GenerateJwtToken(ApplicationUser user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

       
    }

    // Modelos para el registro y login
    public class RegisterModel
    {

        public string Username { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string Nombres { get; set; }
        public string ApellidoPaterno { get; set; }
        public string ApellidoMaterno { get; set; }
        public string FechaNacimiento { get; set; }
        public string Pais { get; set; }
        public string Email { get; set; }
        public string Telefono { get; set; }
    }

    public class LoginModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
